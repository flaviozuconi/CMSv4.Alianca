using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Framework.Utilities;
using CMSv4.Model;
using CMSv4.BusinessLayer;

namespace CMSApp.Areas.ModuloAdmin.Controllers
{
    public class BuscaAdminController : AdminBaseCRUDController<MLBusca, MLBusca>
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
                MLBusca model = null;
                if (id.HasValue) model = CRUD.Obter<MLBusca>(new MLBusca { Codigo = id, CodigoPortal = portal.Codigo }, portal.ConnectionString);

                if (model==null)
                {
                    model = new MLBusca();
                    model.CodigoIdioma = PortalAtual.Obter.CodigoIdioma;
                }

                ViewData["listaModulos"] = BLModulo.Listar(portal);
                ViewData["listaIdiomas"] = BLIdioma.Listar();
                ViewData["listaAgrupadores"] = CRUD.Listar<MLCategoriaAgrupador>(new MLCategoriaAgrupador { CodigoPortal = portal.Codigo }, portal.ConnectionString);
                ViewData["listas"] = CRUD.Listar<MLListaConfig>(new MLListaConfig() { CodigoPortal = portal.Codigo }, portal.ConnectionString);

                return View(model);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        [HttpPost, CheckPermission(global::Permissao.Visualizar)]
        public JsonResult validarUrl(decimal? Codigo, decimal? CodigoModulo, decimal? CodigoIdioma, decimal? CodigoAgrupador, decimal? CodigoLista)
        {
            var portal = PortalAtual.Obter;
            var modulo = BLModulo.Obter(CodigoModulo, portal.ConnectionString);
            var isModuloLista = modulo.CodigoLista.HasValue || modulo.Codigo == 2/*código do módulo lista*/;

            var crud = CRUD.Listar<MLBusca>(new MLBusca
            {
                CodigoModulo = CodigoModulo,
                CodigoIdioma = CodigoIdioma,
                CodigoPortal = portal.Codigo,
                CodigoLista = CodigoLista
            }, portal.ConnectionString);

            if (crud == null || crud.Count == 0 || !crud[0].Codigo.HasValue) //nada semelhante foi encontrado, é válido
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            if (isModuloLista && CodigoAgrupador.HasValue) //possui codigo agrupador
            {
                var item = crud.Find(a => a.CodigoAgrupador == CodigoAgrupador);

                if (item != null && Codigo.HasValue) //possui codigo e foi encontrado um registro com o código de agrupador informado
                {
                    return Json(item.Codigo == Codigo, JsonRequestBehavior.AllowGet); //true: encontrou o proprio registro
                }
                else
                {
                    return Json(item == null, JsonRequestBehavior.AllowGet); //true: nenhum registro com as mesmas regras
                }
            }
            else
            {
                if (Codigo.HasValue)
                {
                    var item = crud.Find(a => a.Codigo == Codigo);

                    return Json(item != null, JsonRequestBehavior.AllowGet); //true: encontrou o proprio registro
                }
            }


            return Json(false, JsonRequestBehavior.AllowGet);
        }

    }
}
