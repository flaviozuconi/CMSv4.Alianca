using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using CMSv4.Model;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class ResultadoAdminController : SecurePortalController
    {
        //
        // GET: /ModuloAdmin/ResultadoAdmin/
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Index()
        {
            return View();
        }


        #region Listar

        /// <summary>
        /// Listagem
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller
        ///     /Area/Controller/Listar
        ///     /Area/Controller/Listar?parametro=1 & page=1 & limit=30 & sort= {JSON}
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Listar(MLResultado criterios)
        {
            try
            {
                var retorno = CRUD.ListarJson(criterios, Request.QueryString, PortalAtual.ConnectionString);
                return retorno;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region Item

        /// <summary>
        /// Visualizar ou Editar o registro conforme permissão do usuário
        /// </summary>
        /// <param name="id">Código do registro</param>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/Item/id
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Item(decimal? id)
        {
            try
            {
                var portal = PortalAtual.Obter;
                MLResultado model = null;
                if (id.HasValue) model = CRUD.Obter<MLResultado>(new MLResultado { Codigo = id }, portal.ConnectionString);

                if (model == null)
                {
                    model = new MLResultado();
                    model.Idioma = PortalAtual.Obter.CodigoIdioma;
                }

                if (!string.IsNullOrEmpty(model.Conteudo))
                {
                    model.Conteudo = Microsoft.JScript.GlobalObject.unescape(model.Conteudo);
                }
                
                ViewData["listaIdioma"] = CRUD.Listar<MLIdioma>(new MLIdioma { Ativo = true });


                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Salvar registro
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Item(MLResultado model, HttpPostedFileBase fileValueBook, HttpPostedFileBase fileReleaseResultados,
            HttpPostedFileBase fileDfp, HttpPostedFileBase fileAudio, HttpPostedFileBase fileTranscricao, HttpPostedFileBase fileApresentação)
        {
            var portal = PortalAtual.Obter;
            try
            {
                if (ModelState.IsValid)
                {
                    //model.CodigoPortal = portal.Codigo;

                    model.Codigo = CRUD.SalvarParcial<MLResultado>(model, portal.ConnectionString);

                    var diretorio = (BLConfiguracao.Pastas.ModuloResultado(portal.Diretorio) + "/" + model.Codigo).Replace("//", "/");
                    var pasta = Server.MapPath(diretorio);
                    if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);
                   //valuebook
                    if (fileValueBook != null && fileValueBook.ContentLength > 0)
                    {
                        fileValueBook.SaveAs(Path.Combine(pasta, fileValueBook.FileName));
                        model.ValueBook = fileValueBook.FileName;
                    }

                    //ReleaseResultados
                    if (fileReleaseResultados != null && fileReleaseResultados.ContentLength > 0)
                    {
                        fileReleaseResultados.SaveAs(Path.Combine(pasta, fileReleaseResultados.FileName));
                        model.ReleaseResultados = fileReleaseResultados.FileName;
                    }

                    //Dfp
                    if (fileDfp != null && fileDfp.ContentLength > 0)
                    {
                        fileDfp.SaveAs(Path.Combine(pasta, fileDfp.FileName));
                        model.Dfp = fileDfp.FileName;
                    }

                    //Audio
                    if (fileAudio != null && fileAudio.ContentLength > 0)
                    {
                        fileAudio.SaveAs(Path.Combine(pasta, fileAudio.FileName));
                        model.Audio = fileAudio.FileName;
                    }
                    
                    //trancrição
                    if (fileTranscricao != null && fileTranscricao.ContentLength > 0)
                    {
                        fileTranscricao.SaveAs(Path.Combine(pasta, fileTranscricao.FileName));
                        model.Transcricao = fileTranscricao.FileName;
                    }

                    //trancrição
                    if (fileApresentação != null && fileApresentação.ContentLength > 0)
                    {
                        fileApresentação.SaveAs(Path.Combine(pasta, fileApresentação.FileName));
                        model.Apresentacao = fileApresentação.FileName;
                    }

                    model.Codigo = CRUD.SalvarParcial<MLResultado>(model, portal.ConnectionString);

                    TempData["Salvo"] = model.Codigo > 0;
                }

                return RedirectToAction("Index");
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
        /// Excluir registro
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/Excluir/id
        /// </remarks>
        [CheckPermission(global::Permissao.Excluir)]
        [HttpPost]
        public ActionResult Excluir(List<string> ids)
        {
            try
            {
                foreach (var item in ids)
                {
                    CRUD.Excluir<MLResultado>(Convert.ToDecimal(item), PortalAtual.ConnectionString);
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


      

    }
}
