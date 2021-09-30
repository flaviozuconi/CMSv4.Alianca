using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace CMSv4.BusinessLayer
{
    public class BLFaleConoscoFormulario : BLCRUD<MLFaleConoscoFormulario>
    {
        public override MLFaleConoscoFormulario Obter(decimal Codigo, string connectionString = "")
        {
            var model = base.Obter(Codigo, connectionString) ?? new MLFaleConoscoFormulario();
         
            model.Script = !string.IsNullOrEmpty(model.Script) ? model.Script.Unescape() : string.Empty;
            model.Conteudo = !string.IsNullOrEmpty(model.Conteudo) ? model.Conteudo.Unescape() : "<form class='jovens-contato' action='@Portal.Url()/modulo/faleconosco/salvar' method='post' novalidate='novalidate'> \n  <input type='hidden' name='codigoPagina' value='@ViewData[\"codigoPagina\"]'>\n  <input type='hidden' name='repositorio' value='@ViewData[\"repositorio\"]'>\n  <input type='hidden' name='modelo' value='@ViewData[\"modelo\"]'>\n  <div class='row'>\n    \n    <div class='col-xs-24 form-group text-right'>\n        <button type='submit' class='btn btn-default'>Enviar</button>\n    </div>\n  </div>\n</form>";

            return model;
        }

        public decimal Salvar(MLFaleConoscoFormulario model, string nomeAntigo, string connectionString = "")
        {
            var portal = BLPortal.Atual;
            var conteudo = model.Conteudo.Unescape();
            var diretorioVirtual = BLConfiguracao.Pastas.ModuloFaleConoscoForm(PortalAtual.Diretorio);
            var diretorioFisico = HttpContextFactory.Current.Server.MapPath(diretorioVirtual);
            var arquivoFisico = string.Empty;
            var arquivoFisicoScript = string.Empty;
            var conteudoScript = string.Empty;

            if (!string.IsNullOrWhiteSpace(model.Script))
            {
                conteudoScript = model.Script.Unescape();
            }

            model.CodigoPortal = portal.Codigo;

            //Quando entrar aqui já validou o nome, não terá nome repetido
            base.Salvar(model, portal.ConnectionString);

            if (!Directory.Exists(diretorioFisico))
                Directory.CreateDirectory(diretorioFisico);

            //o nome do arquivo não pode começar com Script, palavra reservada para o arquivo gerado pelo sistema
            if (model.Nome.ToLower().StartsWith("script"))
                model.Nome.ToLower().Replace("script", "");

            //O arquivo script será sempre Script<NomeFormulario>
            arquivoFisico = Path.Combine(diretorioFisico, model.Nome + ".cshtml");
            arquivoFisicoScript = Path.Combine(diretorioFisico, "Script" + model.Nome + ".cshtml");

            // Salvar no disco
            using (var arquivo = new StreamWriter(arquivoFisico))
            {
                arquivo.Write(conteudo);
                arquivo.Close();
            }

            BLReplicar.Arquivo(arquivoFisico);

            using (var arquivo = new StreamWriter(arquivoFisicoScript))
            {
                arquivo.Write(conteudoScript);
                arquivo.Close();
            }

            BLReplicar.Arquivo(arquivoFisicoScript);

            //Edição de registro com novo nome.
            if (!string.IsNullOrWhiteSpace(nomeAntigo) && model.Nome != nomeAntigo && model.Codigo.HasValue)
            {
                var arquivoAntigo = Path.Combine(diretorioFisico, nomeAntigo + ".cshtml");
                var arquivoScriptAntig = Path.Combine(diretorioFisico, "Script" + nomeAntigo + ".cshtml");

                //Deletar o arquivo antigo
                if (System.IO.File.Exists(arquivoAntigo))
                {
                    System.IO.File.Delete(arquivoAntigo);
                    BLReplicar.ExcluirArquivosReplicados(arquivoAntigo);
                }

                if (System.IO.File.Exists(arquivoScriptAntig))
                {
                    System.IO.File.Delete(arquivoScriptAntig);
                    BLReplicar.ExcluirArquivosReplicados(arquivoScriptAntig);
                }
            }
            return model.Codigo.GetValueOrDefault();
        }

        public override int Excluir(List<string> ids, string connectionString = "")
        {
            int deletados = 0;

            var pasta = BLConfiguracao.Pastas.ModuloFaleConoscoForm(PortalAtual.Diretorio) + "/";

            foreach (var item in ids)
            {
                var codigo = Convert.ToDecimal(item);

                var model = this.Obter(codigo, connectionString);
                var arquivoFisico = HttpContextFactory.Current.Server.MapPath(pasta + model.Nome + ".cshtml");

                if (System.IO.File.Exists(arquivoFisico))
                {
                    System.IO.File.Delete(arquivoFisico);
                    BLReplicar.ExcluirArquivosReplicados(arquivoFisico);
                }

                deletados += base.Excluir(codigo, PortalAtual.ConnectionString);
            }

            return deletados;
        }
    }
}
