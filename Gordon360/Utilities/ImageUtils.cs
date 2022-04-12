﻿using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Threading.Tasks;
using System;

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
                File.Delete(imagePath);
            }
            catch (System.Exception e)
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
        public static string RetrieveImageFromPath(string imagePath)
        {
            string imageData = null;

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
        public static void UploadImage(string imagePath, string imageData, ImageFormat format = null)
        {
            if (imageData == null) { return; }

            if (!Directory.Exists(Path.GetDirectoryName(imagePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(imagePath));
            }

            byte[] imageDataArray = System.Convert.FromBase64String(imageData);

            try
            {
                //Load the image data into a memory stream and save it to
                //the appropriate file:
                using (MemoryStream imageStream = new MemoryStream(imageDataArray))
                {
                    Image image = Image.FromStream(imageStream);

                    System.Diagnostics.Debug.WriteLine(imagePath);
                    System.Diagnostics.Debug.WriteLine(image);

                    image.Save(imagePath, format ?? ImageFormat.Jpeg);
                    return;//Saving image was successful
                }
            }

            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine("Something went wrong trying to save an image to " + imagePath);
            }
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

        /// <summary>
        /// Takes a filepath for an image, navigates to it, collects the raw data
        /// of the file and converts it to base64 format. 
        /// </summary>
        /// <param name="imagePath">The path to the image</param>
        /// <returns>The base64 content of the image</returns>
        private static string GetBase64ImageDataFromPath(string imagePath)
        {
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath))
            using (MemoryStream data = new MemoryStream())
            {
                image.Save(data, image.RawFormat);
                byte[] imageBytes = data.ToArray();
                string base64Data = System.Convert.ToBase64String(imageBytes);
                return base64Data;
            }
        }
    }
}