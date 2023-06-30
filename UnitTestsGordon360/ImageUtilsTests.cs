using Gordon360.Exceptions;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Http;
using System.Drawing.Imaging;
using Xunit;

public class ImageUtilsTests
{
    [Fact]
    public void DeleteImage_ExistingImage_DeletesImage()
    {
        // Arrange
        string imagePath = "path/to/image.jpg";
        File.Create(imagePath).Dispose();

        // Act
        ImageUtils.DeleteImage(imagePath);

        // Assert
        Assert.False(File.Exists(imagePath));
    }

    [Fact]
    public void DeleteImage_NonExistingImage_NoExceptionThrown()
    {
        // Arrange
        string imagePath = "non/existing/path.jpg";

        // Act & Assert
        Assert.Throws<Exception>(() => ImageUtils.DeleteImage(imagePath));
    }

    [Fact]
    public void RetrieveImageFromPath_ExistingImagePath_ReturnsBase64Data()
    {
        // Arrange
        string imagePath = "path/to/image.jpg";
        File.Create(imagePath).Dispose();

        // Act
        string imageData = ImageUtils.RetrieveImageFromPath(imagePath);

        // Assert
        Assert.NotNull(imageData);
        Assert.NotEmpty(imageData);
    }

    [Fact]
    public void RetrieveImageFromPath_NullImagePath_ReturnsEmptyString()
    {
        // Act
        string imageData = ImageUtils.RetrieveImageFromPath(null);

        // Assert
        Assert.Equal(string.Empty, imageData);
    }

    [Fact]
    public void UploadImage_ValidData_ImageSaved()
    {
        // Arrange
        string imagePath = "path/to/save/image.jpg";
        string imageData = "base64-encoded-image-data";

        // Act
        ImageUtils.UploadImage(imagePath, imageData, ImageFormat.Jpeg);

        // Assert
        Assert.True(File.Exists(imagePath));
    }

    [Fact]
    public async Task UploadImageAsync_ValidData_ImageSaved()
    {
        // Arrange
        string imagePath = "path/to/save/image.jpg";
        var imageData = new FormFile(new MemoryStream(new byte[] { 0x12, 0x34, 0x56 }), 0, 3, "imageData", "image.jpg");

        // Act
        ImageUtils.UploadImageAsync(imagePath, imageData);

        // Assert
        await Task.Delay(100); // Delay to ensure the image is saved
        Assert.True(File.Exists(imagePath));
    }

    [Fact]
    public async Task DownloadImageFromURL_ValidURL_ReturnsBase64Data()
    {
        // Arrange
        string imageUrl = "https://example.com/image.jpg";

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => ImageUtils.DownloadImageFromURL(imageUrl));
    }


    [Fact]
    public async Task GetImageFromPathOrDefaultFromURL_ImagePathExists_ReturnsImageData()
    {
        // Arrange
        string imagePath = "path/to/existing/image.jpg";
        File.Create(imagePath).Dispose();
        string defaultURL = "https://example.com/default.jpg";

        // Act
        string imageData = await ImageUtils.GetImageFromPathOrDefaultFromURL(imagePath, defaultURL);

        // Assert
        Assert.NotNull(imageData);
        Assert.NotEmpty(imageData);
    }

    [Fact]
    public async Task GetImageFromPathOrDefaultFromURL_ImagePathDoesNotExist_ReturnsDefaultURLImageData()
    {
        // Arrange
        string imagePath = "path/to/non-existing/image.jpg";
        string defaultURL = "https://example.com/default.jpg";

        // Act
        string imageData = await ImageUtils.GetImageFromPathOrDefaultFromURL(imagePath, defaultURL);

        // Assert
        Assert.NotNull(imageData);
        Assert.NotEmpty(imageData);
    }

    [Fact]
    public void GetImageFormat_ValidImage_ReturnsFormat()
    {
        // Arrange
        var image = new FormFile(new MemoryStream(new byte[] { 0x12, 0x34, 0x56 }), 0, 3, "imageData", "image.jpg");

        // Act
        var (formatExtension, format) = ImageUtils.GetImageFormat(image);

        // Assert
        Assert.Equal("jpg", formatExtension);
        Assert.Equal(ImageFormat.Jpeg, format);
    }

    [Fact]
    public void GetImageFormat_InvalidImage_ThrowsBadInputException()
    {
        // Arrange
        var image = new FormFile(new MemoryStream(new byte[] { 0x12, 0x34, 0x56 }), 0, 3, "imageData", "image.txt");

        // Act & Assert
        Assert.Throws<BadInputException>(() => ImageUtils.GetImageFormat(image));
    }

    [Fact]
    public void GetImageFormat_Base64Image_ReturnsFormat()
    {
        // Arrange
        string base64Image = "data:image/jpeg;base64,base64-encoded-image-data";

        // Act
        var (formatExtension, format, imageData) = ImageUtils.GetImageFormat(base64Image);

        // Assert
        Assert.Equal("jpeg", formatExtension);
        Assert.Equal(ImageFormat.Jpeg, format);
        Assert.Equal("base64-encoded-image-data", imageData);
    }

    [Fact]
    public void GetImageFormat_InvalidBase64Image_ThrowsBadInputException()
    {
        // Arrange
        string base64Image = "data:text/plain;base64,base64-encoded-data";

        // Act & Assert
        Assert.Throws<BadInputException>(() => ImageUtils.GetImageFormat(base64Image));
    }
}
