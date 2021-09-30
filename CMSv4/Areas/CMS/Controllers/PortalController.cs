using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CMSApp.Areas.CMS.Controllers
{
    public class PortalController : SecureController
    {
        #region Index

        /// <summary>
        /// Index
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/Controller
        ///     /Area/Controller/Index
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Index()
        {
            return View();
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
        public JsonResult Listar(MLPortal criterios)
        {
            try
            {
                return CRUD.ListarJson<MLPortal>(criterios, Request.QueryString);
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
        //[CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Item(decimal? id)
        {
            var model = new MLPortal();

            try
            {
                if (id.HasValue)
                    model = CRUD.Obter<MLPortal>(id.Value);

                ViewBag.Menus = CRUD.Listar(new MLMenu());
                ViewBag.MenuViews = CRUD.Listar(new MLMenuView());
                ViewBag.PossuiPermissao = validarPermissao();

                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);

                return View(model);
            }
        }

        /// <summary>
        /// Salvar registro
        /// </summary>
        [HttpPost, CheckPermission(global::Permissao.Modificar)]
        public ActionResult Item(MLPortal model, string optionTipo, string DiretorioAnterior, HttpPostedFileBase arquivosPortal)
        {
            bool novoPortal = !model.Codigo.HasValue;
            bool possuiPermissao = validarPermissao();
            bool prosseguirTransaction = false;
            var usuarioLogado = BLUsuario.ObterLogado();

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.PossuiPermissao = possuiPermissao;

                    return View(model);
                }

                #region Validar informações do banco de dados

                if (novoPortal)
                {
                    var msgret = ValidarConexao(optionTipo == "N", model.ServidorBD, model.UsuarioBD, model.SenhaBD, model.NomeBD);

                    if (!string.IsNullOrEmpty(msgret))
                    {
                        ViewBag.Error = msgret;
                        ViewBag.PossuiPermissao = possuiPermissao;

                        return View(model);
                    }
                }

                #endregion

                #region Validar permissão para criação de diretórios/arquivos

                if (novoPortal && !possuiPermissao)
                {
                    ViewBag.Error = T("Usuário de IIS não possui permissão para criação de diretórios e arquivos. Contate o administrador");
                    ViewBag.PossuiPermissao = possuiPermissao;

                    return View(model);
                }

                #endregion

                var menuItens = CRUD.Listar(new MLMenuItem() { PadraoParaPortal = true });

                using (var trans = new System.Transactions.TransactionScope())
                {
                    model.SenhaBDCript = BLEncriptacao.EncriptarAes(model.SenhaBD);

                    if (model.Manutencao == null)
                    {
                        model.Manutencao = false;
                    }

                    if (model.Ativo == null)
                    {
                        model.Ativo = true;
                    }

                    if (menuItens == null)
                        menuItens = new List<MLMenuItem>();

                    if(menuItens.Count > 0)
                        model.CodigoMenu = menuItens.FirstOrDefault().Codigo;

                    model.Codigo = CRUD.Salvar(model);

                    if (novoPortal) //associar o usuário atual ao novo portal
                    {
                        CRUD.Salvar(new MLUsuarioItemPortal
                        {
                            CodigoPortal = model.Codigo,
                            CodigoUsuario = usuarioLogado.Codigo
                        });
                    }

                    trans.Complete();

                    prosseguirTransaction = true;
                }

                if (prosseguirTransaction)
                {
                    #region novoPortal

                    if (novoPortal)
                    {
                        // Criar registro para o portal no servidor informado
                        var portalPublico = new MLPortalPublico();

                        portalPublico.Codigo = model.Codigo;
                        portalPublico.Restrito = model.RestritoAreaPublica;

                        CRUD.Salvar(portalPublico, model.ConnectionString);

                        // Criar pastas, um layout e template padrão para poder ser testado

                        // Criar Diretorios
                        CriarDiretorio(BLConfiguracao.Pastas.ImagensPortal(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.FlashPortal(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.DocumentosPortal(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.ClientesPortal(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.ModuloFaleConoscoEmail(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.ModuloFaleConoscoForm(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.ModuloArquivos(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.ModuloArquivosThumbImagens(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.ModuloArquivosImagens(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.ModuloMultimidia(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.ModuloMultimidiaThumbImagens(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.ModuloMultimidiaImagens(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.ModuloListas(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.ModuloRegiaoPais(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.ModuloBanner(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.ModuloBannerHover(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.ModuloResultado(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.ModuloEvento(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.ModuloMenu(model.Diretorio));
                        CriarDiretorio(BLConfiguracao.Pastas.ModuloGaleria(model.Diretorio));

                        var arquivos = CriarDiretorio(string.Format("/portal/{0}/arquivos", model.Diretorio));
                        var content = CriarDiretorio(BLConfiguracao.Pastas.ContentPortal(model.Diretorio));
                        var layouts = CriarDiretorio(BLConfiguracao.Pastas.LayoutsPortal(model.Diretorio));
                        var templates = CriarDiretorio(BLConfiguracao.Pastas.TemplatesPortal(model.Diretorio));

                        // Copiar Conteudos
                        CopiarDiretorio(Server.MapPath("~/Portal/_modelo/arquivos"), arquivos);
                        CopiarDiretorio(Server.MapPath("~/Portal/_modelo/content"), content);
                        CopiarDiretorio(Server.MapPath("~/Portal/_modelo/layouts"), layouts);
                        CopiarDiretorio(Server.MapPath("~/Portal/_modelo/templates"), templates);

                        #region MENU

                        //Duplicar os itens exibidos nesse portal para o novo
                        var listPortalMenu = new List<MLPortalMenu>(menuItens.Count);

                        foreach (var item in menuItens)
                            listPortalMenu.Add(new MLPortalMenu()
                            {
                                CodigoPortal = model.Codigo,
                                CodigoMenuItem = item.Codigo
                            });

                        CRUD.Salvar(listPortalMenu, string.Empty);

                        #endregion

                        RenovarRotas(model.Codigo.GetValueOrDefault(), usuarioLogado.Codigo.GetValueOrDefault());
                    }
                    #endregion

                    //renomear diretório caso seja alterado
                    if (!string.IsNullOrWhiteSpace(DiretorioAnterior) && !model.Diretorio.Equals(DiretorioAnterior))
                    {
                        Directory.Move(Server.MapPath(string.Concat("/portal/", DiretorioAnterior)), Server.MapPath(string.Concat("/portal/", model.Diretorio)));

                        RenovarRotas(model.Codigo.GetValueOrDefault(), usuarioLogado.Codigo.GetValueOrDefault());
                    }

                    if (arquivosPortal != null && arquivosPortal.ContentLength > 0)
                    {
                        var pastaFisica = Server.MapPath(BLConfiguracao.Instalacao.PastaConteudo);

                        if (!Directory.Exists(pastaFisica))
                            Directory.CreateDirectory(pastaFisica);

                        var arquivo = Path.Combine(pastaFisica, string.Concat(model.Diretorio, Path.GetExtension(arquivosPortal.FileName)));

                        if (System.IO.File.Exists(arquivo))
                            System.IO.File.Delete(arquivo);

                        arquivosPortal.SaveAs(arquivo);

                        BLUtilitarios.DescompactarZip(arquivo, Server.MapPath(string.Format("~/portal/{0}/", model.Diretorio)));
                    }
                }

                ViewBag.PossuiPermissao = possuiPermissao;

                TempData["Salvo"] = model.Codigo > 0;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);

                ViewBag.PossuiPermissao = possuiPermissao;

                return View(model);
            }
        }

        private void RenovarRotas(decimal codigoPortal, decimal codigoUsuario)
        {
            // Reseta Rota do Projeto. Para que o novo portal seja possível ser roteada
            for (int i = RouteTable.Routes.Count - 1; i >= 0; i--)
            {
                RouteTable.Routes.RemoveAt(i);
            }

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Reset Cache do Portal
            BLCachePortal.ResetarCachePortal(codigoPortal);

            // Remove Usuário da Sessão
            Session.Remove(string.Format("ctxUsuario{0}", codigoUsuario));
        }

        #endregion

        #region CheckPortalAtual
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult CheckPortalAtual()
        {
            var sb = new System.Text.StringBuilder();
            var portal1 = BLPortal.Atual;
            var portal2 = PortalAtual.Obter;

            if (portal1 != null)
            {
                sb.AppendLine("BLPortal:");
                sb.AppendLine(portal1.Nome);
            }

            if (portal2 != null)
            {
                sb.AppendLine("PortalAtual:");
                sb.AppendLine(portal2.Nome);
            }

            return View("Senha", (object)sb.ToString());
        }
        #endregion

        #region WebConfig
        //[CheckPermission(global::Permissao.Visualizar)]
        //public ActionResult WebConfig(string senha)
        //{
        //    try
        //    {
        //        ////var key_provider = "VM2_Braskem_Provider";

        //        //////Não foi possível utilizar a criacao de chave rsa
        //        ////CriarChaveRSA(key_provider, 1024, Server.MapPath("/Portal/Principal")); //Cria chave caso não haja

        //        //var config = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
        //        //var provider = "RsaProtectedConfigurationProvider"; //DataProtectionConfigurationProvider

        //        //var connectionStrings = config.GetSection("connectionStrings").SectionInformation;
        //        //var appSettings = config.GetSection("appSettings").SectionInformation;
        //        //var smtp = config.GetSection("system.net/mailSettings/smtp").SectionInformation;

        //        //connectionStrings.ForceSave = true;
        //        //appSettings.ForceSave = true;
        //        //smtp.ForceSave = true;

        //        //if (string.IsNullOrEmpty(senha)) //Criptografa
        //        //{
        //        //    connectionStrings.ProtectSection(provider);
        //        //    appSettings.ProtectSection(provider);
        //        //    smtp.ProtectSection(provider);

        //        //    config.SaveAs(Path.Combine(Server.MapPath("/Portal/principal/"), "web_prod.config"), System.Configuration.ConfigurationSaveMode.Full);
        //        //}
        //        //else //Descriptografa
        //        //{
        //        //    connectionStrings.UnprotectSection();
        //        //    appSettings.UnprotectSection();
        //        //    smtp.UnprotectSection();

        //        //    config.SaveAs(Path.Combine(Server.MapPath("/Portal/principal/"), "web_prod_des.config"), System.Configuration.ConfigurationSaveMode.Full);
        //        //}

        //        return View("Senha", "OK" as object);
        //    }
        //    catch (Exception ex)
        //    {
        //        return View("Senha", ex.Message as object);
        //    }

        //}
        #endregion

        #region Desativar

        [CheckPermission(global::Permissao.Excluir)]
        public ActionResult Desativar(decimal? id)
        {
            if (id.HasValue)
            {
                return View(id.Value);
            }

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken, CheckPermission(global::Permissao.Excluir)]
        public ActionResult Desativar(decimal id)
        {
            try
            {
                var model = new MLPortal();

                model.Codigo = id;
                model.Ativo = false;

                CRUD.SalvarParcial(model);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        #endregion

        #region Instalar

        ///// <summary>
        ///// Inicia a instalação de um novo portal
        ///// </summary>
        ///// <param name="id">Código do registro</param>
        ///// <remarks>
        ///// GET:
        /////     /Area/Controller/Item/id
        ///// </remarks>
        //[CheckPermission(global::Permissao.Visualizar)]
        //public ActionResult Instalar()
        //{
        //    try
        //    {
        //        return View(new MLPortal());
        //    }
        //    catch (Exception ex)
        //    {
        //        ApplicationLog.ErrorLog(ex);
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// Salvar registro
        ///// </summary>
        //[CheckPermission(global::Permissao.Modificar,Url="instalarportal")]
        //[HttpPost]
        //public ActionResult Instalar(HttpPostedFileBase arquivosPortal, MLPortalComUsuarios model, string optionTipo)
        //{
        //    bool novoPortal = !model.Codigo.HasValue;
        //    //BLPortal.exec();
        //    try
        //    {
        //        if (novoPortal)
        //        {
        //            if (optionTipo == "E" || optionTipo == "N")
        //            {
        //                var msgErr = T("Preencha o campo") + " {0}";
        //                if (string.IsNullOrEmpty(model.ServidorBD))
        //                    ModelState.AddModelError("ServidorBD", string.Format(msgErr, T("Servidor")));
        //                if (string.IsNullOrEmpty(model.UsuarioBD))
        //                    ModelState.AddModelError("UsuarioBD", string.Format(msgErr, T("Usuário")));
        //                if (string.IsNullOrEmpty(model.SenhaBD))
        //                    ModelState.AddModelError("SenhaBD", string.Format(msgErr, T("Senha")));
        //                if (string.IsNullOrEmpty(model.NomeBD))
        //                    ModelState.AddModelError("NomeBD", string.Format(msgErr, T("Nome")));

        //                if (ModelState.IsValid)
        //                {
        //                    var msgret = "";
        //                    msgret = ValidarConexao(optionTipo == "N", model.ServidorBD, model.UsuarioBD, model.SenhaBD, model.NomeBD);
        //                    if (!string.IsNullOrEmpty(msgret))
        //                        ModelState.AddModelError("Conexao", msgret);
        //                }
        //            }
        //        }
        //        ViewBag.TipoInstalacao = optionTipo;

        //        if (ModelState.IsValid)
        //        {
        //            model.SenhaBDCript = BLEncriptacao.EncriptarAes(model.SenhaBD);
        //            model.Ativo = true;
        //            model.Manuntencao = true;
        //            model.Usuarios = new List<MLUsuarioItemPortal>();
        //            model.Usuarios.Add(new MLUsuarioItemPortal { CodigoUsuario = BLUsuario.ObterLogado().Codigo });

        //            var codigo = BLPortal.Salvar(model);

        //            if (arquivosPortal != null)
        //            {
        //                var pastaFisica = Server.MapPath(BLConfiguracao.Instalacao.PastaConteudo);
        //                if (!Directory.Exists(pastaFisica)) Directory.CreateDirectory(pastaFisica);

        //                var arquivo = Path.Combine(pastaFisica, codigo + ".zip");
        //                if (System.IO.File.Exists(arquivo)) System.IO.File.Delete(arquivo);
        //                arquivosPortal.SaveAs(arquivo);
        //            }

        //            //Criar plano de instalação
        //            if (novoPortal)
        //                BLPortalInstalacao.InserirSeqInstalacao(codigo.Value, BLConfiguracao.Instalacao.Etapas(optionTipo));



        //            return View(model);



        //            // Reset Cache
        //            BLCachePortal.ResetarCachePortal(codigo.Value);
        //        }

        //        return Json(new { success = ModelState.IsValid, msg = ModelState.ObterMensagensErro() });
        //    }
        //    catch (Exception ex)
        //    {
        //        ApplicationLog.ErrorLog(ex);
        //        return Json(new { success = false, msg = ex.Message });
        //    }
        //}

        #endregion

        #region ConnectionTest
        [HttpPost]
        public ActionResult ConnectionTest(bool novo, string servidor, string usuario, string senha, string nomebanco)
        {
            var msgret = "";

            try
            {
                msgret = ValidarConexao(novo, servidor, usuario, senha, nomebanco);
                return Json(new { success = string.IsNullOrEmpty(msgret), msg = msgret });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }
        #endregion

        #region ValidarNomeDiretorio
        [HttpPost, CheckPermission(global::Permissao.Visualizar)]
        public JsonResult ValidarNomeDiretorio(string diretorio, decimal? id)
        {
            bool nomeValido = false;

            try
            {
                //VALIDAR NOME DO DIRETÓRIO PARA SABER SE TEM OUTRO PORTAL COM O MESMO NOME
                var listaUrl = CRUD.Listar<MLPortal>(new MLPortal
                {
                    Diretorio = diretorio,
                    Ativo = true
                });

                if (listaUrl != null && listaUrl.Count > 0)
                {
                    if (id.HasValue)
                        nomeValido = listaUrl.Find(a => a.Codigo == id) != null;
                    else
                        nomeValido = false;
                }
                else
                    nomeValido = true;

                //VALIDAR URL REGISTRADA NO GLOBAL.ASAX
                if (nomeValido)
                {
                    var listaUrlsRota = (from e in RouteTable.Routes.OfType<Route>()
                                         where e.Url.Equals(diretorio, StringComparison.InvariantCultureIgnoreCase)
                                         select e.Url).ToList();

                    nomeValido = listaUrlsRota.Count == 0;
                }

                //VALIDAR URL DE PÁGINA COM O MESMO DIRETÓRIO DO PORTAL
                var paginas = CRUD.Listar(new MLPagina() { Url = diretorio }, 1, "Nome", "ASC", PortalAtual.ConnectionString, false);

                if (paginas != null && paginas.Count > 0)
                    nomeValido = false;

                return Json(nomeValido, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        private bool validarPermissao()
        {
            var permissionSet = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.None);
            var writePermission = new System.Security.Permissions.FileIOPermission(System.Security.Permissions.FileIOPermissionAccess.Write, Server.MapPath("~/Portal"));

            permissionSet.AddPermission(writePermission);

            //True - Possui permissão / False - Não possui
            return permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet);
        }

        private string ValidarConexao(bool novo, string servidor, string usuario, string senha, string nomebanco)
        {
            var connectionstring = "server={0};uid={1};pwd={2};database={3}";
            try
            {
                if (novo)
                {
                    var conn = string.Format(connectionstring, servidor, usuario, senha, "master");

                    return BLPortalInstalacao.ConnectionTestMaster(conn, nomebanco);
                }
                else
                {
                    var conn = string.Format(connectionstring, servidor, usuario, senha, nomebanco);

                    var valido = BLPortalInstalacao.ConnectionTest(conn);

                    return !valido ? T("Erro na conexão com o banco de dados") : "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CopiarDiretorio(string diretorioDe, string diretorioPara)
        {
            try
            {
                foreach (string diretorio in Directory.GetDirectories(diretorioDe, "*", SearchOption.AllDirectories))
                    Directory.CreateDirectory(diretorio.Replace(diretorioDe, diretorioPara));

                foreach (string arquivo in Directory.GetFiles(diretorioDe, "*.*", SearchOption.AllDirectories))
                    System.IO.File.Copy(arquivo, arquivo.Replace(diretorioDe, diretorioPara), true);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }
        }

        private string CriarDiretorio(string dir)
        {
            var dirPath = Server.MapPath(dir);

            if (!System.IO.Directory.Exists(dirPath))
            {
                System.IO.Directory.CreateDirectory(dirPath);
            }

            return dirPath;
        }
    }
}
