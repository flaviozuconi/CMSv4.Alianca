using System.Timers;

namespace CMSv4.Rotinas
{
    /// <summary>
    /// Interface para rotina padrão
    /// </summary>
    /// <author>rvissontai</author>
    public interface IRotina
    {
        /// <summary>
        /// Objeto para inicializar o timer
        /// </summary>
        Timer Timer { get; set; }

        /// <summary>
        /// Váriavel de controle para saber se a thread está sendo executada
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Tempo em milesegundos em que será executado o evento Elapsed
        /// </summary>
        double IntervalMiliSeconds { get; }

        /// <summary>
        /// Definir IntervalMiliSeconds, a classe que implementar a interface deve definir
        /// se a info vem do web config, ou da base
        /// </summary>
        void SetInterval();

        /// <summary>
        /// Evento para execução da rotina
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        void Elapsed(object sender, ElapsedEventArgs e);
    }
}
