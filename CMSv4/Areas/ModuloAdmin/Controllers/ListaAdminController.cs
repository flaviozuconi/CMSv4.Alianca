using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Transactions;
using System.Web.Mvc;
using System.Linq;
using CMSv4.Model;
using CMSv4.BusinessLayer;
using System.Web;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class ListaAdminController : SecurePortalController
    {
        private BLListaConfig _BLListaConfig { get; }
        private BLListaConteudo _BLListaConteudo { get; }

        public ListaAdminController()
        {
            _BLListaConfig = new BLListaConfig();
            _BLListaConteudo = new BLListaConteudo();
        }

        #region Index

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Index(decimal idLista, string view)
        {
            if (!_BLListaConfig.ValidarPermissaoLista(idLista))
            {
                return null;
            }

            return View(view ?? "Index", _BLListaConfig.Obter(idLista, PortalAtual.ConnectionString));
        }

        #endregion

        #region Menu

        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Menu()
        {
            var lista = new List<MLListaConfig>();
            var usuario = BLUsuario.ObterLogado();

            if (usuario != null)
            {
                lista = _BLListaConfig.ListarPorUsuario(PortalAtual.Obter, usuario.GruposToString());
                lista.RemoveAll(o => o.ExibirSideBar.HasValue && !o.ExibirSideBar.Value);
            }

            return PartialView(lista);
        }

        #endregion

        #region Listar

        [CheckPermission(global::Permissao.Visualizar)]
        [DataTableHandleError]
        public ActionResult Listar(decimal idLista)
        {

            if (!_BLListaConfig.ValidarPermissaoLista(idLista)) return null;

            var lista = _BLListaConteudo.ListarAdmin(idLista, Request.QueryString);
            var total = lista.Any() ? lista[0].TotalRows : 0;

            return new JsonResult()
            {
                Data = new
                {
                    recordsTotal = total,
                    recordsFiltered = total,
                    data = lista
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region Item

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Item(decimal? id, decimal? idLista, HttpPostedFileBase file = null)
        {
            var portal = PortalAtual.Obter;
            var model = _BLListaConteudo.Obter(id.GetValueOrDefault(), portal.ConnectionString) ?? new MLListaConteudo();

            if (model.Codigo.GetValueOrDefault() > 0)
            {
                model.Conteudo.Unescape();
                idLista = model.CodigoLista;
                if (!model.GUID.HasValue) model.GUID = Guid.NewGuid();

                ViewData["categorias-selecionadas"] = new BLListaConteudoCategoria().Listar(new MLListaConteudoCategoria { CodigoConteudo = id.Value }, portal.ConnectionString);

                #region Galeria

                ViewData["lista"] = new BLListaConteudoImagem().ListarConteudoImagemMaisArquivosEmDisco(model);
                ViewData["diretorioGaleria"] = (BLConfiguracao.Pastas.ModuloListas(portal.Diretorio) + "/" + model.GUID.Value.ToString().Replace("-", "") + "/galeria").Replace("//", "/");

                #endregion

                #region Videos

                ViewData["listaVideo"] = new BLListaConteudoVideo().ListarConteudoVideoMaisArquivosEmDisco(model);
                ViewData["diretorioVideo"] = (BLConfiguracao.Pastas.ModuloListas(portal.Diretorio) + "/" + model.GUID.Value.ToString().Replace("-", "") + "/video").Replace("//", "/");

                #endregion

                #region Audios

                ViewData["listaAudio"] = new BLListaConteudoAudio().ListarConteudoAudioMaisArquivosEmDisco(model);
                ViewData["diretorioAudio"] = (BLConfiguracao.Pastas.ModuloListas(portal.Diretorio) + "/" + model.GUID.Value.ToString().Replace("-", "") + "/Audio").Replace("//", "/");

                #endregion

                #region SEO

                model.Seo = new BLListaConteudoSEO().Obter(id.Value, PortalAtual.ConnectionString);
                model.Seo?.Outros.Unescape();

                #endregion
            }

            if (!string.IsNullOrEmpty(Request.QueryString["codigobase"]))
            {
                model.CodigoBase = Convert.ToDecimal(Request.QueryString["codigobase"]);
                model.CodigoIdioma = Convert.ToDecimal(Request.QueryString["codigoIdioma"]);
            }

            if (!idLista.HasValue) return null;

            if (!_BLListaConfig.ValidarPermissaoLista(idLista.Value)) return null;

            // CONFIGURACAO DA LISTA
            var config = _BLListaConfig.Obter(idLista.Value, portal.ConnectionString);

            ViewData["listaConfig"] = config;

            // CATEGORIAS
            ViewData["categorias-todas"] = new BLListaConfigCategoria().Listar(new MLListaConfigCategoria { CodigoLista = idLista }, portal.ConnectionString);

            // APROVADORES
            if (config.CodigoGrupoAprovador.HasValue)
            {
                ViewData["aprovadores"] = BLUsuario.ListarAprovadores(portal.Codigo.Value, config.CodigoGrupoAprovador.Value);
            }

            return View("Item", model);

        }

        /// <summary>
        /// Salvar registro
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [JsonHandleError]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Item(MLListaConteudo model,
            List<string> GaleriaImagem, List<string> GaleriaTitulo, List<string> GaleriaTexto, List<string> GaleriaFonte,
            List<string> GaleriaVideo, List<string> VideoTitulo, List<string> VideoTexto, List<string> VideoFonte, List<string> VideoGuid,
            List<string> GaleriaAudio, List<string> AudioTitulo, List<string> AudioTexto, List<string> AudioFonte, List<string> AudioGuid,
            bool? RemoverCapa, bool? Autorizar, bool? Publicar, string CodigoUsuarioSolicitado, bool? IsRevisar,
            HttpPostedFileBase Arquivo, HttpPostedFileBase Max1Imagem, HttpPostedFileBase ImagemCapa, string ArquivoHf)
        {

            if (!model.CodigoLista.HasValue) return null;
            if (!_BLListaConfig.ValidarPermissaoLista(model.CodigoLista.Value)) return null;

            var portal = PortalAtual.Obter;

            if (ModelState.IsValid)
            {
                bool publicarAgora = false;

                model = _BLListaConteudo.SalvarAdmin(
                    model, GaleriaImagem, GaleriaTitulo, GaleriaTexto, GaleriaFonte, GaleriaVideo, VideoTitulo, VideoTexto, VideoFonte,
                    VideoGuid, GaleriaAudio, AudioTitulo, AudioTexto, AudioFonte, AudioGuid, RemoverCapa, Autorizar, Publicar,
                    CodigoUsuarioSolicitado, IsRevisar, ArquivoHf, Arquivo, Max1Imagem, ImagemCapa, Request.Form, ref publicarAgora
                );

                if (publicarAgora)
                    BLLista.Publicar(portal, model.Codigo.Value);

                TempData["Salvo"] = model.Codigo > 0;

                return RedirectToAction("Index", new { idLista = model.CodigoLista, salvo = true });
            }


            model.Conteudo.Unescape();

            var config2 = _BLListaConfig.Obter(model.CodigoLista.Value, portal.ConnectionString);
            ViewData["listaConfig"] = config2;
            ViewData["aprovadores"] = BLUsuario.ListarAprovadores(portal.Codigo.Value, config2.CodigoGrupoAprovador.Value);

            return View("Item", model);

        }

        #endregion

        #region Excluir

        [CheckPermission(global::Permissao.Excluir)]
        [HttpPost]
        [JsonHandleError]
        public ActionResult Excluir(List<string> ids)
        {
            return Json(new { Sucesso = _BLListaConteudo.Excluir(ids, PortalAtual.ConnectionString) });
        }

        #endregion



        #region Galeria

        #region Galeria Item

        [CheckPermission(global::Permissao.Modificar, ValidarModelState = false)]
        [JsonHandleError]
        [HttpPost]
        public ActionResult GaleriaItem(MLListaConteudoImagem model, decimal codigoImagem)
        {
            var portal = PortalAtual.Obter;

            if (!model.CodigoConteudo.HasValue) return null;

            var conteudo = _BLListaConteudo.Obter(model.CodigoConteudo.Value, portal.ConnectionString);

            if (conteudo == null || !conteudo.Codigo.HasValue || !conteudo.CodigoLista.HasValue) return null;
            if (!_BLListaConfig.ValidarPermissaoLista(conteudo.CodigoLista.Value)) return null;

            model.Codigo = codigoImagem;

            return Json(new { Sucesso = new BLListaConteudoImagem().Salvar(model, portal.ConnectionString) > 0 });
        }

        #endregion

        #region UploadGaleria

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        [JsonHandleError]
        public ActionResult UploadGaleria(Guid guid)
        {
            return Json(new { Sucesso = new BLListaConteudoImagem().UploadGaleria(guid, Request.Files, PortalAtual.Obter) });
        }

        #endregion

        #region IncluirArquivoGaleria

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public JsonResult IncluirArquivoGaleria(Guid guid, string[] files)
        {
            var lista = new BLListaConteudoImagem().IncluirArquivoGaleria(guid, files, PortalAtual.Obter);

            if (lista == null) return null;

            var diretorio = (BLConfiguracao.Pastas.ModuloListas(PortalAtual.Obter.Diretorio) + "/" + guid.ToString().Replace("-", "") + "/galeria").Replace("//", "/");
            ViewData["diretorioGaleria"] = diretorio;
            ViewData["ajax"] = true;

            var htmlNovoArquivo = BLConteudoHelper.RenderPartialViewToString(this, "ModuloAdmin", "ListaAdmin", "GaleriaItem", lista);

            return Json(new { Sucesso = true, html = htmlNovoArquivo }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region RemoverArquivoGaleria

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult RemoverArquivoGaleria(Guid guid, string file)
        {
            var retorno = new BLListaConteudoImagem().RemoverArquivoGaleria(guid, file, PortalAtual.Obter);
            if (!retorno) return null;

            var diretorio = (BLConfiguracao.Pastas.ModuloListas(PortalAtual.Obter.Diretorio) + "/" + guid.ToString().Replace("-", "") + "/galeria").Replace("//", "/");

            ViewData["diretorioGaleria"] = diretorio;
            ViewData["ajax"] = true;

            return PartialView("GaleriaImportar", new MLListaConteudoImagem { GuidConteudo = guid, Imagem = file });
        }

        #endregion

        #region ExcluirArquivoGaleria

        [CheckPermission(global::Permissao.Excluir)]
        [HttpPost]
        public ActionResult ExcluirArquivoGaleria(Guid guid, string file)
        {
            return Json(new { Sucesso = new BLListaConteudoImagem().ExcluirArquivoGaleria(guid, file, PortalAtual.Obter) });
        }

        #endregion

        #endregion

        #region Videos

        #region UploadVideo

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        [JsonHandleError]
        public ActionResult UploadVideo(Guid guid)
        {
            string caminho = String.Empty;
            bool retorno = new BLListaConteudoVideo().UploadVideo(guid, Request.Files[0], ref caminho, PortalAtual.Obter);
            return Json(new { Sucesso = retorno, name = Path.GetFileName(caminho) });
        }

        #endregion

        #region IncluirArquivoVideo

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult IncluirArquivoVideo(Guid guid, string file)
        {
            var video = new BLListaConteudoVideo().IncluirArquivoVideo(guid, file, PortalAtual.Obter);
            ViewData["diretorioVideo"] = (BLConfiguracao.Pastas.ModuloListas(PortalAtual.Obter.Diretorio) + "/" + guid.ToString().Replace("-", "") + "/video").Replace("//", "/"); ;
            ViewData["ajax"] = true;

            return PartialView("VideoItem", video);
        }

        #endregion

        #region RemoverArquivoVideo

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult RemoverArquivoVideo(Guid guid, string file)
        {
            var retorno = new BLListaConteudoVideo().RemoverArquivoVideo(guid, file, PortalAtual.Obter);
            if (!retorno) return null;

            ViewData["diretorioVideo"] = (BLConfiguracao.Pastas.ModuloListas(PortalAtual.Diretorio) + "/" + guid.ToString().Replace("-", "") + "/video").Replace("//", "/"); ;
            ViewData["ajax"] = true;

            return PartialView("VideoImportar", new MLListaConteudoVideo { GuidConteudo = guid, Video = file });
        }

        #endregion

        #region ExcluirArquivoVideo

        [CheckPermission(global::Permissao.Excluir)]
        [HttpPost]
        [JsonHandleError]
        public ActionResult ExcluirArquivoVideo(Guid guid, string file)
        {
            return Json(new { Sucesso = new BLListaConteudoVideo().ExcluirArquivoVideo(guid, file, PortalAtual.Obter) });
        }

        #endregion

        #region IncluirVideoUrl  (Youtube, Vimeo e etc..)
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult IncluirVideoUrl(Guid guid)
        {
            ViewData["ajax"] = true;
            return PartialView("VideoItem", new BLListaConteudoVideo().IncluirVideoUrl(guid, PortalAtual.Obter));
        }
        #endregion

        #endregion

        #region Audios

        #region UploadAudio

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        [JsonHandleError]
        public ActionResult UploadAudio(Guid guid)
        {
            string caminho = String.Empty;
            bool retorno = new BLListaConteudoAudio().UploadAudio(guid, Request.Files[0], ref caminho, PortalAtual.Obter);
            return Json(new { Sucesso = retorno, name = Path.GetFileName(caminho) });
        }

        #endregion

        #region IncluirArquivoAudio

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult IncluirArquivoAudio(Guid guid, string file)
        {
            var Audio = new BLListaConteudoAudio().IncluirArquivoAudio(guid, file, PortalAtual.Obter);
            ViewData["diretorioAudio"] = (BLConfiguracao.Pastas.ModuloListas(PortalAtual.Diretorio) + "/" + guid.ToString().Replace("-", "") + "/Audio").Replace("//", "/");
            ViewData["ajax"] = true;

            return PartialView("AudioItem", Audio);
        }

        #endregion

        #region RemoverArquivoAudio

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult RemoverArquivoAudio(Guid guid, string file)
        {

            bool retorno = new BLListaConteudoAudio().RemoverArquivoAudio(guid, file, PortalAtual.Obter);
            if (!retorno) return null;

            ViewData["diretorioAudio"] = (BLConfiguracao.Pastas.ModuloListas(PortalAtual.Diretorio) + "/" + guid.ToString().Replace("-", "") + "/Audio").Replace("//", "/"); ;
            ViewData["ajax"] = true;

            return PartialView("AudioImportar", new MLListaConteudoAudio { GuidConteudo = guid, Audio = file });
        }

        #endregion

        #region ExcluirArquivoAudio

        [CheckPermission(global::Permissao.Excluir)]
        [HttpPost]
        [JsonHandleError]
        public ActionResult ExcluirArquivoAudio(Guid guid, string file)
        {
            return Json(new { Sucesso = new BLListaConteudoAudio().ExcluirArquivoAudio(guid, file, PortalAtual.Obter) });
        }

        #endregion

        #region ListarAudio

        [HttpPost]
        [CheckPermission(global::Permissao.Visualizar)]
        [JsonHandleError]
        public JsonResult ListarAudio(decimal? id, string guid)
        {
            return Json(new { Sucesso = true, msg = "Sucesso", data = new BLListaConteudoAudio().ListarAudio(id, guid) });
        }

        #endregion

        #region ListarVideo

        [HttpPost]
        [CheckPermission(global::Permissao.Visualizar)]
        [JsonHandleError]
        public JsonResult ListarVideo(decimal? id, string guid)
        {
            return Json(new { Sucesso = true, msg = "Sucesso", data = new BLListaConteudoVideo().ListarVideo(id, guid) });
        }

        #endregion

        #endregion

        #region Scripts e Seo

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        [JsonHandleError]
        public ActionResult SeoOutros(decimal codigoConteudo, string seooutros)
        {
            return Json(new
            {
                Sucesso = new BLListaConteudoSEO().SalvarParcial(
                new MLListaConteudoSEO() { Codigo = codigoConteudo, Outros = seooutros }, PortalAtual.ConnectionString) > 0
            });
        }

        #endregion



        #region Categoria

        [CheckPermission(global::Permissao.Visualizar)]
        [DataTableHandleError]
        public ActionResult ListarCategorias(decimal idLista)
        {
            if (!_BLListaConfig.ValidarPermissaoLista(idLista)) return null;
            return DataTable.Listar(new MLListaConfigCategoria { CodigoLista = idLista }, Request.QueryString);
        }

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult CategoriaItem(decimal? id, decimal? idLista)
        {
            var model = new MLListaConfigCategoria();
            var portal = PortalAtual.Obter;

            if (id.HasValue)
            {
                model = new BLListaConfigCategoria().Obter(id.Value, portal.ConnectionString);
            }
            else
            {
                model.CodigoLista = idLista;
            }

            if (model.CodigoLista.HasValue)
            {
                var modulo = _BLListaConfig.Obter(model.CodigoLista.Value, portal.ConnectionString);

                ViewBag.Nome = modulo.Nome;
                ViewBag.Icone = modulo.Icone;
            }

            ViewData["categorias"] = new BLCategoriaAgrupador().Listar(new MLCategoriaAgrupador { CodigoPortal = portal.Codigo, Ativo = true }, portal.ConnectionString);

            return View(model);
        }

        [HttpPost, CheckPermission(global::Permissao.Visualizar, ValidarModelState = true)]
        public ActionResult CategoriaItem(MLListaConfigCategoria model)
        {
            TempData["Salvo"] = new BLListaConfigCategoria().Salvar(model) > 0;
            return RedirectToAction("Categoria", new { idLista = model.CodigoLista });
        }

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Categoria(decimal idLista)
        {
            if (!_BLListaConfig.ValidarPermissaoLista(idLista)) return null;
            return View("Categoria", _BLListaConfig.Obter(new MLListaConfig { Codigo = idLista }, PortalAtual.ConnectionString));
        }

        [HttpPost, CheckPermission(global::Permissao.Excluir)]
        [JsonHandleError]
        public ActionResult ExcluirCategoria(List<string> ids)
        {
            return Json(new { Sucesso = new BLListaConfigCategoria().Excluir(ids, PortalAtual.ConnectionString) });
        }

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ListarCategoriasView(decimal idLista)
        {
            if (!_BLListaConfig.ValidarPermissaoLista(idLista)) return null;
            return View("Categoria", _BLListaConfig.Obter(new MLListaConfig { Codigo = idLista }, PortalAtual.ConnectionString));
        }

        #endregion

        #region Vincular
        [HttpGet, CheckPermission(global::Permissao.Modificar)]
        public ActionResult Vincular(decimal? CodigoLista, decimal? CodigoBase, decimal? CodigoIdioma)
        {
            ViewBag.CodigoBase = CodigoBase;
            ViewBag.CodigoIdioma = CodigoIdioma;
            return View(_BLListaConfig.Obter(CodigoLista.Value, PortalAtual.ConnectionString));
        }

        [HttpPost, CheckPermission(global::Permissao.Modificar)]
        public ActionResult Vincular(MLListaConteudo model)
        {
            new BLListaConteudo().Vincular(model, PortalAtual.Obter);
            return Item(model.Codigo, model.CodigoLista);
        }
        #endregion

        #region ListarConteudosPublicados
        [HttpPost]
        [CheckPermission(global::Permissao.Visualizar)]
        [JsonHandleError]
        public ActionResult ListarConteudosPublicados(string busca, decimal? CodigoPortal, decimal? param1, decimal? param2)
        {
            var retorno = BLLista.ListarPublicoAdmin(new MLModuloLista
            {
                CodigoLista = param2,
                Titulo = busca
            }, null, codigoIdioma: param1);

            return Json(new { Sucesso = true, json = retorno });
        }
        #endregion

        #region ListaConfig

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ListaConfig() => View();

        #endregion

        #region ListaConfigItem

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ListaConfigItem(decimal? id)
        {
            var portal = PortalAtual.Obter;
            ViewBag.Aprovadores = new BLGrupo().Listar(new MLGrupo(), portal.ConnectionString);

            return View(_BLListaConfig.Obter(id.GetValueOrDefault(), portal.ConnectionString) ?? new MLListaConfig());
        }

        [CheckPermission(global::Permissao.Modificar, ValidarModelState = true, ViewDeRetornoParaModelStateInvalido = "ListaConfig")]
        [HttpPost]
        [ValidateInput(false)]
        [JsonHandleError]
        public ActionResult ListaConfigItem(MLListaConfig model)
        {
            var portal = PortalAtual.Obter;
            model.CodigoPortal = portal.Codigo;
            TempData["Salvo"] = _BLListaConfig.Salvar(model, portal.ConnectionString) > 0;

            return RedirectToAction("ListaConfig");
        }

        #endregion

        #region ListarListaConfig

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ListarListaConfig(MLListaConfig criterios)
        {
            var portal = PortalAtual.Obter;
            criterios.CodigoPortal = portal.Codigo;
            return DataTable.Listar(criterios, Request.QueryString, portal.ConnectionString);
        }

        #endregion

        #region ListaConfigExcluir

        [CheckPermission(global::Permissao.Excluir)]
        [HttpPost]
        public ActionResult ListaConfigExcluir(List<string> ids)
        {
            try
            {
                return Json(new { Sucesso = _BLListaConfig.Excluir(ids, PortalAtual.Obter.ConnectionString) > 0});
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK_MOD_LIS_LISTA_CONTEUDO_MOD_LIS_LISTA_CONFIG"))
                    return Json(new { Sucesso = false, msg = "Não é possível excluir um registro que possui referências." });

                ApplicationLog.ErrorLog(ex);

                return Json(new { Sucesso = false, msg = ex.Message });
            }
        }

        #endregion

        #region ValidarUrl

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult ValidarUrl(string url, decimal? id, decimal CodigoLista)
        {
            try
            {
                var codigo = BLLista.ObterUrlValidacao(url, id, CodigoLista);

                if ((codigo.HasValue && codigo.Value > 0))
                    return Json(T("Já existe uma matéria cadastrada com essa Url"));
                return Json(true);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(T("Não foi possível validar a URL, entre em contato com o administrador do sistema."));
            }
        }

        #endregion
    }
}
