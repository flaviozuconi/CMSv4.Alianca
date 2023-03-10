using CMSv4.BusinessLayer;
using CMSv4.Model.Base;
using Framework.DataLayer;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class GestaoDeCustosPenalidadesController : AdminBaseCRUDPortalController<MLGestaoDeCustosPenalidades, MLGestaoDeCustosPenalidades>
    {

        #region Item

        ///// <summary>
        ///// Visualizar ou Editar o registro conforme permissão do usuário
        ///// </summary>
        ///// <param name="id">Código do registro</param>
        ///// <remarks>
        ///// GET:
        /////     /Area/Controller/Item/id
        ///// </remarks>

        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true)]
        [HttpPost]       
        public override ActionResult Item(MLGestaoDeCustosPenalidades model)
        {           
            model.Usuario = BLUsuario.ObterLogado().Login;
            model.DataAtualizacao = DateTime.Now;
            
            SetCodigoPortal(model);
            TempData["Salvo"] = CRUD.Salvar<MLGestaoDeCustosPenalidades>(model) > 0;

            return RedirectToAction("Index");

        }
        #endregion               

        #region Importar
        /// <summary>
        /// Importa os dados oriundos da planilha de Gestao De Custos E Penalidades
        /// </summary>
        /// <param name="file"></param>
        /// <param name="excluir"></param>
        /// <returns></returns>
        public ActionResult Importacao(HttpPostedFileBase file, bool? excluir)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");

            if (file == null)
            {
                return Json(new { success = false, msg = TAdm("Planilha não informada para a importação.") });
            }

            var lstModel = BLUtilitarios.EPPlus.LerExcel<MLGestaoDeCustosPenalidades>(file);

            var importacao = new MLGestaoDeCustosPenalidadesHistorico
            {
                DataImportacao = DateTime.Now,
                Usuario = BLUsuario.ObterLogado()?.Login,
                Sucesso = false,
                Finalizado = false,
                Arquivo = ""
            };

            importacao.Codigo = new BLCRUD<MLGestaoDeCustosPenalidadesHistorico>().Salvar(importacao);

            if (!Directory.Exists(Path.Combine(Server.MapPath($"~/portal/{PortalAtual.Diretorio}/arquivos"), "importacao/gestaoCustosPenalidades")))
                Directory.CreateDirectory(Path.Combine(Server.MapPath($"~/portal/{PortalAtual.Diretorio}/arquivos"), "importacao/gestaoCustosPenalidades"));

            var nomeArquivo = $"{importacao.Codigo + Path.GetExtension(file.FileName)}";
            if (file != null)
                file.SaveAs(Path.Combine(Server.MapPath($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/gestaoCustosPenalidades"), nomeArquivo));

            importacao.Arquivo = nomeArquivo;

            string erros = ValidaPlanilha(lstModel);                      

            if (!string.IsNullOrEmpty(erros))
            {
                importacao.Sucesso = false;
                importacao.Finalizado = true;
                new BLCRUD<MLGestaoDeCustosPenalidadesHistorico>().SalvarParcial(importacao);

                return Json(new { success = false, msg = erros });
            }

            Thread thread = new Thread(new ThreadStart(() =>
            {
                InserirPlanilha(lstModel, importacao, excluir);
                
            }));

            thread.Start();

            TempData["SalvoImportacao"] = erros.Length <= 0;

            return Json(new { success = true });
        }
        #endregion

        #region Valida Planilha
        /// <summary>
        /// Valida os dados da planilha de Gestao De Custos E Penalidades
        /// </summary>
        /// <param name="lista"></param>
        /// <returns></returns>
        public string ValidaPlanilha(List<MLGestaoDeCustosPenalidades> lista)
        {
            StringBuilder erros = new StringBuilder();
            int linha = 2;

            List<int> errPOD = new List<int>();
            List<int> errTarifaAdicional = new List<int>();
            List<int> errValorTarifa = new List<int>();
            List<int> errPenalidade = new List<int>();
            List<int> errValorPenalidade = new List<int>();

            if (lista.Count <= 0)
                return TAdm("A planilha está vazia.");

            foreach (var item in lista)
            {
                if (string.IsNullOrEmpty(item.POD))
                    errPOD.Add(linha);

                if (item.ValorTarifa == null)
                    errTarifaAdicional.Add(linha);

                if (string.IsNullOrEmpty(item.ValorTarifa))
                    errValorTarifa.Add(linha);

                if (item.Penalidade == null)
                    errPenalidade.Add(linha);

                if (string.IsNullOrEmpty(item.ValorPenalidade))
                    errValorPenalidade.Add(linha);
             
                linha++;
            }

            if (errPOD.Count > 0)
                erros.AppendLine("O campo \"POD\" é obrigatório na(s) linha(s): " + string.Join(",", errPOD) + "<br>");

            if (errTarifaAdicional.Count > 0)
                erros.AppendLine("O campo \"Tarifa adicional (R$)\" é obrigatório na(s) linha(s): " + string.Join(",", errTarifaAdicional) + "<br>");

            if (errValorTarifa.Count > 0)
                erros.AppendLine("O campo \"Valor da tarifa por extenso\" é obrigatório na(s) linha(s): " + string.Join(",", errValorTarifa) + "<br>");

            if (errPenalidade.Count > 0)
                erros.AppendLine("O campo \"Penalidade Aliança (R$)\" é obrigatório na(s) linha(s): " + string.Join(",", errPenalidade) + "<br>");

            if (errValorPenalidade.Count > 0)
                erros.AppendLine("O campo \"Valor da penalidade por extenso\" é obrigatório na(s) linha(s): " + string.Join(",", errValorPenalidade) + "<br>");

            return erros.ToString();
        }
        #endregion

        #region Inserir Planilha
        /// <summary>
        /// Insere os registros da planilha no banco
        /// </summary>
        /// <param name="lista"></param>
        /// <param name="importacao"></param>
        /// <param name="excluir"></param>
        public void InserirPlanilha(List<MLGestaoDeCustosPenalidades> lista, MLGestaoDeCustosPenalidadesHistorico importacao, bool? excluir)
        {

            try
            {
                if (excluir.HasValue && excluir.Value)
                {
                    Database.ExecuteNonQuery(new SqlCommand("TRUNCATE TABLE MOD_GCP_GESTAO_CUSTO_PENALIDADE"));
                }

                var query = string.Empty;
                foreach (var item in lista)
                {
                    item.Usuario = importacao.Usuario;                
                    item.DataAtualizacao = DateTime.Now;

                    query += $@" IF (EXISTS (SELECT 1 FROM [dbo].[MOD_GCP_GESTAO_CUSTO_PENALIDADE] WHERE [GCP_C_POD] = '{item.POD}'))
                                BEGIN
                                UPDATE [dbo].[MOD_GCP_GESTAO_CUSTO_PENALIDADE]
                                    SET [GCP_C_TARIFA_ADICIONAL] = '{item.TarifaAdicional.ToString().Replace(".", "").Replace(",", ".")}'
                                        ,[GCP_C_VALOR_TARIFA_ADICIONAL] = '{item.ValorTarifa}'
                                        ,[GCP_C_PENALIDADE_ALIANCA] = '{item.Penalidade.ToString().Replace(".", "").Replace(",", ".")}'
                                        ,[GCP_C_VALOR_PENALIDADE_ALIANCA] = '{item.ValorPenalidade}'
                                        ,[GCP_D_DATA_ATUALIZACAO] = '{item.DataAtualizacao?.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
                                        ,[GCP_C_USUARIO] = '{item.Usuario}'
                                    WHERE [GCP_C_POD] = '{item.POD}'
                                END
                                ELSE
                                BEGIN
                                INSERT INTO [dbo].[MOD_GCP_GESTAO_CUSTO_PENALIDADE]
                                            ([GCP_C_POD],[GCP_C_TARIFA_ADICIONAL],[GCP_C_VALOR_TARIFA_ADICIONAL],[GCP_C_PENALIDADE_ALIANCA],[GCP_C_VALOR_PENALIDADE_ALIANCA],[GCP_D_DATA_ATUALIZACAO],[GCP_C_USUARIO])
                                        VALUES
                                            ('{item.POD}'
                                            ,'{item.TarifaAdicional.ToString().Replace(".", "").Replace(",", ".")}'
                                            ,'{item.ValorTarifa}'
                                            ,'{item.Penalidade.ToString().Replace(".", "").Replace(",", ".")}'
                                            ,'{item.ValorPenalidade}'
                                            ,'{item.DataAtualizacao?.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
                                            ,'{item.Usuario}')
                                END".Replace("''", "NULL");
                }

                Database.ExecuteNonQuery(new SqlCommand(query));

                importacao.Sucesso = true;
                importacao.Finalizado = true;

                new BLCRUD<MLGestaoDeCustosPenalidadesHistorico>().SalvarParcial(importacao);
                
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                importacao.Sucesso = false;
                importacao.Finalizado = true;
                new BLCRUD<MLGestaoDeCustosPenalidadesHistorico>().SalvarParcial(importacao);
            }
        }

        #endregion         

        #region Historico Importacao
        /// <summary>
        /// Listar Historico Importacao
        /// </summary>
        /// <returns></returns>
        public ActionResult HistoricoImportacao()
        {
            try
            {
                return DataTable.Listar(new MLGestaoDeCustosPenalidadesHistorico(), Request.QueryString);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false });
            }
        }
        #endregion

        #region Download Modelo
        /// <summary>
        /// Download modelos
        /// </summary>
        /// <param name="modelo"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult DownloadModelo()
        {
            return File("~/Templates/gestao-custos-penalidades-template.xlsx", "application/ms-excel", $"gestao-custos-penalidades-template.xlsx");
        }
        #endregion

        #region Download Importacao
        /// <summary>
        /// Download arquivo de importação
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult DownloadImportacao(decimal? id)
        {
            if (System.IO.File.Exists(Server.MapPath($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/gestaoCustosPenalidades/{id}.xlsx")))
                return File($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/gestaoCustosPenalidades/{id}.xlsx", "application/ms-excel", $"{id}.xlsx");

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region IsValid
        /// <summary>
        /// Verifica se o POD não ficará duplicado ao salvar
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult IsValid(decimal? id, string POD)
        {
            if (!String.IsNullOrEmpty(POD))
            {
                var model = CRUD.Obter(new MLGestaoDeCustosPenalidades { POD = POD });
                if ((id.HasValue && model != null && model.Codigo.Value != id.Value) || (!id.HasValue && model != null && model.Codigo.HasValue))
                    return Json(string.Format(T("Não pode ser inserido '{0}'. POD já existente."), POD));
            }

            return Json(true);
        }
        #endregion
    }
}
