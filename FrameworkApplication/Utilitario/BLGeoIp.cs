using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.DataLayer;

namespace Framework.Utilities
{
    public class BLGeoIp
    {
        #region ListarPaginaHome
        public static List<MLGeoIP> ListarPaginaHome(string ipNumber)
        {
            try
            {
                var lstRetorno = new List<MLGeoIP>();

                using (var command = DatabasePortal.NewCommand("USP_CMS_L_GEO_IP"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@IPNUMBER", SqlDbType.VarChar, 50, ipNumber);

                    // Execucao
                    lstRetorno = Database.ExecuteReader<MLGeoIP>(command);
                }

                return lstRetorno;

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }
        #endregion

        #region Excluir

        /// <summary> 
        /// Exclui um PaginaPais
        /// </summary> 
        /// <param name="pdecCodigo">Codigo</param>
        /// <returns>Quantidade de Registros Excluídos</returns> 
        /// <user>Gerador [1.0.0.0]</user>
        public static int ExcluirPaginasPais(decimal pdecCodigoPagina)
        {
            try
            {
                using (var command = DatabasePortal.NewCommand("USP_BRK_D_PAGINA_PAIS"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@PPA_PAG_N_CODIGO", SqlDbType.Decimal, 18, pdecCodigoPagina);

                    // Execucao
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

        #region ListarAdminsitrativo
        public static List<MLPais> ListarAdminsitrativo(decimal pdecCodigoPagina)
        {

            try
            {
                var lstRetorno = new List<MLPais>();

                using (var command = DatabasePortal.NewCommand("USP_CMS_L_PAGINA_PAIS_ADMIN"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@PPA_PAG_N_CODIGO", SqlDbType.Decimal, 18, pdecCodigoPagina);

                    // Execucao
                    lstRetorno = Database.ExecuteReader<MLPais>(command);
                }

                return lstRetorno;

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }
        #endregion

        #region Inserir

        /// <summary> 
        /// Insere um PaginaPais
        /// </summary> 
        /// <param name="pobjMLPaginaPais">PaginaPais a ser inserido</param>
        /// <returns>Código PaginaPais</returns> 
        /// <user>Gerador [1.0.0.0]</user>
        public static decimal InserirPaginasPais(MLPaginaPais pobjMLPaginaPais)
        {

            try
            {
                using (var command = DatabasePortal.NewCommand("USP_BRK_I_PAGINA_PAIS"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@PPA_PAI_N_CODIGO", SqlDbType.Decimal, 18, pobjMLPaginaPais.Paicodigo);
                    command.NewCriteriaParameter("@PPA_PAG_N_CODIGO", SqlDbType.Decimal, 18, pobjMLPaginaPais.CodigoPagina);

                    // Execucao
                    return Convert.ToDecimal(Database.ExecuteScalar(command));
                }

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region IpToLong
        public static long IpToLong(string ip)
        {
            try
            {
                string[] ipBytes;
                double num = 0;
                if (!string.IsNullOrEmpty(ip))
                {
                    ipBytes = ip.Split('.');
                    for (int i = ipBytes.Length - 1; i >= 0; i--)
                    {
                        num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
                    }
                }
                return (long)num;
            }
            catch
            {
                return 0;
            }
        }
        #endregion

        #region CausarRedirect
        public static bool CausarRedirect(string ipNumber)
        {
            try
            {
                var lstRetorno = new List<MLGeoIP>();

                using (var command = DatabasePortal.NewCommand("USP_BRK_L_GEO_IP_REDIRECT"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@IPNUMBER", SqlDbType.VarChar, 50, ipNumber);

                    // Execucao
                    lstRetorno = Database.ExecuteReader<MLGeoIP>(command);
                }

                return lstRetorno.Count > 0;
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
