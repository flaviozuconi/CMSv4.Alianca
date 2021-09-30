using CMSv4.BusinessLayer;
using Framework.Model;
using System.Collections.Generic;

namespace CMSv4.Model
{
    [Table("FWK_GRP_GRUPO")]
    public class MLGrupoViewModel : MLGrupo
    {
        public MLGrupoViewModel()
        {
            Funcionalidades = new List<MLFuncionalidade>();
        }

        public List<MLFuncionalidade> Funcionalidades { get; set; }
    }
}
