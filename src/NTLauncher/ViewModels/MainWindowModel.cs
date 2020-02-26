using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using NTLauncher.Commands;
using NTLauncher.Models;

namespace NTLauncher.ViewModels
{
    public class MainWindowModel
    {
        private readonly SparkClient _spark;

        public string Email { get; set; }

        public ObservableCollection<InGameAccount> Accounts { get; }
        public InGameAccount SelectedAccount { get; set; }

        public NTCommand StartGameCommand { get; set; }
        public NTCommand OptionsCommand { get; set; }
        public NTCommand QuitCommand { get; set; }
        public NTCommand ClearAccountsCommand { get; set; }
        public NTConnectCommand ConnectAccountsCommand { get; set; }

        public MainWindowModel(SparkClient spark)
        {
            _spark = spark;

            Accounts = new ObservableCollection<InGameAccount>();

            StartGameCommand = new NTCommand { Callback = OnStartPressed };
            OptionsCommand = new NTCommand { Callback = OnOptionsPressed };
            QuitCommand = new NTCommand { Callback = OnQuitPressed };
            ClearAccountsCommand = new NTCommand { Callback = () => Accounts.Clear() };
            ConnectAccountsCommand = new NTConnectCommand { AsyncCallback = OnConnectPressed };
        }

        public void OnStartPressed()
        {

        }

        public void OnOptionsPressed()
        {

        }

        public void OnQuitPressed()
        {
            Application.Current.Shutdown();
        }

        public async Task OnConnectPressed(string password)
        {
            var obj = await _spark.GetSessionAsync(Email, password);
            var accounts = await _spark.GetAccountsAsync(obj["token"]);
            foreach (var account in accounts)
            {
                Application.Current.Dispatcher.Invoke(()
                    => Accounts.Add(new InGameAccount
                    {
                        Id = account.Key,
                        Name = account.Value.AccountName
                    }));
            }
        }
    }
}
