using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Linq;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.CMS.Controllers
{
    /// <summary>
    /// MENU
    /// Acesso aos recursos do sistema
    /// </summary>
    public class MenuController : SecureController
    {
        #region Index

        #region Index

        // GET: /ModuloAdmin/MenuAdmin/
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Index()
        {
            return View();
        }

        #endregion

        #region DefinirDefault

        [HttpPost]
        public JsonResult DefinirDefault(decimal CodigoPortal, decimal CodigoMenu)
        {
            try
            {
                var portal = CRUD.Obter<MLPortal>(CodigoPortal);
                portal.CodigoMenu = CodigoMenu;

                CRUD.SalvarParcial(portal);

                //Atualizar valor do cache por referencia
                PortalAtual.Obter.CodigoMenu = CodigoMenu;

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DefinirViewDefault(decimal CodigoPortal, decimal CodigoMenuView)
        {
            try
            {
                var portal = CRUD.Obter<MLPortal>(CodigoPortal);
                portal.CodigoMenuView = CodigoMenuView;

                CRUD.SalvarParcial(portal);

                //Atualizar valor do cache por referencia
                PortalAtual.Obter.CodigoMenuView = CodigoMenuView;

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

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
        public ActionResult Listar(MLMenu criterios)
        {
            try
            {
                //criterios.CodigoPortal = PortalAtual.Codigo;
                return CRUD.ListarJson(criterios, Request.QueryString, PortalAtual.ConnectionString);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #endregion

        #region Item

        #region Salvar

        /// <summary>
        /// Visualizar ou Editar o registro conforme permissão do usuário
        /// </summary>
        /// <param name="id">Código do registro</param>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult MenuSalvar(MLMenuCompleto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var portal = PortalAtual.Obter;

                    //model.CodigoPortal = portal.Codigo;

                    if (model.Ativo == null)
                    {
                        model.Ativo = false;
                    }

                    if (!model.Codigo.HasValue)
                    {
                        model.Ativo = true;
                        model.Codigo = CRUD.SalvarParcial(model, portal.ConnectionString);
                    }
                    else
                    {
                        CRUD.SalvarParcial(model, portal.ConnectionString);
                    }

                    TempData["Salvo"] = model.Codigo > 0;

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
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
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Item(decimal? id)
        {
            MLMenuCompleto model = new MLMenuCompleto();

            try
            {
                if (id.HasValue)
                    model = BLMenu.ObterCompleto(
                        id.Value,
                        false, //useCache
                        false, //bItensPais
                        null,  //Status
                        null,  //CodigoPortal
                        null   //CodigoUsuario
                    );

                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }
        #endregion

        #region MenuItemSalvar

        /// <summary>
        /// Visualizar ou Editar o registro conforme permissão do usuário
        /// </summary>
        /// <param name="id">Código do registro</param>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult MenuItemSalvar(MLMenuItem model)
        {
            try
            {
                var portal = PortalAtual.Obter;

                using (var scope = new System.Transactions.TransactionScope(portal.ConnectionString))
                {
                    CRUD.Excluir(new MLPortalMenu() { CodigoMenuItem = model.Codigo });

                    List<MLPortalMenu> relacionados = new List<MLPortalMenu>();

                    if (Request["PortaisRelacionados"] != null && !string.IsNullOrWhiteSpace(Request["PortaisRelacionados"]))
                    {
                        var request = Request["PortaisRelacionados"].Replace("multiselect-all", "").TrimStart(',').Split(',').ToList();

                        foreach(var item in request)
                            relacionados.Add(new MLPortalMenu() { CodigoPortal = Convert.ToDecimal(item), CodigoMenuItem = model.Codigo });


                        CRUD.Salvar(relacionados, portal.ConnectionString);
                    }

                    CRUD.SalvarParcial(model, portal.ConnectionString);

                    scope.Complete();
                }

                //Limpar cache após modificação de permissão ou portal
                BLCachePortal.Remove(MLMenu.Config.CacheKey + model.CodigoMenu);

                return new JsonResult() { Data = new { success = true, CodigoMenuItem = model.Codigo, Icon = model.Imagem } };
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return new JsonResult() { Data = new { success = false } };
            }
        }
        #endregion

        #region Menu
        /// <summary>
        /// Monta recursivamente o Menu
        /// </summary>        
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Menu(List<MLMenuItem> itens, decimal? codigo)
        {
            try
            {
                var model = new MLMenuItemAdmin { CodigoAtual = codigo, Itens = itens };
                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }
        #endregion

        #region MenuItem
        /// <summary>
        /// Retorna a View de cada item do menu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult MenuItem(MLMenuItem model)
        {
            return View("_ModalMenuItem", model);
        }
        #endregion

        #region JsTree Post

        #region CriarItem
        /// <summary>
        /// Adiciona um novo item ao item atual
        /// </summary>        
        [HttpPost]
        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult CriarItem(decimal? codigopai, decimal codigoMenu)
        {
            try
            {
                MLMenuItem novoItem = new MLMenuItem();
                novoItem.CodigoPai = codigopai;
                novoItem.CodigoMenu = codigoMenu;
                novoItem.Nome = "Novo Item";
                novoItem.Ativo = true;
                novoItem.AbrirNovaPagina = false;

                novoItem.Codigo = BLMenu.Inserir(novoItem, PortalAtual.ConnectionString);

                var retorno = new JsonResult()
                {
                    Data = new
                    {
                        codigo = novoItem.Codigo,
                        ordem = CRUD.Obter<MLMenuItem>(novoItem.Codigo.Value, PortalAtual.ConnectionString).Ordem
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

                return retorno;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

        }


        #endregion

        #region DeletarItem

        [HttpPost]
        [CheckPermission(global::Permissao.Excluir)]
        public ActionResult ExcluirItem(decimal id)
        {
            bool IsSuccess;

            try
            {
                BLMenu.Excluir(id, PortalAtual.ConnectionString);
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                IsSuccess = false;
            }

            return new JsonResult()
            {
                Data = new
                {
                    success = IsSuccess
                }
            };
        }

        [HttpPost]
        [CheckPermission(global::Permissao.Excluir)]
        public ActionResult Excluir(List<string> ids)
        {
            try
            {
                CRUD.Excluir<MLMenu>(ids);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Listar

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ListarMenu(decimal codigoMenu)
        {
            //Exibir todos os itens do menu na treeview
            var model = BLMenu.ObterCompleto(
                codigoMenu, 
                false, //useCache
                false, //bItensPais
                null,  //Status
                null,  //CodigoPortal
                null   //CodigoUsuario
            );
            return Json((List<MLJsTree>)model.ItensMenu.Select(i => new MLJsTree() { id = i.Codigo.ToString(), parent = (i.CodigoPai.Value > 0) ? i.CodigoPai.ToString() : "#", text = i.Nome, icon = i.Imagem }).ToList());
        }

        #endregion

        #region ObterItem

        [HttpPost]
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ObterItem(decimal codigo)
        {
            try
            {
                var portal = PortalAtual.Obter;
                var model = CRUD.Obter<MLMenuItem>(codigo, portal.ConnectionString);

                ViewData["PortaisRelacionados"] = CRUD.Listar(new MLPortalMenu() { CodigoMenuItem = codigo }, portal.ConnectionString);
                ViewData["Portais"] = CRUD.Listar(new MLPortal { Ativo = true });
                ViewData["Funcionalidades"] = CRUD.Listar<MLFuncionalidade>(portal.ConnectionString).OrderBy(x => x.Nome).ToList();

                return PartialView("_ModalMenuItem", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return null;
            }

        }

        #endregion

        #region OrdenarItem

        /// <summary>
        ///  
        /// </summary>
        /// <param name="codigo">Código do Item que esta sendo movido</param>
        /// <param name="newParent">Código do novo pai</param>
        /// <param name="posicao">index</param>
        /// <returns>json</returns>
        [HttpPost]
        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult OrdenarItem(decimal codigo, decimal? newParent, int posicao)
        {
            bool IsSuccess;
            try
            {
                //Se código for null, item foi movido para a raiz, definir como 0
                if (!newParent.HasValue) newParent = 0;

                IsSuccess = BLMenu.Ordenar(codigo, newParent.Value, posicao, PortalAtual.ConnectionString);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                IsSuccess = false;
            }

            return new JsonResult()
            {
                Data = new
                {
                    success = IsSuccess
                }
            };
        }

        #endregion

        #region Renomear

        [HttpPost]
        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult Renomear(decimal codigo, string novoNome)
        {
            bool IsSuccess;

            try
            {
                MLMenuItem obj = new MLMenuItem();
                obj.Codigo = codigo;
                obj.Nome = novoNome;

                CRUD.SalvarParcial(obj, PortalAtual.ConnectionString);
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                IsSuccess = false;
            }

            return new JsonResult()
            {
                Data = new
                {
                    success = IsSuccess
                }
            };
        }

        #endregion

        #region ValidarUrl

        /// <summary>
        /// Validar se a Url já esta cadastrada no sistema
        /// </summary>
        /// <returns>Json</returns>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult ValidarNome(decimal? Codigo, string Nome)
        {
            try
            {
                var model = CRUD.Obter(new MLMenu() { Codigo = Codigo, Nome = Nome, CodigoPortal = PortalAtual.Codigo }, PortalAtual.ConnectionString);

                if (model != null && model.Codigo.HasValue && model.Codigo.Value > 0 && Codigo != model.Codigo)
                    return Json(false);
                return Json(true);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(T("Não foi possível validar a URL, entre em contato com o administrador do sistema."));
            }
        }

        #endregion

        #endregion

        #endregion

        #region MenuView

        #region Excluir

        [HttpPost]
        [CheckPermission(global::Permissao.Excluir)]
        public ActionResult MenuViewExcluir(List<string> ids)
        {
            try
            {
                CRUD.Excluir<MLMenuView>(ids);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Index

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult MenuView()
        {
            return View("MenuView/Index");
        }

        #endregion

        #region Item

        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult MenuViewItem(decimal? id)
        {
            try
            {
                var model = new MLMenuView();

                if (id.HasValue)
                {
                    model = CRUD.Obter<MLMenuView>(id.Value, BLPortal.Atual.ConnectionString);
                    model.View = Microsoft.JScript.GlobalObject.unescape(model.View);

                    if (!string.IsNullOrWhiteSpace(model.Script))
                    {
                        model.Script = Microsoft.JScript.GlobalObject.unescape(model.Script);
                    }
                }
                else
                {
                    //Formulário padrão
                    //model.View = "<form class='jovens-contato' action='@Portal.Url()/modulo/faleconosco/salvar' method='post' novalidate='novalidate'> \n  <input type='hidden' name='codigoPagina' value='@ViewData[\"codigoPagina\"]'>\n  <input type='hidden' name='repositorio' value='@ViewData[\"repositorio\"]'>\n  <input type='hidden' name='modelo' value='@ViewData[\"modelo\"]'>\n  <div class='row'>\n    \n    <div class='col-xs-24 form-group text-right'>\n        <button type='submit' class='btn btn-default'>Enviar</button>\n    </div>\n  </div>\n</form>";
                }

                return View("MenuView/Item", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult MenuViewItem(MLMenuView model, string nomeAntigo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var portal = BLPortal.Atual;
                    var diretorioFisico = GerarMenuArquivos(model);

                    model.CodigoPortal = portal.Codigo;

                    //Quando entrar aqui já validou o nome, não terá nome repetido
                    CRUD.Salvar(model, portal.ConnectionString);

                    //Edição de registro com novo nome.
                    if (!string.IsNullOrWhiteSpace(nomeAntigo) && model.Nome != nomeAntigo && model.Codigo.HasValue)
                    {
                        var arquivoAntigo = Path.Combine(diretorioFisico, nomeAntigo + ".cshtml");
                        var arquivoScriptAntig = Path.Combine(diretorioFisico, "Script" + nomeAntigo + ".cshtml");

                        //Deletar o arquivo antigo
                        if (System.IO.File.Exists(arquivoAntigo))
                        {
                            System.IO.File.Delete(arquivoAntigo);
                            BLReplicar.ExcluirArquivosReplicados(arquivoAntigo);
                        }
                            

                        if (System.IO.File.Exists(arquivoScriptAntig))
                        {
                            System.IO.File.Delete(arquivoScriptAntig);
                            BLReplicar.ExcluirArquivosReplicados(arquivoScriptAntig);
                        }
                            
                    }
                }

                TempData["Salvo"] = true;

                return RedirectToAction("MenuView");
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return View("Erro", ex);
            }
        }

        private string GerarMenuArquivos(MLMenuView model)
        {
            var portal = BLPortal.Atual;
            var conteudo = Microsoft.JScript.GlobalObject.unescape(model.View);
            var diretorioVirtual = BLConfiguracao.Pastas.ModuloGenerico(PortalAtual.Diretorio, MLMenu.Pasta.Views);
            var diretorioFisico = Server.MapPath(diretorioVirtual);
            var arquivoFisico = string.Empty;
            var arquivoFisicoScript = string.Empty;
            var conteudoScript = string.Empty;

            if (!string.IsNullOrWhiteSpace(model.Script))
            {
                conteudoScript = Microsoft.JScript.GlobalObject.unescape(model.Script);
            }

            if (!Directory.Exists(diretorioFisico))
                Directory.CreateDirectory(diretorioFisico);

            //o nome do arquivo não pode começar com Script, palavra reservada para o arquivo gerado pelo sistema
            if (model.Nome.ToLower().StartsWith("script"))
                model.Nome.ToLower().Replace("script", "");

            //O arquivo script será sempre Script<NomeFormulario>
            arquivoFisico = Path.Combine(diretorioFisico, model.Nome + ".cshtml");
            arquivoFisicoScript = Path.Combine(diretorioFisico, "Script" + model.Nome + ".cshtml");

            // Salvar no disco
            using (var arquivo = new StreamWriter(arquivoFisico))
            {
                arquivo.Write(conteudo);
                arquivo.Close();
            }

            using (var arquivo = new StreamWriter(arquivoFisicoScript))
            {
                arquivo.Write(conteudoScript);
                arquivo.Close();
            }

            BLReplicar.Arquivo(arquivoFisico);
            BLReplicar.Arquivo(arquivoFisicoScript);

            return diretorioFisico;
        }

        #endregion

        #region Listar

        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ListarMenuView(MLMenuView criterios)
        {
            try
            {
                //criterios.CodigoPortal = PortalAtual.Codigo;
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

        #region ValidarNome

        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult MenuViewValidarNome(decimal? codigo, string nome)
        {
            var portal = BLPortal.Atual;
            var model = CRUD.Obter(new MLMenuView() { Nome = nome, CodigoPortal = portal.Codigo }, portal.ConnectionString);

            if (model != null && model.Codigo.HasValue && ((codigo.HasValue && codigo.Value != model.Codigo) || !codigo.HasValue))
                return Json(T("Já existe um arquivo com esse nome."));

            return Json(true);
        }

        #endregion

        #endregion

        #region Carregar Menu Admin

        /// <summary>
        /// Carregar o menu na área admin
        /// </summary>
        /// <param name="id">Código do registro</param>
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult CarregarMenuAdmin()
        {
            MLMenuCompleto model = new MLMenuCompleto();
            MLMenuView modelView = new MLMenuView();
            var portal = PortalAtual.Obter;
            var dirView = string.Format("~/Portal/{0}/Arquivos/{1}/", portal.Diretorio, MLMenu.Pasta.Views);

            try
            {
                if (portal.CodigoMenu.HasValue)
                    model = BLMenu.ObterCompleto(portal.CodigoMenu.Value, portal.Codigo.Value, BLUsuario.ObterLogado().Codigo.Value);

                if (portal.CodigoMenuView.HasValue)
                    modelView = CRUD.Obter<MLMenuView>(portal.CodigoMenuView.Value, portal.ConnectionString);

                //Caso não exista o menu, ou não tenha essa informação cadastrada, retornar menu padrão
                if (model == null || modelView == null)
                {
                    return View("~/areas/cms/views/dashboard/sidebar.cshtml");
                }

                var diretorioFisico = Server.MapPath(BLConfiguracao.Pastas.ModuloGenerico(PortalAtual.Diretorio, MLMenu.Pasta.Views));
                var arquivoFisico = Path.Combine(diretorioFisico, model.Nome + ".cshtml");

                if (!Directory.Exists(diretorioFisico) || !System.IO.File.Exists(arquivoFisico))
                    GerarMenuArquivos(modelView);
                
                return View(dirView + model.Nome + ".cshtml", model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion
    }
}
