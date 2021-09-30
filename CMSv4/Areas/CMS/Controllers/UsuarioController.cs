using System;
using System.Web.Mvc;
using Framework.Utilities;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Transactions;
using CMSv4.Model;

namespace CMSApp.Areas.CMS.Controllers
{
    /// <summary>
    /// Usuarios
    ///     Entidada das pessoas que acessam o sistema
    /// </summary>
    public class UsuarioController : SecureController
    {
        #region Index

        /// <summary>
        /// Listagem
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
        public ActionResult Listar(MLUsuarioGrid criterios)
        {
            try
            {
                return CRUD.ListarJson(criterios, Request.QueryString);
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
            if (id == null)
                id = 0;

            var model = BLUsuario.ObterCompleto(id);

            ViewData["Grupos"] = CRUD.Listar(new MLGrupo { Ativo = true });
            ViewData["Portais"] = CRUD.Listar(new MLPortal { Ativo = true });

            return View(model);

        }

        /// <summary>
        /// Salvar registro
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult Item(MLUsuario model)
        {
            try
            {
                string listaCodigoPortal = Request.Form["listaCodigoPortal"];
                string listaCodigoGrupo = Request.Form["listaCodigoGrupo"];


                if (ModelState.IsValid)
                {
                    string senha = BLUtilitarios.GetNewPassword();

                    

                    if (!model.Codigo.HasValue)
                    {
                        model.Senha = BLEncriptacao.EncriptarSenha(senha);
                        model.DataCadastro = DateTime.Now;
                        model.AlterarSenha = true;
                    }
                    else if (!string.IsNullOrEmpty(model.Senha))
                    {
                        senha = model.Senha;
                        model.Senha = BLEncriptacao.EncriptarSenha(senha);
                    }

                    using (var scope = new TransactionScope())
                    {
                        model.Ativo = model.Ativo.HasValue && model.Ativo.Value ? true : false;
                        var codigo = CRUD.Salvar<MLUsuario>(model);
                        bool salvo = codigo > 0;

                        CRUD.Excluir<MLUsuarioItemGrupo>("CodigoUsuario", codigo);

                        if (!String.IsNullOrEmpty(listaCodigoGrupo))
                        {
                            foreach (var item in listaCodigoGrupo.Split(','))
                            {
                                var novoItem = new MLUsuarioGrupo
                                {
                                    CodigoUsuario = codigo,
                                    CodigoGrupo = Convert.ToDecimal(item)
                                };

                                CRUD.Salvar<MLUsuarioGrupo>(novoItem);
                            }
                        }

                        CRUD.Excluir<MLUsuarioItemPortal>("CodigoUsuario", codigo);

                        if (!String.IsNullOrEmpty(listaCodigoPortal))
                        {
                            foreach (var item in listaCodigoPortal.Split(','))
                            {
                                var novoItem = new MLUsuarioItemPortal
                                {
                                    CodigoUsuario = codigo,
                                    CodigoPortal = Convert.ToDecimal(item)
                                };

                                CRUD.Salvar(novoItem);
                            }
                        }

                        scope.Complete();

                        TempData["Salvo"] = salvo;

                        try
                        {
                            if (!model.Codigo.HasValue)
                            {
                                string modelo = BLEmail.ObterModelo(BLEmail.ModelosPadrao.NovaSenha)
                                    .Replace("[[link-site]]", string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority))
                                    .Replace("[[nome]]", model.Nome)
                                    .Replace("[[senha]]", senha);

                                BLEmail.Enviar(T("Nova Senha"), model.Email, modelo);
                            }
                        }
                        catch { } //O cadastro de registro não deve ser interrompido por não conseguir enviar email

                        if (salvo)
                            return RedirectToAction("Index");
                    }
                }

                return RedirectToAction("Index");
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("IX_FWK_USU_USUARIO_LOGIN"))
                {
                    return RedirectToAction("Item", new { id = model.Codigo, success = false, error = string.Format(T("Não pode ser inserido '{0}'. Login já existente."), model.Login) });
                }
                else if (ex.Message.Contains("IX_FWK_USU_USUARIO_EMAIL"))
                {
                    return RedirectToAction("Item", new { id = model.Codigo, success = false, error = string.Format(T("Não pode ser inserido '{0}'. Email já existente."), model.Email) });
                }
                else
                    return Json(new { success = false, msg = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.Message });
            }
        }


        /// <summary>
        /// Verifica se o login não ficará duplicado ao salvar
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult IsValid(decimal? id, string login, string email)
        {
            if (!String.IsNullOrEmpty(login))
            {
                var model = BLUsuario.ObterCompleto(login);
                if ((id.HasValue && model != null && model.Ativo.GetValueOrDefault() && model.Codigo.Value != id.Value) || (!id.HasValue && model != null && model.Codigo.HasValue && model.Ativo.GetValueOrDefault()))
                    return Json(string.Format(T("Não pode ser inserido '{0}'. Login já existente."), login));
            }

            if (!String.IsNullOrEmpty(email))
            {
                var model = CRUD.Obter<MLUsuario>(new MLUsuario() { Email = email, Ativo = true });
                if ((id.HasValue && model != null && model.Codigo.HasValue && model.Codigo.Value != id.Value) || !id.HasValue && model != null)
                    return Json(string.Format(T("Não pode ser inserido '{0}'. Email já existente."), email));
            }

            return Json(true);
        }

        #endregion

        #region Senha

        /// <summary>
        /// Salvar registro
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult Senha(decimal id)
        {
            try
            {
                var model = CRUD.Obter<MLUsuario>(id);
                var token = Guid.NewGuid().ToString();

                model.AlterarSenha = true;
                model.TokenNovaSenha = token;

                CRUD.SalvarParcial<MLUsuario>(model);

                // enviar email
                BLEmail.Enviar("Nova Senha", model.Email,
                    BLEmail.ObterModelo(BLEmail.ModelosPadrao.AlterarSenha)
                                .Replace("[[link-site]]", string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority))
                                .Replace("[[url]]", string.Format("{0}://{1}{2}{3}", Request.Url.Scheme, Request.Url.Authority, "/cms/?token=", token))
                                .Replace("[[nome]]", model.Nome)
                );

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
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
        ///     /Area/Controller/Excluir/ids
        /// </remarks>
        [CheckPermission(global::Permissao.Excluir)]
        [HttpPost]
        public ActionResult Excluir(List<string> ids)
        {
            try
            {
                CRUD.Excluir<MLUsuario>(ids);

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
