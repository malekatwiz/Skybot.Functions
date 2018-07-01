using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Skybot.Function
{
    public class SkybotClient
    {
        private readonly HttpClient client;
        private string token;

        public SkybotClient()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> RunQuery(string message)
        {
            if (string.IsNullOrEmpty(token))
            {
                await GetToken();
            }

            var queryObject = new
            {
                message
            };

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await client.PostAsJsonAsync($"{Settings.SkybotApiUri}/api/Skybot/Process", queryObject);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<dynamic>(responseContent);
        }

        private async Task GetToken()
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"clientId", Settings.SkybotClientId},
                {"clientSecret", Settings.SkybotClientSecret },
                {"identifier", Settings.SkybotClientIdentifier }
            });

            var response = await client.PostAsync($"{Settings.SkybotApiUri}/api/Skybot/token", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            token = JsonConvert.DeserializeObject<string>(responseContent);
        }
    }
}
