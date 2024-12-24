using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;

namespace operationfichier
{
    public class operationfichier
    {
        private readonly ILogger<operationfichier> _logger;

        public Function1(ILogger<operationfichier> logger)
        {
            _logger = logger;
        }

        [Function(nameof(operationfichier))]
        public async Task Run(
            [ServiceBusTrigger("devoirmessagequeue", Connection = "servicebusconnectionstring")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
