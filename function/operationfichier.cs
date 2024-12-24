using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace OperationFichier
{
    public class OperationFichier
    {
        private readonly ILogger<OperationFichier> _logger;

        public OperationFichier(ILogger<OperationFichier> logger)
        {
            _logger = logger;
        }

        [Function(nameof(OperationFichier))]
        public async Task Run(
            [ServiceBusTrigger("devoirmessagequeue", Connection = "servicebusconnectionstring")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body.ToString());
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}