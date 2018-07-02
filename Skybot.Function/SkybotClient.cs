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
        private readonly HttpClient httpClient;
        private readonly QueueClient queueClient;
        private string token;

        public SkybotClient()
        {
            httpClient = new HttpClient();
            queueClient = new QueueClient(Settings.SkysendConnectionString, "outgoingtexts");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> RunQuery(TextMessage message)
        {
            if (string.IsNullOrEmpty(token))
            {
                await GetToken();
            }

            var queryObject = new
            {
                message.Body
            };

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await httpClient.PostAsJsonAsync($"{Settings.SkybotApiUri}/api/Skybot/Process", queryObject);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<dynamic>(responseContent);
        }

        public Task PushMessage(TextMessage textMessage)
        {
            return queueClient.SendAsync(new Message
            {
                To = "textBrokers",
                Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(textMessage))
            });
        }

        private async Task GetToken()
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"clientId", Settings.SkybotClientId},
                {"clientSecret", Settings.SkybotClientSecret },
                {"identifier", Settings.SkybotClientIdentifier }
            });

            var response = await httpClient.PostAsync($"{Settings.SkybotApiUri}/api/Skybot/token", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            token = JsonConvert.DeserializeObject<string>(responseContent);
        }
    }
}
