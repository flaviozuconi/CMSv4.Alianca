using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class BannerAdminController : AdminBaseCRUDPortalController<MLBanner, MLBanner>
    {
        #region Banner

        #region Excluir

        [JsonHandleError, CheckPermission(global::Permissao.Excluir)]
        public override ActionResult Excluir(List<string> ids) => Json(new { Sucesso = new BLBanner().Excluir(ids) > 0 });

        #endregion

        #region Item

        [HttpPost, CheckPermission(global::Permissao.Modificar)]
        public override ActionResult Item(MLBanner model)
        {
            TempData["Salvo"] = new BLBanner().SalvarParcial(model) > 0;
            return RedirectToAction("Index");
        }

        #endregion

        #endregion

        #region Banner Arquivo

        #endregion

        #region Arquivo

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Arquivo(decimal id) => View(new BLBannerArquivo().ObterArquivo(id));

        #endregion

        #region ArquivoItem

        [JsonHandleError, CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ArquivoItem(decimal id)
        {
           return Json(new { Sucesso = true, model = new BLBannerArquivo().Obter(id, PortalAtual.Obter.ConnectionString) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, JsonHandleError, CheckPermission(global::Permissao.Modificar)]
        public ActionResult ArquivoItem(MLBannerArquivo model)
        {
            model.Codigo = new BLBannerArquivo().Salvar(model);
            return Json(new { Sucesso = true, model });
        }

        #endregion

        #region IncluirArquivo

        /// <summary>
        /// Incluir arquivo do disco para galeria
        /// <param name="CodigoTipo">Define se o item será carregado em um Iframe ou como arquivo (imagem)</param>
        /// </summary>
        [HttpPost, CheckPermission(global::Permissao.Modificar)]
        public ActionResult IncluirArquivo
        (
            decimal id,
            byte CodigoTipo,
            string[] files,
            string UrlIframe
        )
        {
            ViewData["diretorioGaleria"] = BLBanner.ObterDiretorio(id);
            var lista = new BLBannerArquivo().IncluirArquivos(id, CodigoTipo, files, UrlIframe);

            return Json(new { Sucesso = true, html = BLConteudoHelper.RenderPartialViewToString(this, "Modulo", "BannerAdmin", "ArquivoItem", lista) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region MostrarArquivoDisco

        [HttpPost, CheckPermission(global::Permissao.Visualizar)]
        public ActionResult MostrarArquivosDisco(decimal id)
        {
            ViewData["diretorioGaleria"] = BLBanner.ObterDiretorio(id);
            var lista = new BLBannerArquivo().MostrarArquivosDisco(id);

            return Json(new { Sucesso = true, html = BLConteudoHelper.RenderPartialViewToString(this, "ModuloAdmin", "BannerAdmin", "ArquivoItem", lista) });
        }

        #endregion

        #region ListarArquivos

        [HttpGet, CheckPermission(global::Permissao.Modificar)]
        public JsonResult ListarArquivos(decimal id)
        {
            ViewData["diretorioGaleria"] = BLBanner.ObterDiretorio(id);
            var lista = new BLBannerArquivo().Listar(new MLBannerArquivo()
            {
                CodigoBanner = id
            }, "Ordem", "ASC");

            return Json(BLConteudoHelper.RenderPartialViewToString(this, "Modulo", "BannerAdmin", "ArquivoItem", lista), JsonRequestBehavior.AllowGet);
        }

        [HttpGet, CheckPermission(global::Permissao.Modificar)]
        public JsonResult ListarArquivosSelect(decimal id)
        {
            var lista = new BLBannerArquivo().Listar(new MLBannerArquivo()
            {
                CodigoBanner = id
            }, "Codigo", "ASC");

            return Json(lista, JsonRequestBehavior.AllowGet);  
        }

        #endregion

        #region ObterNomeViewPorTipo

        /// <summary>
        /// Obter o nome da view para renderizar o item do banner de acordo com o tipo seleiconado no cadastro
        /// </summary>
        /// <param name="CodigoTipo"></param>
        /// <returns>string nome da view</returns>
        private string ObterNomeViewPorTipo(byte? CodigoTipo) => Enum.GetName(typeof(MLBannerArquivo.Tipo), CodigoTipo ?? 1);
        
        #endregion

        #region Ordenar

        [HttpPost, CheckPermission(global::Permissao.Modificar)]
        public ActionResult Ordenar(decimal id, List<decimal> ordem) => Json(new { Sucesso = new BLBannerArquivo().Ordenar(id, ordem) });

        #endregion

        #region UploadGaleria

        [HttpPost, CheckPermission(global::Permissao.Modificar)]
        public void UploadGaleria(decimal id) => new BLBannerArquivo().UploadGaleria(id);

        #endregion

        #region RemoverArquivo

        [HttpPost, CheckPermission(global::Permissao.Modificar)]
        public ActionResult RemoverArquivo(decimal id, decimal CodigoBanner)
        {
            ViewData["diretorioGaleria"] = BLBanner.ObterDiretorio(CodigoBanner);
            var modelArquivoExcluido = new BLBannerArquivo().Remover(id);

            return PartialView("ArquivoItem", new List<MLBannerArquivo>() { modelArquivoExcluido });
        }

        #endregion

        #region RemoverArquivoServidor

        [HttpGet, CheckPermission(global::Permissao.Modificar)]
        public void RemoverArquivoServidor(decimal id, string imagem) => new BLBannerArquivo().RemoverDoServidor(id, imagem);

        #endregion        
    }
}
