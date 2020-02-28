using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using NTLauncher.Commands;
using NTLauncher.Models;

namespace NTLauncher.ViewModels
{
    public class MainWindowModel
    {
        private readonly SparkClient _spark;
        private readonly string _nostalePath;

        public string Email { get; set; }

        public ObservableCollection<InGameAccount> Accounts { get; }
        public InGameAccount SelectedAccount { get; set; }

        public NTCommand StartGameCommand { get; set; }
        public NTCommand OptionsCommand { get; set; }
        public NTCommand QuitCommand { get; set; }
        public NTCommand ClearAccountsCommand { get; set; }
        public NTCommand<PasswordBox> ConnectAccountsCommand { get; set; }

        public MainWindowModel(SparkClient spark, string nostalePath)
        {
            _spark = spark;
            _nostalePath = nostalePath;

            Accounts = new ObservableCollection<InGameAccount>();

            StartGameCommand = new NTCommand { Callback = OnStartPressed };
            OptionsCommand = new NTCommand { Callback = OnOptionsPressed };
            QuitCommand = new NTCommand { Callback = OnQuitPressed };
            ClearAccountsCommand = new NTCommand { Callback = () => Accounts.Clear() };
            ConnectAccountsCommand = new NTCommand<PasswordBox> { Callback = OnConnectPressed };
        }

        public async void OnStartPressed()
        {
            var token = await _spark.GetTokenAsync(SelectedAccount.Token, SelectedAccount.Id);
            Console.WriteLine(token);

            _ = Task.Run(() =>
            {
                try
                {
                    var pipe = new NamedPipeServerStream(@"GameforgeClientJSONRPC", PipeDirection.InOut, 254, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0);

                    var done = false;
                    while (!done)
                    {
                        try
                        {
                            pipe.WaitForConnection();
                        }
                        catch (IOException)
                        {
                            pipe = new NamedPipeServerStream(@"GameforgeClientJSONRPC", PipeDirection.InOut, 254, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0);
                            pipe.WaitForConnection();
                        }

                        var data = new byte[1024];
                        var amount = pipe.Read(data, 0, data.Length);
                        if (amount == 0)
                        {
                            Console.WriteLine("Broken pipe. Stopping.");
                            return;
                        }

                        var strData = Encoding.Default.GetString(data);
                        Console.WriteLine($"< {strData}");

                        var payload = JsonConvert.DeserializeObject<NTPipeInPayload>(strData);
                        if (payload is null)
                        {
                            Console.WriteLine("Couldn't parse the payload");
                            continue;
                        }

                        var outPayload = new NTPipeOutPayload
                        {
                            Id = payload.Id,
                            JsonRPC = payload.JsonRPC
                        };

                        switch (payload.Method)
                        {
                            case "ClientLibrary.isClientRunning":
                                outPayload.Result = true;
                                break;
                            case "ClientLibrary.initSession":
                                outPayload.Result = payload.Params.SessionId;
                                break;
                            case "ClientLibrary.queryAuthorizationCode":
                                outPayload.Result = token;
                                break;
                            case "ClientLibrary.queryGameAccountName":
                                outPayload.Result = SelectedAccount.Name;
                                done = true;
                                break;
                        }

                        var outString = JsonConvert.SerializeObject(outPayload);
                        Console.WriteLine($"> {outString}");
                        data = Encoding.Default.GetBytes(outString);
                        pipe.Write(data, 0, data.Length);
                        pipe.Flush();
                        pipe.WaitForPipeDrain();
                    }

                    pipe.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Oof (fix that later plz): {e.Message}");
                }
            });

            var psi = new ProcessStartInfo
            {
                FileName = Path.Combine(_nostalePath, "NostaleClientX.exe"),
                Arguments = "gf"
            };
            psi.Environment.Add("_TNT_CLIENT_APPLICATION_ID", "d3b2a0c1-f0d0-4888-ae0b-1c5e1febdafb");
            Process.Start(psi);
        }

        public void OnOptionsPressed()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(_nostalePath, "NtConfig.exe"),
                Verb = "runas"
            });
        }

        public void OnQuitPressed()
        {
            Application.Current.Shutdown();
        }

        public async void OnConnectPressed(PasswordBox passwordBox)
        {
            var obj = await _spark.GetSessionAsync(Email, passwordBox.Password);
            var accounts = await _spark.GetAccountsAsync(obj["token"]);
            foreach (var account in accounts)
            {
                if (account.Value.GameId != "dd4e22d6-00d1-44b9-8126-d8b40e0cd7c9")
                {
                    continue;
                }

                Application.Current.Dispatcher.Invoke(()
                    => Accounts.Add(new InGameAccount
                    {
                        Id = account.Key,
                        Name = account.Value.AccountName,
                        Email = Email,
                        Token = obj["token"]
                    }));
            }
        }
    }
}
