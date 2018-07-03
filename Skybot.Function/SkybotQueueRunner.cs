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

            var textMessage = ConvertToTextMessage(queueItem);
            if (!string.IsNullOrEmpty(textMessage.Body))
            {
                var reply = await skybotClient.RunQuery(textMessage);

                if (!string.IsNullOrEmpty(reply))
                {
                    await skybotClient.PushMessage(new TextMessage
                    {
                        To = textMessage.From,
                        From = Settings.FromNumber,
                        Body = reply
                    });
                }
            }
        }

        private static TextMessage ConvertToTextMessage(string queueItem)
        {
            var item = JsonConvert.DeserializeObject<dynamic>(queueItem);
            return new TextMessage
            {
                Body = item.Body,
                From = item.From,
                To = item.To
            };
        }
    }
}
