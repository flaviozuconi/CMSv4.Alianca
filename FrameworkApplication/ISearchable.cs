using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Utilities
{
    public interface ISearchable
    {
        /// MOD_C_URL de CMS_MOD_MODULO
        string GetUrlModulo();
    
        decimal GetCodigoRegistro();
        decimal GetCodigoPortal();
        decimal GetCodigoIdioma();
        string GetTitulo();
        string GetChamada();
        string GetTermoBusca();
        string GetUrl();
        string GetImagem();
        DateTime? GetInicio();
        DateTime? GetFim();
    }
}
