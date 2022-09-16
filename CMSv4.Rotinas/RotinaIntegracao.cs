using CMSv4.BusinessLayer;
using CMSv4.Model;
using CMSv4.Rotinas;
using Framework.Utilities;

namespace VM2.Rotinas
{
    public class RotinaIntegracao : RotinaBase
    {
        /// <summary>
        /// ROTINA INTEGRACAO
        /// </summary>
        public RotinaIntegracao(): base("rotina_integracao")
        {
        }

        public override void OnIntervalElapses()
        {
            var Scheme = CRUD.Obter(new MLConfiguracao { Chave = "UrlScheme" })?.Valor ?? "http";
            var Authoriry = CRUD.Obter(new MLConfiguracao { Chave = "UrlAuthority" })?.Valor ?? "localhost:55056";

            BLLogIntegracaoAdmin.Integrar(Scheme, Authoriry);
        }
    }
}
