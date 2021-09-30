using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSv4.Model
{
    public class UpdateComments
    {
        public int _total { get; set; }
    }

    public class Company
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Content
    {
        public string description { get; set; }
        public string eyebrowUrl { get; set; }
        public string shortenedUrl { get; set; }
        public string submittedImageUrl { get; set; }
        public string submittedUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string title { get; set; }
    }

    public class Application
    {
        public string name { get; set; }
    }

    public class ServiceProvider
    {
        public string name { get; set; }
    }

    public class Source
    {
        public Source()
        {
            application = new Application();
            serviceProvider = new ServiceProvider();
        }

        public Application application { get; set; }
        public ServiceProvider serviceProvider { get; set; }
        public string serviceProviderShareId { get; set; }
    }

    public class Visibility
    {
        public string code { get; set; }
    }

    public class Share
    {
        public Share()
        {
            content = new Content();
            source = new Source();
            visibility = new Visibility();
        }

        public string comment { get; set; }
        public Content content { get; set; }
        public string id { get; set; }
        public Source source { get; set; }
        public object timestamp { get; set; }
        public Visibility visibility { get; set; }
    }

    public class CompanyStatusUpdate
    {
        public CompanyStatusUpdate()
        {
            share = new Share();
        }

        public Share share { get; set; }
    }

    public class UpdateContent
    {
        public UpdateContent()
        {
            company = new Company();
            companyStatusUpdate = new CompanyStatusUpdate();
        }

        public Company company { get; set; }
        public CompanyStatusUpdate companyStatusUpdate { get; set; }
    }

    public class Value
    {
        public Value()
        {
            updateComments = new UpdateComments();
            updateContent = new UpdateContent();
        }

        public bool isCommentable { get; set; }
        public bool isLikable { get; set; }
        public bool isLiked { get; set; }
        public int numLikes { get; set; }
        public object timestamp { get; set; }
        public UpdateComments updateComments { get; set; }
        public UpdateContent updateContent { get; set; }
        public string updateKey { get; set; }
        public string updateType { get; set; }

        public string updateUrl
        {
            get
            {
                if (string.IsNullOrEmpty(updateKey) || updateKey.IndexOf('-') == -1)
                {
                    return string.Empty;
                }

                //url da página do post
                string url = "https://www.linkedin.com/nhome/updates?topic={0}";
                
                //exemplo json: "updateKey": "UPDATE-c9453-6064447337495830528"
                string topic = updateKey.Substring(updateKey.LastIndexOf('-') + 1);

                return string.Format(url, topic);
            }
        }
    }

    public class MLLinkedIn
    {
        public MLLinkedIn()
        {
            values = new List<Value>();
        }

        public int _count { get; set; }
        public int _start { get; set; }
        public int _total { get; set; }
        public List<Value> values { get; set; }
    }
}
