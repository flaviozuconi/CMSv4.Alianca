using System;
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
using CMSv4.Model.Base.GestaoInformacoesImportacao;
using System.Web;
using System.Security.Cryptography;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using Framework.DataLayer;
using System.Data.SqlClient;

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

        // CEP

        #region BuscarCep
        /// <summary>
        /// Buscar CEP
        /// </summary>
        /// <param name="CEP"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult BuscarCep(string Cep)
        {
            var objCep = new MLCep();
            var url = "https://viacep.com.br/ws/{0}/json";

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

                var request = (HttpWebRequest)WebRequest.Create(string.Format(url, Cep));

                var data = Encoding.ASCII.GetBytes("");

                request.Method = "GET";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                var response = (HttpWebResponse)request.GetResponse();

                objCep = JsonConvert.DeserializeObject<MLCep>(new StreamReader(response.GetResponseStream()).ReadToEnd());
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false });
            }

            if(objCep.uf == null) return Json(new { success = false });

            return Json(new { success = true, localidade = objCep.localidade, bairro = objCep.bairro, logradouro = objCep.logradouro, uf = objCep.uf });
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

        #region Popup Confirmacao

        /// <summary>
        /// Popup Confirmacao
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult PopupConfirmacao()
        {
            return View();
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

                    var  obj = CRUD.Obter(new MLAgendamentoIntermodal { Email = model.Email });

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

                var lista = CRUD.Listar(new MLAgendamentoIntermodalCarga { CodigoExportacao = model.CodigoExportacao, Chave = model.Chave });

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
                var cliente = IntegrarCliente(new MLAgendamentoIntermodal
                    {
                        Nome = model.Nome,
                        Codigo = model.Codigo,
                        Email = model.Email,
                        Tipo = model.Tipo
                    }, 
                    "AgendamentoExportar_"
                );

                if (!string.IsNullOrEmpty(cliente))
                {
                    var objRetorno = JsonConvert.DeserializeObject<MLIntegrarRetorno>(cliente);

                    if (!string.IsNullOrEmpty(objRetorno.id)) IntegrarTicket(objRetorno.id, model, html, model.Tipo == "Exportar" ? "Exportação" : model.Tipo);

                    return Json(new { success = true });
                }
                else
                {
                    var objModel = new MLAgendamentoTicket
                    {
                        type = 2,
                        subject = "Agendamento Intermodal – Exportação – " + model.NumeroBooking + " ",
                        category = "Service Request",
                        urgency = "Normal",
                        ownerTeam = "DS-CX-CI@",
                        createdDate = DateTime.Now,
                        serviceFirstLevelId = "716767"
                    };

                    #region cliente
                    objModel.clients = new List<Client>();
                    objModel.clients.Add(new Client
                    {
                        id = "Agendamento_" + model.Codigo, 
                        personType = 1,
                        profileType = 2,
                        businessName = model.Nome
                    });
                    #endregion

                    #region ação
                    objModel.actions = new List<CMSv4.Model.Action>();
                    objModel.actions.Add(new CMSv4.Model.Action
                    {
                        type = 2,
                        origin = 19,
                        description = html,
                        justification = "Justificativa",
                        createdDate = DateTime.Now
                    });
                    #endregion

                    #region create
                    objModel.createdBy = new Createdby
                    {
                        id = model.Codigo.ToString(),
                        personType = 1,
                        profileType = 2,
                        businessName = model.Nome,
                        email = model.Email
                    };
                    #endregion

                    #region campos adicionais
                    //objModel.customFieldValues = new List<CustomFieldValue>();
                    objModel.customFieldValues.Add(new Customfieldvalue
                    {
                        customFieldId = BLConfiguracao.CodigoPropostaComercial,
                        customFieldRuleId = BLConfiguracao.CodigoCustomFieldRule,
                        line = 1,
                        value = model.PropostaComercial
                    });

                    objModel.customFieldValues.Add(new Customfieldvalue
                    {
                        customFieldId = BLConfiguracao.CodigoBookingNumber,
                        customFieldRuleId = BLConfiguracao.CodigoCustomFieldRule,
                        line = 1,
                        value = model.NumeroBooking
                    });

                    if (!string.IsNullOrEmpty(model.NumeroBL))
                    {
                        objModel.customFieldValues.Add(new Customfieldvalue
                        {
                            customFieldId = BLConfiguracao.CodigoNumeroBl,
                            customFieldRuleId = BLConfiguracao.CodigoCustomFieldRule,
                            line = 1,
                            value = model.NumeroBL
                        });
                    }

                    objModel.customFieldValues.Add(new Customfieldvalue
                    {
                        customFieldId = BLConfiguracao.CodigoLocalColeta,
                        customFieldRuleId = BLConfiguracao.CodigoCustomFieldRule,
                        line = 1,
                        value = Endereco(model)
                    });

                    #endregion

                    var jsonSerialize = JsonConvert.SerializeObject(objModel);

                    #region Grava o json na nossa base
                    CRUD.Salvar<MLAgendamentoIntermodalLog>(new MLAgendamentoIntermodalLog
                    {
                        DataCadastro = DateTime.Now,
                        Json = jsonSerialize,
                        Tipo = model.Tipo,
                        isIntegrado = false,
                        Imagem = string.Join(",", model?.lstCarga.Select(obj => obj.caminhoCompleto)),
                        RetornoAPI = "Error User",

                    });
                    #endregion
                }

                return Json(new { success = false });
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
        private string IntegrarCliente(MLAgendamentoIntermodal model, string prefixo)
        {
            string retorno = string.Empty;
            string jsonSerialize = string.Empty;

            var url = CRUD.Obter(new MLConfiguracao { Chave = "URL.Integracao.Movidesk.Person" })?.Valor ?? "https://api.movidesk.com/public/v1/persons";

            var objModel = new MLAgendamentoPerson
            {
                id = prefixo + model.Codigo.ToString(),
                codRefAdditional = string.Empty,
                isActive = true,
                personType = 1,
                profileType = 2,
                accessProfile = "Clients",
                businessName = model.Nome,
                corporateName =  model.Nome,
                cpfCnpj = string.Empty,
                userName = model.Email
            };

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

                #region Request para inserção de pessoa
                var webRequest = (HttpWebRequest)WebRequest.Create(url + "?token=" + BLConfiguracao.UrlIntegracaoToken + "&returnAllProperties=false");
                webRequest.ContentType = "application/json; charset=utf-8";
                webRequest.Method = "POST";
                
                jsonSerialize = JsonConvert.SerializeObject(objModel);
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
                    var webRequest = (HttpWebRequest)WebRequest.Create(url + "?token=" + BLConfiguracao.UrlIntegracaoToken + "&id=" + prefixo + model.Codigo); // "&id=461505746");
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
                    try
                    {
                        #region Get para recber a pessoa
                        var webRequest = (HttpWebRequest)WebRequest.Create(url + "?token=" + BLConfiguracao.UrlIntegracaoToken + "&$filter=username eq '" + model.Email + "' &select=id"); // "&id=461505746");
                        webRequest.ContentType = "application/json; charset=utf-8";
                        webRequest.Method = "GET";

                        using (var resposta = webRequest.GetResponse())
                        {
                            var streamDados = resposta.GetResponseStream();
                            StreamReader reader = new StreamReader(streamDados);
                            return retorno = reader.ReadToEnd().Replace("[", string.Empty).Replace("]", string.Empty);
                        }
                        #endregion
                    }
                    catch (Exception exNew1)
                    {
                        #region Grava o json na nossa base
                        CRUD.Salvar<MLAgendamentoIntermodalLog>(new MLAgendamentoIntermodalLog
                        {
                            DataCadastro = DateTime.Now,
                            Json = jsonSerialize,
                            Tipo = model.Tipo,
                            isIntegrado = false
                        });
                        #endregion

                        ApplicationLog.ErrorLog(exNew1);
                    }
                   
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
        private string IntegrarTicket(string idCliente, MLAgendamentoIntermodal model, string Html, string Tipo)
        {
            string retorno = string.Empty;
            string jsonSerialize = string.Empty;

            var objModel = new MLAgendamentoTicket
            {
                type = 2,
                subject = "Agendamento Intermodal – " + Tipo + " – " + model.NumeroBooking + " ",
                category = "Service Request",
                urgency = "Normal",
                ownerTeam = "DS-CX-CI@",
                createdDate = DateTime.Now,
                serviceFirstLevelId = "716767"
            };

            #region cliente
            objModel.clients = new List<Client>();
            objModel.clients.Add(new Client
            {
                id = idCliente,
                personType = 1,
                profileType = 2,
                businessName = model.Nome
            });
            #endregion

            #region ação
            objModel.actions = new List<CMSv4.Model.Action>();
            objModel.actions.Add(new CMSv4.Model.Action
            {
                type = 2,
                origin = 19,
                description = Html,
                justification = "Justificativa",
                createdDate = DateTime.Now
            });
            #endregion
            
            #region create
            objModel.createdBy = new Createdby
            {
                id = idCliente,
                personType = 1,
                profileType = 2,
                businessName = model.Nome,
                email = model.Email
            };
            #endregion

            #region campos adicionais
            //objModel.customFieldValues = new List<CustomFieldValue>();
            objModel.customFieldValues.Add(new Customfieldvalue
            {
                customFieldId = BLConfiguracao.CodigoPropostaComercial,
                customFieldRuleId = BLConfiguracao.CodigoCustomFieldRule,
                line = 1,
                value = model.PropostaComercial
            });

            objModel.customFieldValues.Add(new Customfieldvalue
            {
                customFieldId = BLConfiguracao.CodigoBookingNumber,
                customFieldRuleId = BLConfiguracao.CodigoCustomFieldRule,
                line = 1,
                value = model.NumeroBooking
            });

            if (!string.IsNullOrEmpty(model.NumeroBL))
            {
                objModel.customFieldValues.Add(new Customfieldvalue
                {
                    customFieldId = BLConfiguracao.CodigoNumeroBl,
                    customFieldRuleId = BLConfiguracao.CodigoCustomFieldRule,
                    line = 1,
                    value = model.NumeroBL
                });
            }

            objModel.customFieldValues.Add(new Customfieldvalue
            {
                customFieldId = BLConfiguracao.CodigoLocalColeta,
                customFieldRuleId = BLConfiguracao.CodigoCustomFieldRule,
                line = 1,
                value = Endereco(model)
            });

            #endregion

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

                var url = CRUD.Obter(new MLConfiguracao { Chave = "URL.Integracao.Movidesk.Ticket" })?.Valor ?? "https://api.movidesk.com/public/v1/tickets";

                #region Request para inserção de ticket
                var webRequest = (HttpWebRequest)WebRequest.Create(url + "?token=" + BLConfiguracao.UrlIntegracaoToken + "&returnAllProperties=false");
                webRequest.ContentType = "application/json; charset=utf-8";
                webRequest.Method = "POST";
                
                jsonSerialize = JsonConvert.SerializeObject(objModel);
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

                    retorno = Newtonsoft.Json.Linq.JToken.Parse(response).ToString();
                }

                if (model?.lstCarga.Count > 0 && !string.IsNullOrEmpty(retorno)) SendFile(retorno, model?.lstCarga);

                #region Grava o json na nossa base

                var imagens =  string.Join(",", model?.lstCarga.Select(obj => obj.caminhoCompleto));

                CRUD.Salvar<MLAgendamentoIntermodalLog>(new MLAgendamentoIntermodalLog
                {
                    DataCadastro = DateTime.Now,
                    Json = jsonSerialize,
                    Imagem = (!string.IsNullOrEmpty(imagens) ? imagens : string.Empty),
                    RetornoAPI =  (!string.IsNullOrEmpty(retorno) ? retorno : string.Empty),
                    Tipo = Tipo,
                    isIntegrado = true
                });
                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                #region Grava o json na nossa base
                CRUD.Salvar<MLAgendamentoIntermodalLog>(new MLAgendamentoIntermodalLog
                {
                    DataCadastro = DateTime.Now,
                    Json = jsonSerialize,
                    Tipo = Tipo,
                    isIntegrado = false
                });
                #endregion

                ApplicationLog.ErrorLog(ex);
            }

            return retorno;
        }

        #region Send File
        /// <summary>
        /// Envio dos arquivos
        /// </summary>
        /// <param name="retorno"></param>
        /// <param name="lstCarga"></param>
        private void SendFile(string retorno, List<MLAgendamentoIntermodalImportacaoCarga> lstCarga)
        {
            var obj = JsonConvert.DeserializeObject<RetornoMovidesk>(retorno);

            if (obj?.id > 0)
            {
                foreach (var item in lstCarga)
                {
                    string fileName = string.Format("{0}{1}", Server.MapPath("~"), item.caminhoCompleto.Replace(Request.Url.Scheme + "://" + Request.Url.Authority + "/", string.Empty));

                    using (HttpClient client = new HttpClient())
                    using (MultipartFormDataContent content = new MultipartFormDataContent())
                    using (FileStream fileStream = System.IO.File.OpenRead(fileName))
                    using (StreamContent fileContent = new StreamContent(fileStream))
                    {
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            FileName = item.Arquivo
                        };

                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                        fileContent.Headers.Add("name", item.Arquivo);
                        content.Add(fileContent);

                        var url = CRUD.Obter(new MLConfiguracao { Chave = "URL.Integracao.Movidesk.Arquivo" })?.Valor ?? "https://api.movidesk.com/public/v1/ticketFileUpload?";

                        var result = client.PostAsync(url + "?token=" + BLConfiguracao.UrlIntegracaoToken + "&id=" + obj.id + "&actionId=1", content).Result;
                        result.EnsureSuccessStatusCode();
                    }
                }
            }
        }
        #endregion

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

        #region ScriptExportar

        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptExportar(MLModuloAgendamentoIntermodal model)
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

        #region ScriptImportarDTA

        /// <summary>
        ///Script Importar DTA
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptImportarDTA(MLModuloAgendamentoIntermodal model)
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

        #region ScriptImportarCargaDTA

        /// <summary>
        /// Script Importar Carga DTA
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptImportarCargaDTA(MLModuloAgendamentoIntermodal model)
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
        public ActionResult ScriptExportarCarga(MLModuloAgendamentoIntermodal model)
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

        #region ExportarModal

        /// <summary>
        /// ExportarModal
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ExportarModal(MLModuloAgendamentoIntermodal model)
        {
            return PartialView(new MLAgendamentoIntermodal());
        }
        #endregion

        //IMPORTAR

        #region Importar
        /// <summary>
        ///Importar
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Importar(MLAgendamentoIntermodalImportacao model)
        {
            ViewData["estado"] = CRUD.Listar<MLEstado>();

            return PartialView(model);
        }
        #endregion

        #region Importar DTA
        /// <summary>
        ///Importar
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ImportarDTA(MLAgendamentoIntermodalImportacao model)
        {
            ViewData["estado"] = CRUD.Listar<MLEstado>();

            return PartialView(model);
        }
        #endregion

        #region Salvar Importação
        /// <summary>
        /// salvar primeira etapa importação
        /// </summary>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult SalvarImportacao(MLAgendamentoIntermodalImportacao model, string guid)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var portal = PortalAtual.Obter;
                    model.DataRegistro = DateTime.Now;

                    var obj = CRUD.Obter(new MLAgendamentoIntermodalImportacao {  Email = model.Email });

                    if (obj == null || !obj.Codigo.HasValue)
                        model.Codigo = CRUD.Salvar(model, portal.ConnectionString);
                    else
                        model.Codigo = obj.Codigo;

                        if (!string.IsNullOrEmpty(guid))
                        {
                            var diretorioDeclaracaoImportacaoTemp = (BLConfiguracao.Pastas.ModuloImportacaoDeclaracaoTemp(portal.Diretorio) + "/" + guid + "/").Replace("//", "/");
                            var pastaDeclaracaoImportacaoTemp = HttpContextFactory.Current.Server.MapPath(diretorioDeclaracaoImportacaoTemp);

                            if (Directory.Exists(pastaDeclaracaoImportacaoTemp))
                            {
                                var diretorioDeclaracaoImportacao = (BLConfiguracao.Pastas.ModuloImportacaoDeclaracao(portal.Diretorio, model.Codigo.Value.ToString()) + "/").Replace("//", "/");
                                var pastaDeclaracaoImportacao = HttpContextFactory.Current.Server.MapPath(diretorioDeclaracaoImportacao);
                                if (!Directory.Exists(pastaDeclaracaoImportacao)) Directory.CreateDirectory(pastaDeclaracaoImportacao);

                                DirectoryInfo Dir = new DirectoryInfo(pastaDeclaracaoImportacaoTemp);
                                // Busca automaticamente todos os arquivos em todos os subdiretórios
                                FileInfo[] Files = Dir.GetFiles("*", SearchOption.AllDirectories);
                                foreach (FileInfo file in Files)
                                {
                                    string destino = pastaDeclaracaoImportacao + file.Name;
                                    file.CopyTo(destino, true);

                                    var objMLAgendamentoIntermodalArquivoDeclaracaoImportacao = new MLAgendamentoIntermodalArquivoDeclaracaoImportacao { CodigoImportacao = model.Codigo, Arquivo = file.Name };
                                    CRUD.Salvar(objMLAgendamentoIntermodalArquivoDeclaracaoImportacao);

                                    file.Delete();
                                }
                                Directory.Delete(pastaDeclaracaoImportacaoTemp);
                            }

                            var diretorioGuiaArrecadacaoTemp = (BLConfiguracao.Pastas.ModuloImportacaoGuiaArrecadacaoTemp(portal.Diretorio) + "/" + guid + "/").Replace("//", "/");
                            var pastaGuiaArrecadacaoTemp = HttpContextFactory.Current.Server.MapPath(diretorioGuiaArrecadacaoTemp);

                            if (Directory.Exists(pastaGuiaArrecadacaoTemp))
                            {
                                var diretorioGuiaArrecadacao = (BLConfiguracao.Pastas.ModuloImportacaoGuiaArrecadacao(portal.Diretorio, model.Codigo.Value.ToString()) + "/").Replace("//", "/");
                                var pastaGuiaArrecadacao = HttpContextFactory.Current.Server.MapPath(diretorioGuiaArrecadacao);
                                if (!Directory.Exists(pastaGuiaArrecadacao)) Directory.CreateDirectory(pastaGuiaArrecadacao);

                                DirectoryInfo Dir = new DirectoryInfo(pastaGuiaArrecadacaoTemp);
                                // Busca automaticamente todos os arquivos em todos os subdiretórios
                                FileInfo[] Files = Dir.GetFiles("*", SearchOption.AllDirectories);
                                foreach (FileInfo file in Files)
                                {
                                    string destino = pastaGuiaArrecadacao + file.Name;
                                    file.CopyTo(destino, true);

                                    var objMLAgendamentoIntermodalArquivoGare = new MLAgendamentoIntermodalArquivoGare { CodigoImportacao = model.Codigo, Arquivo = file.Name };
                                    CRUD.Salvar(objMLAgendamentoIntermodalArquivoGare);

                                    file.Delete();
                                }
                                Directory.Delete(pastaGuiaArrecadacaoTemp);
                            }


                            var diretorioBlTemp = (BLConfiguracao.Pastas.ModuloImportacaoBlTemp(portal.Diretorio) + "/" + guid + "/").Replace("//", "/");
                            var pastaBlTemp = HttpContextFactory.Current.Server.MapPath(diretorioBlTemp);

                            if (Directory.Exists(pastaBlTemp))
                            {
                                var diretorioBl = (BLConfiguracao.Pastas.ModuloImportacaoBl(portal.Diretorio, model.Codigo.Value.ToString()) + "/").Replace("//", "/");
                                var pastaBl = HttpContextFactory.Current.Server.MapPath(diretorioBl);
                                if (!Directory.Exists(pastaBl)) Directory.CreateDirectory(pastaBl);

                                DirectoryInfo Dir = new DirectoryInfo(pastaBlTemp);
                                // Busca automaticamente todos os arquivos em todos os subdiretórios
                                FileInfo[] Files = Dir.GetFiles("*", SearchOption.AllDirectories);
                                foreach (FileInfo file in Files)
                                {
                                    string destino = pastaBl + file.Name;
                                    file.CopyTo(destino, true);

                                    var objMLAgendamentoIntermodalArquivoBl = new MLAgendamentoIntermodalArquivoBl { CodigoImportacao = model.Codigo, Arquivo = file.Name };
                                    CRUD.Salvar(objMLAgendamentoIntermodalArquivoBl);

                                    file.Delete();
                                }
                                Directory.Delete(pastaBlTemp);
                            }
                        }

                        return Json(new { success = true, codigo = model.Codigo });
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

        #region ValidarCamposImportacao
        /// <summary>
        /// Validar campos importação
        /// </summary>
        /// <param name="proposta"></param>
        /// <param name="booking"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult ValidarCamposImportacao(string proposta, string booking, string numeroBl)
        {
            List<MLGestaoInformacoesImportacao> lstModel = null;

            try
            {
                if (!string.IsNullOrEmpty(proposta))
                    lstModel = CRUD.Listar(new MLGestaoInformacoesImportacao { PropostaComercial = proposta });

                if (lstModel != null && lstModel.Count > 0)
                {
                    if(string.IsNullOrEmpty(booking) && string.IsNullOrEmpty(numeroBl))
                        return Json(new { success = true });

                    if (!string.IsNullOrEmpty(booking))
                    {
                        var aux = lstModel.Find(x => x.NumeroBooking == booking);

                        if (aux != null && !string.IsNullOrEmpty(aux.NumeroBooking))
                            return Json(new { success = true });
                        else
                            return Json(new { success = false, msg = "Informe um Número Booking válido." });
                    }

                    if (!string.IsNullOrEmpty(numeroBl))
                    {
                        var aux = lstModel.Find(x => x.NumeroBL == numeroBl);

                        if (aux != null && !string.IsNullOrEmpty(aux.NumeroBL))
                            return Json(new { success = true });
                        else
                            return Json(new { success = false, msg = "Informe um Número BL válido." });
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }

            return Json(new { success = false, msg = "Informe uma Proposta Comercial válida." });
        }

        #endregion

        #region ScriptImportar
        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptImportar(MLAgendamentoIntermodalImportacao model)
        {
            return PartialView(model);
        }
        #endregion

        #region ScriptImportarCarga
        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptImportarCarga(MLAgendamentoIntermodalImportacao model)
        {
            return PartialView(model);
        }
        #endregion

        #region Importar Carga
        /// <summary>
        ///Escolher Tipo
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ImportarCarga(MLAgendamentoIntermodalImportacao model)
        {
            return PartialView(model);
        }
        #endregion
         
        #region Importar Carga DTA
        /// <summary>
        ///Escolher Tipo
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ImportarCargaDTA(MLAgendamentoIntermodalImportacao model)
        {
            return PartialView(model);
        }
        #endregion

        #region Salvar Importação Carga
        /// <summary>
        /// salvar carga da exportacao
        /// </summary>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult SalvarImportacaoCarga(MLAgendamentoIntermodalImportacaoCarga model, string guid)
        {
            try
            {
                var portal = PortalAtual.Obter;

                var lista = CRUD.Listar(new MLAgendamentoIntermodalImportacaoCarga { CodigoImportacao = model.CodigoImportacao });

                if (lista?.Count <= 50)
                {
                    //Impedir duplicidade
                    var listaDuplicidade = CRUD.Listar(new MLAgendamentoIntermodalImportacaoCarga { CodigoImportacao = model.CodigoImportacao, Container = model.Container }).FindAll(obj => obj.isIntegrado == null);

                    if (listaDuplicidade == null || listaDuplicidade.Count <= 0)
                    {
                        if (!string.IsNullOrEmpty(model.ValorNfeFormatado))
                            model.ValorNfe = Convert.ToDecimal(model.ValorNfeFormatado.Replace("R$ ", "").Replace(".", ""));
                        model.DataRegistro = DateTime.Now;

                        if (!string.IsNullOrEmpty(guid))
                        {
                            var diretorioNfTemp = (BLConfiguracao.Pastas.ModuloImportacaoNfTemp(portal.Diretorio) + "/" + guid + "/").Replace("//", "/");
                            var pastaNfTemp = HttpContextFactory.Current.Server.MapPath(diretorioNfTemp);

                            if (Directory.Exists(pastaNfTemp))
                            {
                                var diretorioNf = (BLConfiguracao.Pastas.ModuloImportacaoNf(portal.Diretorio, model.CodigoImportacao.Value.ToString()) + "/").Replace("//", "/");
                                var pastaNf = HttpContextFactory.Current.Server.MapPath(diretorioNf);
                                if (!Directory.Exists(pastaNf)) Directory.CreateDirectory(pastaNf);

                                DirectoryInfo Dir = new DirectoryInfo(pastaNfTemp);
                                // Busca automaticamente todos os arquivos em todos os subdiretórios
                                FileInfo[] Files = Dir.GetFiles("*", SearchOption.AllDirectories);
                                foreach (FileInfo file in Files)
                                {
                                    string destino = pastaNf + file.Name;
                                    file.CopyTo(destino, true);

                                    model.Arquivo = file.Name;
                                    model.caminhoCompleto = destino;

                                    file.Delete();
                                }
                                Directory.Delete(pastaNfTemp.TrimEnd('\\'));
                            }
                        }

                        model.Codigo = CRUD.Salvar(model, portal.ConnectionString);


                        model.Sequencia = Convert.ToInt32(model.Sequencia).ToString("00");
                        model.ProximaSequencia = (Convert.ToInt32(model.Sequencia) + 1).ToString("00");

                        if (string.IsNullOrEmpty(model.Comentario))
                            model.Comentario = @T("Sem comentários");

                        if (string.IsNullOrEmpty(model.caminhoCompleto))
                        {
                            model.Arquivo = string.Empty;
                            model.caminhoCompleto = string.Empty;
                        }

                        return Json(new { success = true, model = model });
                    }

                    return Json(new { success = false, model = model, msg = string.Format(@T("O container {0} já  foi registrado!"), model.Container)});
                }
                else
                    model.Codigo = 0;

                return Json(new { success = false, msg = @T("Limite de 50 container atingido.") });

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        #endregion

        #region Excluir Importar Carga
        /// <summary>
        /// excluir importar carga
        /// </summary>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult ExcluirImportarCarga(string Codigos)
        {
            try
            {
                Codigos = Codigos.TrimEnd(',');

                foreach (var codigo in Codigos.Split(','))
                {
                    if (!string.IsNullOrEmpty(codigo))
                        CRUD.Excluir(new MLAgendamentoIntermodalImportacaoCarga { Codigo = Convert.ToDecimal(codigo) });
                }

                return Json(new { success = true });

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Salvar Importação Carga VariasNf
        /// <summary>
        /// salvar carga da exportacao
        /// </summary>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult SalvarImportacaoCargaVariasNf(MLAgendamentoIntermodalImportacaoCargaVariasNf model)
        {
            try
            {
                var portal = PortalAtual.Obter;

                var lista = CRUD.Listar(new MLAgendamentoIntermodalImportacaoCarga { CodigoImportacao = model.CodigoImportacao });
                var isContainer = false;

                if (lista?.Count + model.LstNfs.Count <= 50)
                {
                    int seq = 0;
                    foreach(var item in model.LstNfs)
                    {
                        seq++;
                        if (!item.IsLinhaExcluida)
                        {
                            //Impedir duplicidade
                            var listaDuplicidade = CRUD.Listar(new MLAgendamentoIntermodalImportacaoCarga { CodigoImportacao = model.CodigoImportacao, Container = model.Container });

                            if (listaDuplicidade == null || (listaDuplicidade.Count <= 0 || isContainer))
                            {
                                isContainer = true;
                                item.CodigoImportacao = model.CodigoImportacao;
                                item.DataEntrega = model.DataEntrega;
                                item.Container = model.Container;
                                item.Comentario = "";
                                if (!string.IsNullOrEmpty(model.Comentario))
                                    item.Comentario = model.Comentario;
                                else
                                    item.Comentario = "Sem comentários";

                                if (!string.IsNullOrEmpty(item.ValorNfeFormatado))
                                    item.ValorNfe = Convert.ToDecimal(item.ValorNfeFormatado.Replace("R$ ", "").Replace(".", ""));
                                item.DataRegistro = DateTime.Now;

                                if (!string.IsNullOrEmpty(model.guid))
                                {
                                    var diretorioNfTemp = (BLConfiguracao.Pastas.ModuloImportacaoNfTemp(portal.Diretorio) + "/" + model.guid + "/" + seq + "/").Replace("//", "/");
                                    var pastaNfTemp = HttpContextFactory.Current.Server.MapPath(diretorioNfTemp);

                                    if (Directory.Exists(pastaNfTemp))
                                    {
                                        var diretorioNf = (BLConfiguracao.Pastas.ModuloImportacaoNf(portal.Diretorio, model.CodigoImportacao.Value.ToString()) + "/").Replace("//", "/");
                                        var pastaNf = HttpContextFactory.Current.Server.MapPath(diretorioNf);
                                        if (!Directory.Exists(pastaNf)) Directory.CreateDirectory(pastaNf);

                                        DirectoryInfo Dir = new DirectoryInfo(pastaNfTemp);
                                        // Busca automaticamente todos os arquivos em todos os subdiretórios
                                        FileInfo[] Files = Dir.GetFiles("*", SearchOption.AllDirectories);
                                        foreach (FileInfo file in Files)
                                        {
                                            string destino = pastaNf + file.Name;
                                            file.CopyTo(destino, true);

                                            item.Arquivo = file.Name;
                                            item.caminhoCompleto = destino;

                                            file.Delete();
                                        }
                                        Directory.Delete(pastaNfTemp.TrimEnd('\\'));
                                    }
                                }

                                item.Codigo = CRUD.Salvar(item, portal.ConnectionString);

                                if (string.IsNullOrEmpty(item.caminhoCompleto))
                                {
                                    item.Arquivo = "";
                                    item.caminhoCompleto = "";
                                }
                            }
                            else
                            {
                                return Json(new { success = false, msg = @T("O container informado já foi utilizado.") });
                            }
                        }                       
                    }

                    if (string.IsNullOrEmpty(model.Comentario))
                        model.Comentario = "Sem comentários";

                    model.Sequencia = Convert.ToInt32(model.Sequencia).ToString("00");
                    model.ProximaSequencia = (Convert.ToInt32(model.Sequencia) + 1).ToString("00");

                    model.LstNfs.RemoveAll(x => x.IsLinhaExcluida);

                    return Json(new { success = true, model = model });
                }
                else
                    model.CodigoImportacao = 0;

                return Json(new { success = false, msg = @T("Limite de 50 container atingido.") });

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        #endregion

        #region Salvar Importação Carga VariosContainer
        /// <summary>
        /// salvar carga da exportacao
        /// </summary>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult SalvarImportacaoCargaVariosContainer(MLAgendamentoIntermodalImportacaoCargaVariosContainer model)
        {
            try
            {
                var portal = PortalAtual.Obter;
                string strArquivo = null;
                string destino = "";

                var lista = CRUD.Listar(new MLAgendamentoIntermodalImportacaoCarga { CodigoImportacao = model.CodigoImportacao });

                if (lista?.Count + model.LstContainer.Count <= 50)
                {
                    foreach (var item in model.LstContainer)
                    {
                        if (!item.IsLinhaExcluida)
                        {
                            //Impedir duplicidade
                            var listaDuplicidade = CRUD.Listar(new MLAgendamentoIntermodalImportacaoCarga { CodigoImportacao = model.CodigoImportacao, Container = item.Container });

                            if (listaDuplicidade == null || listaDuplicidade.Count <= 0)
                            {
                                item.CodigoImportacao = model.CodigoImportacao;
                                item.NumeroNfe = model.NumeroNfe;
                                item.ValorNfeFormatado = model.ValorNfe;

                                if (!string.IsNullOrEmpty(model.ValorNfe))
                                    item.ValorNfe = Convert.ToDecimal(model.ValorNfe.Replace("R$ ", "").Replace(".", ""));
                                item.DataRegistro = DateTime.Now;

                                
                                if (!string.IsNullOrEmpty(model.guid))
                                {
                                    var diretorioNfTemp = (BLConfiguracao.Pastas.ModuloImportacaoNfTemp(portal.Diretorio) + "/" + model.guid + "/").Replace("//", "/");
                                    var pastaNfTemp = HttpContextFactory.Current.Server.MapPath(diretorioNfTemp);

                                    if (Directory.Exists(pastaNfTemp))
                                    {
                                        var diretorioNf = (BLConfiguracao.Pastas.ModuloImportacaoNf(portal.Diretorio, model.CodigoImportacao.Value.ToString()) + "/").Replace("//", "/");
                                        var pastaNf = HttpContextFactory.Current.Server.MapPath(diretorioNf);
                                        if (!Directory.Exists(pastaNf)) Directory.CreateDirectory(pastaNf);

                                        DirectoryInfo Dir = new DirectoryInfo(pastaNfTemp);
                                        // Busca automaticamente todos os arquivos em todos os subdiretórios
                                        FileInfo[] Files = Dir.GetFiles("*", SearchOption.AllDirectories);
                                        foreach (FileInfo file in Files)
                                        {
                                            destino = pastaNf + file.Name;
                                            file.CopyTo(destino, true);

                                            strArquivo = file.Name;

                                            file.Delete();
                                        }
                                        Directory.Delete(pastaNfTemp);
                                    }
                                }

                                item.Arquivo = strArquivo;
                                item.Codigo = CRUD.Salvar(item, portal.ConnectionString);

                                item.caminhoCompleto = destino;

                                if (string.IsNullOrEmpty(item.Comentario))
                                    item.Comentario = "Sem comentários";
                            }

                            item.ProximaSequencia = (Convert.ToInt32(item.Sequencia) + 1).ToString("00");
                        }
                    }

                    if (string.IsNullOrEmpty(strArquivo))
                    {
                        model.Arquivo = "";
                        model.caminhoCompleto = "";
                    }
                    else
                    {
                        model.Arquivo = strArquivo;
                        model.caminhoCompleto = destino;
                    }

                    model.LstContainer.RemoveAll(x => x.IsLinhaExcluida);

                    return Json(new { success = true, model = model });
                }
                else
                    model.CodigoImportacao = 0;

                return Json(new { success = false, msg = @T("Limite de 50 container atingido.") });

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        #endregion

        #region IntegrarImportar
        /// <summary>
        /// salvar primeira etapa exportacao
        /// </summary>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        [ValidateInput(false)]
        public JsonResult IntegrarImportar(decimal codigo, string Html, string tipo)
        {
            try
            {
                var portal = PortalAtual.Obter;
                var objModelImportacao = CRUD.Obter<MLAgendamentoIntermodalImportacao>(codigo, portal.ConnectionString);
                var model = new MLAgendamentoIntermodal();

                if (objModelImportacao != null && objModelImportacao.Codigo.HasValue)
                {
                    var cliente = IntegrarCliente(new MLAgendamentoIntermodal
                        {
                            Nome = objModelImportacao.Nome,
                            Codigo = objModelImportacao.Codigo,
                            Email = objModelImportacao.Email, 
                            Tipo = tipo == "Importar" ? "Importação" : "Importação DTA"
                    },
                        "AgendamentoImportar_"
                    );

                    if (!string.IsNullOrEmpty(cliente))
                    {
                        var objRetorno = JsonConvert.DeserializeObject<MLIntegrar>(cliente);

                        model = new MLAgendamentoIntermodal
                        {
                            Nome = objModelImportacao.Nome,
                            Email = objModelImportacao.Email,
                            NumeroBL = objModelImportacao.NumeroBL,
                            NumeroBooking = objModelImportacao.NumeroBooking,
                            PropostaComercial = objModelImportacao.PropostaComercial,
                            CNPJ = objModelImportacao.CNPJ,
                            CEP = objModelImportacao.CEP,
                            Endereco = objModelImportacao.Endereco,
                            Complemento = objModelImportacao.Complemento,
                            Bairro = objModelImportacao.Bairro,
                            Cidade = objModelImportacao.Cidade,
                            Estado = objModelImportacao.Estado,
                            Codigo = objModelImportacao.Codigo
                        };

                        model.lstCarga = new List<MLAgendamentoIntermodalImportacaoCarga>();

                        var lstNf = CRUD.Listar(new MLAgendamentoIntermodalImportacaoCarga { CodigoImportacao = codigo }, portal.ConnectionString).FindAll(obj => obj.isIntegrado == null);
                        var diretorioNf = Request.Url.Scheme + "://" + Request.Url.Authority + (BLConfiguracao.Pastas.ModuloImportacaoNf(portal.Diretorio, codigo.ToString()) + "/").Replace("//", "/");
                        //var pastaNf = HttpContextFactory.Current.Server.MapPath(diretorioNf);
                        foreach(var nf in lstNf)
                        {
                            if (!string.IsNullOrEmpty(nf.Arquivo))
                                model.lstCarga.Add(new MLAgendamentoIntermodalImportacaoCarga { Arquivo = nf.Arquivo, caminhoCompleto = diretorioNf + nf.Arquivo });
                        }

                        var lstDeclaracaoImportacao = CRUD.Listar(new MLAgendamentoIntermodalArquivoDeclaracaoImportacao { CodigoImportacao = codigo }, portal.ConnectionString).FindAll(obj => obj.isIntegrado == null); ;
                        var diretorioDeclaracao = Request.Url.Scheme + "://" + Request.Url.Authority + (BLConfiguracao.Pastas.ModuloImportacaoDeclaracao(portal.Diretorio, codigo.ToString()) + "/").Replace("//", "/");
                        //var pastaDeclaracao = HttpContextFactory.Current.Server.MapPath(diretorioDeclaracao);
                        foreach (var item in lstDeclaracaoImportacao)
                        {
                            if (!string.IsNullOrEmpty(item.Arquivo))
                                model.lstCarga.Add(new MLAgendamentoIntermodalImportacaoCarga { Arquivo = item.Arquivo, caminhoCompleto = diretorioDeclaracao + item.Arquivo });
                        }

                        var lstGuia = CRUD.Listar(new MLAgendamentoIntermodalArquivoGare { CodigoImportacao = codigo }, portal.ConnectionString).FindAll(obj => obj.isIntegrado == null);
                        var diretorioGuia = Request.Url.Scheme + "://" + Request.Url.Authority + (BLConfiguracao.Pastas.ModuloImportacaoGuiaArrecadacao(portal.Diretorio, codigo.ToString()) + "/").Replace("//", "/");
                       
                        foreach (var item in lstGuia)
                        {
                            if (!string.IsNullOrEmpty(item.Arquivo))
                                model.lstCarga.Add(new MLAgendamentoIntermodalImportacaoCarga { Arquivo = item.Arquivo, caminhoCompleto = diretorioGuia + item.Arquivo });
                        }

                        var lstBl = CRUD.Listar(new MLAgendamentoIntermodalArquivoBl { CodigoImportacao = codigo }, portal.ConnectionString).FindAll(obj => obj.isIntegrado == null);
                        var diretorioBl = Request.Url.Scheme + "://" + Request.Url.Authority + (BLConfiguracao.Pastas.ModuloImportacaoBl(portal.Diretorio, codigo.ToString()) + "/").Replace("//", "/");

                        foreach (var item in lstBl)
                        {
                            if (!string.IsNullOrEmpty(item.Arquivo))
                                model.lstCarga.Add(new MLAgendamentoIntermodalImportacaoCarga { Arquivo = item.Arquivo, caminhoCompleto = diretorioBl + item.Arquivo });
                        }

                        if (!string.IsNullOrEmpty(objRetorno.id)) IntegrarTicket(objRetorno.id, model, Html, tipo == "Importar" ? "Importação" : "Importação DTA");

                        Database.ExecuteNonQuery(new SqlCommand("UPDATE MOD_AIIC_AGENDAMENTO_INTERMODAL_IMPORTACAO_CARGA SET AIIC_B_INTEGRADO = 1 WHERE AIIC_AII_N_CODIGO =" + codigo));
                        Database.ExecuteNonQuery(new SqlCommand("UPDATE MOD_AIDI_ARQUIVO_IMPORTACAO_DECLARACAO_IMPORTACAO SET AIDI_B_INTEGRADO = 1 WHERE AIDI_AII_N_CODIGO =" + codigo));
                        Database.ExecuteNonQuery(new SqlCommand("UPDATE MOD_AIBL_ARQUIVO_IMPORTACAO_BL SET AIBL_B_INTEGRADO = 1 WHERE AIBL_AII_N_CODIGO =" + codigo));
                        Database.ExecuteNonQuery(new SqlCommand("UPDATE MOD_AIGE_ARQUIVO_IMPORTACAO_GARE SET AIGE_B_INTEGRADO = 1 WHERE AIGE_AII_N_CODIGO =" + codigo));

                        return Json(new { success = true });
                    }
                    else
                    {
                        var objModel = new MLAgendamentoTicket
                        {
                            type = 2,
                            subject = "Agendamento Intermodal –" + tipo + " – " + model.NumeroBooking + " ",
                            category = "Service Request",
                            urgency = "Normal",
                            ownerTeam = "DS-CX-CI@",
                            createdDate = DateTime.Now,
                            serviceFirstLevelId = "716767"
                        };

                        #region cliente
                        objModel.clients = new List<Client>();
                        objModel.clients.Add(new Client
                        {
                            id = "AgendamentoImportar_" + model.Codigo, 
                            personType = 1,
                            profileType = 2,
                            businessName = model.Nome
                        });
                        #endregion

                        #region ação
                        objModel.actions = new List<CMSv4.Model.Action>();
                        objModel.actions.Add(new CMSv4.Model.Action
                        {
                            type = 2,
                            origin = 19,
                            description = Html,
                            justification = "Justificativa",
                            createdDate = DateTime.Now
                        });
                        #endregion

                        #region create
                        objModel.createdBy = new Createdby
                        {
                            id = "AgendamentoImportar_" + model.Codigo,
                            personType = 1,
                            profileType = 2,
                            businessName =  model.Nome,
                            email = model.Email
                        };
                        #endregion

                        #region campos adicionais
                        //objModel.customFieldValues = new List<CustomFieldValue>();
                        objModel.customFieldValues.Add(new Customfieldvalue
                        {
                            customFieldId = BLConfiguracao.CodigoPropostaComercial,
                            customFieldRuleId = BLConfiguracao.CodigoCustomFieldRule,
                            line = 1,
                            value = model.PropostaComercial
                        });

                        objModel.customFieldValues.Add(new Customfieldvalue
                        {
                            customFieldId = BLConfiguracao.CodigoBookingNumber,
                            customFieldRuleId = BLConfiguracao.CodigoCustomFieldRule,
                            line = 1,
                            value = model.NumeroBooking
                        });

                        if (!string.IsNullOrEmpty(objModelImportacao.NumeroBL))
                        {
                            objModel.customFieldValues.Add(new Customfieldvalue
                            {
                                customFieldId = BLConfiguracao.CodigoNumeroBl,
                                customFieldRuleId = BLConfiguracao.CodigoCustomFieldRule,
                                line = 1,
                                value = model.NumeroBL
                            });
                        }

                        objModel.customFieldValues.Add(new Customfieldvalue
                        {
                            customFieldId = BLConfiguracao.CodigoLocalColeta,
                            customFieldRuleId = BLConfiguracao.CodigoCustomFieldRule,
                            line = 1,
                            value = Endereco(model)
                        });

                        #endregion

                        var jsonSerialize = JsonConvert.SerializeObject(objModel);

                        #region Grava o json na nossa base
                        CRUD.Salvar<MLAgendamentoIntermodalLog>(new MLAgendamentoIntermodalLog
                        {
                            DataCadastro = DateTime.Now,
                            Json = jsonSerialize,
                            Tipo = tipo,
                            isIntegrado = false,
                            Imagem = string.Join(",", model?.lstCarga.Select(obj => obj.caminhoCompleto)),
                            RetornoAPI = "Error User",

                        });
                        #endregion
                    }
                }

                Database.ExecuteNonQuery(new SqlCommand("UPDATE MOD_AIIC_AGENDAMENTO_INTERMODAL_IMPORTACAO_CARGA SET AIIC_B_INTEGRADO = 1 WHERE AIIC_AII_N_CODIGO =" + codigo));
                Database.ExecuteNonQuery(new SqlCommand("UPDATE MOD_AIDI_ARQUIVO_IMPORTACAO_DECLARACAO_IMPORTACAO SET AIDI_B_INTEGRADO = 1 WHERE AIDI_AII_N_CODIGO =" + codigo));
                Database.ExecuteNonQuery(new SqlCommand("UPDATE MOD_AIBL_ARQUIVO_IMPORTACAO_BL SET AIBL_B_INTEGRADO = 1 WHERE AIBL_AII_N_CODIGO =" + codigo));
                Database.ExecuteNonQuery(new SqlCommand("UPDATE MOD_AIGE_ARQUIVO_IMPORTACAO_GARE SET AIGE_B_INTEGRADO = 1 WHERE AIGE_AII_N_CODIGO =" + codigo));

                return Json(new { success = false });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message, codigo = 0 });
            }
        }

        #endregion

        #region Importar Sucesso
        /// <summary>
        /// Exportar Sucesso
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ImportarSucesso(MLAgendamentoIntermodalImportacao model)
        {
            return PartialView(new MLAgendamentoIntermodalImportacao());
        }
        #endregion

        #region ExcluirArquivoDeclaracao
        /// <summary>
        /// Upload
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        [HttpPost]
        public ActionResult uploadDeclaracaoImportacao(string guid)
        {
            try
            {
                string nomeArquivo = "";
                if(Request.Files.Count > 0)
                {
                    if (string.IsNullOrEmpty(guid))
                        guid = Guid.NewGuid().ToString();

                    var portal = PortalAtual.Obter;

                    var diretorio = (BLConfiguracao.Pastas.ModuloImportacaoDeclaracaoTemp(portal.Diretorio) + "/" + guid + "/").Replace("//", "/");
                    var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);
                    if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);

                    foreach (string fileName in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[fileName];
                        nomeArquivo = file.FileName;

                        file.SaveAs(Path.Combine(pasta, file.FileName));
                    }
                }

                return Json(new { success = true, Guid = guid, NomeArquivo = nomeArquivo, NomeLinha = nomeArquivo.Replace(" ", "").Replace("/", "").Replace(@"\", "").Replace(".", "-") });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        #endregion

        #region uploadDeclaracaoImportacao
        /// <summary>
        /// Upload
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        [HttpPost]
        public ActionResult ExcluirArquivoDeclaracao(string guid, string NomeArquivo)
        {
            try
            {

                var portal = PortalAtual.Obter;

                var diretorio = (BLConfiguracao.Pastas.ModuloImportacaoDeclaracaoTemp(portal.Diretorio) + "/" + guid + "/").Replace("//", "/");
                var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);

                System.IO.File.Delete(Path.Combine(pasta, NomeArquivo));

                return Json(new { success = true, qtdeFiles = Directory.GetFiles(pasta).Length.ToString() });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        #endregion

        #region uploadGuiaArrecadacao
        /// <summary>
        /// Upload
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        [HttpPost]
        public ActionResult uploadGuiaArrecadacao(string guid)
        {
            try
            {
                string nomeArquivo = "";

                if (Request.Files.Count > 0)
                {
                    if (string.IsNullOrEmpty(guid))
                        guid = Guid.NewGuid().ToString();

                    var portal = PortalAtual.Obter;

                    var diretorio = (BLConfiguracao.Pastas.ModuloImportacaoGuiaArrecadacaoTemp(portal.Diretorio) + "/" + guid + "/").Replace("//", "/");
                    var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);
                    if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);

                    foreach (string fileName in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[fileName];
                        nomeArquivo = file.FileName;

                        file.SaveAs(Path.Combine(pasta, file.FileName));
                    }
                }

                return Json(new { success = true, Guid = guid, NomeArquivo = nomeArquivo, NomeLinha = nomeArquivo.Replace(" ", "").Replace("/", "").Replace(@"\", "").Replace(".", "-") });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        #endregion

        #region ExcluirArquivoGare
        /// <summary>
        /// Upload
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        [HttpPost]
        public ActionResult ExcluirArquivoGare(string guid, string NomeArquivo)
        {
            try
            {
                var portal = PortalAtual.Obter;

                var diretorio = (BLConfiguracao.Pastas.ModuloImportacaoGuiaArrecadacaoTemp(portal.Diretorio) + "/" + guid + "/").Replace("//", "/");
                var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);

                System.IO.File.Delete(Path.Combine(pasta, NomeArquivo));

                return Json(new { success = true, qtdeFiles = Directory.GetFiles(pasta).Length.ToString() });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        #endregion

        #region uploadBL
        /// <summary>
        /// Upload
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        [HttpPost]
        public ActionResult uploadBL(string guid)
        {
            try
            {
                string nomeArquivo = "";

                if (Request.Files.Count > 0)
                {
                    if (string.IsNullOrEmpty(guid))
                        guid = Guid.NewGuid().ToString();

                    var portal = PortalAtual.Obter;

                    var diretorio = (BLConfiguracao.Pastas.ModuloImportacaoBlTemp(portal.Diretorio) + "/" + guid + "/").Replace("//", "/");
                    var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);
                    if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);

                    foreach (string fileName in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[fileName];
                        nomeArquivo = file.FileName;

                        file.SaveAs(Path.Combine(pasta, file.FileName));
                    }
                }

                return Json(new { success = true, Guid = guid, NomeArquivo = nomeArquivo, NomeLinha = nomeArquivo.Replace(" ", "").Replace("/", "").Replace(@"\", "").Replace(".", "-") });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        #endregion

        #region ExcluirArquivoBl
        /// <summary>
        /// Upload
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        [HttpPost]
        public ActionResult ExcluirArquivoBl(string guid, string NomeArquivo)
        {
            try
            {
                var portal = PortalAtual.Obter;

                var diretorio = (BLConfiguracao.Pastas.ModuloImportacaoBlTemp(portal.Diretorio) + "/" + guid + "/").Replace("//", "/");
                var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);

                System.IO.File.Delete(Path.Combine(pasta, NomeArquivo));

                return Json(new { success = true, qtdeFiles = Directory.GetFiles(pasta).Length.ToString() });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        #endregion

        #region uploadContainerNf
        /// <summary>
        /// Upload
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        [HttpPost]
        public ActionResult uploadContainerNf(string guid, int? seq)
        {
            try
            {
                string nome = "";
                if (Request.Files.Count > 0)
                {
                    if (string.IsNullOrEmpty(guid))
                        guid = Guid.NewGuid().ToString();

                    var portal = PortalAtual.Obter;

                    string complemento = "";
                    if (seq.HasValue && seq.Value > 0)
                        complemento = seq.ToString() + "/";

                    var diretorio = (BLConfiguracao.Pastas.ModuloImportacaoNfTemp(portal.Diretorio) + "/" + guid + "/" + complemento).Replace("//", "/");
                    var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);
                    if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);

                    foreach (string fileName in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[fileName];

                        file.SaveAs(Path.Combine(pasta, file.FileName));

                        nome = file.FileName;
                    }
                }

                return Json(new { success = true, Guid = guid, nomeArquivo = nome });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        #endregion

        #region ExcluirArquivoNf
        /// <summary>
        /// Upload
        /// </summary>        
        [CheckPermission(global::Permissao.Publico)]
        [HttpPost]
        public ActionResult ExcluirArquivoNf(string guid, string NomeArquivo, int? seq)
        {
            try
            {
                var portal = PortalAtual.Obter;

                string complemento = "";
                if (seq.HasValue && seq.Value > 0)
                    complemento = seq.ToString() + "/";

                var diretorio = (BLConfiguracao.Pastas.ModuloImportacaoNfTemp(portal.Diretorio) + "/" + guid + "/" + complemento).Replace("//", "/");
                var pasta = HttpContextFactory.Current.Server.MapPath(diretorio);

                System.IO.File.Delete(Path.Combine(pasta, NomeArquivo));

                return Json(new { success = true, qtdeFiles = Directory.GetFiles(pasta).Length.ToString() });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        #endregion

        #region GetHash
        /// <summary>
        /// GetHash
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string GetHash(string file)
        {
            /*var arquivoBytes = System.IO.File.ReadAllBytes(file);

            return Convert.ToBase64String(arquivoBytes);*/

            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(file));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }
        #endregion

        #region DownloadFile
        /// <summary>
        /// Metodo para fazer o download de NF
        /// </summary>
        /// <param name="codigoPedido"></param>
        /// <param name="extensao"></param>
        /// <param name="isNFePeople"></param>
        /// <returns></returns>
        [HttpGet]
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult DownloadFile(decimal codigo, string arquivo)
        {
            try
            {
                var portal = PortalAtual.Obter;
                var diretorioNf = (BLConfiguracao.Pastas.ModuloImportacaoNf(portal.Diretorio, codigo.ToString()) + "/").Replace("//", "/");
                var pastaNf = HttpContextFactory.Current.Server.MapPath(diretorioNf);

                var filepath = pastaNf + arquivo;

                byte[] filedata = System.IO.File.ReadAllBytes(filepath);
                string contentType = MimeMapping.GetMimeMapping(filepath);

                
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = arquivo,
                    Inline = false,
                };

                return File(filedata, contentType, arquivo);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return View();
            }
        }
        #endregion
    }
}
