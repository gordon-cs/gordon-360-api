using Gordon360.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gordon360.Utilities
{
    public static class ImageUtils
    {
        /// <summary>
        /// Deletes an image from the filesystem, if there is one.
        /// </summary>
        /// <param name="imagePath">The path to which the image belonged.</param>
        public static void DeleteImage(string imagePath)
        {
            try
            {
                // remove server address from imagePath, otherwise Student News image not found
                string splitSubstring = "browseable";
                string realImagePath = splitSubstring + imagePath.Split(splitSubstring).Last();

                File.Delete(realImagePath);
            }
            catch (Exception e)
            {
                //If there wasn't an image there, the only reason
                //was that no image was associated with the news item,
                //so this catch handles that and there's no cause for concern.
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Takes an image path and returns the data of the image at that path.
        /// If given path is null it returns an empty string.
        /// </summary>
        /// <param name="imagePath">The path to the image</param>
        /// <returns>The base64 data of the image</returns>
        public static string? RetrieveImageFromPath(string imagePath)
        {
            string? imageData = null;

            if (imagePath != null)
            {
                imageData = GetBase64ImageDataFromPath(imagePath);
            }

            return imageData;
        }

        /// <summary>
        /// Uploads a news image
        /// </summary>
        /// <remarks>
        /// Takes base64 image data and writes it to an image file. Note that if the target path
        /// already has a file, the method will overwrite it (which gives no errors)
        /// </remarks>
        /// <param name="imagePath">The path to which the image belongs</param>
        /// <param name="imageData">The base64 image data to be stored</param>
        /// <param name="format">The format to save the image as. Defaults to Jpeg</param>
        public static void UploadImage(string imagePath, string imageData, ImageFormat? format = null)
        {
            if (imageData == null) { return; }

            if (Path.GetDirectoryName(imagePath) is string path && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            byte[] imageDataArray = Convert.FromBase64String(imageData.Split(",").Last());

            try
            {
                //Load the image data into a memory stream and save it to
                //the appropriate file:
                using MemoryStream imageStream = new MemoryStream(imageDataArray);
                Image image = Image.FromStream(imageStream);

                System.Diagnostics.Debug.WriteLine(imagePath);
                System.Diagnostics.Debug.WriteLine(image);

                image.Save(imagePath, format ?? ImageFormat.Jpeg);
                return;//Saving image was successful
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine("Something went wrong trying to save an image to " + imagePath);
            }
        }

        /// <summary>
        /// Uploads image from HTTP FormFile
        /// </summary>
        /// <remarks>
        /// Takes image data and writes it to an image file. Note that if the target path
        /// already has a file, the method will overwrite it (which gives no errors)
        /// </remarks>
        /// <param name="imagePath">The path to which the image belongs</param>
        /// <param name="imageData">The image data to be stored</param>
        public async static void UploadImageAsync(string imagePath, IFormFile imageData)
        {
            if (Path.GetDirectoryName(imagePath) is string path && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using var stream = File.Create(imagePath);
            await imageData.CopyToAsync(stream);
        }

        public static async Task<string> DownloadImageFromURL(string url)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var imageBytes = await response.Content.ReadAsByteArrayAsync();
                return Convert.ToBase64String(imageBytes);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public static async Task<string> GetImageFromPathOrDefaultFromURL(string imagePath, string defaultURL)
        {
            try
            {
                return RetrieveImageFromPath(imagePath);
            }
            catch (FileNotFoundException)
            {
                return await DownloadImageFromURL(defaultURL);
            }
        }

        public static (string formatExtension, ImageFormat format) GetImageFormat(IFormFile image)
        {
            Match match = Regex.Match(image.ContentType, @"^image/(?<filetype>jpeg|jpg|png)$");
            if (!match.Success)
            {
                throw new BadInputException();
            }
            string formatExtension = match.Groups["filetype"].Value;
            var format = formatExtension switch
            {
                "png" => ImageFormat.Png,
                "jpeg" or "jpg" => ImageFormat.Jpeg,
                _ => throw new BadInputException()
            };
            return (formatExtension, format);
        }

        public static (string formatExtension, ImageFormat format, string imageData) GetImageFormat(string base64Image)
        {
            Match match = Regex.Match(base64Image, @"^data:image/(?<filetype>jpeg|jpg|png);base64,");
            if (!match.Success)
            {
                throw new BadInputException();
            }
            string formatExtension = match.Groups["filetype"].Value;
            var format = formatExtension switch
            {
                "png" => ImageFormat.Png,
                "jpeg" or "jpg" => ImageFormat.Jpeg,
                _ => throw new BadInputException()
            };
            string rawImageData = base64Image[match.Length..];
            return (formatExtension, format, rawImageData);
        }

        /// <summary>
        /// Takes a filepath for an image, navigates to it, collects the raw data
        /// of the file and converts it to base64 format. 
        /// </summary>
        /// <param name="imagePath">The path to the image</param>
        /// <returns>The base64 content of the image</returns>
        private static string GetBase64ImageDataFromPath(string imagePath)
        {
            // remove server address from imagePath, otherwise Student News image not found
            string splitSubstring = "browseable";
            string realImagePath = splitSubstring + imagePath.Split(splitSubstring).Last();

            using Image image = Image.FromFile(realImagePath);
            using MemoryStream data = new MemoryStream();
            image.Save(data, image.RawFormat);
            byte[] imageBytes = data.ToArray();
            string base64Data = Convert.ToBase64String(imageBytes);
            return base64Data;
        }
    }
}
