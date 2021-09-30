using System.Collections.Generic;

namespace CMSv4.UnitTest
{
    public interface ICrud
    {
        void Listar();

        void Salvar();

        void Obter();

        void Excluir();
    }
}
