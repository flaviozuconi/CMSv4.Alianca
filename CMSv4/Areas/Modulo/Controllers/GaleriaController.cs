using Framework.Utilities;
using System;
using System.IO;
using System.Web.Mvc;
using VM2.Areas.CMS.Helpers;
using CMSv4.Model;

namespace CMSApp.Areas.Modulo.Controllers
{
    public class GaleriaController : ModuloBaseController<MLModuloGaleriaMultimidiaEdicao, MLModuloGaleriaMultimidiaHistorico, MLModuloGaleriaMultimidiaPublicado>
    {
        #region Index

        /// <summary>
        /// Área Pública / Apenas conteúdos publicados
        /// </summary>
        public override ActionResult Index(decimal? codigoPagina, int? repositorio)
        {
            try
            {
                var portal = BLPortal.Atual;

                if (!codigoPagina.HasValue || !repositorio.HasValue) return null;

                // Visualizar Publicado
                MLModuloGaleriaMultimidiaPublicado model = CRUD.Obter<MLModuloGaleriaMultimidiaPublicado>(new MLModuloGaleriaMultimidiaPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);

                if (model == null)
                    model = new MLModuloGaleriaMultimidiaPublicado();
                else
                {
                    var filtro = new MLGaleriaMultimidiaArquivo();

                    filtro.CodigoGaleria = model.CodigoGaleria;

                    if (model.Destaques.GetValueOrDefault())
                    {
                        filtro.Destaque = true;
                    }

                    model.ArquivosGaleria = CRUD.Listar<MLGaleriaMultimidiaArquivo>(filtro, model.Quantidade, "Data", "DESC", portal.ConnectionString);
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
                MLModuloGaleriaMultimidia model = null;

                ViewBag.ModoEdicao = false;

                if (edicao.HasValue && edicao.Value)
                {
                    ViewBag.ModoEdicao = true;

                    // Visualizar em Edição
                    model = CRUD.Obter<MLModuloGaleriaMultimidiaEdicao>(new MLModuloGaleriaMultimidiaEdicao { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }
                else if (!string.IsNullOrEmpty(codigoHistorico))
                {
                    // Visualizar Histórico
                    model = CRUD.Obter<MLModuloGaleriaMultimidiaHistorico>(new MLModuloGaleriaMultimidiaHistorico { CodigoHistorico = new Guid(codigoHistorico), CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }
                else
                {
                    // Visualizar Publicado
                    model = CRUD.Obter<MLModuloGaleriaMultimidiaPublicado>(new MLModuloGaleriaMultimidiaPublicado { CodigoPagina = codigoPagina, Repositorio = repositorio }, portal.ConnectionString);
                }

                if (model == null)
                    model = new MLModuloGaleriaMultimidiaEdicao();
                else
                {
                    var filtro = new MLGaleriaMultimidiaArquivo();

                    filtro.CodigoGaleria = model.CodigoGaleria;

                    if (model.Destaques.GetValueOrDefault())
                    {
                        filtro.Destaque = true;
                    }

                    model.ArquivosGaleria = CRUD.Listar<MLGaleriaMultimidiaArquivo>(filtro, model.Quantidade, "Data", "ASC", portal.ConnectionString);
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
        public override ActionResult Editar(MLModuloGaleriaMultimidiaEdicao model)
        {
            try
            {
                if (model.DataRegistro == null)
                    model.DataRegistro = DateTime.Now;

                if (model.Destaques == null)
                    model.Destaques = false;

                if (model.CodigoUsuario == null || model.CodigoUsuario == 0)
                    model.CodigoUsuario = BLUsuario.ObterLogado().Codigo;

                //Salvar
                CRUD.Salvar(model, PortalAtual.ConnectionString);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Lista Publico
        /// <summary>
        /// Action da lista
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult ListarPublico(MLModuloGaleriaMultimidia model)
        {
            try
            {
                ListarTipos();

                return View(model.NomeView ?? "Galeria", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw ex;
            }
        }
        #endregion

        #region Script
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Script(MLModuloGaleriaMultimidia model)
        {
            return PartialView(model);
        }
        #endregion

        #region Download
        [CheckPermission(global::Permissao.Publico)]
        public FileResult Download(string f)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(f)) 
                {
                    return null;
                }

                var portal = BLPortal.Atual;

                f = f.Replace("style=target=_blank", "");

                var arquivoDescriptografado = BLEncriptacao.DesencriptarQueryString(f); //replace para remover sujeira gerada pelo internet explorer

                if (f == arquivoDescriptografado)
                {
                    return null;
                }

                var file = Server.MapPath(string.Concat("~/Portal/", portal.Diretorio, "/arquivos/galeria/", arquivoDescriptografado));

                if (System.IO.File.Exists(file))
                {
                    return File(file, "application/octet-stream", Path.GetFileName(file).Replace(".resource", ""));
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }
            return null;
        }
        #endregion

        #region Campanha
        /// <summary>
        /// Action da lista
        /// </summary>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Campanha(decimal codigoGaleria)
        {
            try
            {
                var portal = BLPortal.Atual;
                var model = new MLModuloGaleriaMultimidia();
                
                model.CodigoPagina = 1;
                model.Repositorio = 1;
                model.ArquivosGaleria = CRUD.Listar<MLGaleriaMultimidiaArquivo>(new MLGaleriaMultimidiaArquivo
                {
                    CodigoGaleria = codigoGaleria,
                    CodigoIdioma = BLIdioma.CodigoAtual
                }, model.Quantidade, "Data", "ASC", portal.ConnectionString);

                //loop em arquivos para obter largura e altura das imagens cadastradas
                if (model.ArquivosGaleria != null && model.ArquivosGaleria.Count > 0)
                {
                    for (int i = 0; i < model.ArquivosGaleria.Count; i++)
                    {
                        if (model.ArquivosGaleria[i].Tipo == 1) //somente imagens
	                    {
                            var arquivoServidor = Server.MapPath(string.Concat(Portal.Url(), BLConfiguracao.Pastas.ModuloGaleria(portal.Diretorio), "/", model.ArquivosGaleria[i].CodigoGaleria, "/", model.ArquivosGaleria[i].Imagem));
                            var info = new FileInfo(arquivoServidor);
                            if (info.Exists)
                            {
                                using (var img = System.Drawing.Image.FromFile(info.FullName))
                                {
                                    //analisa a altura e largura do arquivo
                                    model.ArquivosGaleria[i].ImagemVertical = img.Height > img.Width;
                                }
                            }
	                    }
                    }
                }

                return View("Galeria", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw ex;
            }
        }
        #endregion

        private void ListarTipos()
        {
            var portal = BLPortal.Atual;

            ViewBag.Tipos = CRUD.Listar<MLGaleriaMultimidiaTipo>(new MLGaleriaMultimidiaTipo
            {
                CodigoPortal = portal.Codigo,
                Ativo = true
            }, portal.ConnectionString);
        }
    }
}