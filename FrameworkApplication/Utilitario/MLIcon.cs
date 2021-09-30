
using System.Collections.Generic;

namespace Framework.Utilities
{
    public class MLIcon
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Src { get; set; }
    }

    public class MLIconPack
    {
        public MLIconPack()
        {
            Icons = new List<MLIcon>();
        }

        public int total { get; set; }
        public List<MLIcon> Icons { get; set; }
    }
}
