using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp
{
    public static class SendMessage
    {
        public class InboxRule
        {
            public string RowKey { get; set; }

            public string Queues { get; set; }

            public bool Accepted(string destination)
            {
                if (string.IsNullOrEmpty(destination))
                    return false;

                if (string.IsNullOrEmpty(Queues))
                    return false;

                foreach (var acceptedCountry in Queues.Split(','))
                {
                    if (acceptedCountry.ToLower() == destination.ToLower())
                        return true;
                }
                return false;
            }
        }

        public class Message
        { 
            public string From { get; set; }

            public string To { get; set; }

            public string Body { get; set; }
        }

        [FunctionName("SendMessage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "send")] [FromBody] Message message,
            [Table("inboxrules", partitionKey:"InboundRules", rowKey: "{From}")] InboxRule rule,
            [ServiceBus("inbox-{To}", Connection = "ServiceBusConnectionString")] IAsyncCollector<Message> serviceBus,
            ILogger log)
        {
            if (message == null)
                return new BadRequestResult();

            log.LogDebug($"Processed a request from '{message.From}' to '{message.To};");

            if (rule == null || !rule.Accepted(message.To))
                return new StatusCodeResult(403);
            try
            {
                await serviceBus.AddAsync(message);
            }
            catch (MessagingEntityNotFoundException)
            {
                return new NotFoundResult();
            }
            return new OkResult();
        }
    }
}
