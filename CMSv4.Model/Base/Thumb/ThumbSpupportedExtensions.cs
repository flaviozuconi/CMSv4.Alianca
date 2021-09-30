using System.Collections.Generic;

namespace CMSv4.Model
{
    public class ThumbSpupportedExtensions
    {
        public static readonly Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
        {
            { ".gif", "image/gif" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" }
        };
    }
}
