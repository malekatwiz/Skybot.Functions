using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace Skybot.Function
{
    public static class SkybotQueueRunner
    {
        private static readonly SkybotClient SkybotClient;

        static SkybotQueueRunner()
        {
            SkybotClient = new SkybotClient();
        }

        [FunctionName("SkybotQueueRunner")]
        public static async System.Threading.Tasks.Task RunAsync([ServiceBusTrigger("incomingtexts", AccessRights.Listen, Connection = "SkybotBusConnectionString")]string queueItem, TraceWriter log)
        {
            log.Info($"C# ServiceBus queue trigger function processed message: {queueItem}");

            var textMessage = ConvertToTextMessage(queueItem);
            if (!string.IsNullOrEmpty(textMessage.Message))
            {
                var reply = await SkybotClient.RunQuery(textMessage);

                if (!string.IsNullOrEmpty(reply))
                {
                    await SkybotClient.PushMessage(new TextMessage
                    {
                        ToNumber = textMessage.FromNumber,
                        FromNumber = Settings.FromNumber,
                        Message = reply
                    });
                }
            }
        }

        private static TextMessage ConvertToTextMessage(string queueItem)
        {
            return JsonConvert.DeserializeObject<TextMessage>(queueItem);
        }
    }
}
