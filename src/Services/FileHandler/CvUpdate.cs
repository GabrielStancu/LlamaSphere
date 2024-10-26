using System.Text.Json;
using DocumentFormat.OpenXml.Packaging;
using FileHandler.DTOs;
using FileHandler.Entities;
using FileHandler.Models;
using FileHandler.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FileHandler
{
    public class CvUpdate
    {
        private readonly IFileParserService _fileParserService;
        private readonly ICvTableStorageService _cvTableStorageService;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<CvUpdate> _logger;

        public CvUpdate(IFileParserService fileParserService,
            ICvTableStorageService cvTableStorageService,
            [FromKeyedServices("alert")] IEmailSender emailSender,
            ILogger<CvUpdate> logger)
        {
            _fileParserService = fileParserService;
            _cvTableStorageService = cvTableStorageService;
            _emailSender = emailSender;
            _logger = logger;
        }

        [Function(nameof(CvUpdate))]
        public async Task Run([BlobTrigger("cvs/{file}", Connection = "StorageAccount:ConnectionString")] Stream stream, string file)
        {
            try
            {
                _logger.LogInformation("Parsing file {File}", file);

                var fileContent = ExtractFileContentAsync(stream);
                var parsedCv = new ParsedCv
                {
                    FirstName = "Gabriel",
                    LastName = "Stancu",
                    Id = file.Substring(0, file.IndexOf("_", StringComparison.Ordinal))
                }; //await _fileParserService.ParseCvFileAsync(fileContent);

                await _cvTableStorageService.UpdateTableStorageContent(parsedCv);
                var emailModel = new NewCvEmailModel
                {
                    Name = $"{parsedCv.FirstName} {parsedCv.LastName}"
                };
                await _emailSender.SendEmailAlertAsync(emailModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing blob. Exception: {ex.Message}");
            }
        }

        private static string ExtractFileContentAsync(Stream stream)
        {
            using var wordDoc = WordprocessingDocument.Open(stream, false);
            var body = wordDoc.MainDocumentPart?.Document.Body;
            var content = body?.InnerText ?? string.Empty;

            return content;
        }
    }
}
