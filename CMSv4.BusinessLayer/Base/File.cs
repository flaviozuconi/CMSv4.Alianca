using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSv4.BusinessLayer
{
    public static class FileCms
    {
        public static void Delete(string path)
        {
            System.IO.File.Delete(path);
        }
    }
}
