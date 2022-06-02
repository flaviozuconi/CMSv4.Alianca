﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Framework.Utilities;
using VM2.Areas.CMS.Helpers;
using CMSv4.Model;
using CMSv4.Model.Base.GestaoInformacoesExportacao;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class AgendamentoIntermodalController : ModuloBaseController<MLModuloAgendamentoIntermodalEdicao, MLModuloAgendamentoIntermodalHistorico, MLModuloAgendamentoIntermodalPublicado>
    {
        #region Index
        /// <summary>
        /// Área Pública / Apenas conteúdos publicados
        /// </summary>
        public override ActionResult Index(decimal? codigoPagina, int? repositorio)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var model = CRUD.Obter<MLModuloAgendamentoIntermodalPublicado>(new MLModuloAgendamentoIntermodalPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, BLPortal.Atual.ConnectionString);

                if (model == null || (!model.CodigoPagina.HasValue))
                {
                    model = new MLModuloAgendamentoIntermodalPublicado()
                    {
                        CodigoPagina = codigoPagina,
                        Repositorio = repositorio
                    };
                }

                ViewBag.Action = string.IsNullOrEmpty(model.NomeView) ? "lista" : model.NomeView;

                return PartialView(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }
        #endregion

        #region Visualizar
        /// <summary>
        /// Área de Construção
        /// </summary>
        public override ActionResult Visualizar(decimal? codigoPagina, int? repositorio, bool? edicao, string codigoHistorico)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var portal = PortalAtual.Obter;
                var model = new MLModuloAgendamentoIntermodal();

                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloAgendamentoIntermodalEdicao>(new MLModuloAgendamentoIntermodalEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (model == null) model = new MLModuloAgendamentoIntermodal { CodigoPagina = codigoPagina, Repositorio = repositorio };
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    var historico = CRUD.Obter<MLModuloAgendamentoIntermodalHistorico>(new MLModuloAgendamentoIntermodalHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (historico != null)
                    { CRUD.CopiarValores(historico, model); }
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloAgendamentoIntermodalPublicado>(new MLModuloAgendamentoIntermodalPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null) model = new MLModuloAgendamentoIntermodal();

                ViewData["editavel"] = false;

                return PartialView("Index", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }
        #endregion

        #region Edição

        /// <summary>
        /// Editar
        /// </summary>
        public override ActionResult Editar(decimal? codigoPagina, int? repositorio, bool? edicao)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var portal = PortalAtual.Obter;
                var model = CRUD.Obter<MLModuloAgendamentoIntermodalEdicao>(new MLModuloAgendamentoIntermodalEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null) model = new MLModuloAgendamentoIntermodalEdicao();
                if (!model.CodigoPagina.HasValue) model.CodigoPagina = codigoPagina;
                if (!model.Repositorio.HasValue) model.Repositorio = repositorio;

                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

        /// <summary>
        /// Salvar
        /// </summary>
        public override ActionResult Editar(MLModuloAgendamentoIntermodalEdicao model)
        {
            try
            {
                model.CodigoUsuario = BLUsuario.ObterLogado().Codigo;
                model.DataRegistro = DateTime.Now;

                CRUD.Salvar(model, PortalAtual.Obter.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Excluir

        /// <summary>
        /// Excluir
        /// </summary>
        public override ActionResult Excluir(decimal? codigoPagina, int? repositorio)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                CRUD.Excluir<MLModuloAgendamentoIntermodalEdicao>(codigoPagina.Value, repositorio.Value, PortalAtual.Obter.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Script

        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Script(MLModuloAgendamentoIntermodal model)
        {
            try
            {
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        #endregion

        #region Script Carga

        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptCarga(MLModuloAgendamentoIntermodal model)
        {
            try
            {
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        #endregion

        #region Script Sucesso

        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptSucesso(MLModuloAgendamentoIntermodal model)
        {
            try
            {
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        #endregion

        // AREA PUBLICA

        #region EscolherTipo

        /// <summary>
        ///Escolher Tipo
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult EscolherTipo(MLModuloAgendamentoIntermodal model)
        {
            return PartialView(model);
        }
        #endregion

        #region Importar

        /// <summary>
        ///Importar
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Importar(MLModuloAgendamentoIntermodal model)
        {
            return PartialView(model);
        }
        #endregion

        // EXPORTAR

        #region Exportar

        /// <summary>
        ///Escolher Tipo
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Exportar(MLModuloAgendamentoIntermodal model)
        {
            ViewData["estado"] = CRUD.Listar<MLEstado>();

            return PartialView(new MLAgendamentoIntermodal());
        }
        #endregion

        #region Exportar Carga

        /// <summary>
        ///Escolher Tipo
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ExportarCarga(MLModuloAgendamentoIntermodal model)
        {
            return PartialView(new MLAgendamentoIntermodal());
        }
        #endregion

        #region Exportar Sucesso
        /// <summary>
        /// Exportar Sucesso
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ExportarSucesso(MLModuloAgendamentoIntermodal model)
        {
            return PartialView(new MLAgendamentoIntermodal());
        }
        #endregion

        #region Salvar Exportação
        /// <summary>
        /// salvar primeira etapa exportacao
        /// </summary>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult SalvarExportacao(MLAgendamentoIntermodal model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var portal = PortalAtual.Obter;

                    model.DataRegistro = DateTime.Now;

                    var  obj = CRUD.Obter(new MLAgendamentoIntermodal { Nome = model.Nome, Email = model.Email });

                    if(obj == null || !obj.Codigo.HasValue) model.Codigo = CRUD.Salvar(model, portal.ConnectionString);

                    return Json(new { success = true, codigo = (model.Codigo ?? obj.Codigo) });
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                    return Json(new { success = false, msg = ex.Message, codigo = 0 });
                }
            }

            return Json(new { success = false });
        }

        #endregion

        #region Salvar Exportação Carga
        /// <summary>
        /// salvar carga da exportacao
        /// </summary>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult SalvarExportacaoCarga(MLAgendamentoIntermodalCarga model)
        {
            try
            {
                var portal = PortalAtual.Obter;

                var lista = CRUD.Listar(new MLAgendamentoIntermodalCarga { CodigoExportacao = model.CodigoExportacao });

                if (lista?.Count <= 50)
                {
                    model.DataRegistro = DateTime.Now;
                    model.Codigo = CRUD.Salvar(model, portal.ConnectionString);

                    return Json(new { success = true, model = model });
                }

                return Json(new { success = false, msg = @T("Limite de 50 container atingido.") });

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Integrar
        /// <summary>
        /// salvar primeira etapa exportacao
        /// </summary>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        [ValidateInput(false)]
        public JsonResult Integrar(MLAgendamentoIntermodal model, string html)
        {
            try
            {
                var cliente = IntegrarCliente(new MLAgendamentoIntermodal { Nome = model.Nome, Codigo = model.Codigo, Email = model.Email });

                if(!string.IsNullOrEmpty(cliente)) IntegrarTicket(model, html);

                return Json(new { success = true});
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message, codigo = 0 });
            }
        }

        #endregion

        #region Excluir Carga
        /// <summary>
        /// excluir carga
        /// </summary>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult ExcluirCarga(decimal Codigo)
        {
            try
            {
                var portal = PortalAtual.Obter;

                CRUD.Excluir(new MLAgendamentoIntermodalCarga { Codigo = Codigo });

                return Json(new { success = true });

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region ValidarCamposExportacao
        /// <summary>
        /// Validar campos exportação
        /// </summary>
        /// <param name="proposta"></param>
        /// <param name="booking"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult ValidarCamposExportacao(string proposta, string booking)
        {
            #region Proposta Comercial

            if (!string.IsNullOrEmpty(proposta) && string.IsNullOrEmpty(booking))
            {
                try
                {
                    var model = CRUD.Obter(new MLGestaoInformacoesExportacao { PropostaComercial = proposta });

                    if(model?.Codigo > 0)
                        return Json(new { success = true });
                    else
                        return Json(new { success = false });
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                    return Json(new { success = false, msg = ex.Message });
                }
            }

            #endregion

            #region Numero Booking

            if (!string.IsNullOrEmpty(booking) && string.IsNullOrEmpty(proposta))
            {
                try
                {
                    var model = CRUD.Obter(new MLGestaoInformacoesExportacao { NumeroBooking = booking });

                    if (model?.Codigo > 0)
                        return Json(new { success = true });
                    else
                        return Json(new { success = false });
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                    return Json(new { success = false, msg = ex.Message });
                }
            }

            #endregion

            #region Validar se a proposta e o booking são validos

            if (!string.IsNullOrEmpty(proposta) && !string.IsNullOrEmpty(booking))
            {
                try
                {
                    var model = CRUD.Obter(new MLGestaoInformacoesExportacao { PropostaComercial = proposta, NumeroBooking = booking });

                    if (model?.Codigo > 0)
                        return Json(new { success = true });
                    else
                        return Json(new { success = false });
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                    return Json(new { success = false, msg = ex.Message });
                }
            }
            #endregion 

            return Json(new { success = false });
        }

        #endregion

        #region Integração

        #region Cliente
        /// <summary>
        /// Integrar Cliente
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string IntegrarCliente(MLAgendamentoIntermodal model)
        {
            string retorno = string.Empty;

            var objModel = new MLAgendamentoPerson
            {
                id = "461505746", //"Agendamento_" + model.Codigo,
                codRefAdditional = string.Empty,
                isActive = true,
                personType = 1,
                profileType = 2,
                accessProfile = "Clients",
                businessName = "DMRSE-1200", // model.Nome,
                corporateName = "DMRSE-1200", // model.Nome,
                cpfCnpj = string.Empty,
                userName = "email@gerador.com" //model.Email
            };

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

                #region Request para inserção de pessoa
                var webRequest = (HttpWebRequest)WebRequest.Create(BLConfiguracao.UrlIntegracaoPerson + "?token=" + BLConfiguracao.UrlIntegracaoToken + "&returnAllProperties=false");
                webRequest.ContentType = "application/json; charset=utf-8";
                webRequest.Method = "POST";
                
                string jsonSerialize = JsonConvert.SerializeObject(objModel);
                var dados = Encoding.UTF8.GetBytes(jsonSerialize);

                using (var stream = webRequest.GetRequestStream())
                {
                    stream.Write(dados, 0, dados.Length);
                    stream.Close();
                }

                using (var resposta = webRequest.GetResponse())
                {
                    var streamDados = resposta.GetResponseStream();
                    StreamReader reader = new StreamReader(streamDados);
                    string response = reader.ReadToEnd();

                    return retorno = Newtonsoft.Json.Linq.JToken.Parse(response).ToString();
                }
                #endregion
            }
            catch (Exception ex)
            {
                try
                {
                    #region Get para recber a pessoa
                    var webRequest = (HttpWebRequest)WebRequest.Create(BLConfiguracao.UrlIntegracaoPerson + "?token=" + BLConfiguracao.UrlIntegracaoToken + "&id=461505746"); // "&id=Agendamento_" + model.Codigo);
                    webRequest.ContentType = "application/json; charset=utf-8";
                    webRequest.Method = "GET";

                    using (var resposta = webRequest.GetResponse())
                    {
                        var streamDados = resposta.GetResponseStream();
                        StreamReader reader = new StreamReader(streamDados);
                        string response = reader.ReadToEnd();

                        return retorno = Newtonsoft.Json.Linq.JToken.Parse(response).ToString();
                    }
                    #endregion
                }
                catch (Exception exNew)
                {
                    ApplicationLog.ErrorLog(exNew);
                }
            }

            return retorno;
        }
        #endregion

        #region IntegrarTicket
        /// <summary>
        /// Integrar Tickect
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string IntegrarTicket(MLAgendamentoIntermodal model, string Html)
        {
            string retorno = string.Empty;

            var objModel = new MLAgendamentoTicket
            {
                type = 2,
                subject = "[DMRSE-1200] Agendamento Intermodal – Tipo de operação – Booking",
                serviceFull = "278116",
                category = "Service Request",
                urgency = "Normal",
                ownerTeam = "DS-CX-CI@",
                createdDate = DateTime.Now,
                description = Html
            };

            #region cliente
            objModel.clients.Add(new Client
            {
                id = "461505746",//cliente, "Agendamento_" + modelCliente.Codigo, 
                personType = 1,
                profileType = 2,
                businessName = "DMRSE-1200"//nome
            });
            #endregion 

            #region ação
            objModel.actions.Add(new CMSv4.Model.Action
            {
                type = 2,
                origin = 19,
                description = "Descrição da ação",
                justification = "Justificativa",
                createdDate = DateTime.Now
            });
            #endregion

            #region create
            objModel.createdBy = new Createdby
            {
                id = "461505746",//cliente,
                personType = 1,
                profileType = 2,
                businessName = "DMRSE-1200", //nome,
                email = "email@gerador.com"//email
            };
            #endregion

            #region campos adicionais

            objModel.customFieldValues.Add(new Customfieldvalue
            {
                customFieldId = BLConfiguracao.CodigoPropostaComercial,
                customFieldRuleId = BLConfiguracao.CodigoCustomFieldRule,
                line = 1,
                value = "CX.IMD - PROPOSTA COMERCIAL - " + model.PropostaComercial
            });

            objModel.customFieldValues.Add(new Customfieldvalue
            {
                customFieldId = BLConfiguracao.CodigoBookingNumber,
                customFieldRuleId = BLConfiguracao.CodigoCustomFieldRule,
                line = 1,
                value = "BOOKING NUMBER - " + model.NumeroBooking
            });

            //objModel.customFieldValues.Add(new Customfieldvalue
            //{
            //    customFieldId = BLConfiguracao.CodigoNumeroBl,
            //    customFieldRuleId = BLConfiguracao.CodigoCustomFieldRule,
            //    line = 1,
            //    value = "BL number - "
            //});

            objModel.customFieldValues.Add(new Customfieldvalue
            {
                customFieldId = BLConfiguracao.CodigoLocalColeta,
                customFieldRuleId = BLConfiguracao.CodigoCustomFieldRule,
                line = 1,
                value = "Local de coleta - " + Endereco(model)
            });

            #endregion

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

                #region Request para inserção de ticket
                var webRequest = (HttpWebRequest)WebRequest.Create(BLConfiguracao.UrlIntegracaoTicket + "?token=" + BLConfiguracao.UrlIntegracaoToken + "&returnAllProperties=false");
                webRequest.ContentType = "application/json; charset=utf-8";
                webRequest.Method = "POST";
                
                string jsonSerialize = JsonConvert.SerializeObject(objModel);
                var dados = Encoding.UTF8.GetBytes(jsonSerialize);

                using (var stream = webRequest.GetRequestStream())
                {
                    stream.Write(dados, 0, dados.Length);
                    stream.Close();
                }

                using (var resposta = webRequest.GetResponse())
                {
                    var streamDados = resposta.GetResponseStream();
                    StreamReader reader = new StreamReader(streamDados);
                    string response = reader.ReadToEnd();

                    return retorno = Newtonsoft.Json.Linq.JToken.Parse(response).ToString();
                }
                #endregion
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }

            return retorno;
        }

        #region Endereço
        /// <summary>
        /// Endereco
        /// </summary>
        /// <param name="modelCliente"></param>
        /// <returns></returns>
        private string Endereco(MLAgendamentoIntermodal modelCliente)
        {
            var separador = ";";

            return (modelCliente.CNPJ ?? string.Empty) + separador + (modelCliente.CEP ?? string.Empty) + separador +
                   (modelCliente.Endereco ?? string.Empty) + separador + (modelCliente.Complemento ?? string.Empty) +
                   (modelCliente.Bairro ?? string.Empty) + separador + (modelCliente.Cidade ?? string.Empty) + separador +
                   (modelCliente.Estado ?? string.Empty);
        }
        #endregion

        #endregion

        #endregion

    }
}
