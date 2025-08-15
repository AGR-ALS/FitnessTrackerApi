using FitnessTracker.Business.Exceptions;
using FitnessTracker.Infrastructure.Files;

namespace FitnessTacker.Tests.InfrastructureTests.Files;

public class FileDeletingServiceTests
{
    private readonly string _tempDir;
        private readonly string _existingFile;
        private readonly FileDeletingService _service;

        public FileDeletingServiceTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);
            _existingFile = Path.Combine(_tempDir, "test.txt");
            File.WriteAllText(_existingFile, "content");

            _service = new FileDeletingService();
        }

        [Fact]
        public async Task DeleteFileAsync_ExistingFile_DeletesFile()
        {
            var files = new[] { _existingFile };
            
            await _service.DeleteFileAsync(new List<string>(files));
            
            Assert.False(File.Exists(_existingFile));
        }

        [Fact]
        public async Task DeleteFileAsync_NonExistingFile_ThrowsNotFoundException()
        {
            var missingPath = Path.Combine(_tempDir, "missing.txt");
            var files = new[] { missingPath };
            
            var ex = await Assert.ThrowsAsync<NotFoundException>(
                () => _service.DeleteFileAsync(new List<string>(files))
            );
            Assert.Contains("Files was not found", ex.Message);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, recursive: true);
            }
        }
}