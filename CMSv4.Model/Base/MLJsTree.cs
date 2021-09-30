namespace CMSv4.Model
{
    public class MLJsTree
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public string position { get; set; }
        public string icon { get; set; }
    }

    public class MLJsTreePaginas
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public string position { get; set; }

        //public MLJsTreePaginas[] children { get; set; }
    }
}
