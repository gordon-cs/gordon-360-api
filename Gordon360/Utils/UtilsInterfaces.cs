// <summary>
// Namespace with all the Utils Interfaces that are to be implemented. I don't think making this interface is required, the utilities can work fine on their own.
// However, building the interfaces first does give a general sense of structure to their implementations. A certain cohesiveness :p.
// </summary>
using System.Drawing.Imaging;

namespace Gordon360.Utils
{
    public interface IImageUtils
    {
        string RetrieveImageFromPath(string imagePath);
        void UploadImage(string imagePath, string imageData, ImageFormat format = null);
        void DeleteImage(string imagePath);
    };
}
