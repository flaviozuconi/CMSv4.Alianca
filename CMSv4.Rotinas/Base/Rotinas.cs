using System.Collections.Generic;
using System.Web;
using Framework.Utilities;

namespace CMSv4.Rotinas
{
    public sealed class Rotinas
    {
        private HttpContext _context;
        private List<IRotina> _iRotinas { get; set; }
        private static readonly Rotinas _instancia = new Rotinas();

        //private static readonly Rotinas instancia = new Rotinas();

        /// <summary>
        /// Adicionar na lista todas as rotinas que devem ser executadas
        /// </summary>
        public Rotinas()
        {
            var rotinas = Util.ListarClassesPorHeranca<RotinaBase>();

            _iRotinas = new List<IRotina>();
            _iRotinas.AddRange(rotinas);

            //Adicionar abaixo as rotinas que não estão herdando a classe RotinaBase
        }

        #region Disparar Rotinas

        /// <summary>
        /// Disparar todas as rotinas registradas na lista
        /// </summary>
        public void DisparaRotinas()
        {
            _context = HttpContext.Current;

            foreach (var rotina in _iRotinas)
                rotina.Timer.Start();

            ApplicationLog.Log("Rotinas inicializadas pela aplicação.");
        }

        #endregion

        #region GetInstance

        public static Rotinas GetInstance
        {
            get
            {
                return _instancia;
            }
        }

        #endregion

        #region Parar Rotinas

        /// <summary>
        /// Encerrar todas as rotinas registras na lista
        /// </summary>
        public void ParaRotinas()
        {
            foreach (var rotina in _iRotinas)
                rotina.Timer.Stop();

            ApplicationLog.Log("Rotinas encerradas pela aplicação.");
        }

        #endregion
    }
}
