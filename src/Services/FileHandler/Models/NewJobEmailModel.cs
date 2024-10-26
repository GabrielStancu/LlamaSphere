namespace FileHandler.Models;
public class NewJobEmailModel : NewAlertEmailModel
{
    public new string AlertType => "Job";
    public new string Recipient => "gabriel.stancu07@yahoo.com";
}
