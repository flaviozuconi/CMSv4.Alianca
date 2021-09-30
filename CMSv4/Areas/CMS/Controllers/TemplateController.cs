using CMSv4.BusinessLayer;
using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CMSApp.Areas.CMS.Controllers
{
    public class TemplateController : SecurePortalController
    {
        //
        // GET: /CMS/Template/
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Index()
        {
            return View();
        }

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
                var model = new MLTemplate();
                var pasta = BLConfiguracao.Pastas.TemplatesPortal(PortalAtual.Diretorio);

                if(id.GetValueOrDefault(0) > 0)
                {
                    model = CRUD.Obter<MLTemplate>(id.Value);

                    var imagemCaminhoFisico = $"{pasta}/{model.Nome}.jpg";

                    if (System.IO.File.Exists(imagemCaminhoFisico))
                        ViewBag.Imagem = $"{pasta}/{model.Nome}.jpg";
                }

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
        public ActionResult Item(MLTemplate model, HttpPostedFileBase Imagem, string NomeAnterior)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool salvo = false;

                    // Salvar
                    var pasta = Server.MapPath(BLConfiguracao.Pastas.TemplatesPortal(PortalAtual.Diretorio));
                    var portal = PortalAtual.Obter;

                    model.CodigoPortal = portal.Codigo;
                    model.Conteudo = Microsoft.JScript.GlobalObject.unescape(model.Conteudo);
                    model.Ativo = true;
                    model.Repositorios = Regex.Matches(model.Conteudo, @"{R\d+}").Count;

                    var codigo = CRUD.Salvar(model, portal.ConnectionString);

                    salvo = codigo > 0;

                    //Excluir arquivo anterior
                    if (!string.IsNullOrWhiteSpace(NomeAnterior) && !model.Nome.Equals(NomeAnterior))
                    {
                        var anterior = Path.Combine(pasta, NomeAnterior + ".cshtml");
                        if (System.IO.File.Exists(anterior)) System.IO.File.Delete(anterior);
                    }

                    //Criar novo arquivo
                    if (!Directory.Exists(pasta))
                        Directory.CreateDirectory(pasta);

                    var file = Path.Combine(pasta, model.Nome + ".cshtml");

                    System.IO.File.WriteAllText(file, model.Conteudo, System.Text.Encoding.UTF8);
                    if (Imagem != null && Imagem.ContentLength > 0)
                    {
                        var arquivoImagem = $"{pasta}/{model.Nome}{Path.GetExtension(Imagem.FileName)}";
                        Imagem.SaveAs(arquivoImagem);
                        BLReplicar.Arquivo(arquivoImagem);
                    }

                    BLReplicar.Arquivo(file);

                    TempData["Salvo"] = salvo;

                    if (salvo)
                    {
                        ViewBag.ListaTemplates = BLTemplate.ListarArquivos(PortalAtual.Obter);
                    }

                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ItemVisual(decimal id)
        {
            return View("ItemVisual", new MLTemplate());
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
                    if (!string.IsNullOrEmpty(item))
                    {
                        string caminho = Server.MapPath(BLConfiguracao.Pastas.TemplatesPortal(PortalAtual.Diretorio) + "\\" + item + ".cshtml");
                        if (System.IO.File.Exists(caminho))
                        {
                            System.IO.File.Delete(caminho);
                            BLReplicar.ExcluirArquivosReplicados(caminho);
                        }
                    }
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

        //
        // GET: /CMS/Template/ListaJson
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ListaJson()
        {
            var listaArquivos = BLTemplate.ListarArquivos(PortalAtual.Obter);

            return new JsonResult()
            {
                Data = new { data = listaArquivos  },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
