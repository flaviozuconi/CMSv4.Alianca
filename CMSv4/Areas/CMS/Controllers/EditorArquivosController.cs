using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CMSv4.BusinessLayer;
using CMSv4.Model;
using Framework.Utilities;
using System.Net.Mime;

namespace CMSApp.Areas.CMS.Controllers
{
    public class EditorArquivosController : SecurePortalController
    {
        #region Index

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Index()
        {
            ViewData["GruposCliente"] = new BLGrupo().Listar(new MLGrupo { Ativo = true }, PortalAtual.ConnectionString);
            return View();
        }

        #endregion

        #region Item

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Item(string id)
        {
            var model = BLEditorArquivo.Obter(id);

            if (model.Editavel)
                return View(model);

            return File(model.ArquivoFisico, MediaTypeNames.Application.Octet, model.FileInfo.Name);
        }

        [HttpPost]
        [ValidateInput(false)]
        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult Item(string NomeAnterior, string Diretorio, string Nome, string Conteudo)
        {
            BLEditorArquivo.Salvar(NomeAnterior, Diretorio, Nome, Conteudo);

            TempData["Salvo"] = true;
            return RedirectToAction("Index");
        }

        #endregion

        #region Upload

        /// <summary>
        /// Upload
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public void Upload()
        {
            BLEditorArquivo.Upload(Request.Form["DiretorioSalvar"]);
        }

        #endregion

        // Pastas
        #region Nova Pasta

        [HttpPost]
        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult NovaPasta(string pasta, string novaPasta)
        {
            if (BLGaleria.NovaPasta(pasta, novaPasta))
                return Json(new { success = true });
                
            return Json(new { success = false, msg = T("Diretório existente ou inválido") });
        }

        #endregion

        #region Excluir Pasta

        [HttpPost]
        [CheckPermission(global::Permissao.Excluir)]
        public ActionResult ExcluirPasta(string dataPath)
        {
            if (BLGaleria.ExcluirPasta(dataPath))
                return Json(new { Sucesso = true });
            
            return Json(new { Sucesso = false, msg = T("Para excluir este diretório, antes remova os arquivos e diretórios") });
        }

        #endregion

        #region Renomear Pasta

        /// <summary>
        /// Renomear pasta
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult RenomearPasta(string dataPath, string novoNome)
        {
            if (BLGaleria.RenomearPasta(dataPath, novoNome))
                return Json(new { success = true });
                
            return Json(new { success = false, msg = T("Não foi possível renomear o diretório") });
        }

        #endregion

        #region Listar

        /// <summary>
        /// Listagem
        /// </summary>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Diretorios(string pasta)
        {
            pasta = pasta
                .Replace("..|", "")
                .Replace("..", "")
                .Replace(".|", "");

            return Json(BLGaleria.ListarPastas(pasta), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Bread crumb arquivos
        
        [JsonHandleError]
        [CheckPermission(global::Permissao.Visualizar)]
        public JsonResult BreadCrumbArquivos(string pasta)
        {
            return Json(BLEditorArquivo.BreadCrumb(pasta), JsonRequestBehavior.AllowGet);
        }

        #endregion

        // Arquivos

        #region Excluir

        /// <summary>
        /// Excluir lista de arquivos
        /// </summary>
        [CheckPermission(global::Permissao.Excluir)]
        public ActionResult Excluir(List<string> ids)
        {
            BLEditorArquivo.Excluir(ids);
            return Json(new { Sucesso = true });
        }

        #endregion

        #region Arquivos

        [JsonHandleError]
        [CheckPermission(global::Permissao.Visualizar)]
        public JsonResult Arquivos(string pasta)
        {
            return new DataTableResult()
            {
                Data = BLGaleria.ListarArquivos(pasta)
            };
        }

        #endregion

        #region Permissoes

        #region ObterPermissao

        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult ObterPermissao(string pasta)
        {
            try
            {
                var infoDir = CRUD.Obter(new MLPasta() { Caminho = pasta }, PortalAtual.ConnectionString) ?? new MLPasta();
                var permissoes = CRUD.Listar(new MLPastaPermissao() { CodigoDiretorio = infoDir?.Codigo }, PortalAtual.ConnectionString) ?? new List<MLPastaPermissao>();

                return Json(new { restrito = infoDir?.Restrito, permissoes = permissoes }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region SalvarPermissao

        [HttpPost]
        [JsonHandleError]
        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult SalvarPermissao(string pasta, bool Restrito, List<int> grupos)
        {
            BLEditorArquivoPermissao.Salvar(pasta, Restrito, grupos);
            return Json(new { restrito = 0 }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region CompactarZip

        [HttpPost]
        [JsonHandleError]
        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult CompactarZip(string[] arquivos, string diretorioAtual, string nome)
        {
            if(arquivos !=null && arquivos.Length > 0)
                BLEditorArquivoZip.Compactar(arquivos, diretorioAtual, nome);
            else
                BLEditorArquivoZip.CompactarDiretorio(diretorioAtual, nome);
            return Json(true);
        }

        #endregion

        #region DescompactarZip

        [HttpPost]
        [JsonHandleError]
        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult DescompactarZip(string arquivo, string diretorioAtual)
        {
            BLEditorArquivoZip.Descompactar(arquivo, diretorioAtual);
            return Json(true);
        }

        #endregion

        #region ObterUrlPublicaZip

        [JsonHandleError]
        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult ObterUrlPublicaZip(string arquivo, string diretorio)
        {
            return Json(new { success = true, url = BLEditorArquivoZip.ObterUrlPublica(arquivo, diretorio) }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}