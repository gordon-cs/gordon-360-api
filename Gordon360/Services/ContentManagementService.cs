using Gordon360.Models.Gordon360.Context;
using Gordon360.Models.Gordon360;
using Gordon360.Models.ViewModels;
using Gordon360.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gordon360.Services;

/// <summary>
/// Service class that facilitates data (specifically, site content) passing between the ContentManagementController and the database model.
/// </summary>
public class ContentManagementService(Gordon360Context context) : IContentManagementService
{
    private readonly string SlideUploadPath = "browseable/slider";

    /// <summary>
    /// Retrieve all banner slides from the database
    /// </summary>
    /// <returns>An IEnumerable of the slides in the database</returns>
    public IEnumerable<Slider_Images> GetBannerSlides() => context.Slider_Images.AsEnumerable();

    /// <summary>
    /// Inserts a banner slide in the database and uploads the image to the local slider folder
    /// </summary>
    /// <param name="slide">The slide to add</param>
    /// <param name="serverURL">The url of the server that the image is being posted to.
    /// This is needed to save the image path into the database. The URL is different depending on where the API is running.
    /// The URL is trivial to access from the controller, but not available from within the service, so it has to be passed in.
    /// </param>
    /// <param name="contentRootPath">The path to the root of the web server's content, from which we can access the physical filepath where slides are uploaded.</param>
    /// <returns>The inserted slide</returns>
    public Slider_Images AddBannerSlide(BannerSlidePostViewModel slide, string serverURL, string contentRootPath)
    {
        var (extension, format, data) = ImageUtils.GetImageFormat(slide.ImageData);
        string fileName = $"{slide.Title}.{extension}";
        string localImagePath = Path.Combine(contentRootPath, SlideUploadPath, fileName);
        ImageUtils.UploadImage(localImagePath, data, format);

        var entry = new Slider_Images
        {
            Path = $"{serverURL}/{SlideUploadPath}/{fileName}",
            Title = slide.Title,
            LinkURL = slide.LinkURL,
            SortOrder = slide.SortOrder,
            Width = 1500,
            Height = 600
        };
        context.Slider_Images.Add(entry);
        context.SaveChanges();
        return entry;
    }

    /// <summary>
    /// Deletes a banner slide from the database and deletes the local image file
    /// </summary>
    /// <returns>The deleted slide</returns>
    public Slider_Images DeleteBannerSlide(int slideID)
    {
        var slide = context.Slider_Images.Find(slideID);
        ImageUtils.DeleteImage(slide.Path);
        context.Slider_Images.Remove(slide);
        context.SaveChanges();
        return slide;
    }
}