using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NTLauncher
{
    public sealed class SparkClient
    {
        private const string TokensEndpoint = "https://spark.gameforge.com/api/v1/auth/thin/codes";
        private const string SessionsEndpoint = "https://spark.gameforge.com/api/v1/auth/thin/sessions";
        private const string AccountsEndpoint = "https://spark.gameforge.com/api/v1/user/accounts";

        private readonly HttpClient _webClient;
        private readonly HttpClient _gfLauncherClient;

        public SparkClient()
        {
            _webClient = new HttpClient();
            _webClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:73.0) Gecko/20100101 Firefox/73.0");
            _webClient.DefaultRequestHeaders.Add("TNT-Installation-Id", "d3b2a0c1-f0d0-4888-ae0b-1c5e1febdafb");
            _webClient.DefaultRequestHeaders.Add("Origin", "spark://www.gameforge.com");

            _gfLauncherClient = new HttpClient();
            _gfLauncherClient.DefaultRequestHeaders.Add("User-Agent", "GameforgeClient/2.0.48");
            _gfLauncherClient.DefaultRequestHeaders.Add("TNT-Installation-Id", "d3b2a0c1-f0d0-4888-ae0b-1c5e1febdafb");
            _gfLauncherClient.DefaultRequestHeaders.Add("Origin", "spark://www.gameforge.com");
        }

        public async Task<Dictionary<string, string>> GetSessionAsync(string email, string password, string gfLang = "fr", string locale = "fr_FR")
        {
            var content = new Dictionary<string, string>
            {
                ["gfLang"] = gfLang,
                ["locale"] = locale,
                ["identity"] = email,
                ["password"] = password,
                ["platformGameId"] = "dd4e22d6-00d1-44b9-8126-d8b40e0cd7c9"
            };

            var json = JsonConvert.SerializeObject(content);

            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            var request = await _webClient.PostAsync(SessionsEndpoint, requestContent).ConfigureAwait(false);
            var response = await request.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
        }

        public async Task<Dictionary<string, NTAccount>> GetAccountsAsync(string token)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, AccountsEndpoint);
            requestMessage.Headers.Add("Authorization", $"Bearer {token}");
            requestMessage.Headers.Add("Connection", "Keep-Alive");

            var request = await _webClient.SendAsync(requestMessage).ConfigureAwait(false);
            var response = await request.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<Dictionary<string, NTAccount>>(response);
        }

        public async Task<string> GetTokenAsync(string token, string id)
        {
            try
            {
                var content = new Dictionary<string, string>
                {
                    ["platformGameAccountId"] = id
                };

                var json = JsonConvert.SerializeObject(content);

                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, TokensEndpoint)
                {
                    Content = requestContent
                };
                requestMessage.Headers.Add("Authorization", $"Bearer {token}");
                requestMessage.Headers.Add("Connection", "Keep-Alive");

                var request = await _gfLauncherClient.SendAsync(requestMessage).ConfigureAwait(false);
                var response = await request.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JObject.Parse(response)["code"].ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("GetTokenAsync: " + e.Message);
            }

            return null;
        }

        public class NTAccount
        {
            [JsonProperty("displayName")]
            public string AccountName { get; set; }

            [JsonProperty("gameId")]
            public string GameId { get; set; }
        }
    }
}
