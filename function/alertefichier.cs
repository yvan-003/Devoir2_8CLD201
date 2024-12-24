using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;

namespace alertefichier
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;  // Déclaration du logger pour enregistrer des informations sur l'exécution
        private readonly ServiceBusClient _serviceBusClient;  // Client pour se connecter à Azure Service Bus
        private readonly ServiceBusSender _serviceBusSender;  // Permet d'envoyer des messages à la queue Azure Service Bus

        // Constructeur qui initialise le client et le sender pour Azure Service Bus
        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;  // Initialisation du logger
            _serviceBusClient = new ServiceBusClient("servicebusconnectionstring");  // Connexion à Service Bus avec la chaîne de connexion
            _serviceBusSender = _serviceBusClient.CreateSender("devoirmessagequeue");  // Création d'un sender pour la queue spécifiée
        }

        // Définition de la fonction qui sera exécutée lorsque le trigger Blob est activé
        [Function(nameof(Function1))]
        public async Task Run([BlobTrigger("blob-fichiers-images/{name}", Connection = "blobconnectionstring")] Stream stream, string name)
        {
            // Lecture du contenu du fichier blob
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();  // Lecture du contenu du blob en entier
            _logger.LogInformation($"C# Blob trigger function processed blob\n Name: {name} \n Data: {content}");  // Enregistrement des informations sur le blob traité

            // Création d'un message à envoyer à la queue Azure Service Bus contenant le nom du fichier
            var message = new ServiceBusMessage(name);
            await _serviceBusSender.SendMessageAsync(message);  // Envoi du message à la queue
            _logger.LogInformation($"Message sent to Service Bus queue: {name}");  // Enregistrement d'un message d'information sur l'envoi du message
        }
    }
}
