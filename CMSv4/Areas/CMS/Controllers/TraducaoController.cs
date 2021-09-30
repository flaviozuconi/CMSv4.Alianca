using Framework.DataLayer;
using Framework.Utilities;
using System;
using System.Data;
using System.Web.Mvc;

namespace CMSApp.Areas.CMS.Controllers
{
    public class TraducaoController : SecurePortalController
    {
        //
        // GET: /CMS/Traducao
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
        public ActionResult Listar(string idioma, string buscaGenerica)
        {
            try
            {
                // Busca lista no banco de dados

                using (var command = Database.NewCommand("USP_CMS_L_TERMO_TRADUCAO", PortalAtual.ConnectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, PortalAtual.Codigo);
                    command.NewCriteriaParameter("@TER_C_TERMO", SqlDbType.VarChar, -1, buscaGenerica);
                    command.NewCriteriaParameter("@TRA_C_IDIOMA", SqlDbType.VarChar, 5, string.IsNullOrEmpty(idioma) ? "pt-BR" : idioma );

                    // Execucao
                    var lista = Database.ExecuteReader<MLTraducao>(command);

                    // Retorna os resultados

                    //var response = new ActionResult();
                    //response.Data = lista;
                    //response.Total = (int)total;

                    //return response;
                    return null;
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
        /// <remarks>
        /// GET:
        ///     /Area/Controller/Item/id
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Item(decimal id, string idioma)
        {
            try
            {
                using (var command = Database.NewCommand("USP_CMS_S_DICIONARIO_TRADUCAO", PortalAtual.ConnectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, PortalAtual.Codigo);
                    command.NewCriteriaParameter("@TER_N_CODIGO", SqlDbType.VarChar, -1, id);
                    command.NewCriteriaParameter("@TRA_C_IDIOMA", SqlDbType.VarChar, 5, idioma);

                    command.NewOutputParameter("@OUT_TRA_N_CODIGO", SqlDbType.Decimal);
                    command.NewOutputParameter("@OUT_POR_N_CODIGO", SqlDbType.Decimal);
                    command.NewOutputParameter("@OUT_TER_N_CODIGO", SqlDbType.Decimal);
                    command.NewOutputParameter("@OUT_TER_C_TERMO", SqlDbType.VarChar, -1);
                    command.NewOutputParameter("@OUT_TRA_C_IDIOMA", SqlDbType.Char, 5);
                    command.NewOutputParameter("@OUT_TRA_C_TERMO", SqlDbType.VarChar, -1);

                    // Execucao
                    Database.ExecuteNonQuery(command);

                    var model = Database.FillModel<MLTraducao>(command);
                    if (string.IsNullOrEmpty(model.Idioma)) model.Idioma = idioma;

                    return View(model);
                }
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
        public ActionResult Item(MLTraducao model, FormCollection values)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var modelTermo = new MLTermo();
                    modelTermo.Codigo = model.Codigo;
                    modelTermo.CodigoPortal = PortalAtual.Codigo;
                    modelTermo.Termo = model.Termo;

                    var traducao = CRUD.Listar<MLTraducaoUpdate>(new MLTraducaoUpdate { Idioma = model.Idioma, CodigoTermo = model.Codigo }, PortalAtual.ConnectionString);

                    var modelTraducao = new MLTraducaoUpdate();
                    if (traducao != null && traducao.Count > 0) modelTraducao.Codigo = traducao[0].Codigo;
                    modelTraducao.CodigoTermo = model.Codigo;
                    modelTraducao.Idioma = model.Idioma;
                    modelTraducao.Traducao = model.Traducao;

                    // Salvar

                    var T = new BLTraducao(PortalAtual.Obter);
                    if (!modelTermo.CodigoPortal.HasValue) modelTermo.CodigoPortal = PortalAtual.Codigo;
                    if (!BLUsuario.ObterLogado().CheckPortal(modelTermo.CodigoPortal)) return Json(new { success = false, msg = T.Obter(T.MSG_PERMISSAO_NEGADA) });

                    var codigoTermo = CRUD.Salvar(modelTermo, PortalAtual.ConnectionString);

                    modelTraducao.CodigoTermo = codigoTermo;

                    CRUD.Salvar(modelTraducao, PortalAtual.ConnectionString);
                }

                return Json(new { success = ModelState.IsValid });
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
        public ActionResult Excluir(decimal id)
        {
            try
            {
                var model = CRUD.Obter<MLTermo>(id, PortalAtual.ConnectionString);
                var T = new BLTraducao(PortalAtual.Obter);
                if (!BLUsuario.ObterLogado().CheckPortal(model.CodigoPortal)) return Json(new { success = false, msg = T.Obter(T.MSG_PERMISSAO_NEGADA) }); 

                CRUD.Excluir<MLTermo>(id, PortalAtual.ConnectionString);
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
