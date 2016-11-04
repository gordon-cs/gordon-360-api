using System.IO;
using System.Net.Http;

namespace Gordon360.Providers
{
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path) : base(path)
        { }

        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            return headers.ContentDisposition.FileName.Replace("\"", "");
        }
    }
}
