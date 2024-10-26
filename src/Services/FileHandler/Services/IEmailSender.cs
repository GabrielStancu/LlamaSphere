using FileHandler.Models;

namespace FileHandler.Services;

public interface IEmailSender
{
    Task SendEmailAlertAsync(NewAlertEmailModel emailAlert);
}