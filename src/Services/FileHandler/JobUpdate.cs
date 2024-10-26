using DocumentFormat.OpenXml.Packaging;
using FileHandler.Models;
using FileHandler.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FileHandler
{
    public class JobUpdate
    {
        private readonly IFileParserService _fileParserService;
        private readonly IJobsTableStorageService _jobsTableStorageService;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<JobUpdate> _logger;

        public JobUpdate(IFileParserService fileParserService,
            IJobsTableStorageService jobsTableStorageService,
            [FromKeyedServices("alert")] IEmailSender emailSender,
            ILogger<JobUpdate> logger)
        {
            _fileParserService = fileParserService;
            _jobsTableStorageService = jobsTableStorageService;
            _emailSender = emailSender;
            _logger = logger;
        }

        [Function(nameof(JobUpdate))]
        public async Task Run([BlobTrigger("job/{name}", Connection = "StorageAccount:ConnectionString")] Stream stream, string file)
        {
            try
            {
                _logger.LogInformation("Parsing file {File}", file);

                var fileContent = ExtractFileContentAsync(stream);
                var parsedJob = await _fileParserService.ParseJobFileAsync(fileContent);

                await _jobsTableStorageService.UpdateTableStorageContent(parsedJob);
                await _emailSender.SendEmailAlertAsync(new NewCvEmailModel
                {
                    Name = parsedJob.Title
                });
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
