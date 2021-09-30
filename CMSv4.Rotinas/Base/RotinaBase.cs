using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Timers;

namespace CMSv4.Rotinas
{
    /// <summary>
    ///     Classe Base para execução de rotina
    /// </summary>
    /// <author>rvissontai</author>
    public abstract class RotinaBase : IRotina
    {
        private MLConfiguracao ModelConfig { get; set; }
        private string KeyConfig { get; set; }
        public bool IsRunning { get; private set; }
        public double IntervalMiliSeconds { get; set; }
        public Timer Timer { get; set; }

        #region Construtor

        /// <summary>
        /// Por padrão, a classe RotinaBase recebe uma Chave para obter a info da tabela de configuração
        /// </summary>
        /// <param name="keyConfig"></param>
        public RotinaBase(string keyConfig)
        {
            KeyConfig = keyConfig;

            InitializeAndGetConfigurationModel();

            SetInterval();

            InitializeTimerAndRegisterElapsedEvent();
        }

        #endregion

        #region InitializeAndGetConfigurationModel

        private void InitializeAndGetConfigurationModel()
        {
            ModelConfig = CRUD.Obter(new MLConfiguracao() { Chave = KeyConfig });

            if (ModelConfig == null)
            {
                ModelConfig = new MLConfiguracao()
                {
                    Valor = "5"
                };
            }
        }

        #endregion

        #region SetInterval

        public virtual void SetInterval()
        {
            double interval;

            if (double.TryParse(ModelConfig.Valor, out interval))
                IntervalMiliSeconds = TimeSpan.FromMinutes(interval).TotalMilliseconds;
        }

        #endregion

        #region InitializeTimerAndRegisterElapsedEvent

        private void InitializeTimerAndRegisterElapsedEvent()
        {
            Timer = new Timer(IntervalMiliSeconds);
            Timer.Elapsed += (sender, e) => Elapsed(sender, e);
        }

        #endregion

        #region Elapsed

        public virtual void Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsRunning)
                return;

            IsRunning = true;

            try
            {
                OnIntervalElapses();
            }
            catch (Exception ex)
            {
                ApplicationLog.Log($"Erro na execução da rotina: {KeyConfig}");
                ApplicationLog.ErrorLog(ex);
            }
            finally
            {
                IsRunning = false;
            }
        }

        #endregion

        #region OnIntervalElapses

        public abstract void OnIntervalElapses();

        #endregion
    }
}
