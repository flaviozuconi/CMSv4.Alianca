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

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class ProgramacaoNavioAdminController : AdminBaseCRUDPortalController<MLProgramacaoNavio, MLProgramacaoNavio>
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

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        [ValidateInput(false)]
        public override ActionResult Item(MLProgramacaoNavio model)
        {

            model.DataImportacao = DateTime.Now;

            SetCodigoPortal(model);

            model.SaidaPrevisto = Add3Horas(model.SaidaPrevisto);
            model.SaidaRealizado = Add3Horas(model.SaidaRealizado);
            model.ChegadaPrevisto = Add3Horas(model.ChegadaPrevisto);
            model.ChegadaRealizado = Add3Horas(model.ChegadaRealizado);
            model.Deadline = Add3Horas(model.Deadline);
            model.ChegadaTransbordoPrevisto1 = Add3Horas(model.ChegadaTransbordoPrevisto1);
            model.ChegadaTransbordoRealizado1 = Add3Horas(model.ChegadaTransbordoRealizado1);
            model.ChegadaTransbordoPrevisto2 = Add3Horas(model.ChegadaTransbordoPrevisto2);
            model.ChegadaTransbordoRealizado2 = Add3Horas(model.ChegadaTransbordoRealizado2);
            model.ChegadaTransbordoPrevisto3 = Add3Horas(model.ChegadaTransbordoPrevisto3);
            model.ChegadaTransbordoRealizado3 = Add3Horas(model.ChegadaTransbordoRealizado3);
            model.ChegadaTransbordoPrevisto4 = Add3Horas(model.ChegadaTransbordoPrevisto4);
            model.ChegadaTransbordoRealizado4 = Add3Horas(model.ChegadaTransbordoRealizado4);

            TempData["Salvo"] = CRUD.Salvar<MLProgramacaoNavio>(model) > 0;

            return RedirectToAction("Index");

        }
        #endregion

        #region IsValidOrigemDestino
        /// <summary>
        /// Verifica se a Origem e Destiuno são únicos
        /// </summary>
        /// <param name="Codigo"></param>
        /// <param name="Origem"></param>
        /// <param name="Destino"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult IsValidOrigemDestino(decimal? Codigo, string Origem, string Destino, string NavioViagem)
        {
            if (Origem.Length == 0 || Destino.Length == 0 || NavioViagem.Length == 0)
                return Json(true);

            var list = CRUD.Listar(new MLProgramacaoNavio { Origem = Origem, Destino = Destino, NavioViagem = NavioViagem }).FindAll(x => x.Codigo != Codigo && x.Origem == Origem && x.Destino == Destino && x.NavioViagem == NavioViagem);

            return Json(list.Count == 0);
        }
        #endregion

        #region Importar
        /// <summary>
        /// Importa os dados oriundos da planilha de programação navio
        /// </summary>
        /// <param name="file"></param>
        /// <param name="excluir"></param>
        /// <returns></returns>
        public ActionResult Importar(HttpPostedFileBase file, bool? excluir)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");

            if (file == null)
            {
                return Json(new { success = false, msg = TAdm("Planilha não informada para a importação.") });
            }

            var lstModel = BLUtilitarios.EPPlus.LerExcel<MLProgramacaoNavio>(file);

            var importacao = new MLProgramacaoNavioHistorico
            {
                DataImportacao = DateTime.Now,
                Usuario = BLUsuario.ObterLogado()?.Login,
                Sucesso = false,
                Finalizado = false,
                Arquivo = ""
            };

            importacao.Codigo = new BLCRUD<MLProgramacaoNavioHistorico>().Salvar(importacao);

            if (!Directory.Exists(Path.Combine(Server.MapPath($"~/portal/{PortalAtual.Diretorio}/arquivos"), "importacao/programacaonavio")))
                Directory.CreateDirectory(Path.Combine(Server.MapPath($"~/portal/{PortalAtual.Diretorio}/arquivos"), "importacao/programacaonavio"));

            var nomeArquivo = $"{importacao.Codigo + Path.GetExtension(file.FileName)}";
            if (file != null)
                file.SaveAs(Path.Combine(Server.MapPath($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/programacaonavio"), nomeArquivo));

            importacao.Arquivo = nomeArquivo;

            string erros = ValidaPlanilha(lstModel);

            if (!string.IsNullOrEmpty(erros))
            {
                importacao.Sucesso = false;
                importacao.Finalizado = true;
                new BLCRUD<MLProgramacaoNavioHistorico>().SalvarParcial(importacao);

                return Json(new { success = false, msg = erros });
            }

            Thread thread = new Thread(new ThreadStart(() =>
            {
                InserirPlanilha(lstModel, importacao, excluir);
            }));

            thread.Start();

            return Json(new { success = true });
        }
        #endregion

        #region Valida Planilha
        /// <summary>
        /// Valida os dados da planilha de ProgramacaoNavio
        /// </summary>
        /// <param name="lista"></param>
        /// <returns></returns>
        public string ValidaPlanilha(List<MLProgramacaoNavio> lista)
        {
            StringBuilder erros = new StringBuilder();
            int linha = 2;

            List<int> errOrigem = new List<int>();
            List<int> errNavio = new List<int>();
            List<int> errDeadline = new List<int>();

            if (lista.Count <= 0)
                return TAdm("A planilha está vazia.");

            foreach (var item in lista)
            {
                if (string.IsNullOrEmpty(item.Origem))
                    errOrigem.Add(linha);

                if (string.IsNullOrEmpty(item.NavioViagem))
                    errNavio.Add(linha);

                if (item.Deadline == null)
                    errDeadline.Add(linha);

                linha++;
            }

            if (errOrigem.Count > 0)
                erros.AppendLine("O campo \"Origem\" é obrigatório na(s) linha(s): " + string.Join(",", errOrigem) + "<br>");

            if (errNavio.Count > 0)
                erros.AppendLine("O campo \"Navio/Viagem\" é obrigatório na(s) linha(s): " + string.Join(",", errNavio) + "<br>");

            if (errDeadline.Count > 0)
                erros.AppendLine("O campo \"Deadline\" é obrigatório na(s) linha(s): " + string.Join(",", errDeadline) + "<br>");

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
        public void InserirPlanilha(List<MLProgramacaoNavio> lista, MLProgramacaoNavioHistorico importacao, bool? excluir)
        {

            try
            {
                if (excluir.HasValue && excluir.Value)
                {
                    Database.ExecuteNonQuery(new SqlCommand("DELETE FROM MOD_TKP_TAKE_OR_PAY_PROGRAMACAO_NAVIOS"));
                }

                var query = string.Empty;

                foreach (var item in lista)
                {
                    item.Usuario = importacao.Usuario;
                    item.DataImportacao = importacao.DataImportacao;
                    item.SaidaPrevisto = Add3Horas(item.SaidaPrevisto);
                    item.SaidaRealizado = Add3Horas(item.SaidaRealizado);
                    item.ChegadaPrevisto = Add3Horas(item.ChegadaPrevisto);
                    item.ChegadaRealizado = Add3Horas(item.ChegadaRealizado);
                    item.Deadline = Add3Horas(item.Deadline);
                    item.ChegadaTransbordoPrevisto1 = Add3Horas(item.ChegadaTransbordoPrevisto1);
                    item.ChegadaTransbordoRealizado1 = Add3Horas(item.ChegadaTransbordoRealizado1);
                    item.ChegadaTransbordoPrevisto2 = Add3Horas(item.ChegadaTransbordoPrevisto2);
                    item.ChegadaTransbordoRealizado2 = Add3Horas(item.ChegadaTransbordoRealizado2);
                    item.ChegadaTransbordoPrevisto3 = Add3Horas(item.ChegadaTransbordoPrevisto3);
                    item.ChegadaTransbordoRealizado3 = Add3Horas(item.ChegadaTransbordoRealizado3);
                    item.ChegadaTransbordoPrevisto4 = Add3Horas(item.ChegadaTransbordoPrevisto4);
                    item.ChegadaTransbordoRealizado4 = Add3Horas(item.ChegadaTransbordoRealizado4);

                    query += $@" IF (EXISTS (SELECT 1 FROM MOD_TKP_TAKE_OR_PAY_PROGRAMACAO_NAVIOS WHERE PRN_C_ORIGEM = '{item.Origem}' AND PRN_C_DESTINO = '{item.Destino}' AND PRN_C_NAVIO_VIAGEM = '{item.NavioViagem}'))
                        BEGIN
	                        UPDATE MOD_TKP_TAKE_OR_PAY_PROGRAMACAO_NAVIOS
							SET
							PRN_C_ORIGEM='{item.Origem}'
							,PRN_D_SAIDA_PREVISTO='{item.SaidaPrevisto?.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
							,PRN_D_SAIDA_REALIZADO='{item.SaidaRealizado?.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
							,PRN_C_DESTINO='{item.Destino}'
							,PRN_D_CHEGADA_PREVISTO='{item.ChegadaPrevisto?.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
							,PRN_D_CHEGADA_REALIZADO='{item.ChegadaRealizado?.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
							,PRN_C_NAVIO_VIAGEM='{item.NavioViagem}'
							,PRN_C_TRANSIT_TIME='{item.TransitTime}'
							,PRN_D_DEADLINE='{item.Deadline?.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
							,PRN_C_NAVIO_TRANSBORDO_1='{item.NavioTransbordo1}'
							,PRN_C_PORTO_TRANSBORDO_1='{item.PortoTransbordo1}'
							,PRN_C_TRANSIT_TIME_TRANSBORDO_1='{item.TransitTimeTransbordo1}'
							,PRN_D_CHEGADA_TRANSBORDO_PREVISTO_1='{item.ChegadaTransbordoPrevisto1?.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
							,PRN_D_CHEGADA_TRANSBORDO_REALIZADO_1='{item.ChegadaTransbordoRealizado1?.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
							,PRN_C_NAVIO_TRANSBORDO_2='{item.NavioTransbordo2}'
							,PRN_C_PORTO_TRANSBORDO_2='{item.PortoTransbordo2}'
							,PRN_C_TRANSIT_TIME_TRANSBORDO_2='{item.TransitTimeTransbordo2}'
							,PRN_D_CHEGADA_TRANSBORDO_PREVISTO_2='{item.ChegadaTransbordoPrevisto2?.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
							,PRN_D_CHEGADA_TRANSBORDO_REALIZADO_2='{item.ChegadaTransbordoRealizado2?.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
							,PRN_C_NAVIO_TRANSBORDO_3='{item.NavioTransbordo3}'
							,PRN_C_PORTO_TRANSBORDO_3='{item.PortoTransbordo3}'
							,PRN_C_TRANSIT_TIME_TRANSBORDO_3='{item.TransitTimeTransbordo3}'
							,PRN_D_CHEGADA_TRANSBORDO_PREVISTO_3='{item.ChegadaTransbordoPrevisto3?.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
							,PRN_D_CHEGADA_TRANSBORDO_REALIZADO_3='{item.ChegadaTransbordoRealizado3?.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
							,PRN_C_NAVIO_TRANSBORDO_4='{item.NavioTransbordo4}'
							,PRN_C_PORTO_TRANSBORDO_4='{item.PortoTransbordo4}'
							,PRN_C_TRANSIT_TIME_TRANSBORDO_4='{item.TransitTimeTransbordo4}'
							,PRN_D_CHEGADA_TRANSBORDO_PREVISTO_4='{item.ChegadaTransbordoPrevisto4?.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
							,PRN_D_CHEGADA_TRANSBORDO_REALIZADO_4='{item.ChegadaTransbordoRealizado4?.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
							,PRN_C_USUARIO_IMPORTACAO='{item.Usuario}'
							,PRN_D_DATA_IMPORTACAO='{item.DataImportacao?.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
							WHERE PRN_C_ORIGEM = '{item.Origem}' AND PRN_C_DESTINO = '{item.Destino}';
                        END
                        ELSE
                        BEGIN
                            INSERT INTO MOD_TKP_TAKE_OR_PAY_PROGRAMACAO_NAVIOS
								(PRN_C_ORIGEM,PRN_D_SAIDA_PREVISTO,PRN_D_SAIDA_REALIZADO,PRN_C_DESTINO,PRN_D_CHEGADA_PREVISTO,PRN_D_CHEGADA_REALIZADO,PRN_C_NAVIO_VIAGEM,PRN_C_TRANSIT_TIME,PRN_D_DEADLINE,PRN_C_NAVIO_TRANSBORDO_1,PRN_C_PORTO_TRANSBORDO_1,PRN_C_TRANSIT_TIME_TRANSBORDO_1,PRN_D_CHEGADA_TRANSBORDO_PREVISTO_1,PRN_D_CHEGADA_TRANSBORDO_REALIZADO_1,PRN_C_NAVIO_TRANSBORDO_2,PRN_C_PORTO_TRANSBORDO_2,PRN_C_TRANSIT_TIME_TRANSBORDO_2,PRN_D_CHEGADA_TRANSBORDO_PREVISTO_2,PRN_D_CHEGADA_TRANSBORDO_REALIZADO_2,PRN_C_NAVIO_TRANSBORDO_3,PRN_C_PORTO_TRANSBORDO_3,PRN_C_TRANSIT_TIME_TRANSBORDO_3,PRN_D_CHEGADA_TRANSBORDO_PREVISTO_3,PRN_D_CHEGADA_TRANSBORDO_REALIZADO_3,PRN_C_NAVIO_TRANSBORDO_4,PRN_C_PORTO_TRANSBORDO_4,PRN_C_TRANSIT_TIME_TRANSBORDO_4,PRN_D_CHEGADA_TRANSBORDO_PREVISTO_4,PRN_D_CHEGADA_TRANSBORDO_REALIZADO_4,PRN_C_USUARIO_IMPORTACAO,PRN_D_DATA_IMPORTACAO)
							VALUES
								('{item.Origem}',
                                '{item.SaidaPrevisto?.ToString("yyyy-MM-dd HH:mm:ss.fff")}',
                                '{item.SaidaRealizado?.ToString("yyyy-MM-dd HH:mm:ss.fff")}',
                                '{item.Destino}',
                                '{item.ChegadaPrevisto?.ToString("yyyy-MM-dd HH:mm:ss.fff")}',
                                '{item.ChegadaRealizado?.ToString("yyyy-MM-dd HH:mm:ss.fff")}',
                                '{item.NavioViagem}',
                                '{item.TransitTime}',
                                '{item.Deadline?.ToString("yyyy-MM-dd HH:mm:ss.fff")}',
                                '{item.NavioTransbordo1}',
                                '{item.PortoTransbordo1}',
                                '{item.TransitTimeTransbordo1}',
                                '{item.ChegadaTransbordoPrevisto1?.ToString("yyyy-MM-dd HH:mm:ss.fff")}',
                                '{item.ChegadaTransbordoRealizado1?.ToString("yyyy-MM-dd HH:mm:ss.fff")}',
                                '{item.NavioTransbordo2}',
                                '{item.PortoTransbordo2}',
                                '{item.TransitTimeTransbordo2}',
                                '{item.ChegadaTransbordoPrevisto2?.ToString("yyyy-MM-dd HH:mm:ss.fff")}',
                                '{item.ChegadaTransbordoRealizado2?.ToString("yyyy-MM-dd HH:mm:ss.fff")}',
                                '{item.NavioTransbordo3}',
                                '{item.PortoTransbordo3}',
                                '{item.TransitTimeTransbordo3}',
                                '{item.ChegadaTransbordoPrevisto3?.ToString("yyyy-MM-dd HH:mm:ss.fff")}',
                                '{item.ChegadaTransbordoRealizado3?.ToString("yyyy-MM-dd HH:mm:ss.fff")}',
                                '{item.NavioTransbordo4}',
                                '{item.PortoTransbordo4}',
                                '{item.TransitTimeTransbordo4}',
                                '{item.ChegadaTransbordoPrevisto4?.ToString("yyyy-MM-dd HH:mm:ss.fff")}',
                                '{item.ChegadaTransbordoRealizado4?.ToString("yyyy-MM-dd HH:mm:ss.fff")}',
                                '{item.Usuario}',
                                '{item.DataImportacao?.ToString("yyyy-MM-dd HH:mm:ss.fff")}');
                        END".Replace("''", "NULL");
                }

                Database.ExecuteNonQuery(new SqlCommand(query));

                importacao.Sucesso = true;
                importacao.Finalizado = true;
                new BLCRUD<MLProgramacaoNavioHistorico>().SalvarParcial(importacao);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                importacao.Sucesso = false;
                importacao.Finalizado = true;
                new BLCRUD<MLProgramacaoNavioHistorico>().SalvarParcial(importacao);
            }
        }

        private DateTime? Add3Horas(DateTime? Data)
        {
            if (Data.HasValue)
            {
                return Data.Value.AddHours(3);
            }

            return null;
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
                return DataTable.Listar(new MLProgramacaoNavioHistorico(), Request.QueryString);
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
            return File("~/Templates/programacao-navio-template.xlsx", "application/ms-excel", $"programacao-navio-template.xlsx");
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
            if (System.IO.File.Exists(Server.MapPath($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/programacaonavio/{id}.xlsx")))
                return File($"/portal/{PortalAtual.Diretorio}/arquivos/importacao/programacaonavio/{id}.xlsx", "application/ms-excel", $"{id}.xlsx");

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
