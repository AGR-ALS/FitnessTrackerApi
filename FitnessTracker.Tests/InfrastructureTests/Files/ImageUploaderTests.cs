using FitnessTracker.Business.Abstractions.Files;
using FitnessTracker.Infrastructure.Files;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace FitnessTacker.Tests.InfrastructureTests.Files;

public class ImageUploaderTests
{
    private readonly string _tempRoot;
        private readonly Mock<IWebHostEnvironment> _envMock;
        private readonly ImageUploader _uploader;

        public ImageUploaderTests()
        {
            _tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempRoot);

            _envMock = new Mock<IWebHostEnvironment>();
            _envMock.Setup(e => e.WebRootPath).Returns(_tempRoot);

            _uploader = new ImageUploader(_envMock.Object);
        }

        [Fact]
        public async Task UploadImageAsync_NullFile_ReturnsNull()
        {
            var result = await _uploader.UploadImageAsync(null, "uploads");
            
            Assert.Null(result);
        }

        [Fact]
        public async Task UploadImageAsync_NoWebRoot_ThrowsNullReferenceException()
        {
            var envNull = new Mock<IWebHostEnvironment>();
            envNull.Setup(e => e.WebRootPath).Throws<NullReferenceException>();
            var uploaderNull = new ImageUploader(envNull.Object);
            var fileMock = new Mock<IFormFileAdapter>();
            
            var ex = await Assert.ThrowsAsync<NullReferenceException>(
                () => uploaderNull.UploadImageAsync(fileMock.Object, "uploads")
            );
            Assert.Contains("No web root folder was configured", ex.Message);
        }

        [Fact]
        public async Task UploadImageAsync_ValidFile_WritesToDiskAndReturnsPath()
        {
            var fileMock = new Mock<IFormFileAdapter>();
            fileMock.Setup(f => f.FileName).Returns("test.png");
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>()))
                    .Returns<Stream>(stream =>
                    {
                        var buffer = new byte[] { 0x1 };
                        return stream.WriteAsync(buffer, 0, buffer.Length);
                    });
            
            var resultPath = await _uploader.UploadImageAsync(fileMock.Object, "uploads");
            
            Assert.NotNull(resultPath);
            Assert.StartsWith(_tempRoot, resultPath);
            Assert.True(File.Exists(resultPath), "Файл должен существовать на диске");
            
            var bytes = await File.ReadAllBytesAsync(resultPath);
            Assert.Single(bytes);
            Assert.Equal(0x1, bytes[0]);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempRoot))
            {
                Directory.Delete(_tempRoot, recursive: true);
            }
        }
}