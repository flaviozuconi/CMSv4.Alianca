using CMSv4.Model;
using Framework.DataLayer;
using Framework.Utilities;
using System.Collections.Generic;
using System.Data;

namespace CMSv4.BusinessLayer
{
    public class BLAgrupador : BLCRUD<MLAgrupador>
    {
        #region ListarPublico

        /// <summary>
        /// LISTAR PUBLICO
        /// </summary>
        public static MLAgrupadorPublico ListarPublico(MLModuloAgrupador model, int? ano, decimal? idioma, bool? destaque = null, string listas = "")
        {
            var retorno = new MLAgrupadorPublico();
            var portal = BLPortal.Atual;

            using (var command = Database.NewCommand("USP_MOD_AGR_L_CONTEUDO_PUBLICO", portal.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@AGR_C_CATEGORIAS", SqlDbType.VarChar, 100, model.Categorias);
                command.NewCriteriaParameter("@AGR_C_LISTAS", SqlDbType.VarChar, 100, listas);
                command.NewCriteriaParameter("@ANO", SqlDbType.Int, ano);
                command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Decimal, idioma);
                command.NewCriteriaParameter("@QUANTIDADE", SqlDbType.Int, model.Quantidade);
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, portal.Codigo);

                // Execucao
                var dataset = Database.ExecuteDataSet(command);
                retorno.Categorias = Database.FillList<MLCategoriaAgrupador>(dataset.Tables[0]);
                retorno.Conteudos = Database.FillList<MLAgrupadorConteudoPublico>(dataset.Tables[1]);

                return retorno;
            }
        }

        #endregion

        #region ListarPublicoAnos

        /// <summary>
        /// LISTAR PUBLICO
        /// </summary>
        public static List<MLAgrupadorAnos> ListarPublicoAnos(MLModuloAgrupador model, decimal? idioma, string listas = "")
        {
            var portal = BLPortal.Atual;

            using (var command = Database.NewCommand("USP_MOD_AGR_L_CONTEUDO_PUBLICO_ANOS", portal.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@AGR_C_CATEGORIAS", SqlDbType.VarChar, 100, model.Categorias);
                command.NewCriteriaParameter("@AGR_C_LISTAS", SqlDbType.VarChar, 100, listas);
                command.NewCriteriaParameter("@IDI_N_CODIGO", SqlDbType.Decimal, idioma);
                command.NewCriteriaParameter("@POR_N_CODIGO", SqlDbType.Decimal, portal.Codigo);

                // Execucao
                return Database.ExecuteReader<MLAgrupadorAnos>(command);

            }
        }

        #endregion

        #region Salvar

        #endregion

    }
}
