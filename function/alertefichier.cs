using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;

namespace alertefichier
{
    public class alertefichier
    {
        private readonly ILogger<alertefichier> _logger;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusSender _serviceBusSender;

        public Function1(ILogger<alertefichier> logger)
        {
            _logger = logger;

            // Récupérer la chaîne de connexion depuis les variables d'environnement
            string serviceBusConnectionString = Environment.GetEnvironmentVariable("servicebusconnectionstring");
            _serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
            _serviceBusSender = _serviceBusClient.CreateSender("devoirmessagequeue");
        }

        [Function(nameof(alertefichier))]
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