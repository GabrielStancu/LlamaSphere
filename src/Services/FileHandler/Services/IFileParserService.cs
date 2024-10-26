using FileHandler.DTOs;

namespace FileHandler.Services;

public interface IFileParserService
{
    Task<ParsedCv> ParseCvFileAsync(string content);
    Task<ParsedJob> ParseJobFileAsync(string content);
}