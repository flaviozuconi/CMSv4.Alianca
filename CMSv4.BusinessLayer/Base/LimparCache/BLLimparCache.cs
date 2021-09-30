using Framework.Utilities;

namespace CMSv4.BusinessLayer
{
    public class BLLimparCache
    {
        public static void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                BLCachePortal.RemoveAll();
            else
                BLCachePortal.Remove(key);
        }
    }
}
