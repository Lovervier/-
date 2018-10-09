#region Version Info

/*******************************************************************************************************************
 *【本类功能概述】
 * MainWindowViewModel
 *******************************************************************************************************************
 * 名　　称：剪切板操作.ViewModel
 * 作　　者：凌轩
 * Q　　Q ：39972802
 * 版　　权：Copyright ©  2018
 * 版　　本：V 1.0.0.0
 * 创建日期：2018/10/8 22:33:54
 *******************************************************************************************************************
 * 修 改  人：
 * 修改日期：2018/10/8 22:33:54
 * 说     明： 版权所有，请与作者联系。
 *******************************************************************************************************************/

#endregion Version Info

using System;

namespace 剪切板操作.ViewModel
{
    internal class MainWindowViewModel : NotificationObject
    {
        private string clipboardString;

        /// <summary>
        /// 剪贴板字符串。
        /// </summary>
        public string ClipboardString
        {
            get { return clipboardString; }
            set
            {
                clipboardString = value;
                this.RaisePropertyChanged("ClipboardString");
            }
        }

        public DelegateCommand StartCommand { get; set; }

        private void StartOperation()
        {
        }
    }
}