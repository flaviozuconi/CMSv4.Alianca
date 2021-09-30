using System;

namespace CMSv4.Model
{
    public class MLFace
    {
        public MLFacePost[] data { get; set; }
    }

    public class MLFacePost
    {
        public string created_time { get; set; }
        public string id { get; set; }
        public string message { get; set; }
        public string link { get; set; }
        public MLFaceFrom from { get; set; }
        public MLFacePost()
        {
            from = new MLFaceFrom();
        }
    }

    public class MLFaceFrom
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class MLTwitter
    {
        public string text { get; set; }
        public string created_at { get; set; }
        public string source { get; set; }
        public string id_str { get; set; }
        public MLTwitterPost user { get; set; }
        public MLTwitter()
        {
            user = new MLTwitterPost();
        }

    }

    public class MLTwitterPost
    {
        public string name { get; set; }
        public string screen_name { get; set; }
    }

    public class MLFeeds
    {
        public DateTime Data { get; set; }
        public string Mensagem { get; set; }
        public string Link { get; set; }
        public string NomeRedeSocial { get; set; }
        public string Class { get; set; }
    }


}
