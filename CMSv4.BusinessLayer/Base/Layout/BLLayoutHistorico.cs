using CMSv4.Model;
using Framework.Utilities;
using System.IO;
using System.Text;

namespace CMSv4.BusinessLayer
{
    public class BLLayoutHistorico : BLCRUD<MLLayout>
    {
        #region Recuperar Histórico

        public static bool RecuperarHistorico(decimal id)
        {
            var portal = PortalAtual.Obter;
            var modelHistorico = CRUD.Obter<MLLayout>(id, portal.ConnectionString);
            var pasta = HttpContextFactory.Current.Server.MapPath(BLConfiguracao.Pastas.LayoutsPortal(portal.Diretorio));
            var conteudo = modelHistorico.Conteudo.Unescape();
            var file = Path.Combine(pasta, modelHistorico.Nome + ".cshtml");

            File.WriteAllText(file, conteudo, Encoding.UTF8);
            BLReplicar.Arquivo(file);

            return true;
        }

        #endregion
    }
}
