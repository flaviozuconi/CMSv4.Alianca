using System;
using System.Web.Mvc;
using Framework.Utilities;
using VM2.Areas.CMS.Helpers;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class EnqueteController : ModuloBaseController<MLModuloEnqueteEdicao, MLModuloEnqueteHistorico, MLModuloEnquetePublicado>
    {
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
                var model = new MLModuloEnquete();
                MLEnquete objMLEnquete = new MLEnquete();

                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloEnqueteEdicao>(new MLModuloEnqueteEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (model == null) model = new MLModuloEnqueteEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio };
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    var historico = CRUD.Obter<MLModuloEnqueteHistorico>(new MLModuloEnqueteHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                    if (historico != null)
                    { CRUD.CopiarValores(historico, model); }
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloEnquetePublicado>(new MLModuloEnquetePublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null) 
                {   
                    model = new MLModuloEnqueteEdicao();
                    model.CodigoPagina = codigoPagina;
                    model.Repositorio = repositorio;
                    model.CodigoUsuario = BLUsuario.ObterLogado().Codigo;
                    model.DataRegistro = DateTime.Now;
                }

                if (!model.CodigoEnquete.HasValue)
                {
                    var enquete = BLModuloEnquete.ObterMaisRecente(portal.Codigo.Value);

                    if (enquete != null && enquete.CodigoEnquete.HasValue)
                    {
                        CRUD.Salvar(
                            new MLModuloEnqueteEdicao() 
                            { 
                                CodigoPagina = codigoPagina, 
                                Repositorio = repositorio, 
                                CodigoUsuario = BLUsuario.ObterLogado().Codigo,
                                DataRegistro = DateTime.Now,
                                CodigoEnquete = enquete.CodigoEnquete,
                                VotarRestrito = false,
                                ResultadoRestrito = true

                            }, 
                            portal.ConnectionString
                        );
                    }
                    
                }

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
                ViewData["enquetes"] = BLModuloEnquete.Listar(null, true);
                return base.Editar(codigoPagina, repositorio, edicao);
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
        public override ActionResult Editar(MLModuloEnqueteEdicao model)
        {
            try
            {
                model.DataRegistro = DateTime.Now;
                CRUD.Salvar(model, PortalAtual.ConnectionString);

                //Após Salvar, limpa o cache para que a atualização seja aplicada
                //nas páginas públicas
                BLCachePortal.RemoveAll("modEnquete");

                return Json(new
                        {
                            success = true,
                            Repositorio = model.Repositorio,
                            VotarRestrito = model.VotarRestrito,
                            CodigoEnquete = model.CodigoEnquete,
                            CodigoPagina = model.CodigoPagina,
                            ResultadoRestrito = model.ResultadoRestrito,
                            portaldiretorio = BLPortal.Atual.Diretorio,
                            modulo = "enquete"
                        });
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
        public ActionResult Script(MLModuloEnquete model)
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

        /// <summary>
        /// Recarregar scrip após editar configuração do módulo (areaconstrucao.js)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptAreaConstrucao(int Repositorio, bool? VotarRestrito, decimal CodigoEnquete, decimal CodigoPagina, bool? ResultadoRestrito)
        {
            try
            {
                MLModuloEnquete model = new MLModuloEnquete();
                model.VotarRestrito = VotarRestrito;
                model.CodigoEnquete = CodigoEnquete;
                model.Repositorio = Repositorio;
                model.CodigoPagina = CodigoPagina;
                model.ResultadoRestrito = ResultadoRestrito;

                return PartialView("Script", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region ScriptSucesso

        /// <summary>
        /// Adicionar Script
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ScriptSucesso(MLEnqueteResultado model)
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

        #region Enquete

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Enquete(MLModuloEnquete model)
        {
            try
            {
                decimal? decCodigoCliente = null;
                var cliente = BLCliente.ObterLogado();
                string strIP = Request.UserHostAddress;

                if (cliente != null) decCodigoCliente = cliente.Codigo;

                model.Enquete = BLModuloEnquete.ListarResultado(model.CodigoEnquete.Value, decCodigoCliente, strIP, true);

                return View("Enquete", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Content(string.Format("<p style='color: red;'>Erro: {0}</p>", ex.Message));
            }
        }

        #endregion

        #region Resultado
        /// <summary>
        /// Resultado
        /// </summary>
        /// <param name="CodigoEnquete"></param>
        /// <param name="ResultadoRestrito"></param>
        /// <param name="codRepositorio"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Resultado(decimal CodigoEnquete, bool? ResultadoRestrito, int codRepositorio)
        {
            MLModuloEnquete model = new MLModuloEnquete();
            decimal? decCodigoCliente = null;
            var cliente = BLCliente.ObterLogado();

            if (cliente != null && cliente.Codigo.HasValue) decCodigoCliente = cliente.Codigo;

            var resultado = BLModuloEnquete.ListarResultado(CodigoEnquete, decCodigoCliente, Request.UserHostAddress, true);
            model.Repositorio = codRepositorio;
            model.ResultadoRestrito = ResultadoRestrito;
            model.VotarRestrito = resultado.VotarRestrito;
            model.Enquete = resultado;

            if (ResultadoRestrito.HasValue && ResultadoRestrito.Value && !decCodigoCliente.HasValue) return Content("É necessário estar logado para visualizar o resultado desta enquete.");

            model.ResultadoRestrito = ResultadoRestrito;

            return PartialView("Resultado", model);
        }

        #region Controle de Exibição

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ControleExibicaoAjax(decimal codigoPagina, int repositorio)
        {
            MLModuloEnquete model = CRUD.Obter<MLModuloEnquetePublicado>(new MLModuloEnquetePublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, BLPortal.Atual.ConnectionString);
            return ControleExibicao(model);
        }

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ControleExibicao(MLModuloEnquete model)
        {
            try
            {
                decimal? decCodigoCliente = null;
                var cliente = BLCliente.ObterLogado();
                string strIP = Request.UserHostAddress;

                if (cliente != null) decCodigoCliente = cliente.Codigo;

                //Caso não tenha Enquete configurada no módulo, selecionar a mais recente.
                if (!model.CodigoEnquete.HasValue)
                {
                    var ultimapublicada = BLModuloEnquete.UltimaPublicada(BLPortal.Atual.Codigo.Value);
                    if (ultimapublicada.CodigoEnquete.HasValue) model.CodigoEnquete = ultimapublicada.CodigoEnquete.Value;
                }

                if (!model.CodigoEnquete.HasValue)
                    return null;

                //Lista de opções da enquete
                model.Enquete = BLModuloEnquete.ListarResultado(model.CodigoEnquete.Value, decCodigoCliente, strIP, true);

                if (model.Enquete.IsFechada || (model.Enquete.IsVotou.HasValue && model.Enquete.IsVotou.Value))
                //if (model.Enquete.IsFechada)
                {
                    if (model.ResultadoRestrito.HasValue && model.ResultadoRestrito.Value && cliente == null)
                        return Content("É necessário estar logado para visualizar o resultado desta enquete.");

                    return PartialView("Resultado", model);
                }
                else
                    return PartialView("Opcoes", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return PartialView("Resultado", model);
            }
        }

        #endregion

        #endregion

        #region Votar

        [CheckPermission(global::Permissao.Publico)]
        public JsonResult Votar(decimal codigoOpcao, bool? Restrito, decimal codigoEnquete, int Repositorio, decimal pagina, bool? ResultadoRestrito)
        {
            try
            {
                bool IsCadastrar = (!Restrito.HasValue || (Restrito.HasValue && !Restrito.Value));
                decimal? codigoUser = null;
                var user = BLCliente.ObterLogado();

                //Verifica se o cliente esta logado
                if (user != null && user.Codigo.HasValue && user.Codigo.Value > 0)
                {
                    codigoUser = user.Codigo;
                    IsCadastrar = true;
                }

                //Se tiver permissão para participar da enquete
                if (IsCadastrar)
                {
                    MLEnqueteVoto objMLVoto = new MLEnqueteVoto();
                    objMLVoto.DataResposta = DateTime.Now;
                    objMLVoto.IP = Request.UserHostAddress;
                    objMLVoto.CodigoOpcao = codigoOpcao;
                    objMLVoto.CodigoUsuario = codigoUser;

                    CRUD.Salvar<MLEnqueteVoto>(objMLVoto, BLPortal.Atual.ConnectionString);

                    var model = BLModuloEnquete.ListarResultado(codigoEnquete, codigoUser, Request.UserHostAddress, false);
                    model.Repositorio = Repositorio;
                    model.CodigoPagina = pagina;
                    model.ResultadoRestrito = ResultadoRestrito;

                    var conteudo = BLConteudoHelper.RenderPartialViewToString(this, "Modulo", "Enquete", "Sucesso", new MLModuloEnquete() { Repositorio = Repositorio });

                    return Json(new { success = true, conteudo = conteudo });
                }

                return Json(new { success = false, conteudo = T("É necessário estar logado para participar da enquete.") });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, conteudo = T("Não foi possível contabilizar seu voto.") });
            }
        }

        #endregion

        #region Sucesso

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Sucesso(MLEnqueteResultado model)
        {
            return PartialView("Sucesso", model);
        }

        #endregion
    }
}
