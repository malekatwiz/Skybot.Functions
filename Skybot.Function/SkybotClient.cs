using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace Skybot.Function
{
    public class SkybotClient
    {
        private readonly HttpClient _httpClient;
        private readonly QueueClient _queueClient;
        private string _token;

        public SkybotClient()
        {
            _httpClient = new HttpClient();
            _queueClient = new QueueClient(Settings.SkysendConnectionString, "outgoingtexts");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> RunQuery(TextMessage message)
        {
            if (string.IsNullOrEmpty(_token))
            {
                await GetToken();
            }

            var queryObject = new
            {
                Message = message.Body
            };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");

            var response = await _httpClient.PostAsJsonAsync($"{Settings.SkybotUri}/api/Skybot/Process", queryObject);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<dynamic>(responseContent);
        }

        public Task PushMessage(TextMessage textMessage)
        {
            return _queueClient.SendAsync(new Message
            {
                To = "textBrokers",
                Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(textMessage))
            });
        }

        private async Task GetToken()
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"client_id", Settings.ClientId},
                {"client_secret", Settings.ClientSecret },
                {"grant_type", "client_credentials" }
            });

            var response = await _httpClient.PostAsync($"{Settings.SkybotAuthUri}/connect/token", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            var deserializedResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

            _token = deserializedResponse.access_token;
        }
    }
}
