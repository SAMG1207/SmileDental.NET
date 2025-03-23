using Azure.Core;
using Serilog;
using SmileDental.DTOs.Dentista;

namespace SmileDental.Utils
{
    public class FileHandler
    {
        // Usamos una ruta fuera de wwwroot para los archivos no estáticos
        private static readonly string BasePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        private static readonly HashSet<string> AllowedImageExtensions = new() { ".jpg", ".png" };
        private static readonly string PdfFolder = Path.Combine(BasePath, "Historias");
        private static readonly string ImageFolder = Path.Combine(BasePath, "Imagenes");

        public static async Task<FileUploadResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                Log.Warning("El fichero está vacío.");
                return new FileUploadResult { Success = false, Message = "El fichero está vacío." };
            }

            string fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!IsValidExtension(fileExtension))
            {
                Log.Warning($"Formato de archivo no válido: {fileExtension}");
                return new FileUploadResult { Success = false, Message = $"Formato de archivo no válido: {fileExtension}" };
            }

            string destinationFolder = GetDestinationFolder(fileExtension);
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            string fileName = $"{Guid.NewGuid()}{fileExtension}";
            string filePath = Path.Combine(destinationFolder, fileName);

            try
            {
                using FileStream stream = new(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
                Log.Information($"Archivo guardado en {filePath}");
                return new FileUploadResult { Success = true, FileName = fileName };
            }
            catch (IOException ex)
            {
                Log.Error($"Error al guardar archivo: {ex.Message}");
                return new FileUploadResult { Success = false, Message = $"Error al guardar archivo: {ex.Message}", FileName = filePath };
            }
        }

        private static bool IsValidExtension(string extension)
        {
            return extension == ".pdf" || extension == ".jpg" || extension == ".png";
        }

        private static string GetDestinationFolder(string extension)
        {
            if (extension == ".pdf")
            {
                return PdfFolder;
            }
            else if(extension == ".jpg" || extension == ".png")
            {
                return ImageFolder;
            }
            else
            {
                throw new ArgumentException("Extensión de archivo no válida");
            }
        }
    }
}
