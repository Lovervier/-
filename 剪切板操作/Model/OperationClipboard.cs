#region Version Info

/*******************************************************************************************************************
 *【本类功能概述】
 * OperationClipboard
 *******************************************************************************************************************
 * 名　　称：剪切板操作.Model
 * 作　　者：凌轩
 * Q　　Q ：39972802
 * 版　　权：Copyright ©  2018
 * 版　　本：V 1.0.0.0
 * 创建日期：2018/10/8 22:57:01
 *******************************************************************************************************************
 * 修 改  人：
 * 修改日期：2018/10/8 22:57:01
 * 说     明： 版权所有，请与作者联系。
 *******************************************************************************************************************/

#endregion Version Info

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace 剪切板操作.Model
{
    internal class OperationClipboard : Window
    {
        /// <summary>
        /// 剪贴板内容改变时API函数向windows发送的消息
        /// </summary>
        private const int WM_CLIPBOARDUPDATE = 0x031D;

        private Action ClipboardChangedHandle;

        /// <summary>
        /// 剪贴板内容改变
        /// </summary>
        //private void OnClipboardChanged()
        //{
        //    // HTodo ：复制的文件路径
        //    string text = Clipboard.GetText();

        // if (!string.IsNullOrEmpty(text)) { if (this._viewModel.ClipBoradSource.Count > 0) {
        // ClipBoradBindModel last = this._viewModel.ClipBoradSource.First();

        // if (last.Detial != text) { ClipBoradBindModel f = new ClipBoradBindModel(text,
        // ClipBoardType.Text); this._viewModel.ClipBoradSource.Insert(0, f); } } else {
        // ClipBoradBindModel f = new ClipBoradBindModel(text, ClipBoardType.Text);
        // this._viewModel.ClipBoradSource.Insert(0, f); } }

        // // HTodo ：复制的文件 System.Collections.Specialized.StringCollection list = Clipboard.GetFileDropList();

        // foreach (var item in list) { if (Directory.Exists(item) || File.Exists(item)) { if
        // (this._viewModel.ClipBoradSource.Count > 0) { ClipBoradBindModel last = this._viewModel.ClipBoradSource.First();

        // if (last.Detial != item) { ClipBoradBindModel f = new ClipBoradBindModel(item,
        // ClipBoardType.FileSystem); this._viewModel.ClipBoradSource.Insert(0, f); } } else {
        // ClipBoradBindModel f = new ClipBoradBindModel(item, ClipBoardType.FileSystem);
        // this._viewModel.ClipBoradSource.Insert(0, f); } } }

        // //// HTodo ：复制的图片 //BitmapSource bit = System.Windows.Clipboard.GetImage();

        // //if (bit != null) //{ // if (this._viewModel.ClipBoradSource.Count > 0) // { //
        // ClipBoradBindModel last = this._viewModel.ClipBoradSource.First();

        // // if (last.Detial != bit.ToString()) { ClipBoradBindModel f = new //
        // ClipBoradBindModel(bit.ToString(), ClipBoardType.Image); //
        // this._viewModel.ClipBoradSource.Insert(0, f); } } else { ClipBoradBindModel f = new //
        // ClipBoradBindModel(bit.ToString(), ClipBoardType.Image); //
        // this._viewModel.ClipBoradSource.Insert(0, f); }

        //    //}
        //}

        /// <summary>
        /// 添加监视消息
        /// </summary>
        private void Win_SourceInitialized(object sender, EventArgs e)
        {
            //HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            //source.AddHook(WndProc);
            if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
            {
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }

        /// <summary>
        /// WPF窗口重写
        /// </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            Win_SourceInitialized(this, e);

            // HTodo ：添加剪贴板监视
            IntPtr handle = (new WindowInteropHelper(this)).Handle;

            AddClipboardFormatListener(handle);
        }

        /// <summary>
        /// windows用于监视剪贴板的API函数
        /// </summary>
        /// <param name="hwnd"> 要监视剪贴板的窗口的句柄 </param>
        /// <returns> 成功则返回true </returns>
        [DllImport("user32.dll")]//引用dll,确保API可用
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        /// 取消对剪贴板的监视
        /// </summary>
        /// <param name="hwnd"> 监视剪贴板的窗口的句柄 </param>
        /// <returns> 成功则返回true </returns>
        [DllImport("user32.dll")]//引用dll,确保API可用
        public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        #region 系统消息

        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_CLIPBOARDUPDATE:
                    {
                        if (ClipboardChangedHandle != null)
                        {
                            ClipboardChangedHandle.Invoke();
                        }
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        #endregion 系统消息
    }
}