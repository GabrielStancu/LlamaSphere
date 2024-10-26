namespace FileHandler.Models;

public class NewCvEmailModel : NewAlertEmailModel
{
    public new string AlertType => "Candidate";
    public new string Recipient => "biavulsan@yahoo.com";
}
