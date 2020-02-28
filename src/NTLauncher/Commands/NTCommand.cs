using System;
using System.Windows.Input;

namespace NTLauncher.Commands
{
    public class NTCommand<T> : ICommand
    {
        public Action<T> Callback { get; set; }

        public virtual event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(object _)
        {
            return true;
        }

        public virtual void Execute(object _)
        {
            Callback((T)_);
        }
    }

    public class NTCommand : NTCommand<object>
    {
        public new Action Callback { get; set; }

        public override void Execute(object _)
        {
            Callback();
        }
    }
}
