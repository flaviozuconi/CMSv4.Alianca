using CMSv4.Model.DataTableFilter;
using System.Collections.Specialized;

namespace CMSv4.BusinessLayer.DataTableFilter
{
    public class DataTableFilter
    {
        private NameValueCollection QueryString { get; set; }

        public DataTableFilter(NameValueCollection queryString)
        {
            QueryString = queryString;
        }

        public MLDataTableFilter Get()
        {
            int.TryParse(QueryString["order[0][column]"], out var orderIndex);
            int.TryParse(QueryString["start"], out var start);
            int.TryParse(QueryString["length"], out var length);
            int.TryParse(QueryString["order[0][column]"], out orderIndex);

            var retorno = new MLDataTableFilter()
            {
                SearchedValue = QueryString["search[value]"],
                Start = start,
                Length = length,
                OrderBy = QueryString[$"columns[{orderIndex}][data]"],
                Sort = QueryString["order[0][dir]"]
            };

            return retorno;
        }
    }
}
