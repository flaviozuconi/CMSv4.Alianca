using CMSv4.BusinessLayer;
using CMSv4.Model.Base;
using Framework.DataLayer;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CMSv4.BusinessLayer.Base;
using CMSv4.Model;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class ScheduleAdminController : AdminBaseCRUDPortalController<MLScheduleDataTable, MLScheduleDataTable>
    {
        #region Listar Schedules
        /// <summary>
        /// Listagem das schedules
        /// </summary>
        /// <returns></returns>
        public JsonResult ListarSchedules()
        {
            string filtro = Request.QueryString["search[value]"];

            int orderBy;
            int.TryParse(Request.QueryString["order[0][column]"], out orderBy);

            bool isAsc = Request.QueryString["order[0][dir]"].Equals("asc");

            int start;
            int length;
            int.TryParse(Request.QueryString["start"], out start);
            int.TryParse(Request.QueryString["length"], out length);

            var lista = new BLScheduleAdmin().Listar(filtro, start, start + length, orderBy, isAsc);

            // Retorna os resultados
            return new JsonResult()
            {
                Data = new
                {
                    recordsTotal = lista.Count > 0 ? lista[0]?.Total : 0,
                    recordsFiltered = lista.Count > 0 ? lista[0]?.Total : 0,
                    data = lista
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = 50000000
            };
        }
        #endregion

        #region Importar
        /// <summary>
        /// Importa os dados da planilha base de rotas
        /// </summary>
        /// <param name="file"></param>
        /// <param name="excluir"></param>
        /// <returns></returns>
        public ActionResult Importar(HttpPostedFileBase file, bool? excluir)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");

                if (file == null)
                {
                    return Json(new { success = false, msg = TAdm("Planilha não informada para a importação") });
                }

                var lstModel = BLUtilitarios.EPPlus.LerExcel<MLSchedule>(file);

                var importacao = new MLScheduleAdminHistorico
                {
                    DataImportacao = DateTime.Now,
                    CodigoUsuario = BLUsuario.ObterLogado()?.Codigo,
                    Sucesso = false,
                    Finalizado = false,
                    Arquivo = ""
                };

                importacao.Codigo = new BLCRUD<MLScheduleAdminHistorico>().Salvar(importacao);

                if (!Directory.Exists(Path.Combine(Server.MapPath($"~/portal/{PortalAtual.Diretorio}/arquivos"), "importacao/scheduleadmin")))
                    Directory.CreateDirectory(Path.Combine(Server.MapPath($"~/portal/{PortalAtual.Diretorio}/arquivos"), "importacao/scheduleadmin"));

                var nomeArquivo = $"{importacao.Codigo + Path.GetExtension(file.FileName)}";
                if (file != null)
                    file.SaveAs(Path.Combine(Server.MapPath($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/scheduleadmin"), nomeArquivo));

                importacao.Arquivo = nomeArquivo;

                string erros = ValidaPlanilha(lstModel);

                if (!string.IsNullOrEmpty(erros))
                {
                    importacao.Sucesso = false;
                    importacao.Finalizado = true;
                    new BLCRUD<MLScheduleAdminHistorico>().SalvarParcial(importacao);

                    ApplicationLog.Log($"Erro integração \"Schedule Admin\": {erros}");

                    return Json(new { success = false, msg = erros });
                }

                Thread thread = new Thread(new ThreadStart(() =>
                {
                    InserirPlanilha(lstModel, importacao, excluir);
                }));

                thread.Start();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.Log($"Erro geral: {ex.Message}");

                return Json(new { success = false });
            }
        }
        #endregion

        #region Valida Planilha
        /// <summary>
        /// Valida os dados da planilha de Schedule Admin
        /// </summary>
        /// <param name="lista"></param>
        /// <returns></returns>
        public string ValidaPlanilha(List<MLSchedule> lista)
        {
            StringBuilder erros = new StringBuilder();
            int linha = 2;

            List<int> erroProposta = new List<int>();

            if (lista.Count <= 0)
                return TAdm("A planilha está vazia.");

            foreach (var item in lista)
            {
                if (string.IsNullOrEmpty(item.Origem) || string.IsNullOrEmpty(item.Destino) || string.IsNullOrEmpty(item.Servicos) || string.IsNullOrEmpty(item.Mapa))                
                    erroProposta.Add(linha);

                linha++;
            }

            if (erroProposta.Count > 0)
                erros.AppendLine("Os campos \"Origem\", \"Destino\", \"Serviços\" e \"Mapa\" são obrigatórios na(s) linha(s):" + string.Join(",", erroProposta) + "<br>");

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
        public void InserirPlanilha(List<MLSchedule> lista, MLScheduleAdminHistorico importacao, bool? excluir)
        {
            try
            {
                if (excluir.HasValue && excluir.Value)
                {
                    Database.ExecuteNonQuery(new SqlCommand("TRUNCATE TABLE MOD_ALI_SCH_SCHEDULE"));
                }

                foreach (var item in lista)
                {
                    item.Tipo = "C";
                    new BLCRUD<MLSchedule>().Salvar(item);
                }

                importacao.Sucesso = true;
                importacao.Finalizado = true;
                new BLCRUD<MLScheduleAdminHistorico>().SalvarParcial(importacao);
            }
            catch (Exception ex)
            {
                ApplicationLog.Log($"Erro interno: {ex.Message}");
                ApplicationLog.ErrorLog(ex);
                importacao.Finalizado = true;
                importacao.Sucesso = false;
                new BLCRUD<MLScheduleAdminHistorico>().SalvarParcial(importacao);
            }
        }
        #endregion

        #region Historico Importação
        /// <summary>
        /// Listar Historico Importação
        /// </summary>
        /// <returns></returns>
        public ActionResult HistoricoImportacao()
        {
            try
            {
                return DataTable.Listar(new MLScheduleAdminHistorico(), Request.QueryString);
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
        /// Download modelo
        /// </summary>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult DownloadModelo()
        {
            return File("~/Templates/base-de-rotas-template.xlsx", "application/ms-excel", "base-de-rotas-template.xlsx");
        }
        #endregion

        #region Download Importacao
        /// <summary>
        /// Download arquivo de importacao
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult DownloadImportacao(decimal? id)
        {
            if (System.IO.File.Exists(Server.MapPath($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/scheduleadmin/{id}.xlsx")))
                return File($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/scheduleadmin/{id}.xlsx", "application/ms-excel", $"{id}.xlsx");

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}
