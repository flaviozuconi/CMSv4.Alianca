using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace CMSv4.BusinessLayer
{
    public class BLFaleConoscoModeloEmail : BLCRUD<MLFaleConoscoModeloEmail>
    {
        public override MLFaleConoscoModeloEmail Obter(decimal Codigo, string connectionString = "")
        {
            var model = base.Obter(Codigo, connectionString) ?? new MLFaleConoscoModeloEmail();
            model.Conteudo.Unescape();

            return model;    
        }

        public decimal Salvar(MLFaleConoscoModeloEmail model, string nomeAntigo, string connectionString = "")
        {
            var portal = BLPortal.Atual;
            var conteudo = model.Conteudo.Unescape();
            var diretorioVirtual = BLConfiguracao.Pastas.ModuloFaleConoscoEmail(PortalAtual.Diretorio);
            var diretorioFisico = HttpContextFactory.Current.Server.MapPath(diretorioVirtual);
            var arquivoFisico = string.Empty;

            model.CodigoPortal = portal.Codigo;

            //Quando entrar aqui já validou o nome, não terá nome repetido
            model.Codigo = base.Salvar(model, portal.ConnectionString);

            if (!Directory.Exists(diretorioFisico))
                Directory.CreateDirectory(diretorioFisico);

            arquivoFisico = Path.Combine(diretorioFisico, model.Nome + ".htm");

            // Salvar no disco
            using (var arquivo = new StreamWriter(arquivoFisico))
            {
                arquivo.Write(conteudo);
                arquivo.Close();
            }

            BLReplicar.Arquivo(arquivoFisico);

            //Edição de registro com novo nome.
            if (!string.IsNullOrWhiteSpace(nomeAntigo) && model.Nome != nomeAntigo && model.Codigo.HasValue)
            {
                var arquivoAntigo = Path.Combine(diretorioFisico, nomeAntigo + ".htm");

                //Deletar o arquivo antigo
                if (System.IO.File.Exists(arquivoAntigo))
                {
                    System.IO.File.Delete(arquivoAntigo);
                    BLReplicar.ExcluirArquivosReplicados(arquivoAntigo);
                }
            }

            return model.Codigo.GetValueOrDefault();
        }

        public override int Excluir(List<string> ids, string connectionString = "")
        {
            int deletados = 0;
            var pasta = BLConfiguracao.Pastas.ModuloFaleConoscoEmail(PortalAtual.Diretorio) + "/";

            foreach (var item in ids)
            {
                var codigo = Convert.ToDecimal(item);

                var model = this.Obter(codigo, connectionString);
                var arquivoFisico = HttpContextFactory.Current.Server.MapPath(pasta + model.Nome + ".htm");

                if (System.IO.File.Exists(arquivoFisico))
                {
                    System.IO.File.Delete(arquivoFisico);
                    BLReplicar.ExcluirArquivosReplicados(arquivoFisico);
                }

                deletados += this.Excluir(codigo, connectionString);
            }

            return deletados;
        }
    }
}
