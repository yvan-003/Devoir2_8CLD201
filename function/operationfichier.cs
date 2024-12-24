using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Drawing.Drawing2D;

namespace operationfichier
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Function1))]
        public async Task Run(
            [ServiceBusTrigger("devoirmessagequeue", Connection = "servicebusconnectionstring")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            string blobName = message.Body.ToString();
            _logger.LogInformation($"Processing message for file: {blobName}");

            string blobConnectionString = Environment.GetEnvironmentVariable("blobconnectionstring");
            var blobServiceClient = new BlobServiceClient(blobConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient("blob-fichiers-images");

            try
            {
                // Télécharger le fichier source
                var blobClient = containerClient.GetBlobClient(blobName);

                // Vérifier si le blob existe
                if (await blobClient.ExistsAsync())
                {
                    // Télécharger dans un flux local
                    using var memoryStream = new MemoryStream();
                    await blobClient.DownloadToAsync(memoryStream);
                    memoryStream.Position = 0;

                    // Vérifier si le fichier est une image
                    if (IsImage(blobName))
                    {
                        // Traiter l'image (redimensionner)
                        using var processedStream = ResizeImage(memoryStream, 800, 600); // Redimensionner à 800x600

                        // Créer un nouveau nom de blob avec "modified" en préfixe
                        string modifiedBlobName = "modified_" + blobName;

                        // Télécharger l'image traitée dans le même container avec le nouveau nom
                        var modifiedBlobClient = containerClient.GetBlobClient(modifiedBlobName);
                        await modifiedBlobClient.UploadAsync(processedStream, overwrite: true);
                        _logger.LogInformation($"File resized and uploaded back to the container as: {modifiedBlobName}");

                        // Supprimer le fichier original après l'avoir traité
                        await blobClient.DeleteIfExistsAsync();
                        _logger.LogInformation($"Original file {blobName} deleted after processing.");
                    }
                    else
                    {
                        _logger.LogWarning($"The file {blobName} is not an image. Skipping processing.");
                        // Optionnellement, vous pouvez choisir de télécharger le fichier original
                        await blobClient.UploadAsync(memoryStream, overwrite: true);
                    }

                    // Compléter le message
                    await messageActions.CompleteMessageAsync(message);
                    _logger.LogInformation($"Message completed: {blobName}");
                }
                else
                {
                    _logger.LogWarning($"The file {blobName} does not exist in the source container.");
                    await messageActions.AbandonMessageAsync(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing file {blobName}: {ex.Message}");
                await messageActions.AbandonMessageAsync(message);
            }
        }

        // Méthode pour vérifier si le fichier est une image en fonction de l'extension
        private bool IsImage(string fileName)
        {
            // Vérifier l'extension du fichier pour déterminer si c'est une image
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif" || extension == ".bmp";
        }

        // Méthode pour redimensionner une image
        private Stream ResizeImage(Stream inputStream, int width, int height)
        {
            using var originalImage = Image.FromStream(inputStream);
            var resizedImage = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(resizedImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(originalImage, 0, 0, width, height);
            }

            var outputStream = new MemoryStream();
            resizedImage.Save(outputStream, ImageFormat.Jpeg); // Sauvegarder en JPEG ou changer selon le format désiré
            outputStream.Position = 0; // Réinitialiser la position du flux pour la lecture
            return outputStream;
        }
    }
}
