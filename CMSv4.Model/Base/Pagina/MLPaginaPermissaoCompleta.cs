using System.Collections.Generic;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    public class MLPaginaPermissaoCompleta
    {
        public MLPaginaPermissaoCompleta()
        {
            GruposPagina = new List<MLPaginaPermissao>();
            GruposSecao = new List<MLSecaoPermissao>();            
        }

        [DataField("PER_PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("PAG_B_RESTRITO", SqlDbType.Bit)]
        public bool? PaginaRestrita { get; set; }

        [DataField("SEC_B_RESTRITO", SqlDbType.Bit)]
        public bool? SecaoRestrita { get; set; }

        [DataField("POR_B_RESTRITO", SqlDbType.Bit)]
        public bool? PortalRestrito { get; set; }

        public List<MLPaginaPermissao> GruposPagina { get; set; }

        public List<MLSecaoPermissao> GruposSecao { get; set; }

        public string PortalRestritoTexto
        {
            get
            {
                if (!PortalRestrito.HasValue) return "Acesso Liberado";
                if (PortalRestrito.Value) return "Acesso Restrito";
                else return "Acesso Liberado";
            }
        }

        public string SecaoRestritaTexto
        {
            get
            {
                if (!SecaoRestrita.HasValue) return "Usar permissões do portal";
                if (SecaoRestrita.Value) return "Acesso Restrito";
                else return "Acesso Liberado";
            }
        }
    }
}
