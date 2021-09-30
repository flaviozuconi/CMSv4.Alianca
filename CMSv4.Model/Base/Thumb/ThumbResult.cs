using System.Net;

namespace CMSv4.Model
{
    public class ThumbResult
    {
        public HttpStatusCode StatusCode { get; set; }

        public string ContentType { get; set; }

        public string File { get; set; }
    }
}
