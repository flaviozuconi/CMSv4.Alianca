using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace CMSApp.Areas.CMS.Controllers
{
    public class TraduzirController : SecurePortalController
    {
        //
        // GET: /CMS/Traduzir/
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
        ///     /Area/Traduzir
        ///     /Area/Traduzir/Listar
        ///     /Area/Traduzir/Listar?parametro=1 & page=1 & limit=30 & sort= {JSON}
        /// </remarks>
        [CheckPermission(global::Permissao.Visualizar)]
        public ActionResult Listar()
        {

            var adm = Request["adm"] != null;
            try
            {
                int start, length;
                double total;

                int.TryParse(Request["start"], out start);
                int.TryParse(Request["length"], out length);
                int.TryParse(Request["length"], out length);

                var buscaGenerica = Request["search[value]"];
                var orderby = Request["order[0][dir]"] == "asc" ? 1 : 0;

                var lista = BLTraducao.Listar(PortalAtual.Obter, buscaGenerica, start, length, orderby, out total, adm);

                // Retorna os resultados
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
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }
        #endregion

        #region Atualizar

        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult Atualizar(decimal codigo, string sigla, string traducao)
        {
            try
            {
                BLTraducao.Atualizar(codigo, sigla, traducao);

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return Json(new { success = false, msg = "Não foi possível atualizar o Termo" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region AtualizarCache

        /// <summary>
        /// Atualizar Cache
        /// </summary>
        /// <remarks>
        /// GET:
        ///     /Area/AtualizarCache
        /// </remarks>
        [CheckPermission(global::Permissao.Modificar)]
        public ActionResult AtualizarCache()
        {
            var adm = Request["adm"] != null;
            try
            {
                var portal = PortalAtual.Obter;
                string key = string.Format("portal_{0}_dicionario_traducao", adm ? 0 : portal.Codigo);
                BLCachePortal.Remove(key);
                BLCachePortal.Add(portal.Codigo.GetValueOrDefault(0), key, adm ? new BLTraducao().CarregarDicionario() : new BLTraducao(portal).CarregarDicionario());
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Traducao 

        #region Tradução em massa

        #region TraduzirTermos

        /// <summary>
        /// Tradução em massa de todos os termos pendentes do portal ou da área admin
        /// </summary>
        /// <returns></returns>
        public ActionResult TraducaoTermos()
        {
            //As duas telas de tradução utilizam a mesma view
            //Quando a tradução for só para os termos gerados com o helper TAdm, a var adm será true
            var adm = Request["adm"] != null;
            var portal = PortalAtual.Obter;
            var codigoPortal = Request["adm"] != null ? 0 : portal.Codigo;
            var idiomas = BLIdioma.Listar().FindAll(a => !a.Sigla.StartsWith("pt"));

            using (TransactionScope transaction = new TransactionScope())
            {
                for (int y = 0; y < idiomas.Count; y++)
                    TraduzirLote(codigoPortal.Value, idiomas[y].Sigla);

                transaction.Complete();
            }

            return View("Index");
        }

        #endregion

        #region TraduzirLote

        /// <summary>
        /// Traduzir todos os termos pendentes do portal para um idioma
        /// </summary>
        /// <param name="codigoportal"></param>
        /// <param name="para">sigla do idioma que será feita a tradução</param>
        /// <returns></returns>
        [CheckPermission(global::Permissao.Modificar)]
        private bool TraduzirLote(decimal codigoportal, string para)
        {
            //var lista = BLTraducao.ListarPendentes(codigoportal, para);
            //var traducao = TraducaoLote(lista, "pt", para);

            return true;
        }

        #endregion

        #endregion

        #region Tradução Individual

        [HttpGet]
        [CheckPermission(global::Permissao.Modificar)]
        public JsonResult Traduzir(decimal idtermo, string para, string traducao)
        {
            BLTraducao.Atualizar(idtermo, para, traducao);
            return Json(traducao, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion
    }
}
