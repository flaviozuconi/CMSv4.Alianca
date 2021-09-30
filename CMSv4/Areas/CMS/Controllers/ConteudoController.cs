using CMSv4.BusinessLayer;
using Framework.Utilities;

namespace CMSApp.Areas.CMS.Controllers
{
    public class ConteudoController : SecurePortalController
    {
        #region Visualizar

        /// <summary>
        /// </summary>
        /// <param name="id">Código da página</param>
        /// <param name="TempTemplate">Nome do template que será utilizada para visualizar a página</param>
        /// <param name="TempLayout">Nome do layout que será utilizada para visualizar a página</param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Visualizar)]
        public string Visualizar(decimal id, string TempTemplate, string TempLayout)
        {
            return new BLConteudo().Visualizar(this, id, TempTemplate, TempLayout);
        }

        #endregion

        #region Historico

        /// <summary>
        /// Visualizar um histórico de página
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Visualizar)]
        public string Historico(string id)
        {
            return new BLConteudoHistorico().Visualizar(this, id);
        }

        #endregion
    }
}
