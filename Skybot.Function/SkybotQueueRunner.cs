using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace Skybot.Function
{
    public static class SkybotQueueRunner
    {
        private static readonly SkybotClient skybotClient;

        static SkybotQueueRunner()
        {
            skybotClient = new SkybotClient();
        }

        [FunctionName("SkybotQueueRunner")]
        public static async System.Threading.Tasks.Task RunAsync([ServiceBusTrigger("incomingtexts", AccessRights.Listen, Connection = "SkybotBusConnectionString")]string queueItem, TraceWriter log)
        {
            log.Info($"C# ServiceBus queue trigger function processed message: {queueItem}");

            var textMessage = ConvertToTextMessage(ExtractMessage(queueItem));
            if (!string.IsNullOrEmpty(textMessage.Body))
            {
                var reply = await skybotClient.RunQuery(textMessage);

                if (!string.IsNullOrEmpty(reply))
                {
                    await skybotClient.PushMessage(new TextMessage
                    {
                        To = textMessage.To,
                        From = Settings.FromNumber,
                        Body = reply
                    });
                }
            }
        }

        private static string ExtractMessage(string queueItem)
        {
            var item = JsonConvert.DeserializeObject<dynamic>(queueItem);
            return item.Body;
        }

        private static TextMessage ConvertToTextMessage(string item)
        {
            return JsonConvert.DeserializeObject<TextMessage>(item);
        }
    }
}
