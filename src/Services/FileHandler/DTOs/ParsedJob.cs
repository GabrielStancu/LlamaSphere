using System.Text.Json.Serialization;

namespace FileHandler.DTOs;

public class ParsedJob
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }
}
