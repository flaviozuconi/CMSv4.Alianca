using System;
using System.Data;
using Framework.DataLayer;
using CMSv4.Model;
using Framework.Utilities;
using System.Transactions;
using System.IO;
using System.Collections.Generic;

namespace CMSv4.BusinessLayer
{
    public class BLMultimidiaArquivo : BLCRUD<MLMultimidiaArquivo>
    {
        #region Obter Arquivo

        public MLMultimidiaArquivo ObterArquivo(decimal decCodigo)
        {

            try
            {
                string key = String.Format("OBTER_MULTIMIDIA_ARQUIVO_{0}", decCodigo);

                var retorno = BLCachePortal.Get<MLMultimidiaArquivo>(key);

                if (retorno != null)
                    return retorno;

                retorno = CRUD.Obter<MLMultimidiaArquivo>(decCodigo, BLPortal.Atual.ConnectionString);
                BLCachePortal.Add(key, retorno);

                return retorno;
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region Obter

        /// <summary>
        /// Obter
        /// </summary>
        public decimal? Obter(string nome, decimal? id)
        {
            using (var command = Database.NewCommand("USP_MOD_ARQ_S_MULTIMIDIA_CATEGORIAS", BLPortal.Atual.ConnectionString))
            {
                // Parametros   

                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, 18, BLPortal.Atual.Codigo);
                command.NewCriteriaParameter("@MCA_C_NOME", SqlDbType.VarChar, 250, nome);
                command.NewCriteriaParameter("@MCA_N_CODIGO", SqlDbType.Decimal, 18, id);

                return Convert.ToDecimal(Database.ExecuteScalar(command));
            }
        }

        #endregion

        #region Salvar

        public decimal Salvar(ref MLMultimidiaArquivo model, string connectionString = "")
        {
            var portal = PortalAtual.Obter;

            using (var scope = new TransactionScope(portal.ConnectionString))
            {
                if (string.IsNullOrEmpty(model.Nome))
                    throw new Exception("Nenhum arquivo selecionado!");

                var modelCategoria = new BLMultimidiaCategoria().Obter(model.CodigoCategoria.Value, portal.ConnectionString);

                if (!File.Exists(Path.Combine(modelCategoria.PastaFisica(portal.Diretorio), model.Nome)))
                    throw new Exception("Falha ao realizar upload do arquivo!");

                if (!model.Ativo.HasValue)
                    model.Ativo = false;

                if (!model.Destaque.HasValue)
                    model.Destaque = false;

                model.CodigoPortal = portal.Codigo;
                model.LogPreencher(model.Codigo.HasValue);
                model.PastaRelativa = modelCategoria.PastaRelativa(portal.Diretorio);

                var objTipo = new BLMultimidiaTipo().Obter(model.CodigoTipo.Value, portal.ConnectionString);

                List<string[]> tags = new List<string[]>();
                string dir = Path.Combine(modelCategoria.PastaRelativa(portal.Diretorio));
                string arquivo = Path.Combine(dir, model.Nome);

                switch (Convert.ToInt16(objTipo.Codigo))
                {
                    case 1:
                        tags.Add(new string[] { "SRC", arquivo });
                        tags.Add(new string[] { "ALT", model.Nome });
                        model.HtmlOutput = SubstituirTags(objTipo.Html, tags);
                        break;

                    case 2:
                        tags.Add(new string[] { "SOURCE", arquivo });
                        tags.Add(new string[] { "TYPE", "video/" + Path.GetExtension(arquivo).Replace(".", "").ToLower() });
                        model.HtmlOutput = SubstituirTags(objTipo.Html, tags);
                        break;

                    case 3:
                        tags.Add(new string[] { "HREF", arquivo });
                        tags.Add(new string[] { "TEXTO", "Download" });
                        model.HtmlOutput = SubstituirTags(objTipo.Html, tags);
                        break;
                }

                model.Codigo = base.Salvar(model, portal.ConnectionString);

                scope.Complete();
            }

            return model.Codigo.GetValueOrDefault();
        }

        private static string SubstituirTags(string strHtmlInput, List<string[]> lstTermos)
        {
            string strHtmlOutput = strHtmlInput;

            foreach (var array in lstTermos)
                strHtmlOutput = strHtmlOutput.Replace("%%" + array[0] + "%%", array[1]);

            return strHtmlOutput;
        }
        #endregion

        #region UploadFile

        public string UploadFile(MLMultimidiaArquivo model)
        {
            var portal = PortalAtual.Obter;
            string caminhoArquivo = string.Empty;

            if (model.CodigoCategoria.HasValue)
            {
                var file = HttpContextFactory.Current.Request.Files[0];

                var modelCategoria = CRUD.Obter<MLMultimidiaCategoria>(model.CodigoCategoria.Value, portal.ConnectionString);
                caminhoArquivo = modelCategoria.PastaFisica(portal.Diretorio) + "/" + file.FileName;

                DirectoryInfo di = new DirectoryInfo(modelCategoria.PastaFisica(portal.Diretorio));
                if (!di.Exists)
                {
                    di.Create();
                }

                file.SaveAs(caminhoArquivo);
            }
            else
                throw new Exception("Pasta não encontrada");

            return caminhoArquivo;
        }

        #endregion
    }

    public class BLMultimidiaTipo : BLCRUD<MLMultimidiaTipo> { }
}
