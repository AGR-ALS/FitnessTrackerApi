using FitnessTracker.Business.Abstractions.Files;
using Microsoft.AspNetCore.Http;

namespace FitnessTracker.Infrastructure.Files;

public class FormFileAdapter : IFormFileAdapter
{
    private readonly IFormFile _formFile;

    public FormFileAdapter(IFormFile formFile)
    {
        _formFile = formFile;
    }

    public string FileName => _formFile.FileName;

    public Task CopyToAsync(Stream target)
    {
        return _formFile.CopyToAsync(target);
    }
}