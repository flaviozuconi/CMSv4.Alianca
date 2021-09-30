using Framework.Utilities;
using System;
using System.Web.Mvc;
using VM2.Areas.CMS.Helpers;
using CMSv4.Model;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class CompartilharController : ModuloBaseController<MLModuloCompartilharEdicao, MLModuloCompartilharHistorico, MLModuloCompartilharPublicado>
    {
        [CheckPermission(global::Permissao.Publico)]
        public override ActionResult Index(decimal? codigoPagina, int? repositorio)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                // Visualizar Publicado
                var portal = BLPortal.Atual;
                var model = CRUD.Obter<MLModuloCompartilharPublicado>(new MLModuloCompartilharPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null)
                {
                    model = new MLModuloCompartilharPublicado();
                    model.CodigoPagina = codigoPagina;
                    model.Repositorio = repositorio;
                    model.CodigoPortal = portal.Codigo;
                }

                return PartialView("Index", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        public override ActionResult Visualizar(decimal? codigoPagina, int? repositorio, bool? edicao, string codigoHistorico)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var portal = BLPortal.Atual;
                MLModuloCompartilhar model = null;

                ViewBag.ModoEdicao = false;

                if (edicao.HasValue && edicao.Value)
                {
                    ViewBag.ModoEdicao = true;

                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloCompartilharEdicao>(new MLModuloCompartilharEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    model = CRUD.Obter<MLModuloCompartilharHistorico>(new MLModuloCompartilharHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloCompartilharPublicado>(new MLModuloCompartilharPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null)
                {
                    var usuario = BLUsuario.ObterLogado();

                    model = new MLModuloCompartilharEdicao();
                    model.CodigoPagina = codigoPagina;
                    model.Repositorio = repositorio;
                    model.CodigoPortal = portal.Codigo;
                    model.DataRegistro = DateTime.Now;
                    model.View = "Padrao";

                    if (usuario != null)
                    {
                        model.CodigoUsuario = usuario.Codigo;
                    }

                    CRUD.Salvar(model as MLModuloCompartilharEdicao, portal.ConnectionString);
                }

                return PartialView("Index", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        public override ActionResult Excluir(decimal? codigoPagina, int? repositorio)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;
                var portal = BLPortal.Atual;

                CRUD.Excluir<MLModuloCompartilharEdicao>(codigoPagina.Value, repositorio.Value, portal.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        public override ActionResult Editar(decimal? codigoPagina, int? repositorio, bool? edicao)
        {
            try
            {
                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                var portal = BLPortal.Atual;
                var model = CRUD.Obter<MLModuloCompartilharEdicao>(new MLModuloCompartilharEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, PortalAtual.ConnectionString);

                if (model == null)
                    model = new MLModuloCompartilharEdicao();

                if (!model.CodigoPagina.HasValue) model.CodigoPagina = codigoPagina;
                if (!model.Repositorio.HasValue) model.Repositorio = repositorio;

                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        public override ActionResult Editar(MLModuloCompartilharEdicao model)
        {
            try
            {
                //Salvar
                var portal = BLPortal.Atual;

                if (model.Titulo == null)
                    model.Titulo = string.Empty;

                if (model.Css == null)
                    model.Css = string.Empty;

                CRUD.Salvar(model, portal.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Renderizar(MLModuloCompartilhar model)
        {
            return View(model.View ?? "Padrao", model);
        }

        [HttpPost, CheckPermission(global::Permissao.Publico)]
        public JsonResult Email(string titulo, string chamada, string nome, string email, string nomeAmigo, string emailAmigo, string comentario, string url)
        {
            try
            {
                if (String.IsNullOrEmpty(url))
                    url = Request.Url.ToString();
                
                var model = new MLCompartilharEmail();
                var portal = BLPortal.Atual;

                model.DataCadastro = DateTime.Now;
                model.Email = email;
                model.Nome = nome;
                model.NomeAmigo = nomeAmigo;
                model.EmailAmigo = emailAmigo;
                model.UrlCompartilhada = Server.UrlDecode(url);
                model.Comentario = comentario;
                model.UrlSite = Portal.Url();
                model.CodigoTipo = 2; //páginas

                CRUD.Salvar<MLCompartilharEmail>(model, portal.ConnectionString);

                var htmlEmail = BLEmail.ObterModelo("/templates/email-compartilhar-pagina.html")
                            .Replace("[[nome_amigo]]", nomeAmigo)
                            .Replace("[[nome]]", nome)
                            .Replace("[[email]]", email)
                            .Replace("[[url]]", model.UrlCompartilhada)
                            .Replace("[[SRC_IMAGEM]]", String.Format("{0}/content/img/img-share-default-small.jpg", model.UrlSite))
                            .Replace("[[URL_SITE]]", model.UrlSite)
                            .Replace("[[COMENTARIO]]", !String.IsNullOrEmpty(comentario) ? T("Comentário") + ":<br />" + comentario : "")
                            .Replace("[[DESCRICAO]]", chamada)
                            .Replace("[[TITULO]]", titulo);

                BLEmail.Enviar(
                        String.Format("{0} compartilhou com você!", nome),
                        emailAmigo,
                        htmlEmail
                    );

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
    }
}
