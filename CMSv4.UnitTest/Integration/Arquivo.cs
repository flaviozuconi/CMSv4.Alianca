using CMSv4.BusinessLayer;
using CMSv4.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CMSv4.UnitTest
{
    [TestClass]
    public class Arquivo : IntegrationTest<MLArquivo>
    {
        //public override int MockExcluir()
        //{
        //    return new BLArquivos().Excluir(0);
        //}

        //public override List<MLArquivo> MockListar()
        //{
        //    return new BLArquivos().Listar(new MLArquivo());
        //}

        //public override MLArquivo MockObter()
        //{
        //    return new BLArquivos().Obter(1);
        //}

        //public override decimal MockSalvar()
        //{
        //    return new BLArquivos().Salvar(new MLArquivo()
        //    {

        //    });
        //}
        public override MLArquivo BindModel()
        {
            return new MLArquivo()
            {
                Ativo = true,
                CodigoIdioma = 1,
                CodigoPortal = 1,
                CodigoCategoria = 1,
                Data = DateTime.Now,
                Descricao = "Arquivo unit test",
                Nome = "Nome do arquivo para unit test",
                Titulo = "Título"
            };
        }

        public override int Excluir(decimal Codigo)
        {
            return new BLArquivos().Excluir(Codigo);
        }

        public override List<MLArquivo> Listar()
        {
            return new BLArquivos().Listar(new MLArquivo(), 10, "Codigo", "DESC");
        }

        public override MLArquivo Obter(decimal Codigo)
        {
            return new BLArquivos().Obter(Codigo, 1);
        }

        public override decimal Salvar(MLArquivo model)
        {
            MockFile(new List<string>() { "imgThumb" });
            return new BLArquivos().Salvar(model, null, false);
        }
    }
}
