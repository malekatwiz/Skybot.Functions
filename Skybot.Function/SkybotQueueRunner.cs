using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace Skybot.Function
{
    public static class SkybotQueueRunner
    {
        private static readonly SkybotClient skybotClient;
        private static readonly TextoClient textoClient;

        static SkybotQueueRunner()
        {
            skybotClient = new SkybotClient();
            textoClient= new TextoClient();
        }

        [FunctionName("SkybotQueueRunner")]
        public static async System.Threading.Tasks.Task RunAsync([ServiceBusTrigger("incomingtexts", AccessRights.Listen, Connection = "SkybotBusConnectionString")]string queueItem, TraceWriter log)
        {
            log.Info($"C# ServiceBus queue trigger function processed message: {queueItem}");

            var query = ExtractMessage(queueItem);
            if (!string.IsNullOrEmpty(query))
            {
                var reply = await skybotClient.RunQuery(query);

                if (!string.IsNullOrEmpty(reply))
                {
                    await textoClient.Send(reply);
                }
            }
        }

        private static string ExtractMessage(string queueItem)
        {
            var item = JsonConvert.DeserializeObject<dynamic>(queueItem);
            return item.Body;
        }
    }
}
