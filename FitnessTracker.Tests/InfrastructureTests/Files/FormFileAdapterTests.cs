using FitnessTracker.Infrastructure.Files;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FitnessTacker.Tests.InfrastructureTests.Files;

public class FormFileAdapterTests
{
    [Fact]
    public void FileName_ReturnsFileName()
    {
        var mockFormFile = new Mock<IFormFile>();
        mockFormFile.Setup(f => f.FileName).Returns("example.png");
        var adapter = new FormFileAdapter(mockFormFile.Object);
        
        var result = adapter.FileName;
        
        Assert.Equal("example.png", result);
    }

    [Fact]
    public async Task CopyToAsync_InvokesCopyToAsync()
    {
        var mockFormFile = new Mock<IFormFile>();
        var adapter = new FormFileAdapter(mockFormFile.Object);
        var targetStream = new MemoryStream();

        mockFormFile
            .Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
            .Returns<Stream, System.Threading.CancellationToken>((stream, token) =>
            {
                
                return stream.WriteAsync(new byte[] { 0x1 }, 0, 1, token);
            });
        
        await adapter.CopyToAsync(targetStream);
        
        mockFormFile.Verify(f => f.CopyToAsync(targetStream, default), Times.Once);
        
        Assert.Equal(1, targetStream.Length);
    }
}