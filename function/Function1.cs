using System.IO;  // Utilisation de la bibliothèque pour manipuler les flux de fichiers
using System.Threading.Tasks;  // Pour les opérations asynchrones
using Microsoft.Azure.Functions.Worker;  // Pour la gestion des fonctions Azure
using Microsoft.Extensions.Logging;  // Pour l'enregistrement des logs
using Azure.Messaging.ServiceBus;  // Pour l'envoi de messages à Azure Service Bus

namespace function
{
    // Classe principale contenant la fonction Azure
    public class Function1
    {
        private readonly ILogger<Function1> _logger;  // Interface pour le logging
        private readonly ServiceBusClient _serviceBusClient;  // Client pour se connecter à Azure Service Bus
        private readonly ServiceBusSender _serviceBusSender;  // Sender pour envoyer des messages à une queue Service Bus

        // Constructeur qui initialise les dépendances via l'injection de dépendance
        public Function1(ILogger<Function1> logger, ServiceBusClient serviceBusClient)
        {
            _logger = logger;  // Initialisation du logger
            _serviceBusClient = serviceBusClient;  // Initialisation du client Service Bus
            // Création du sender pour une queue spécifique
            _serviceBusSender = _serviceBusClient.CreateSender("your-queue-name"); // Remplacez par le nom de votre queue
        }

        // Définition de la fonction Azure qui sera déclenchée par un événement Blob
        [Function(nameof(Function1))]  // Déclaration du nom de la fonction pour Azure
        public async Task Run(
            [BlobTrigger("blob-fichiers-images/{name}", Connection = "blobconnectionstring")] Stream stream,  // Déclencheur Blob, avec le nom du fichier capturé dans 'name'
            string name  // Le nom du fichier Blob
        )
        {
            // Création d'un StreamReader pour lire le contenu du fichier Blob
            using var blobStreamReader = new StreamReader(stream);
            // Lecture asynchrone du contenu du fichier Blob
            var content = await blobStreamReader.ReadToEndAsync();
            // Enregistrement d'un log avec le nom du fichier et son contenu (attention à ne pas loguer des informations sensibles dans un environnement de production)
            _logger.LogInformation($"C# Blob trigger function processed blob\n Name: {name} \n Data: {content}");

            // Création d'un message pour Azure Service Bus avec le nom du fichier
            var message = new ServiceBusMessage(name);
            // Envoi du message au Service Bus de manière asynchrone
            await _serviceBusSender.SendMessageAsync(message);
            // Enregistrement d'un log pour confirmer l'envoi du message à la queue Service Bus
            _logger.LogInformation($"Message sent to Service Bus queue: {name}");
        }
    }
}
