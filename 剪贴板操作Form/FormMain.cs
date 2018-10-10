using BestString;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using 剪切板操作Form.Properties;

namespace 剪贴板操作Form
{
    public partial class FormMain : Form
    {
        private List<string> DianBiao = new List<string>();
        private int index = 1;
        private IntPtr nextClipboardViewer;
        private bool textchangedmark = true;

        public FormMain()
        {
            InitializeComponent();
            nextClipboardViewer = SetClipboardViewer(this.Handle);
        }

        private void Btn_Start_Click(object sender, EventArgs e)
        {
            if (Btn_Start.Text == "开始")
            {
                nextClipboardViewer = SetClipboardViewer(this.Handle);
                Btn_Start.Text = "停止";
                toolStripStatusLabel1.Text = "剪贴板监控中...";
                timer1.Enabled = true;
            }
            else
            {
                ChangeClipboardChain(this.Handle, nextClipboardViewer);
                Btn_Start.Text = "开始";
                toolStripStatusLabel1.Text = "停止监控";
                timer1.Enabled = false;
            }
        }

        #region Definitions

        private const int WM_CHANGECBCHAIN = 0x30D;

        //Constants for API Calls...
        private const int WM_DRAWCLIPBOARD = 0x308;

        private bool bTransparent = false;

        //Handle for next clipboard viewer...
        private IntPtr mNextClipBoardViewerHWnd;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr HWnd, IntPtr HWndNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        //API declarations...
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        #endregion Definitions

        #region Message Process

        private void DisplayClipboardData()
        {
            //显示剪贴板中的文本信息
            if (Clipboard.ContainsText())
            {
                Txt_Dangqianneirong.Text = Clipboard.GetText();
            }
            ////显示剪贴板中的图片信息
            //if (Clipboard.ContainsImage())
            //{
            //    pictureBox1.Image = Clipboard.GetImage();
            //    pictureBox1.Update();
            //}
        }

        //Override WndProc to get messages...
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    {
                        //The clipboard has changed...
                        //##########################################################################
                        // Process Clipboard Here :)........................
                        //##########################################################################
                        SendMessage(mNextClipBoardViewerHWnd, m.Msg, m.WParam.ToInt32(), m.LParam.ToInt32());

                        DisplayClipboardData();

                        break;
                    }
                case WM_CHANGECBCHAIN:
                    {
                        //Another clipboard viewer has removed itself...
                        if (m.WParam == (IntPtr)mNextClipBoardViewerHWnd)
                        {
                            mNextClipBoardViewerHWnd = m.LParam;
                        }
                        else
                        {
                            SendMessage(mNextClipBoardViewerHWnd, m.Msg, m.WParam.ToInt32(), m.LParam.ToInt32());
                        }
                        break;
                    }

                default:
                    base.WndProc(ref m);
                    break;
            }
            //base.WndProc(ref m);
        }

        #endregion Message Process

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
            else
            {
                WindowState = FormWindowState.Normal;
            }
        }

        private void LstV_Dianbiaowenjian_DragDrop(object sender, DragEventArgs e)
        {
            string[] data = (string[])e.Data.GetData("FileDrop");
            //TODO do something....
            foreach (var path in data)
            {
                OpenDianbiao(path);
            }

            //((TextBox)sender).Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
        }

        private void LstV_Dianbiaowenjian_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Visible == false)
                {
                    Show();
                    WindowState = FormWindowState.Normal;
                    Activate();
                }
                else
                {
                    Hide();
                }
            }
        }

        private void OpenDianbiao(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (StreamReader sr = new StreamReader(filePath, Encoding.Default))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        string strm = @"(?<Index>\d+)\s+(?<IOValue>.+)";
                        Regex matchesRegex = new Regex(strm, RegexOptions.IgnoreCase);

                        Match m = matchesRegex.Match(line);

                        if (m.Success)
                        {
                            DianBiao.Add(m.Groups["IOValue"].Value);
                        }
                    }
                    LstV_Dianbiaowenjian.Items.Add(
                        new ListViewItem(
                        new string[]{
                            index++.ToString(),
                            Path.GetFileName(filePath)
                         }));
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.bTransparent)
            {
                this.bTransparent = false;
                //显示透明图标
                this.notifyIcon1.Icon = Resources.chinaz52;
            }
            else
            {
                this.bTransparent = true;
                //显示托盘图标
                this.notifyIcon1.Icon = Resources.chinaz53;
            }
        }

        private void Txt_Dangqianneirong_TextChanged(object sender, EventArgs e)
        {
            if (textchangedmark)
            {
                string[] datas = DianBiao.ToArray();
                QueryHelper.Query(Txt_Dangqianneirong.Text, datas, (key, result, elapsed) => this.Invoke(new MethodInvoker(() =>
                {
                    if (result == null || result.Length == 0)
                    {
                        result = new string[0];
                        return;
                    }

                    listBox1.BeginUpdate();
                    listBox1.Items.Insert(0, Txt_Dangqianneirong.Text);
                    listBox1.EndUpdate();

                    textchangedmark = false;
                    if (!Txt_Dangqianneirong.Text.StartsWith(Txt_Qianzhui.Text))
                    {
                        string text = $"{Txt_Qianzhui.Text}{Txt_Dangqianneirong.Text}";
                        Clipboard.SetDataObject(text, true);
                    }

                    //Txt_Dangqianneirong.Text += string.Format("搜索参数为 {0}" + Environment.NewLine + "共找到 {1} 条结果" + Environment.NewLine + "耗时 {2}" + Environment.NewLine + Environment.NewLine, key, result.Length, elapsed);
                    //Txt_Dangqianneirong.SelectionStart = Txt_Dangqianneirong.Text.Length;
                    //Txt_Dangqianneirong.ScrollToCaret();
                })));
            }
            else
            {
                textchangedmark = true;
            }
        }

        private void 还原ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 最小化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
    }
}