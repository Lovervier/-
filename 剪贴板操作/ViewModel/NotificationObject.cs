#region Version Info

/*******************************************************************************************************************
 *【本类功能概述】
 * NotificationObject绑定数据属性。这个类的作用是实现了INotifyPropertyChanged接口。
 * WPF中类要实现这个接口，其属性成员才具备通知UI的能力。
 *******************************************************************************************************************
 * 名　　称：剪切板操作
 * 作　　者：凌轩
 * Q　　Q ：39972802
 * 版　　权：Copyright ©  2018
 * 版　　本：V 1.0.0.0
 * 创建日期：2018/10/8 16:16:53
 *******************************************************************************************************************
 * 修 改  人：
 * 修改日期：2018/10/8 16:16:53
 * 说     明： 版权所有，请与作者联系。
 *******************************************************************************************************************/

#endregion Version Info

using System;
using System.ComponentModel;

namespace 剪切板操作.ViewModel
{
    internal class NotificationObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 通知发生改变的属性名称。
        /// </summary>
        /// <param name="propertyName">  </param>
        public void RaisePropertyChanged(string propertyName)
        {
            //if (this.PropertyChanged != null)
            //{
            //    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            //}
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}