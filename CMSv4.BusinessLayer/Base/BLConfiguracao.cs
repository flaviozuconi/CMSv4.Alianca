using CMSv4.Model;
using Framework.Utilities;
using System.Collections.Generic;

namespace CMSv4.BusinessLayer
{
    public class BLCmsConfiguracao : BLCRUD<MLConfiguracao>
    {
        public static List<MLConfiguracao> Listar()
        {
            var key = "configuracoes";
            var retorno = BLCachePortal.Get<List<MLConfiguracao>>(key);
            if (retorno == null)
            {
                retorno = CRUD.Listar(new MLConfiguracao());
                BLCachePortal.Add(key, retorno,4);
            }

            return retorno;
        }

    }
}
