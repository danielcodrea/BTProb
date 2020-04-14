using BTProb.Models.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BTProb.Services
{
    public class FileService
    {
        private readonly FilesSettings _filesSettings;
        public FileService(IConfiguration configuration)
        {
            _filesSettings = configuration.GetSection(nameof(FilesSettings)).Get<FilesSettings>();
        }

        public async Task WriteFileToDisk(IFormFile file, string localFileName, bool isProcessed = false)
        {
            var filePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, isProcessed ? _filesSettings.ProcessedDirectory : _filesSettings.WriteDirectory);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            using (var fileStream = new FileStream(Path.Combine(filePath, localFileName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }

        public async Task<string[]> ReadFile(string fileName)
        {
            var filePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, _filesSettings.WriteDirectory, fileName);

            try
            {
                string[] fileLines = File.ReadAllLines(filePath);
                return fileLines;
            }
            catch (Exception ex)
            {
                Log.Error($"Error reading file {fileName}, path: {filePath}");
                return null;
            }
        }

        public async Task<bool> WriteProcessedFile(string fileName, string[] lines)
        {
            var directoryPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, _filesSettings.ProcessedDirectory);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var filePath = Path.Combine(directoryPath, fileName);

            try
            {
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    foreach (string line in lines)
                    {
                        sw.WriteLine(line);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"File {fileName} was NOT written to Processed Directory");
                return false;
            }
        }
    }
}
