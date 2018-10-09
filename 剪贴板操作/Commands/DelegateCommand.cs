#region Version Info

/*******************************************************************************************************************
 *【本类功能概述】
 * DelegateCommand
 *******************************************************************************************************************
 * 名　　称：剪切板操作
 * 作　　者：凌轩
 * Q　　Q ：39972802
 * 版　　权：Copyright ©  2018
 * 版　　本：V 1.0.0.0
 * 创建日期：2018/10/8 16:22:50
 *******************************************************************************************************************
 * 修 改  人：
 * 修改日期：2018/10/8 16:22:50
 * 说     明： 版权所有，请与作者联系。
 *******************************************************************************************************************/

#endregion Version Info

using System;
using System.Windows.Input;

namespace 剪切板操作
{
    internal class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        //A method prototype return a bool type.
        public Func<object, bool> CanExecuteCommand { get; set; }

        //A method prototype without return value.
        public Action<object> ExecuteCommand { get; set; }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteCommand != null)
            {
                return CanExecuteCommand(parameter);
            }
            else
            {
                return true;
            }
        }

        public void Execute(object parameter)
        {
            ExecuteCommand?.Invoke(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}