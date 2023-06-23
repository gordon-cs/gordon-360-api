using Gordon360.Exceptions;
using Microsoft.AspNetCore.Http;
// using Moq;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Gordon360.Utilities.Tests
{
    public class ImageUtilsTests
    {
        private const string TestImagePath = "path/to/test/image.jpg";
        private const string TestImageData = "base64-encoded-image-data";
        private const string NonExistingImagePath = "path/to/non/existing/image.jpg";
        private const string DefaultImageURL = "https://example.com/default-image.jpg";

        [Fact]
        public void DeleteImage_ShouldDeleteImageFromPath()
        {
            // Arrange
            File.Create(TestImagePath).Dispose(); // Create a test image file

            // Act
            ImageUtils.DeleteImage(TestImagePath);

            // Assert
            Assert.False(File.Exists(TestImagePath));
        }

        [Fact]
        public void RetrieveImageFromPath_WithValidImagePath_ShouldReturnImageData()
        {
            // Arrange
            File.Create(TestImagePath).Dispose(); // Create a test image file

            // Act
            string imageData = ImageUtils.RetrieveImageFromPath(TestImagePath);

            // Assert
            Assert.Equal(TestImageData, imageData);
        }

        [Fact]
        public void RetrieveImageFromPath_WithNullImagePath_ShouldReturnEmptyString()
        {
            // Act
            string imageData = ImageUtils.RetrieveImageFromPath(null);

            // Assert
            Assert.Equal(string.Empty, imageData);
        }

        [Fact]
        public void UploadImage_WithValidImageData_ShouldSaveImage()
        {
            // Act
            ImageUtils.UploadImage(TestImagePath, TestImageData, ImageFormat.Jpeg);

            // Assert
            Assert.True(File.Exists(TestImagePath));
        }

        [Fact]
        public async Task UploadImageAsync_WithValidImageFile_ShouldSaveImage()
        {
            // Arrange
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            writer.Write(TestImageData);
            writer.Flush();
            stream.Position = 0;
            var formFile = new FormFile(stream, 0, stream.Length, "image", "image.jpg");

            // Act
            ImageUtils.UploadImageAsync(TestImagePath, formFile);

            // Assert
            await Task.Delay(500); // Wait for the file to be saved asynchronously
            Assert.True(File.Exists(TestImagePath));
        }

        /*
        [Fact]
        public async Task DownloadImageFromURL_WithValidURL_ShouldReturnImageData()
        {
            // Arrange
            string testImageURL = "https://example.com/image.jpg";
            var httpClientMock = new Mock<HttpClientWrapper>();
            httpClientMock.Setup(m => m.GetAsync(testImageURL))
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(TestImageData)
                });

            // Act
            string imageData = await ImageUtils.DownloadImageFromURL(testImageURL, httpClientMock.Object);

            // Assert
            Assert.Equal(TestImageData, imageData);
        }
        */

        [Fact]
        public async Task GetImageFromPathOrDefaultFromURL_WithExistingImagePath_ShouldRetrieveImage()
        {
            // Arrange
            File.Create(TestImagePath).Dispose(); // Create a test image file

            // Act
            string imageData = await ImageUtils.GetImageFromPathOrDefaultFromURL(TestImagePath, DefaultImageURL);

            // Assert
            Assert.Equal(TestImageData, imageData);
        }

        /*
        [Fact]
        public async Task GetImageFromPathOrDefaultFromURL_WithNonExistingImagePath_ShouldDownloadDefaultImage()
        {
            // Arrange
            var httpClientMock = new Mock<HttpClientWrapper>();
            httpClientMock.Setup(m => m.GetAsync(DefaultImageURL))
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(TestImageData)
                });

            // Act
            string imageData = await ImageUtils.GetImageFromPathOrDefaultFromURL(NonExistingImagePath, DefaultImageURL, httpClientMock.Object);

            // Assert
            Assert.Equal(TestImageData, imageData);
        }
        */

        [Fact]
        public void GetImageFormat_WithValidImageFile_ShouldReturnFormatExtensionAndFormat()
        {
            // Arrange
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            writer.Write(TestImageData);
            writer.Flush();
            stream.Position = 0;
            var formFile = new FormFile(stream, 0, stream.Length, "image", "image.jpg");

            // Act
            var (formatExtension, format) = ImageUtils.GetImageFormat(formFile);

            // Assert
            Assert.Equal("jpg", formatExtension);
            Assert.Equal(ImageFormat.Jpeg, format);
        }

        [Fact]
        public void GetImageFormat_WithInvalidImageFile_ShouldThrowBadInputException()
        {
            // Arrange
            using var stream = new MemoryStream();
            var formFile = new FormFile(stream, 0, stream.Length, "image", "image.txt");

            // Act & Assert
            Assert.Throws<BadInputException>(() => ImageUtils.GetImageFormat(formFile));
        }

        [Fact]
        public void GetImageFormat_WithValidBase64Image_ShouldReturnFormatExtensionAndFormat()
        {
            // Arrange
            string base64Image = $"data:image/jpeg;base64,{TestImageData}";

            // Act
            var (formatExtension, format, imageData) = ImageUtils.GetImageFormat(base64Image);

            // Assert
            Assert.Equal("jpeg", formatExtension);
            Assert.Equal(ImageFormat.Jpeg, format);
            Assert.Equal(TestImageData, imageData);
        }

        [Fact]
        public void GetImageFormat_WithInvalidBase64Image_ShouldThrowBadInputException()
        {
            // Arrange
            string base64Image = "invalid-base64-image";

            // Act & Assert
            Assert.Throws<BadInputException>(() => ImageUtils.GetImageFormat(base64Image));
        }
    }
}
