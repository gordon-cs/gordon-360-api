using System.Net.Http;

namespace Gordon360.Providers
{
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path) : base(path)
        { }

        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            string name;
            if (string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName))
            {
                name = "";
            }
            else
            {
                name = headers.ContentDisposition.FileName;
            }
            return name.Replace("\"", string.Empty); ;
        }
    }
}
