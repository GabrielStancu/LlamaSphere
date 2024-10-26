using System.Text.Json.Serialization;

namespace LlamaSphere.API.DTOs;

public class ReasoningResponse
{
    [JsonPropertyName("cv_id")]
    public string CvId { get; set; }

    [JsonPropertyName("domain_reasoning")]
    public string DomainReasoning { get; set; }

    [JsonPropertyName("general_reasoning")]
    public string GeneralReasoning { get; set; }

    [JsonPropertyName("job_id")]
    public int JobId { get; set; }

    [JsonPropertyName("score")]
    public double Score { get; set; }

    [JsonPropertyName("tehnical_reasoning")]
    public string TechnicalReasoning { get; set; }
}
