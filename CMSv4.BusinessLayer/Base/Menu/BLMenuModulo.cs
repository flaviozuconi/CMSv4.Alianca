using CMSv4.Model;
using Framework.DataLayer;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;

namespace CMSv4.BusinessLayer
{
    public class BLMenuModulo : BLCRUD<MLMenuModulo>
    {
        #region Excluir

        public override int Excluir(decimal decCodigo, string connectionString)
        {
            try
            {
                using (var command = Database.NewCommand("USP_MOD_MEN_D_ITEM", connectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@MNI_N_CODIGO", SqlDbType.Decimal, 18, decCodigo);

                    return Database.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region Obter Completo

        /// <summary>
        /// Obtem um objeto completo do MENU e de seus itens
        /// </summary>
        public MLMenuCompletoModulo ObterCompleto(decimal pdecCodigoMenu, string connectionString, bool bItensPai)
        {
            return ObterCompleto(pdecCodigoMenu, false, connectionString, bItensPai, null);
        }

        /// <summary>
        /// Obtem um objeto completo do MENU e de seus itens
        /// </summary>
        /// <param name="useCache">Indica se o menu deve ser buscado primeiro no cache</param>
        /// <param name="bItensPai">Listar somente os itens com código pai = null e seus filhos</param>
        public MLMenuCompletoModulo ObterCompleto(decimal pdecCodigoMenu, bool useCache, string connectionString, bool bItensPai, bool? Ativo)
        {
            MLMenuCompletoModulo retorno = null;
            var objItem = new MLMenuItemModulo();

            try
            {
                // Procura no cache caso esteja configurado, e escolhido pelo parametro
                if (useCache && HttpContext.Current.Request.QueryString["reset"] == null) retorno = ObterCache(pdecCodigoMenu);

                if (retorno == null)
                {
                    // Senao encontrou, buscar na base de dados

                    using (var command = Database.NewCommand("USP_MOD_MEN_S_MENU_COMPLETO", connectionString))
                    {
                        // Parametros
                        command.NewCriteriaParameter("@MEN_N_CODIGO", SqlDbType.Decimal, 18, pdecCodigoMenu);
                        command.NewCriteriaParameter("@MNI_B_STATUS", SqlDbType.Bit, Ativo);

                        // Execucao
                        var dataset = Database.ExecuteDataSet(command);
                        retorno = new MLMenuCompletoModulo();

                        if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                        {
                            // Preenche dados do menu
                            retorno = Database.FillModel<MLMenuCompletoModulo>(dataset.Tables[0].Rows[0]);

                            if (dataset.Tables.Count > 1 && dataset.Tables[1].Rows.Count > 0)
                            {
                                // Preenche itens do menu
                                List<MLMenuItemModulo> ItensMenuAux = Database.FillList<MLMenuItemModulo>(dataset.Tables[1]);
                                List<MLMenuItemModulo> ItensMenu = new List<MLMenuItemModulo>();

                                foreach (var item in ItensMenuAux)
                                    ItensMenu.Add(item);

                                if (bItensPai) ItensMenu.RemoveAll(item => item.CodigoPai != 0);

                                foreach (var item in ItensMenu)
                                    EncontrarFilhos(item, ItensMenuAux);

                                retorno.ItensMenu = ItensMenu;
                            }
                        }
                    }

                    if (useCache) ArmazenarCache(pdecCodigoMenu, retorno);
                }

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

            return retorno;
        }


        #endregion

        #region Salvar

        public decimal Salvar(MLMenuCompletoModulo model, string connectionString = "")
        {
            model.CodigoPortal = model.CodigoPortal.GetValueOrDefault(PortalAtual.Obter.Codigo.GetValueOrDefault());
            model.Ativo = model.Codigo.HasValue ? model.Ativo.GetValueOrDefault(false) : true;

            return base.SalvarParcial(model, connectionString);
        }

        #endregion

        #region ObterDefault

        public MLMenuCompletoModulo ObterDefault(string connectionString)
        {
            MLMenuCompletoModulo retorno = new MLMenuCompletoModulo();

            try
            {
                using (var command = Database.NewCommand("USP_MOD_MEN_S_MENU_DEFAULT", connectionString))
                {
                    var dataset = Database.ExecuteDataSet(command);

                    if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                        retorno = Database.FillModel<MLMenuCompletoModulo>(dataset.Tables[0].Rows[0]);
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region EncontrarFilhos

        private void EncontrarFilhos(MLMenuItemModulo model, List<MLMenuItemModulo> itens)
        {
            List<MLMenuItemModulo> lstFilhos = itens.FindAll(m => m.CodigoPai == model.Codigo);

            if (lstFilhos.Count > 0)
            {
                foreach (var item in lstFilhos)
                {
                    EncontrarFilhos(item, itens);
                    model.Filhos.Add(item);
                }
            }
        }

        #endregion

        #region Salvar Menu Item

        public MLMenuItemModulo InserirMenuItem(decimal? codigopai, decimal codigoMenu, string connectionString)
        {
            try
            {
                MLMenuItemModulo objML = new MLMenuItemModulo();
                objML.CodigoPai = codigopai;
                objML.CodigoMenu = codigoMenu;
                objML.Nome = "Novo Item";
                objML.Ativo = true;
                objML.AbrirNovaPagina = false;

                using (var command = Database.NewCommand("USP_MOD_MEN_I_MENU_ITEM", connectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@MNI_MEN_N_CODIGO", SqlDbType.Decimal, 18, objML.CodigoMenu);
                    command.NewCriteriaParameter("@MNI_N_CODIGO_PAI", SqlDbType.Decimal, 18, objML.CodigoPai);
                    command.NewCriteriaParameter("@MNI_C_NOME", SqlDbType.VarChar, 100, objML.Nome);
                    command.NewCriteriaParameter("@MNI_B_STATUS", SqlDbType.Bit, objML.Ativo);
                    command.NewCriteriaParameter("@MNI_B_NOVA_PAGINA", SqlDbType.Bit, objML.AbrirNovaPagina);

                    // Execucao
                    objML.Codigo = (decimal)Database.ExecuteScalar(command);
                }
                return CRUD.Obter<MLMenuItemModulo>(objML.Codigo.GetValueOrDefault());
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        public decimal SalvarMenuItem(MLMenuItemModulo model, string connectionString) {

            return CRUD.SalvarParcial(model, connectionString);
        }

        #endregion

        #region Obter Menu Item

        public MLMenuItemModulo ObterMenuItem(decimal codigoMenu, string connectionString)
        {
            try
            {
                return CRUD.Obter<MLMenuItemModulo>(codigoMenu, connectionString);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion


        #region Obter Menu View

        public MLMenuModuloView ObterMenuView(MLMenuModuloView model, string connectionString)
        {
            return CRUD.Obter<MLMenuModuloView>(model, connectionString);
        }
        #endregion

        #region Excluir Menu View

        public int ExcluirMenuView(List<string> ids) {

            return CRUD.Excluir<MLMenuModuloView>(ids);
        }
        #endregion

        #region Salvar Menu View

        public bool SalvarMenuView(MLMenuModuloView model, string nomeAntigo)
        {

            var portal = BLPortal.Atual;
            var conteudo = model.View;
            var diretorioVirtual = BLConfiguracao.Pastas.ModuloGenerico(PortalAtual.Diretorio, MLMenuModulo.Pasta.Views);
            var diretorioFisico = HttpContextFactory.Current.Server.MapPath(diretorioVirtual);
            var arquivoFisico = string.Empty;
            var arquivoFisicoScript = string.Empty;
            var conteudoScript = string.Empty;

            if (!string.IsNullOrWhiteSpace(model.Script))
            {
                conteudoScript = model.Script;
            }

            model.CodigoPortal = portal.Codigo;

            //Quando entrar aqui já validou o nome, não terá nome repetido
            CRUD.Salvar(model, portal.ConnectionString);

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
                arquivo.Write(conteudo.Unescape());
                arquivo.Close();
            }

            using (var arquivo = new StreamWriter(arquivoFisicoScript))
            {
                arquivo.Write(conteudoScript.Unescape());
                arquivo.Close();
            }

            //Edição de registro com novo nome.
            if (!string.IsNullOrWhiteSpace(nomeAntigo) && model.Nome != nomeAntigo && model.Codigo.HasValue)
            {
                var arquivoAntigo = Path.Combine(diretorioFisico, nomeAntigo + ".cshtml");
                var arquivoScriptAntig = Path.Combine(diretorioFisico, "Script" + nomeAntigo + ".cshtml");

                //Deletar o arquivo antigo
                if (File.Exists(arquivoAntigo))
                    File.Delete(arquivoAntigo);

                if (File.Exists(arquivoScriptAntig))
                    File.Delete(arquivoScriptAntig);
            }

            return true;
        }

        #endregion


        #region Ordenar

        public bool Ordenar(decimal decCodigoSource, decimal decCodigoTarger, int intIndex, string connectionString)
        {
            try
            {
                using (var command = Database.NewCommand("USP_MOD_MEN_U_MENU_ORDEM", connectionString))
                {
                    // Parametros
                    command.NewCriteriaParameter("@MNI_N_CODIGO_SOURCE", SqlDbType.Decimal, 18, decCodigoSource);
                    command.NewCriteriaParameter("@MNI_N_CODIGO_TARGET", SqlDbType.Decimal, 18, decCodigoTarger);
                    command.NewCriteriaParameter("@MNI_N_INDEX", SqlDbType.Int, intIndex);

                    // Execucao
                    Database.ExecuteNonQuery(command);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return false;
            }
        }

        #endregion

        #region Cache do Menu

        /// <summary>
        /// Armazena o menu em cache
        /// </summary>
        private void ArmazenarCache(decimal pdecCodigoMenu, MLMenuCompletoModulo retorno)
        {
            try
            {
                int TEMPO_CACHE = 60;

                HttpRuntime.Cache.Remove("cacCMSMenuModulo" + pdecCodigoMenu);
                HttpRuntime.Cache.Insert(
                    "cacCMSMenuModulo" + pdecCodigoMenu
                    , retorno
                    , null
                    , DateTime.Now.AddMinutes(TEMPO_CACHE)
                    , System.Web.Caching.Cache.NoSlidingExpiration
                    , System.Web.Caching.CacheItemPriority.Default
                    , null);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Obtem o menu do Cache (caso exista)
        /// </summary>
        /// <param name="pdecCodigoMenu">codigo do menu</param>
        /// <returns></returns>
        private MLMenuCompletoModulo ObterCache(decimal pdecCodigoMenu)
        {
            return (MLMenuCompletoModulo)HttpRuntime.Cache.Get("cacCMSMenuModulo" + pdecCodigoMenu);
        }
        #endregion
    }
}
