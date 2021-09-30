using System;
using System.Web.Mvc;
using Framework.Utilities;
using VM2.Areas.CMS.Helpers;
using CMSv4.Model;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class NewsletterController : ModuloBaseController<MLModuloNewsletterEdicao, MLModuloNewsletterHistorico, MLModuloNewsletterPublicado>
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

                var model = new MLModuloNewsletter();

                if (edicao.HasValue && edicao.Value)
                {
                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloNewsletterEdicao>(new MLModuloNewsletterEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, BLPortal.Atual.ConnectionString);
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    var historico = CRUD.Obter<MLModuloNewsletterHistorico>(new MLModuloNewsletterHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, BLPortal.Atual.ConnectionString);
                    if (historico != null)
                    {
                        model = new MLModuloNewsletterEdicao
                        {
                            CodigoPagina = historico.CodigoPagina,
                            CodigoUsuario = historico.CodigoUsuario,
                            DataRegistro = historico.DataRegistro,
                            Repositorio = historico.Repositorio,
                            Titulo = historico.Titulo
                        };
                    }
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloNewsletterPublicado>(new MLModuloNewsletterPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, BLPortal.Atual.ConnectionString);
                }

                if (model == null)
                {
                    //Configuração Default
                    MLModuloNewsletterEdicao objModel = new MLModuloNewsletterEdicao();
                    objModel.CodigoPagina = codigoPagina;
                    objModel.Repositorio = repositorio;
                    objModel.DataRegistro = DateTime.Now;
                    objModel.CodigoUsuario = BLUsuario.ObterLogado().Codigo;

                    CRUD.Salvar(objModel, BLPortal.Atual.ConnectionString);
                    return PartialView("Index", objModel);
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
        /// Salvar
        /// </summary>
        [JsonHandleError]
        public override ActionResult Editar(MLModuloNewsletterEdicao model)
        {
            model.CodigoUsuario = BLUsuario.ObterLogado().Codigo;
            model.DataRegistro = DateTime.Now;

            CRUD.Salvar(model, BLPortal.Atual.ConnectionString);

            return Json(new { success = true });
        }

        #endregion

        // SALVAR

        #region Salvar

        [HttpPost]
        [JsonHandleError]
        [ValidateAntiForgeryToken()]
        [CheckPermission(global::Permissao.Publico)]
        public JsonResult Salvar(string nome, string email, string assunto)
        {
            if (ModelState.IsValid)
            {
                var model = new MLNewsletter();

                var portal = BLPortal.Atual;
                var existe = CRUD.Obter<MLNewsletter>(new MLNewsletter() { CodigoPortal = portal.Codigo, Email = email }, portal.ConnectionString);

                if (existe != null && existe.Codigo.HasValue)
                {
                    model = existe;
                }

                model.Nome = nome;
                model.Email = email;
                model.Assuntos = assunto;

                model.CodigoPortal = portal.Codigo;
                model.DataCadastro = DateTime.Now;
                model.ChaveSecreta = Guid.NewGuid();

                CRUD.Salvar(model, portal.ConnectionString);

                return Json(new { success = true });
            }
            return Json(new { success = false });

        }

        #endregion

        #region Script

        [JsonHandleError]
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Script(MLModuloNewsletter model) => PartialView(model);
        
        #endregion

        #region Erro

        public ActionResult Erro() => PartialView();
        
        #endregion
    }
}
