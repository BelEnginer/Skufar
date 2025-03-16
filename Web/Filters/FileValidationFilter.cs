using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Filters;

public class FileValidationFilter(ILogger<FileValidationFilter> _logger) : ActionFilterAttribute
{
    private readonly List<string> _validExtensions = [".jpeg", ".jpg", ".png"];
    private const long MaxFileSize = 5 * 1024 * 1024;
    private const int NumberOfFiles = 7;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var files = context.HttpContext.Request.Form.Files;
        if (files is null or { Count: 0 })
        {
            _logger.LogWarning("File upload failed: No files provided.");
            context.Result = new BadRequestObjectResult("Files not found");
            return;
        }

        if (files.Count > NumberOfFiles)
        {
            _logger.LogWarning("Too many files uploaded: {Count}, max allowed is {Max}.", files.Count, NumberOfFiles);
            context.Result = new BadRequestObjectResult("The number of files should not exceed 7");
            return;
        }

        foreach (var file in files)
        {
            var extension = Path.GetExtension(file.FileName)?.ToLower();
            var fileSize = file.Length;
            if (extension is null || !_validExtensions.Contains(extension))
            {
                _logger.LogWarning("File '{FileName}' rejected: Invalid extension '{Extension}'.", file.FileName, extension);
                context.Result = new BadRequestObjectResult($"The file '{file.FileName}' has an incorrect extension");
                return;
            }

            if (fileSize > MaxFileSize)
            {
                _logger.LogWarning("File '{FileName}' rejected: Size {Size} bytes exceeds limit {MaxSize} bytes.", 
                    file.FileName, fileSize, MaxFileSize);
                context.Result = new BadRequestObjectResult($"File '{file.FileName}' is over 5MB");
                return;
            }
        }
    }
}

