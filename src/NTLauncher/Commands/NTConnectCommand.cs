using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTLauncher.Commands
{
    public class NTConnectCommand : ICommand
    {
        public Func<string, Task> AsyncCallback { get; set; }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object _)
        {
            return true;
        }

        public void Execute(object __)
        {
            _ = Task.Run(() => AsyncCallback(((PasswordBox)__).Password).ConfigureAwait(false));
        }
    }
}
