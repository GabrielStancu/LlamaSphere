using Azure;
using Azure.Communication.Email;
using FileHandler.Configurations;
using FileHandler.Models;
using Microsoft.Extensions.Logging;

namespace FileHandler.Services;
public class AlertEmailSender : IEmailSender
{
    private readonly EmailConfiguration _emailConfiguration;
    private readonly ILogger<AlertEmailSender> _logger;

    public AlertEmailSender(EmailConfiguration emailConfiguration,
        ILogger<AlertEmailSender> logger)
    {
        _emailConfiguration = emailConfiguration;
        _logger = logger;
    }

    public async Task SendEmailAlertAsync(NewAlertEmailModel emailAlert)
    {
        var emailClient = new EmailClient(_emailConfiguration.ConnectionString);
        var emailMessage = FormatAlertEmail(emailAlert);

        try
        {
            await emailClient.SendAsync(WaitUntil.Completed, emailMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError("Could not send the email alert, marking it as failed. Exception: <{Exception}>", ex.Message);
        }
    }

    private EmailMessage FormatAlertEmail(NewAlertEmailModel emailAlert)
    {
        var emailContent = new EmailContent($"New {emailAlert.AlertType}!")
        {
            PlainText = $"A new {emailAlert.AlertType} was found! Head into the application for more details about {emailAlert.Name}"
        };
        var emailMessage = new EmailMessage(_emailConfiguration.FromEmail, emailAlert.Recipient, emailContent);

        return emailMessage;
    }
}
