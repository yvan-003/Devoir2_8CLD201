using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;

namespace alertefichier
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusSender _serviceBusSender;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
            _serviceBusClient = new ServiceBusClient("servicebusconnectionstring");
            _serviceBusSender = _serviceBusClient.CreateSender("devoirmessagequeue");
        }

        [Function(nameof(Function1))]
        public async Task Run([BlobTrigger("blob-fichiers-images/{name}", Connection = "blobconnectionstring")] Stream stream, string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function processed blob\n Name: {name} \n Data: {content}");

            // Envoyer le nom du fichier à la queue Azure Service Bus
            var message = new ServiceBusMessage(name);
            await _serviceBusSender.SendMessageAsync(message);
            _logger.LogInformation($"Message sent to Service Bus queue: {name}");
        }
    }
}