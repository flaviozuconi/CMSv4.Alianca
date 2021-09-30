using CMSv4.BusinessLayer;

namespace CMSv4.Rotinas
{
    //<author>rvissontai</author>
    public class PublicarAgendadosModuloLista : RotinaBase
    {
        public PublicarAgendadosModuloLista() : base("agendamento_publicacao_lista")
        {
        }

        public override void OnIntervalElapses()
        {
            BLLista.PublicarTodos();
        }
    }
}
