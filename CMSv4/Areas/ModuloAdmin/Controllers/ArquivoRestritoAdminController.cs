using System;
using System.Collections.Generic;
using System.IO;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Framework.Utilities;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class ArquivoRestritoAdminController : AdminBaseCRUDPortalController<MLArquivoRestritoCategoria, MLArquivoRestritoCategoria>
    {     
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
        public override ActionResult Item(decimal? id)
        {
            try
            {
                var portal = PortalAtual.Obter;
                var model = new MLArquivoRestritoCategoria();
                if (id.HasValue) model = CRUD.Obter<MLArquivoRestritoCategoria>(new MLArquivoRestritoCategoria { Codigo = id }, portal.ConnectionString);

                ///Grupos cliente  
                ViewBag.GruposCliente = CRUD.Listar(new MLGrupoCliente { CodigoPortal = PortalAtual.Codigo, Ativo = true }, portal.ConnectionString);
                if (id > 0)
                    ViewData["GruposClientesSelecionados"] = CRUD.Listar<MLArquivoRestritoCategoriaGrupoCliente>(new MLArquivoRestritoCategoriaGrupoCliente { CodigoCategoria = id });

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
        public override ActionResult Item(MLArquivoRestritoCategoria model)
        {
            var portal = PortalAtual.Obter;

            try
            {
                if (ModelState.IsValid)
                {
                    using (var scope = new TransactionScope())
                    {
                        model.CodigoPortal = portal.Codigo;
                        model.Ativo = model.Ativo ?? false;
                        model.Codigo = CRUD.Salvar<MLArquivoRestritoCategoria>(model, portal.ConnectionString);

                        /*Relacionamento de categoria arquivo restrito*/
                        string GruposUsuario = Request.Form["GruposUsuario"];
                        CRUD.Excluir<MLArquivoRestritoCategoriaGrupoCliente>(new MLArquivoRestritoCategoriaGrupoCliente { CodigoCategoria = model.Codigo.Value });
                        List<MLArquivoRestritoCategoriaGrupoCliente> lstMLVitrineGrupoCliente = new List<MLArquivoRestritoCategoriaGrupoCliente>();

                        if (!string.IsNullOrEmpty(GruposUsuario))
                        {
                            foreach (var item in GruposUsuario.Replace("multiselect-all", "").TrimStart(',').Split(','))
                            {
                                if (!String.IsNullOrWhiteSpace(item))
                                    lstMLVitrineGrupoCliente.Add(new MLArquivoRestritoCategoriaGrupoCliente { CodigoCategoria = model.Codigo.Value, CodigoGrupoCliente = Convert.ToDecimal(item) });
                            }
                            if (lstMLVitrineGrupoCliente.Count > 0)
                                CRUD.Salvar(lstMLVitrineGrupoCliente, String.Empty);
                        }

                        scope.Complete();

                        TempData["Salvo"] = model.Codigo > 0;
                        return RedirectToAction("Index");
                    }
                }
               
                ViewBag.GruposCliente = CRUD.Listar(new MLGrupoCliente { CodigoPortal = PortalAtual.Codigo, Ativo = true }, portal.ConnectionString);

                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

        #region Validar nome

        /// <summary>
        /// Validar Nome do Registro
        /// </summary>
        /// <param name="url"></param>
        /// <param name="id"></param>
        /// <returns>Json</returns>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
        public ActionResult ValidarNome(string nome, decimal? id)
        {
            try
            {
                var portal = PortalAtual.Obter;
                MLArquivoRestritoCategoria objMLArquivoRestritoCategoria = CRUD.Obter<MLArquivoRestritoCategoria>(new MLArquivoRestritoCategoria { Nome = nome }, portal.ConnectionString);

                if (objMLArquivoRestritoCategoria != null && objMLArquivoRestritoCategoria.Codigo.HasValue && objMLArquivoRestritoCategoria.Codigo.Value > 0 && id != objMLArquivoRestritoCategoria.Codigo)
                    return Json(T("Já existe uma categoria de arquivo cadastrada com esse Nome"));
                return Json(true);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(T("Já existe uma categoria de arquivo cadastrada com esse Nome"));
            }
        }

        #endregion

        //////////////////ARQUIVOS
        #region Arquivo
        /// <summary>
        /// Arquivo
        /// </summary>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Arquivo()
        {
            return View();
        }
        #endregion

        #region ArquivoItem

        /// <summary>
        /// Visualizar ou Editar o registro conforme permissão do usuário
        /// </summary>
        /// <param name="id">Código do registro</param>
        /// <remarks>
        /// GET:
        ///     /Area/Controller/Item/id
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult ArquivoItem(MLArquivoRestrito model)
        {
            try
            {
                MLPortal portal = PortalAtual.Obter;

                if (model != null && model.Codigo.HasValue)
                {
					model = CRUD.Obter<MLArquivoRestrito>(model.Codigo.Value, portal.ConnectionString);

                    if (!String.IsNullOrEmpty(model.Descricao))
                        model.Descricao = Microsoft.JScript.GlobalObject.unescape(model.Descricao);
                }
                else
                    model = new MLArquivoRestrito();

                ViewBag.Tipos = CRUD.Listar(new MLArquivoRestritoTipo { Ativo = true }, portal.ConnectionString);

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
        public ActionResult ArquivoItem(MLArquivoRestrito model, bool? IsRemoverCapa)
        {
            MLPortal portal = PortalAtual.Obter;

            try
            {
                if (ModelState.IsValid)
                {
					using (TransactionScope scope = new TransactionScope(portal.ConnectionString))
                    {
                        if (!model.Ativo.HasValue)
                            model.Ativo = false;

                        if (!model.Destaque.HasValue)
                            model.Destaque = false;
                                                
                        model.LogPreencher(model.Codigo.HasValue);
                        model.Codigo = CRUD.Salvar<MLArquivoRestrito>(model, portal.ConnectionString);

                        scope.Complete();
                    }

                    return Redirect("/cms/" + PortalAtual.Diretorio + "/ArquivoRestritoAdmin/arquivo?codigocategoria=" + model.CodigoCategoria);
                }

                return Json(new { success = false, msg = this.GetModelStateErrors() });
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = ex.Message });
            }
        }

        #endregion

		#region ExcluirArquivos
		/// <summary>
		/// Excluir registro
		/// </summary>
		/// <remarks>
		/// GET:
		///     /Area/Controller/Excluir/id
		/// </remarks>
		[CheckPermission(global::Permissao.Excluir)]
		[HttpPost]
		public ActionResult ExcluirArquivos(List<string> ids)
		{
			try
			{
				MLPortal portal = PortalAtual.Obter;
				string caminhoArquivo;

				foreach (var item in ids)
				{
					
					MLArquivoRestrito model = CRUD.Obter<MLArquivoRestrito>(Convert.ToDecimal(item), portal.ConnectionString);
					MLArquivoRestritoCategoria categoria = CRUD.Obter<MLArquivoRestritoCategoria>(model.CodigoCategoria.Value, portal.ConnectionString);

					if (!String.IsNullOrEmpty(model.Imagem))
					{
						caminhoArquivo = Path.Combine(categoria.PastaFisicaImagem(portal.Diretorio), model.Imagem);

						if (System.IO.File.Exists(caminhoArquivo))
							System.IO.File.Delete(caminhoArquivo);

                        BLReplicar.ExcluirArquivosReplicados(categoria.PastaRelativaImagem(portal.Diretorio), caminhoArquivo);
					}

					if (!String.IsNullOrEmpty(model.ArquivoUrl))
					{
						caminhoArquivo = Path.Combine(categoria.PastaFisicaArquivo(portal.Diretorio), model.ArquivoUrl);

						if (System.IO.File.Exists(caminhoArquivo))
							System.IO.File.Delete(caminhoArquivo);

                        BLReplicar.ExcluirArquivosReplicados(categoria.PastaRelativaArquivo(portal.Diretorio), caminhoArquivo);
                    }

					CRUD.Excluir<MLArquivoRestrito>(Convert.ToDecimal(item), PortalAtual.ConnectionString);
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

        #region ListarArquivos
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
        public ActionResult ListarArquivos(MLArquivoRestrito criterios)
        {
            try
            {    
                return CRUD.ListarJson(criterios, Request.QueryString, PortalAtual.ConnectionString);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }
        #endregion

        #region UploadFile
        /// <summary>
        /// Upload
        /// </summary>
        [CheckPermission(global::Permissao.Modificar)]
        [HttpPost]
		public ActionResult UploadFile(decimal? codigoCategoria, bool isImagem)
        {
            try
            {
                MLPortal portal = PortalAtual.Obter;
				HttpPostedFileBase file = Request.Files[0];
				MLArquivoRestritoCategoria categoria;
                string caminhoPastaVirtual;
                string caminhoArquivo;
				string caminhoPastaFisica;

                if (codigoCategoria.HasValue)
                {
                    categoria = CRUD.Obter<MLArquivoRestritoCategoria>(codigoCategoria.Value, portal.ConnectionString);
					caminhoPastaFisica  = isImagem ? categoria.PastaFisicaImagem(portal.Diretorio) : categoria.PastaFisicaArquivo(portal.Diretorio);
                    caminhoPastaVirtual = isImagem ? categoria.PastaRelativaImagem(portal.Diretorio) : categoria.PastaRelativaArquivo(portal.Diretorio);
                    caminhoArquivo = Path.Combine(caminhoPastaFisica, file.FileName);

					if (!Directory.Exists(caminhoPastaFisica))
						Directory.CreateDirectory(caminhoPastaFisica);

                    file.SaveAs(caminhoArquivo);
                    BLReplicar.Arquivo(caminhoArquivo, caminhoPastaVirtual);

					return Json(new { success = true, name = file.FileName });
                }
                else
					return Json(new { success = false, msg = "Erro ao subir arquivo." });
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
