using FitnessTracker.Business.Abstractions.Files;
using FitnessTracker.Business.Abstractions.Files.ImageUploading;
using FitnessTracker.Infrastructure.Files;
using Moq;

namespace FitnessTacker.Tests.InfrastructureTests.Files;

public class ImageUploadingServiceTests
{
    private readonly Mock<IImageUploader> _uploaderMock;
        private readonly ImageUploadingService _service;
        public ImageUploadingServiceTests()
        {
            _uploaderMock = new Mock<IImageUploader>();
            _service = new ImageUploadingService(_uploaderMock.Object);
        }

        [Fact]
        public async Task UploadImageAsync_NullFiles_ReturnsEmptyList()
        {
            var result = await _service.UploadImageAsync(null, "anydir");
            
            Assert.Empty(result);
            _uploaderMock.Verify(u => u.UploadImageAsync(It.IsAny<IFormFileAdapter>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UploadImageAsync_FilesProvided_CallsUploaderAndReturnsPaths()
        {
            var file1 = new Mock<IFormFileAdapter>().Object;
            var file2 = new Mock<IFormFileAdapter>().Object;
            var dir = "uploads";
            _uploaderMock
                .Setup(u => u.UploadImageAsync(file1, dir))
                .ReturnsAsync("path1");
            _uploaderMock
                .Setup(u => u.UploadImageAsync(file2, dir))
                .ReturnsAsync("path2");
            
            var result = await _service.UploadImageAsync(new[] { file1, file2 }, dir);
            
            Assert.Equal(new List<string> { "path1", "path2" }, result);
            _uploaderMock.Verify(u => u.UploadImageAsync(file1, dir), Times.Once);
            _uploaderMock.Verify(u => u.UploadImageAsync(file2, dir), Times.Once);
        }
}