using System;
using System.Windows.Input;

namespace NTLauncher.Commands
{
    public class NTCommand : ICommand
    {
        public Action Callback { get; set; }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object _)
        {
            return true;
        }

        public void Execute(object _)
        {
            Callback();
        }
    }
}
