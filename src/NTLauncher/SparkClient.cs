using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NTLauncher
{
    public sealed class SparkClient
    {
        private const string SessionsEndpoint = "https://spark.gameforge.com/api/v1/auth/thin/sessions";
        private const string AccountsEndpoint = "https://spark.gameforge.com/api/v1/user/accounts";

        private readonly HttpClient _client;

        public SparkClient()
        {
            _client = new HttpClient();

            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:73.0) Gecko/20100101 Firefox/73.0");
            _client.DefaultRequestHeaders.Add("TNT-Installation-Id", Guid.NewGuid().ToString());
            _client.DefaultRequestHeaders.Add("Origin", "spark://www.gameforge.com");
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

            var request = await _client.PostAsync(SessionsEndpoint, requestContent).ConfigureAwait(false);
            var response = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
        }

        public async Task<Dictionary<string, NTAccount>> GetAccountsAsync(string token)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, AccountsEndpoint);
            requestMessage.Headers.Add("Authorization", $"Bearer {token}");
            requestMessage.Headers.Add("Connection", "Keep-Alive");

            var request = await _client.SendAsync(requestMessage).ConfigureAwait(false);
            var response = await request.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<Dictionary<string, NTAccount>>(response);
        }

        public class NTAccount
        {
            [JsonProperty("displayName")]
            public string AccountName { get; set; }
        }
    }
}
