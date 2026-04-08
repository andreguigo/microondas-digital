using System.Text;

namespace Microondas.Api.Logging;

public interface IExceptionTextLogger
{
    Task LogAsync(Exception exception, HttpContext context);
}

public class ExceptionTextLogger : IExceptionTextLogger
{
    private readonly string _filePath;

    public ExceptionTextLogger(IHostEnvironment environment)
    {
        var folder = Path.Combine(environment.ContentRootPath, "logs");
        Directory.CreateDirectory(folder);
        _filePath = Path.Combine(folder, "exceptions.txt");
    }

    public async Task LogAsync(Exception exception, HttpContext context)
    {
        var sb = new StringBuilder();
        sb.AppendLine("================ EXCEPTION ================");
        sb.AppendLine($"DataUtc: {DateTime.UtcNow:O}");
        sb.AppendLine($"TraceIdentifier: {context.TraceIdentifier}");
        sb.AppendLine($"Request: {context.Request.Method} {context.Request.Path}");
        sb.AppendLine($"QueryString: {context.Request.QueryString}");
        sb.AppendLine($"ExceptionType: {exception.GetType().FullName}");
        sb.AppendLine($"Message: {exception.Message}");
        sb.AppendLine($"InnerException: {exception.InnerException}");
        sb.AppendLine($"StackTrace: {exception.StackTrace}");
        sb.AppendLine();

        await File.AppendAllTextAsync(_filePath, sb.ToString());
    }
}