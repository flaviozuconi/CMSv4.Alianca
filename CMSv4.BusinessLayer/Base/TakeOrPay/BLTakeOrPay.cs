using CMSv4.Model;
using CMSv4.Model.Base;
using Framework.DataLayer;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Mail;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using NReco.PdfGenerator;
using System.Linq;
using System.Web.Hosting;
using System.Web;

namespace CMSv4.BusinessLayer
{
    public class BLTakeOrPay
    {
        #region ObterTakeOrPay
        /// <summary>
        /// Obter Take Or Pay
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public static MLTakeOrPayEmbarqueCertoCompleto ObterCompleto(decimal Codigo)
        {
            MLTakeOrPayEmbarqueCertoCompleto retorno = null;

            try
            {
                // Senao encontrou, buscar na base de dados

                using (var command = Database.NewCommand("USP_MOD_S_TAKE_OR_PAY_ADMIN"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@TPE_N_CODIGO", SqlDbType.VarChar, 200, Codigo);

                    // Execucao
                    var dataset = Database.ExecuteDataSet(command);
                    retorno = new MLTakeOrPayEmbarqueCertoCompleto();

                    if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    {
                        retorno = Database.FillModel<MLTakeOrPayEmbarqueCertoCompleto>(dataset.Tables[0].Rows[0]);
                    }

                    if (dataset.Tables.Count > 1 && dataset.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow row in dataset.Tables[1].Rows)
                        {
                            retorno.lstProposta.Add(Database.FillModel<MLTakeOrPayEmbarqueCertoXProposta>(row));
                        }
                    }

                    if (dataset.Tables.Count > 2 && dataset.Tables[2].Rows.Count > 0)
                    {
                        foreach (DataRow row in dataset.Tables[2].Rows)
                        {
                            retorno.lstContainer.Add(Database.FillModel<MLTakeOrPayEmbarqueCertoXContainers>(row));
                        }
                    }
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

        #region ObterTakeOrPayHistorico
        /// <summary>
        /// Obter Take Or Pay
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public static MLTakeOrPayEmbarqueCertoHistoricoCompleto ObterHistoricoCompleto(decimal Codigo)
        {
            MLTakeOrPayEmbarqueCertoHistoricoCompleto retorno = null;

            try
            {
                // Senao encontrou, buscar na base de dados

                using (var command = Database.NewCommand("USP_MOD_S_TAKE_OR_PAY_HISTORICO_ADMIN"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@TPE_N_CODIGO", SqlDbType.VarChar, 200, Codigo);

                    // Execucao
                    var dataset = Database.ExecuteDataSet(command);
                    retorno = new MLTakeOrPayEmbarqueCertoHistoricoCompleto();

                    if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                    {
                        retorno = Database.FillModel<MLTakeOrPayEmbarqueCertoHistoricoCompleto>(dataset.Tables[0].Rows[0]);
                    }

                    if (dataset.Tables.Count > 1 && dataset.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow row in dataset.Tables[1].Rows)
                        {
                            retorno.lstProposta.Add(Database.FillModel<MLTakeOrPayEmbarqueCertoXPropostaHistorico>(row));
                        }
                    }

                    if (dataset.Tables.Count > 2 && dataset.Tables[2].Rows.Count > 0)
                    {
                        foreach (DataRow row in dataset.Tables[2].Rows)
                        {
                            retorno.lstContainer.Add(Database.FillModel<MLTakeOrPayEmbarqueCertoXContainersHistorico>(row));
                        }
                    }
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
        /// <summary>
        /// Salvar
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool Salvar(MLTakeOrPayEmbarqueCertoCompleto model, string lstNumeroProposta)
        {
            try
            {
                // Senao encontrou, buscar na base de dados
                using (var command = Database.NewCommand("USP_MOD_U_TAKE_OR_PAY_ADMIN"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@TPE_N_CODIGO", SqlDbType.Decimal, 18, model.Codigo);
                    command.NewCriteriaParameter("@TPE_C_NOME", SqlDbType.VarChar, 150, model.Nome);
                    command.NewCriteriaParameter("@TPE_C_EMAIL", SqlDbType.VarChar, 100, model.Email);
                    command.NewCriteriaParameter("@TPE_C_CNPJ", SqlDbType.VarChar, 20, model.CNPJ);
                    command.NewCriteriaParameter("@TPE_C_CEP", SqlDbType.VarChar, 15, model.CEP);
                    command.NewCriteriaParameter("@TPE_C_LOGADOURO", SqlDbType.VarChar, 50, model.Logadouro);
                    command.NewCriteriaParameter("@TPE_C_BAIRRO", SqlDbType.VarChar, 50, model.Bairro);
                    command.NewCriteriaParameter("@TPE_C_CIDADE", SqlDbType.VarChar, 100, model.Cidade);
                    command.NewCriteriaParameter("@TPE_C_ESTADO", SqlDbType.VarChar, 2, model.Estado);
                    command.NewCriteriaParameter("@TPE_B_IS_BID", SqlDbType.Bit, model.isBID);
                    command.NewCriteriaParameter("@TPE_B_RESERVAR_ESPACO", SqlDbType.Bit,  model.ReservarEspaco);
                    command.NewCriteriaParameter("@TPE_B_IS_SEMANAL", SqlDbType.Bit, model.IsSemanal);
                    command.NewCriteriaParameter("@TPE_B_TERMO_ACEITO", SqlDbType.Bit, model.TermoAceito);
                    command.NewCriteriaParameter("@PROPOSTAS", SqlDbType.VarChar, -1, lstNumeroProposta.Trim(','));

                    Database.ExecuteNonQuery(command);
                }

                return true;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return false;
            }
        }
        #endregion

        #region SalvarHistorico
        /// <summary>
        /// Salvar
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool SalvarHistorico(MLTakeOrPayEmbarqueCertoHistoricoCompleto model, string lstNumeroProposta)
        {
            try
            {
                // Senao encontrou, buscar na base de dados
                using (var command = Database.NewCommand("USP_MOD_U_TAKE_OR_PAY_HISTORICO_ADMIN"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@TPE_N_CODIGO", SqlDbType.Decimal, 18, model.Codigo);
                    command.NewCriteriaParameter("@TPE_C_NOME", SqlDbType.VarChar, 150, model.Nome);
                    command.NewCriteriaParameter("@TPE_C_EMAIL", SqlDbType.VarChar, 100, model.Email);
                    command.NewCriteriaParameter("@TPE_C_CNPJ", SqlDbType.VarChar, 20, model.CNPJ);
                    command.NewCriteriaParameter("@TPE_C_CEP", SqlDbType.VarChar, 15, model.CEP);
                    command.NewCriteriaParameter("@TPE_C_LOGADOURO", SqlDbType.VarChar, 50, model.Logadouro);
                    command.NewCriteriaParameter("@TPE_C_BAIRRO", SqlDbType.VarChar, 50, model.Bairro);
                    command.NewCriteriaParameter("@TPE_C_CIDADE", SqlDbType.VarChar, 100, model.Cidade);
                    command.NewCriteriaParameter("@TPE_C_ESTADO", SqlDbType.VarChar, 2, model.Estado);
                    command.NewCriteriaParameter("@TPE_B_IS_BID", SqlDbType.Bit, model.isBID);
                    command.NewCriteriaParameter("@TPE_B_RESERVAR_ESPACO", SqlDbType.Bit, model.ReservarEspaco);
                    command.NewCriteriaParameter("@TPE_B_IS_SEMANAL", SqlDbType.Bit, model.IsSemanal);
                    command.NewCriteriaParameter("@TPE_B_TERMO_ACEITO", SqlDbType.Bit, model.TermoAceito);
                    command.NewCriteriaParameter("@PROPOSTAS", SqlDbType.VarChar, -1, lstNumeroProposta.Trim(','));

                    Database.ExecuteNonQuery(command);
                }

                return true;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return false;
            }
        }
        #endregion

        #region ListarNavioViagem
        /// <summary>
        /// Obter Navios
        /// </summary>
        /// <returns></returns>
        public static List<MLProgramacaoNavio> ListarNavioViagem()
        {
            try
            {
                var retorno = new List<MLProgramacaoNavio>();

                using (var command = Database.NewCommand("USP_MOD_L_TAKE_OR_PAY_LISTAR_NAVIO_VIAGEM"))
                {
                    // Execucao
                    var dataSet = Database.ExecuteDataSet(command);

                    // Grupos Pagina
                    if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                        retorno = Database.FillList<MLProgramacaoNavio>(dataSet.Tables[0]);

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

        #region ObterNavios
        /// <summary>
        /// Obter Navios
        /// </summary>
        /// <param name="NavioViagem"></param>
        /// <returns></returns>
        public static MLProgramacaoNavioProc ObterNavios(string NavioViagem, string Origem = null, string Destino = null)
        {
            try
            {
                var retorno = new MLProgramacaoNavioProc();

                using (var command = Database.NewCommand("USP_MOD_L_TAKE_OR_PAY_LISTAR_NAVIO_CONTAINER"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@NAVIO_VIAGEM", SqlDbType.VarChar, 150, NavioViagem);
                    command.NewCriteriaParameter("@PRN_C_ORIGEM", SqlDbType.VarChar, 150, Origem);
                    command.NewCriteriaParameter("@PRN_C_DESTINO", SqlDbType.VarChar, 150, Destino);

                    // Execucao
                    var dataSet = Database.ExecuteDataSet(command);

                    // Grupos Pagina
                    if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                        retorno.Origem = Database.FillList<MLProgramacaoNavioProcOrigem>(dataSet.Tables[0]);

                    if (dataSet.Tables.Count > 1 && dataSet.Tables[1].Rows.Count > 0)
                        retorno.Destino = Database.FillList<MLProgramacaoNavioProcDestino>(dataSet.Tables[1]);

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

        #region ObterNaviosHistorico
        /// <summary>
        /// Obter Navios
        /// </summary>
        /// <param name="NavioViagem"></param>
        /// <returns></returns>
        public static List<MLProgramacaoNavioProcHistorico> ObterNaviosHistorico(string NavioViagem)
        {
            try
            {
                // Senao encontrou, buscar na base de dados

                using (var command = Database.NewCommand("USP_MOD_L_TAKE_OR_PAY_LISTAR_NAVIO_CONTAINER_HISTORICO"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@NAVIO_VIAGEM", SqlDbType.VarChar, 150, NavioViagem);

                    // Execucao
                    return Database.ExecuteReader<MLProgramacaoNavioProcHistorico>(command);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }
        #endregion

        #region CarregarValorAdicional
        /// <summary>
        /// CarregarValorAdicional
        /// </summary>
        /// <param name="PortoDestino"></param>
        /// <returns></returns>
        public static MLGestaoDeCustosPenalidades CarregarValorAdicional(string PortoDestino)
        {
            try
            {
                using (var command = Database.NewCommand("USP_MOD_L_TAKE_OR_PAY_LISTAR_VALOR_ADICIONAL"))
                {
                    // Parametros
                    command.NewCriteriaParameter("@PRN_C_DESTINO", SqlDbType.VarChar, 150, PortoDestino);

                    // Execucao
                    var retorno = Database.ExecuteReader<MLGestaoDeCustosPenalidades>(command);

                    if(retorno != null && retorno.Count > 0)
                        return retorno[0];

                    return new MLGestaoDeCustosPenalidades();
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }
        #endregion

        #region EnviarEmail
        /// <summary>
        /// Envia os emails
        /// </summary>
        /// <param name="Codigo"></param>
        /// <param name="modelModulo"></param>
        /// <returns></returns>
        public static bool EnviarEmail(decimal? Codigo, MLModuloTakeOrPayPublicado modelModulo)
        {
            try
            {
                #region Lista os dados

                var assuntoAdmin = string.Empty;
                var conteudoAdmin = string.Empty;
                var emailEnviadoAdmin = false;
                var destinatarioAdmin = new List<string>();
                var anexosAdmin = new List<Attachment>();

                var assuntoCliente = string.Empty;
                var conteudoCliente = string.Empty;
                var emailEnviadoCliente = false;
                var destinatarioCliente = new List<string>();
                var anexosCliente = new List<Attachment>();
                var proposta = new MLProgramacaoProposta();

                var modelEmbarqueCerto = BLTakeOrPay.ObterCompleto(Codigo.GetValueOrDefault(0));

                if(modelEmbarqueCerto?.lstProposta.Count > 0)
                    proposta = CRUD.Obter(new MLProgramacaoProposta { NumeroProposta = modelEmbarqueCerto?.lstProposta[0]?.NumeroProposta });

                var nomeEmpresa = modelEmbarqueCerto.Email?.Substring(modelEmbarqueCerto.Email.IndexOf('@') + 1, modelEmbarqueCerto.Email.IndexOf('.', modelEmbarqueCerto.Email.IndexOf('@')) - modelEmbarqueCerto.Email.IndexOf('@') - 1);
                var nomeCliente = modelEmbarqueCerto.isBID.GetValueOrDefault(false) ? nomeEmpresa : proposta.Cliente ?? nomeEmpresa;
                #endregion

                if (modelEmbarqueCerto != null && modelEmbarqueCerto.lstProposta != null && modelEmbarqueCerto.lstContainer != null && modelModulo != null)
                {
                    #region Preenche o email

                    // Adiciona os destinatários
                    destinatarioAdmin.Add(modelModulo.Email);
                    destinatarioCliente.Add(modelEmbarqueCerto.Email);

                    // Adiciona os assuntos
                    if (!modelEmbarqueCerto.ReservarEspaco.GetValueOrDefault(false))
                        assuntoAdmin = $"Take or Pay Spot – Navio {modelEmbarqueCerto.lstContainer[0].NavioViagem} – Cliente {nomeCliente}";
                    else if (modelEmbarqueCerto.IsSemanal.GetValueOrDefault(false))
                        assuntoAdmin = $"Take or Pay Regular – Cliente {nomeCliente}";
                    else
                        assuntoAdmin = $"Take or Pay Regular – Navio {modelEmbarqueCerto.lstContainer[0].NavioViagem} – Cliente {nomeCliente}";

                    assuntoCliente = "Sua solicitação foi feita com sucesso.";

                    // Adiciona os conteúdos
                    conteudoAdmin = BLUtilitarios.ObterConteudoArquivo(BLConfiguracao.Pastas.ModuloFaleConoscoEmail(PortalAtual.Diretorio) + "/" + CRUD.Obter(new MLFaleConoscoModeloEmail { Codigo = modelModulo.ModeloEmailAdmin }).Nome + ".htm")
                        .Replace("[DD/MM/YYYY]", modelEmbarqueCerto.DataCadastro.Value.ToShortDateString())
                        .Replace("[XXXXXX]", nomeCliente)
                        .Replace("[Codigo]", modelEmbarqueCerto.Codigo.ToString())
                        .Replace("[caminhoImagem]", $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}");

                    conteudoCliente = BLUtilitarios.ObterConteudoArquivo(BLConfiguracao.Pastas.ModuloFaleConoscoEmail(PortalAtual.Diretorio) + "/" + CRUD.Obter(new MLFaleConoscoModeloEmail { Codigo = modelModulo.ModeloEmailCliente }).Nome + ".htm")
                        .Replace("[nome]", nomeCliente)
                        .Replace("[XXXXXX]", modelEmbarqueCerto.Codigo.ToString())
                        .Replace("[caminhoImagem]", $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}");
                    #endregion

                    anexosAdmin.Add(GerarExcel(modelEmbarqueCerto, modelEmbarqueCerto.lstProposta, modelEmbarqueCerto.lstContainer));

                    anexosAdmin.Add(GerarPdf(modelEmbarqueCerto, modelEmbarqueCerto.lstProposta, modelEmbarqueCerto.lstContainer));

                    emailEnviadoAdmin = BLEmail.Enviar(assuntoAdmin, destinatarioAdmin, null, null, conteudoAdmin, anexosAdmin);
                    //emailEnviadoCliente = BLEmail.Enviar(assuntoCliente, destinatarioCliente, conteudoCliente);
                    emailEnviadoCliente = true;
                }

                return emailEnviadoAdmin && emailEnviadoCliente;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return false;
            }
        }
        #endregion

        #region EnviarEmailAdmin
        /// <summary>
        /// Envia os emails
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public static bool EnviarEmailAdmin(decimal? Codigo)
        {
            try
            {
                #region Lista os dados

                var assuntoAdmin = string.Empty;
                var conteudoAdmin = string.Empty;
                var emailEnviadoAdmin = false;
                var destinatarioAdmin = new List<string>();
                var anexosAdmin = new List<Attachment>();

                var assuntoCliente = string.Empty;
                var conteudoCliente = string.Empty;
                var emailEnviadoCliente = false;
                var destinatarioCliente = new List<string>();
                var anexosCliente = new List<Attachment>();
                var proposta = new MLProgramacaoProposta();

                var modelEmbarqueCerto = BLTakeOrPay.ObterCompleto(Codigo.GetValueOrDefault(0));

                if (modelEmbarqueCerto?.lstProposta.Count > 0)
                    proposta = CRUD.Obter(new MLProgramacaoProposta { NumeroProposta = modelEmbarqueCerto?.lstProposta[0]?.NumeroProposta });

                var nomeEmpresa = modelEmbarqueCerto.Email?.Substring(modelEmbarqueCerto.Email.IndexOf('@') + 1, modelEmbarqueCerto.Email.IndexOf('.', modelEmbarqueCerto.Email.IndexOf('@')) - modelEmbarqueCerto.Email.IndexOf('@') - 1);
                var nomeCliente = modelEmbarqueCerto.isBID.GetValueOrDefault(false) ? nomeEmpresa : proposta.Cliente ?? nomeEmpresa;
                #endregion

                if (modelEmbarqueCerto != null && modelEmbarqueCerto.lstProposta != null && modelEmbarqueCerto.lstContainer != null)
                {
                    #region Preenche o email

                    // Adiciona os destinatários
                    destinatarioAdmin.Add(CRUD.Obter(new MLConfiguracao { Chave = "CMS.EmbarqueCerto.EmailAdmin" })?.Valor);
                    destinatarioCliente.Add(modelEmbarqueCerto.Email);

                    // Adiciona os assuntos
                    if (!modelEmbarqueCerto.ReservarEspaco.GetValueOrDefault(false))
                        assuntoAdmin = $"Take or Pay Spot – Navio {modelEmbarqueCerto.lstContainer[0].NavioViagem} – Cliente {nomeCliente}";
                    else if (modelEmbarqueCerto.IsSemanal.GetValueOrDefault(false))
                        assuntoAdmin = $"Take or Pay Regular – Cliente {nomeCliente}";
                    else
                        assuntoAdmin = $"Take or Pay Regular – Navio {modelEmbarqueCerto.lstContainer[0].NavioViagem} – Cliente {nomeCliente}";

                    assuntoCliente = "Aliança | Proposta de Embarque Certo Alterada.";

                    // Adiciona os conteúdos
                    conteudoAdmin = BLUtilitarios.ObterConteudoArquivo(BLConfiguracao.Pastas.ModuloFaleConoscoEmail(PortalAtual.Diretorio) + "/takeorpayadministrativo.htm")
                        .Replace("[DD/MM/YYYY]", modelEmbarqueCerto.DataCadastro.Value.ToShortDateString())
                        .Replace("[XXXXXX]", nomeCliente)
                        .Replace("[Codigo]", modelEmbarqueCerto.Codigo.ToString())
                        .Replace("[caminhoImagem]", $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}");

                    conteudoCliente = BLUtilitarios.ObterConteudoArquivo(BLConfiguracao.Pastas.ModuloFaleConoscoEmail(PortalAtual.Diretorio) + "/takeorpayclientereenvio.htm")
                        .Replace("[nome]", nomeCliente)
                        .Replace("[XXXXXX]", modelEmbarqueCerto.Codigo.ToString())
                        .Replace("[caminhoImagem]", $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}");
                    #endregion

                    anexosAdmin.Add(GerarExcel(modelEmbarqueCerto, modelEmbarqueCerto.lstProposta, modelEmbarqueCerto.lstContainer));

                    anexosAdmin.Add(GerarPdf(modelEmbarqueCerto, modelEmbarqueCerto.lstProposta, modelEmbarqueCerto.lstContainer));

                    emailEnviadoAdmin = BLEmail.Enviar(assuntoAdmin, destinatarioAdmin, null, null, conteudoAdmin, anexosAdmin);
                    //emailEnviadoCliente = BLEmail.Enviar(assuntoCliente, destinatarioCliente, conteudoCliente);
                    emailEnviadoCliente = true;
                }

                return emailEnviadoAdmin && emailEnviadoCliente;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return false;
            }
        }
        #endregion

        #region EnviarEmailHistoricoAdmin
        /// <summary>
        /// Envia os emails
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public static bool EnviarEmailHistoricoAdmin(decimal? Codigo)
        {
            try
            {
                #region Lista os dados

                var assuntoAdmin = string.Empty;
                var conteudoAdmin = string.Empty;
                var emailEnviadoAdmin = false;
                var destinatarioAdmin = new List<string>();
                var anexosAdmin = new List<Attachment>();

                var assuntoCliente = string.Empty;
                var conteudoCliente = string.Empty;
                var emailEnviadoCliente = false;
                var destinatarioCliente = new List<string>();
                var anexosCliente = new List<Attachment>();
                var proposta = new MLProgramacaoProposta();

                var modelEmbarqueCerto = BLTakeOrPay.ObterHistoricoCompleto(Codigo.GetValueOrDefault(0));

                if (modelEmbarqueCerto?.lstProposta.Count > 0)
                    proposta = CRUD.Obter(new MLProgramacaoProposta { NumeroProposta = modelEmbarqueCerto?.lstProposta[0]?.NumeroProposta });

                var nomeEmpresa = modelEmbarqueCerto.Email?.Substring(modelEmbarqueCerto.Email.IndexOf('@') + 1, modelEmbarqueCerto.Email.IndexOf('.', modelEmbarqueCerto.Email.IndexOf('@')) - modelEmbarqueCerto.Email.IndexOf('@') - 1);
                var nomeCliente = modelEmbarqueCerto.isBID.GetValueOrDefault(false) ? nomeEmpresa : proposta.Cliente ?? nomeEmpresa;
                #endregion

                if (modelEmbarqueCerto != null && modelEmbarqueCerto.lstProposta != null && modelEmbarqueCerto.lstContainer != null)
                {
                    #region Preenche o email

                    // Adiciona os destinatários
                    destinatarioAdmin.Add(CRUD.Obter(new MLConfiguracao { Chave = "CMS.EmbarqueCerto.EmailAdmin" })?.Valor);
                    destinatarioCliente.Add(modelEmbarqueCerto.Email);

                    // Adiciona os assuntos
                    if (!modelEmbarqueCerto.ReservarEspaco.GetValueOrDefault(false))
                        assuntoAdmin = $"Take or Pay Spot – Navio {modelEmbarqueCerto.lstContainer[0].NavioViagem} – Cliente {nomeCliente}";
                    else if (modelEmbarqueCerto.IsSemanal.GetValueOrDefault(false))
                        assuntoAdmin = $"Take or Pay Regular – Cliente {nomeCliente}";
                    else
                        assuntoAdmin = $"Take or Pay Regular – Navio {modelEmbarqueCerto.lstContainer[0].NavioViagem} – Cliente {nomeCliente}";

                    assuntoCliente = "Aliança | Proposta de Embarque Certo Alterada.";

                    // Adiciona os conteúdos
                    conteudoAdmin = BLUtilitarios.ObterConteudoArquivo(BLConfiguracao.Pastas.ModuloFaleConoscoEmail(PortalAtual.Diretorio) + "/takeorpayadministrativo.htm")
                        .Replace("[DD/MM/YYYY]", modelEmbarqueCerto.DataCadastro.Value.ToShortDateString())
                        .Replace("[XXXXXX]", nomeCliente)
                        .Replace("[Codigo]", modelEmbarqueCerto.Codigo.ToString())
                        .Replace("[caminhoImagem]", $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}");

                    conteudoCliente = BLUtilitarios.ObterConteudoArquivo(BLConfiguracao.Pastas.ModuloFaleConoscoEmail(PortalAtual.Diretorio) + "/takeorpayclientereenvio.htm")
                        .Replace("[nome]", nomeCliente)
                        .Replace("[XXXXXX]", modelEmbarqueCerto.Codigo.ToString())
                        .Replace("[caminhoImagem]", $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}");
                    #endregion

                    anexosAdmin.Add(GerarExcelHistorico(modelEmbarqueCerto, modelEmbarqueCerto.lstProposta, modelEmbarqueCerto.lstContainer));

                    anexosAdmin.Add(GerarPdfHistorico(modelEmbarqueCerto, modelEmbarqueCerto.lstProposta, modelEmbarqueCerto.lstContainer));

                    emailEnviadoAdmin = BLEmail.Enviar(assuntoAdmin, destinatarioAdmin, null, null, conteudoAdmin, anexosAdmin);
                    //emailEnviadoCliente = BLEmail.Enviar(assuntoCliente, destinatarioCliente, conteudoCliente);
                    emailEnviadoCliente = true;
                }

                return emailEnviadoAdmin && emailEnviadoCliente;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return false;
            }
        }
        #endregion

        #region GerarExcel
        /// <summary>
        /// Retorna o Attachment do excel
        /// </summary>
        /// <param name="modelEmbarqueCerto"></param>
        /// <param name="listaPropostas"></param>
        /// <param name="listaContainers"></param>
        /// <returns></returns>
        private static Attachment GerarExcel(MLTakeOrPayEmbarqueCerto modelEmbarqueCerto, List<MLTakeOrPayEmbarqueCertoXProposta> listaPropostas, List<MLTakeOrPayEmbarqueCertoXContainers> listaContainers)
        {
            MemoryStream streamExcel = new MemoryStream();

            using (ExcelPackage excel = new ExcelPackage(streamExcel))
            {
                #region DadosCadastrais

                List<string> propriedadesDadosCadastrais = new List<string> {
                        "Data do registro",
                        "Nome",
                        "Email",
                        "CEP",
                        "Rua",
                        "Bairro",
                        "Cidade",
                        "Estado",
                        "É BID",
                        "Reservar espaço",
                        "É semanal",
                        "Termo foi aceito"
                    };

                //Cria a planilha no arquivo.
                ExcelWorksheet worksheetDadosCadastrais = excel.Workbook.Worksheets.Add("Dados cadastrais");
                ExcelRange cellsDadosCadastrais = worksheetDadosCadastrais.Cells;

                worksheetDadosCadastrais.Column(1).Width = 15;
                worksheetDadosCadastrais.Column(2).Width = 40;
                worksheetDadosCadastrais.Column(3).Width = 40;
                worksheetDadosCadastrais.Column(4).Width = 10;
                worksheetDadosCadastrais.Column(5).Width = 40;
                worksheetDadosCadastrais.Column(6).Width = 40;
                worksheetDadosCadastrais.Column(7).Width = 25;
                worksheetDadosCadastrais.Column(8).Width = 10;
                worksheetDadosCadastrais.Column(9).Width = 10;
                worksheetDadosCadastrais.Column(10).Width = 18;
                worksheetDadosCadastrais.Column(11).Width = 16;
                worksheetDadosCadastrais.Column(12).Width = 19;

                /*Formatação Generica*/
                cellsDadosCadastrais.Style.Font.Name = "Arial";
                cellsDadosCadastrais.Style.Font.Size = 10;
                cellsDadosCadastrais.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cellsDadosCadastrais.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                /*Cabeçalho*/
                for (int i = 0; i < propriedadesDadosCadastrais.Count; i++)
                {
                    cellsDadosCadastrais[1, (i + 1)].Value = propriedadesDadosCadastrais[i];
                    cellsDadosCadastrais[1, (i + 1)].Style.Font.Bold = true;
                    cellsDadosCadastrais[1, (i + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cellsDadosCadastrais[1, (i + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                cellsDadosCadastrais[2, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                cellsDadosCadastrais[2, 1].Value = modelEmbarqueCerto.DataCadastro.Value.ToShortDateString();
                cellsDadosCadastrais[2, 2].Value = modelEmbarqueCerto.Nome;
                cellsDadosCadastrais[2, 3].Value = modelEmbarqueCerto.Email;
                cellsDadosCadastrais[2, 4].Value = modelEmbarqueCerto.CEP;
                cellsDadosCadastrais[2, 5].Value = modelEmbarqueCerto.Logadouro;
                cellsDadosCadastrais[2, 6].Value = modelEmbarqueCerto.Bairro;
                cellsDadosCadastrais[2, 7].Value = modelEmbarqueCerto.Cidade;
                cellsDadosCadastrais[2, 8].Value = modelEmbarqueCerto.Estado;
                cellsDadosCadastrais[2, 9].Value = modelEmbarqueCerto.isBID.GetValueOrDefault(false) ? "Sim" : "Não";
                cellsDadosCadastrais[2, 10].Value = modelEmbarqueCerto.ReservarEspaco.GetValueOrDefault(false) ? "Sim" : "Não";
                cellsDadosCadastrais[2, 11].Value = modelEmbarqueCerto.IsSemanal.GetValueOrDefault(false) ? "Sim" : "Não";
                cellsDadosCadastrais[2, 12].Value = modelEmbarqueCerto.TermoAceito.GetValueOrDefault(false) ? "Sim" : "Não";
                #endregion

                #region Dados X Propostas

                if (!modelEmbarqueCerto.isBID.GetValueOrDefault(false) && listaPropostas.Count > 0)
                {
                    List<string> propriedadesPropostas = new List<string> {
                            "Propostas"
                        };

                    //Cria a planilha no arquivo.
                    ExcelWorksheet worksheetPropostas = excel.Workbook.Worksheets.Add("Propostas");
                    ExcelRange cellsPropostas = worksheetPropostas.Cells;

                    worksheetPropostas.Column(1).Width = 11;

                    /*Formatação Generica*/
                    cellsPropostas.Style.Font.Name = "Arial";
                    cellsPropostas.Style.Font.Size = 10;
                    cellsPropostas.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cellsPropostas.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    /*Cabeçalho*/
                    for (int i = 0; i < propriedadesPropostas.Count; i++)
                    {
                        cellsPropostas[1, (i + 1)].Value = propriedadesPropostas[i];
                        cellsPropostas[1, (i + 1)].Style.Font.Bold = true;
                        cellsPropostas[1, (i + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cellsPropostas[1, (i + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    int rowPropostas = 1;

                    foreach (var item in listaPropostas)
                    {
                        rowPropostas++;

                        cellsPropostas[rowPropostas, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        cellsPropostas[rowPropostas, 1].Value = item.NumeroProposta.ToString();
                    }
                }
                #endregion

                #region Dados X Container

                List<string> propriedadesContainers = new List<string> {
                    "Navio/Viagem",
                    "Navio/Viagem 2ª opçao",
                    "Navio/Viagem 3ª opçao",
                    "Tipo do container",
                    "Tamanho do container",
                    "Tonelagem média",
                    "Porto de Origem",
                    "Porto de Destino",
                    "Unidades",
                    "Tarifa adicional",
                    "Valor por extenso",
                    "Penalidade Aliança",
                    "Valor por extenso (Aliança)"
                };

                //Cria a planilha no arquivo.
                ExcelWorksheet worksheetContainers = excel.Workbook.Worksheets.Add("Containers");
                ExcelRange cellsContainers = worksheetContainers.Cells;

                worksheetContainers.Column(1).Width = 39;
                worksheetContainers.Column(2).Width = 39;
                worksheetContainers.Column(3).Width = 39;
                worksheetContainers.Column(4).Width = 18;
                worksheetContainers.Column(5).Width = 22;
                worksheetContainers.Column(6).Width = 18;
                worksheetContainers.Column(7).Width = 30;
                worksheetContainers.Column(8).Width = 30;
                worksheetContainers.Column(9).Width = 15;
                worksheetContainers.Column(10).Width = 16;
                worksheetContainers.Column(11).Width = 28;
                worksheetContainers.Column(12).Width = 20;
                worksheetContainers.Column(13).Width = 28;

                /*Formatação Generica*/
                cellsContainers.Style.Font.Name = "Arial";
                cellsContainers.Style.Font.Size = 10;
                cellsContainers.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cellsContainers.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                /*Cabeçalho*/
                for (int i = 0; i < propriedadesContainers.Count; i++)
                {
                    cellsContainers[1, (i + 1)].Value = propriedadesContainers[i];
                    cellsContainers[1, (i + 1)].Style.Font.Bold = true;
                    cellsContainers[1, (i + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cellsContainers[1, (i + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                int rowContainers = 1;

                foreach (var item in listaContainers)
                {
                    rowContainers++;

                    cellsContainers[rowContainers, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    cellsContainers[rowContainers, 1].Value = item.NavioViagem;
                    cellsContainers[rowContainers, 2].Value = item.NavioViagemOp2;
                    cellsContainers[rowContainers, 3].Value = item.NavioViagemOp3;
                    cellsContainers[rowContainers, 4].Value = item.TipoContainer;
                    cellsContainers[rowContainers, 5].Value = item.TamanhoContainer;
                    cellsContainers[rowContainers, 6].Value = item.TonelagemMedia;
                    cellsContainers[rowContainers, 7].Value = item.PortoOrigem;
                    cellsContainers[rowContainers, 8].Value = item.PortoDestino;
                    cellsContainers[rowContainers, 9].Value = item.Unidades;
                    cellsContainers[rowContainers, 10].Value = item.TarifaAdicional;
                    cellsContainers[rowContainers, 11].Value = item.ValorTarifa;
                    cellsContainers[rowContainers, 12].Value = item.Penalidade;
                    cellsContainers[rowContainers, 13].Value = item.ValorPenalidade;
                }
                #endregion

                excel.Save();
            }

            streamExcel.Seek(0, SeekOrigin.Begin);

            return new Attachment(streamExcel, "Dados.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
        #endregion

        #region GerarExcelHistorico
        /// <summary>
        /// Retorna o Attachment do excel 
        /// </summary>
        /// <param name="modelEmbarqueCerto"></param>
        /// <param name="listaPropostas"></param>
        /// <param name="listaContainers"></param>
        /// <returns></returns>
        private static Attachment GerarExcelHistorico(MLTakeOrPayEmbarqueCertoHistorico modelEmbarqueCerto, List<MLTakeOrPayEmbarqueCertoXPropostaHistorico> listaPropostas, List<MLTakeOrPayEmbarqueCertoXContainersHistorico> listaContainers)
        {
            MemoryStream streamExcel = new MemoryStream();

            using (ExcelPackage excel = new ExcelPackage(streamExcel))
            {
                #region DadosCadastrais

                List<string> propriedadesDadosCadastrais = new List<string> {
                        "Data do registro",
                        "Nome",
                        "Email",
                        "CEP",
                        "Rua",
                        "Bairro",
                        "Cidade",
                        "Estado",
                        "É BID",
                        "Reservar espaço",
                        "É semanal",
                        "Termo foi aceito"
                    };

                //Cria a planilha no arquivo.
                ExcelWorksheet worksheetDadosCadastrais = excel.Workbook.Worksheets.Add("Dados cadastrais");
                ExcelRange cellsDadosCadastrais = worksheetDadosCadastrais.Cells;

                worksheetDadosCadastrais.Column(1).Width = 15;
                worksheetDadosCadastrais.Column(2).Width = 40;
                worksheetDadosCadastrais.Column(3).Width = 40;
                worksheetDadosCadastrais.Column(4).Width = 10;
                worksheetDadosCadastrais.Column(5).Width = 40;
                worksheetDadosCadastrais.Column(6).Width = 40;
                worksheetDadosCadastrais.Column(7).Width = 25;
                worksheetDadosCadastrais.Column(8).Width = 10;
                worksheetDadosCadastrais.Column(9).Width = 10;
                worksheetDadosCadastrais.Column(10).Width = 18;
                worksheetDadosCadastrais.Column(11).Width = 16;
                worksheetDadosCadastrais.Column(12).Width = 19;

                /*Formatação Generica*/
                cellsDadosCadastrais.Style.Font.Name = "Arial";
                cellsDadosCadastrais.Style.Font.Size = 10;
                cellsDadosCadastrais.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cellsDadosCadastrais.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                /*Cabeçalho*/
                for (int i = 0; i < propriedadesDadosCadastrais.Count; i++)
                {
                    cellsDadosCadastrais[1, (i + 1)].Value = propriedadesDadosCadastrais[i];
                    cellsDadosCadastrais[1, (i + 1)].Style.Font.Bold = true;
                    cellsDadosCadastrais[1, (i + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cellsDadosCadastrais[1, (i + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                cellsDadosCadastrais[2, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cellsDadosCadastrais[2, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                cellsDadosCadastrais[2, 1].Value = modelEmbarqueCerto.DataCadastro.Value.ToShortDateString();
                cellsDadosCadastrais[2, 2].Value = modelEmbarqueCerto.Nome;
                cellsDadosCadastrais[2, 3].Value = modelEmbarqueCerto.Email;
                cellsDadosCadastrais[2, 4].Value = modelEmbarqueCerto.CEP;
                cellsDadosCadastrais[2, 5].Value = modelEmbarqueCerto.Logadouro;
                cellsDadosCadastrais[2, 6].Value = modelEmbarqueCerto.Bairro;
                cellsDadosCadastrais[2, 7].Value = modelEmbarqueCerto.Cidade;
                cellsDadosCadastrais[2, 8].Value = modelEmbarqueCerto.Estado;
                cellsDadosCadastrais[2, 9].Value = modelEmbarqueCerto.isBID.GetValueOrDefault(false) ? "Sim" : "Não";
                cellsDadosCadastrais[2, 10].Value = modelEmbarqueCerto.ReservarEspaco.GetValueOrDefault(false) ? "Sim" : "Não";
                cellsDadosCadastrais[2, 11].Value = modelEmbarqueCerto.IsSemanal.GetValueOrDefault(false) ? "Sim" : "Não";
                cellsDadosCadastrais[2, 12].Value = modelEmbarqueCerto.TermoAceito.GetValueOrDefault(false) ? "Sim" : "Não";
                #endregion

                #region Dados X Propostas

                if (!modelEmbarqueCerto.isBID.GetValueOrDefault(false) && listaPropostas.Count > 0)
                {
                    List<string> propriedadesPropostas = new List<string> {
                            "Propostas"
                        };

                    //Cria a planilha no arquivo.
                    ExcelWorksheet worksheetPropostas = excel.Workbook.Worksheets.Add("Propostas");
                    ExcelRange cellsPropostas = worksheetPropostas.Cells;

                    worksheetPropostas.Column(1).Width = 11;

                    /*Formatação Generica*/
                    cellsPropostas.Style.Font.Name = "Arial";
                    cellsPropostas.Style.Font.Size = 10;
                    cellsPropostas.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cellsPropostas.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    /*Cabeçalho*/
                    for (int i = 0; i < propriedadesPropostas.Count; i++)
                    {
                        cellsPropostas[1, (i + 1)].Value = propriedadesPropostas[i];
                        cellsPropostas[1, (i + 1)].Style.Font.Bold = true;
                        cellsPropostas[1, (i + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cellsPropostas[1, (i + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    int rowPropostas = 1;

                    foreach (var item in listaPropostas)
                    {
                        rowPropostas++;

                        cellsPropostas[rowPropostas, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        cellsPropostas[rowPropostas, 1].Value = item.NumeroProposta.ToString();
                    }
                }
                #endregion

                #region Dados X Container

                List<string> propriedadesContainers = new List<string> {
                    "Navio/Viagem",
                    "Navio/Viagem 2ª opçao",
                    "Navio/Viagem 3ª opçao",
                    "Tipo do container",
                    "Tamanho do container",
                    "Tonelagem média",
                    "Unidades de RIG para MAO",
                    "Unidades de IBB para MAO",
                    "Unidades de IOA para MAO",
                    "Unidades de SSZ para MAO",
                    "Unidades de SPB para MAO",
                    "Unidades de VIX para MAO",
                    "Unidades de SSA para MAO",
                    "Unidades de SUA para MAO",
                    "Unidades de PEC para MAO",
                };

                //Cria a planilha no arquivo.
                ExcelWorksheet worksheetContainers = excel.Workbook.Worksheets.Add("Containers");
                ExcelRange cellsContainers = worksheetContainers.Cells;

                worksheetContainers.Column(1).Width = 39;
                worksheetContainers.Column(2).Width = 39;
                worksheetContainers.Column(3).Width = 39;
                worksheetContainers.Column(4).Width = 18;
                worksheetContainers.Column(5).Width = 22;
                worksheetContainers.Column(6).Width = 18;
                worksheetContainers.Column(7).Width = 26;
                worksheetContainers.Column(8).Width = 26;
                worksheetContainers.Column(9).Width = 26;
                worksheetContainers.Column(10).Width = 26;
                worksheetContainers.Column(11).Width = 26;
                worksheetContainers.Column(12).Width = 26;
                worksheetContainers.Column(13).Width = 26;
                worksheetContainers.Column(14).Width = 26;
                worksheetContainers.Column(15).Width = 26;

                /*Formatação Generica*/
                cellsContainers.Style.Font.Name = "Arial";
                cellsContainers.Style.Font.Size = 10;
                cellsContainers.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cellsContainers.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                /*Cabeçalho*/
                for (int i = 0; i < propriedadesContainers.Count; i++)
                {
                    cellsContainers[1, (i + 1)].Value = propriedadesContainers[i];
                    cellsContainers[1, (i + 1)].Style.Font.Bold = true;
                    cellsContainers[1, (i + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cellsContainers[1, (i + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                int rowContainers = 1;

                foreach (var item in listaContainers)
                {
                    rowContainers++;

                    cellsContainers[rowContainers, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cellsContainers[rowContainers, 15].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    cellsContainers[rowContainers, 1].Value = item.NavioViagem;
                    cellsContainers[rowContainers, 2].Value = item.NavioViagemOp2;
                    cellsContainers[rowContainers, 3].Value = item.NavioViagemOp3;
                    cellsContainers[rowContainers, 4].Value = item.TipoContainer;
                    cellsContainers[rowContainers, 5].Value = item.TamanhoContainer;
                    cellsContainers[rowContainers, 6].Value = item.TonelagemMedia;
                    cellsContainers[rowContainers, 7].Value = item.RIG;
                    cellsContainers[rowContainers, 8].Value = item.IBB;
                    cellsContainers[rowContainers, 9].Value = item.IOA;
                    cellsContainers[rowContainers, 10].Value = item.SSZ;
                    cellsContainers[rowContainers, 11].Value = item.SPB;
                    cellsContainers[rowContainers, 12].Value = item.VIX;
                    cellsContainers[rowContainers, 13].Value = item.SSA;
                    cellsContainers[rowContainers, 14].Value = item.SUA;
                    cellsContainers[rowContainers, 15].Value = item.PEC;
                }
                #endregion

                excel.Save();
            }

            streamExcel.Seek(0, SeekOrigin.Begin);

            return new Attachment(streamExcel, "Dados.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
        #endregion

        #region GerarPdf
        /// <summary>
        /// Retorna o Attachment do pdf
        /// </summary>
        /// <param name="modelEmbarqueCerto"></param>
        /// <param name="listaPropostas"></param>
        /// <param name="listaContainers"></param>
        /// <returns></returns>
        private static Attachment GerarPdf(MLTakeOrPayEmbarqueCerto modelEmbarqueCerto, List<MLTakeOrPayEmbarqueCertoXProposta> listaPropostas, List<MLTakeOrPayEmbarqueCertoXContainers> listaContainers)
        {
            var htmltermos = PreencherTermos(modelEmbarqueCerto, listaPropostas, listaContainers);

            var html = BLUtilitarios.ObterConteudoArquivo(Path.Combine("~/Areas/Modulo/Views/TakeOrPayTermos/TermoPdf.cshtml"));

            html = html.Replace("[[ConteudoTermos]]", htmltermos).Replace("[[NomeCliente]]", modelEmbarqueCerto.Nome).Replace("[[ImagemHeader]]", $"data:image/png;base64, {Convert.ToBase64String(System.IO.File.ReadAllBytes(HostingEnvironment.MapPath($"~/Portal/{PortalAtual.Diretorio}/img/alianca_footer.png")))}");

            var pdfConverter = new NReco.PdfGenerator.HtmlToPdfConverter();

            pdfConverter.PageFooterHtml = $"<figure class=\"header--logo\" style=\"text-align:right;\"><img src=\"data:image/png;base64, {Convert.ToBase64String(System.IO.File.ReadAllBytes(HostingEnvironment.MapPath($"~/Portal/{PortalAtual.Diretorio}/img/FooterPdf.png")))}\" alt=\"\" height=\"60\" width=\"60\"></ figure>";

            pdfConverter.Margins = new PageMargins { Top = 10, Bottom = 20, Left = 10, Right = 10 };

            var bytes = pdfConverter.GeneratePdf(html);

            if (!Directory.Exists(Path.Combine(string.Format("/portal/{0}/arquivos/takeorpay/pdf/", PortalAtual.Diretorio))))
                Directory.CreateDirectory(HostingEnvironment.MapPath(string.Format("/portal/{0}/arquivos/takeorpay/pdf/", PortalAtual.Diretorio)));

            System.IO.File.WriteAllBytes(HostingEnvironment.MapPath(string.Format("/portal/{0}/arquivos/takeorpay/pdf/{1}.pdf", PortalAtual.Diretorio, modelEmbarqueCerto.Codigo)), bytes);

            return new Attachment(new MemoryStream(bytes), "Termos de uso.pdf", "application/pdf");
        }
        #endregion

        #region GerarPdfHistorico
        /// <summary>
        /// Retorna o Attachment do pdf
        /// </summary>
        /// <param name="modelEmbarqueCerto"></param>
        /// <param name="listaPropostas"></param>
        /// <param name="listaContainers"></param>
        /// <returns></returns>
        private static Attachment GerarPdfHistorico(MLTakeOrPayEmbarqueCertoHistorico modelEmbarqueCerto, List<MLTakeOrPayEmbarqueCertoXPropostaHistorico> listaPropostas, List<MLTakeOrPayEmbarqueCertoXContainersHistorico> listaContainers)
        {
            var htmltermos = PreencherTermosHistorico(modelEmbarqueCerto, listaPropostas, listaContainers);

            var html = BLUtilitarios.ObterConteudoArquivo(Path.Combine("~/Areas/Modulo/Views/TakeOrPayTermos/TermoPdfHistorico.cshtml"));

            html = html.Replace("[[ConteudoTermos]]", htmltermos).Replace("[[NomeCliente]]", modelEmbarqueCerto.Nome).Replace("[[ImagemHeader]]", $"data:image/png;base64, {Convert.ToBase64String(System.IO.File.ReadAllBytes(HostingEnvironment.MapPath($"~/Portal/{PortalAtual.Diretorio}/img/alianca_footer.png")))}");

            var pdfConverter = new NReco.PdfGenerator.HtmlToPdfConverter();

            pdfConverter.PageFooterHtml = $"<figure class=\"header--logo\" style=\"text-align:right;\"><img src=\"data:image/png;base64, {Convert.ToBase64String(System.IO.File.ReadAllBytes(HostingEnvironment.MapPath($"~/Portal/{PortalAtual.Diretorio}/img/FooterPdf.png")))}\" alt=\"\" height=\"60\" width=\"60\"></ figure>";

            pdfConverter.Margins = new PageMargins { Top = 10, Bottom = 20, Left = 10, Right = 10 };

            var bytes = pdfConverter.GeneratePdf(html);

            if (!Directory.Exists(Path.Combine(string.Format("/portal/{0}/arquivos/takeorpayhistorico/pdf/", PortalAtual.Diretorio))))
                Directory.CreateDirectory(HostingEnvironment.MapPath(string.Format("/portal/{0}/arquivos/takeorpayhistorico/pdf/", PortalAtual.Diretorio)));

            System.IO.File.WriteAllBytes(HostingEnvironment.MapPath(string.Format("/portal/{0}/arquivos/takeorpayhistorico/pdf/{1}.pdf", PortalAtual.Diretorio, modelEmbarqueCerto.Codigo)), bytes);

            return new Attachment(new MemoryStream(bytes), "Termos de uso.pdf", "application/pdf");
        }
        #endregion

        #region PreencherTermos
        /// <summary>
        /// Preenche o termos seguindo o modelo
        /// </summary>
        /// <param name="modelEmbarqueCerto"></param>
        /// <param name="listaPropostas"></param>
        /// <param name="listaContainers"></param>
        /// <returns></returns>
        public static string PreencherTermos(MLTakeOrPayEmbarqueCerto modelEmbarqueCerto, List<MLTakeOrPayEmbarqueCertoXProposta> listaPropostas, List<MLTakeOrPayEmbarqueCertoXContainers> listaContainers)
        {
            try
            {
                var propostas = string.Empty;
                var termos = string.Empty;
                var proposta = new MLProgramacaoProposta();

                if (modelEmbarqueCerto != null && listaPropostas != null && listaContainers != null)
                {
                    var culture = new System.Globalization.CultureInfo("pt-BR");

                    if(listaPropostas.Count > 0)
                        proposta = CRUD.Obter(new MLProgramacaoProposta { NumeroProposta = listaPropostas[0].NumeroProposta });

                    var nomeEmpresa = modelEmbarqueCerto.Email?.Substring(modelEmbarqueCerto.Email.IndexOf('@') + 1, modelEmbarqueCerto.Email.IndexOf('.', modelEmbarqueCerto.Email.IndexOf('@')) - modelEmbarqueCerto.Email.IndexOf('@') - 1);
                    var nomeCliente = modelEmbarqueCerto.isBID.GetValueOrDefault(false) ? nomeEmpresa : proposta.Cliente ?? nomeEmpresa;

                    if (!modelEmbarqueCerto.isBID.GetValueOrDefault(false))
                    {
                        propostas = string.Join(", ", listaPropostas.Select(x => x.NumeroProposta.ToString()));
                    }

                    if (modelEmbarqueCerto.ReservarEspaco.GetValueOrDefault(false))
                        termos = BLUtilitarios.ObterConteudoArquivo(Path.Combine("~/Areas/Modulo/Views/TakeOrPayTermos/TermoRegular.cshtml"));
                    else
                        termos = BLUtilitarios.ObterConteudoArquivo(Path.Combine("~/Areas/Modulo/Views/TakeOrPayTermos/TermoSpot.cshtml"));

                    var portos = "";
                    var valorAdicional = "";
                    var valorPenalidade = "";

                    foreach (var item in listaContainers)
                    {
                        portos += $"Porto {item.PortoOrigem}: {item.Unidades} unidades; ";
                        valorAdicional += $"R$ {item.TarifaAdicional} ({item.ValorTarifa} reais)/unidade; ";
                        valorPenalidade += $"R$ {item.Penalidade} ({item.ValorPenalidade} reais); ";
                    }

                    // Preenche os campos dinâmicos do termo
                    termos = termos.Replace("[[NomeCliente]]", nomeCliente)
                        .Replace("[[CNPJ]]", modelEmbarqueCerto.isBID.GetValueOrDefault(false) ? modelEmbarqueCerto.CNPJ : Convert.ToUInt64(proposta?.CNPJ).ToString(@"00\.000\.000\/0000\-00") ?? modelEmbarqueCerto.CNPJ)
                        .Replace("[[EnderecoCompleto]]", $"{modelEmbarqueCerto.Logadouro}, {modelEmbarqueCerto.Bairro} - {modelEmbarqueCerto.Cidade}/{modelEmbarqueCerto.Estado} {modelEmbarqueCerto.CEP}")
                        .Replace("[[NomeNavio]]", modelEmbarqueCerto.IsSemanal.GetValueOrDefault(false) && modelEmbarqueCerto.ReservarEspaco.GetValueOrDefault(false) ? BLConfiguracao.Pastas.ModuloTakeOrPayNomeNavio() : listaContainers[0].NavioViagem)
                        .Replace("[[TotalUnidades]]", listaContainers.Sum(x => x.Unidades).ToString())
                        .Replace("[[Portos]]", portos)
                        .Replace("[[ValorAdicional]]", valorAdicional.TrimEnd().TrimEnd(';'))
                        .Replace("[[ValorPenalidade]]", valorPenalidade.TrimEnd().TrimEnd(';'))
                        .Replace("[[TextoPropostaComercial]]", modelEmbarqueCerto.isBID.GetValueOrDefault(false) ? BLConfiguracao.Pastas.ModuloTakeOrPayTextoPropostaComercial() : string.Format(BLConfiguracao.Pastas.ModuloTakeOrPayTextoPropostaComercialPropostas(), propostas))
                        .Replace("[[Semanal/Mensal]]", modelEmbarqueCerto.IsSemanal.GetValueOrDefault(false) ? "semanal" : "mensal")
                        .Replace("[[Dia]]", DateTime.Now.Day.ToString())
                        .Replace("[[Mes]]", culture.DateTimeFormat.GetMonthName(DateTime.Now.Month));
                }

                return termos;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return string.Empty;
            }
        }
        #endregion

        #region PreencherTermosHistorico
        /// <summary>
        /// Preenche o termos seguindo o modelo
        /// </summary>
        /// <param name="modelEmbarqueCerto"></param>
        /// <param name="listaPropostas"></param>
        /// <param name="listaContainers"></param>
        /// <returns></returns>
        public static string PreencherTermosHistorico(MLTakeOrPayEmbarqueCertoHistorico modelEmbarqueCerto, List<MLTakeOrPayEmbarqueCertoXPropostaHistorico> listaPropostas, List<MLTakeOrPayEmbarqueCertoXContainersHistorico> listaContainers)
        {
            try
            {
                var propostas = string.Empty;
                var termos = string.Empty;
                var proposta = new MLProgramacaoProposta();

                if (modelEmbarqueCerto != null && listaPropostas != null && listaContainers != null)
                {
                    var culture = new System.Globalization.CultureInfo("pt-BR");

                    if (listaPropostas.Count > 0)
                        proposta = CRUD.Obter(new MLProgramacaoProposta { NumeroProposta = listaPropostas[0].NumeroProposta });

                    var unidadesRIG = listaContainers.Sum(x => x.RIG).GetValueOrDefault(0);
                    var unidadesIBB = listaContainers.Sum(x => x.IBB).GetValueOrDefault(0);
                    var unidadesIOA = listaContainers.Sum(x => x.IOA).GetValueOrDefault(0);
                    var unidadesSSZ = listaContainers.Sum(x => x.SSZ).GetValueOrDefault(0);
                    var unidadesSPB = listaContainers.Sum(x => x.SPB).GetValueOrDefault(0);
                    var unidadesVIX = listaContainers.Sum(x => x.VIX).GetValueOrDefault(0);
                    var unidadesSSA = listaContainers.Sum(x => x.SSA).GetValueOrDefault(0);
                    var unidadesSUA = listaContainers.Sum(x => x.SUA).GetValueOrDefault(0);
                    var unidadesPEC = listaContainers.Sum(x => x.PEC).GetValueOrDefault(0);

                    var unidadesTotais = unidadesRIG + unidadesIBB + unidadesIOA + unidadesSSZ + unidadesSPB + unidadesVIX + unidadesSSA + unidadesSUA + unidadesPEC;

                    var nomeEmpresa = modelEmbarqueCerto.Email?.Substring(modelEmbarqueCerto.Email.IndexOf('@') + 1, modelEmbarqueCerto.Email.IndexOf('.', modelEmbarqueCerto.Email.IndexOf('@')) - modelEmbarqueCerto.Email.IndexOf('@') - 1);
                    var nomeCliente = modelEmbarqueCerto.isBID.GetValueOrDefault(false) ? nomeEmpresa : proposta.Cliente ?? nomeEmpresa;

                    if (!modelEmbarqueCerto.isBID.GetValueOrDefault(false))
                    {
                        propostas = string.Join(", ", listaPropostas.Select(x => x.NumeroProposta.ToString()));
                    }

                    if (modelEmbarqueCerto.ReservarEspaco.GetValueOrDefault(false))
                        termos = BLUtilitarios.ObterConteudoArquivo(Path.Combine("~/Areas/Modulo/Views/TakeOrPayTermos/TermoRegularHistorico.cshtml"));
                    else
                        termos = BLUtilitarios.ObterConteudoArquivo(Path.Combine("~/Areas/Modulo/Views/TakeOrPayTermos/TermoSpotHistorico.cshtml"));

                    // Preenche os campos dinâmicos do termo
                    termos = termos.Replace("[[NomeCliente]]", nomeCliente)
                        .Replace("[[CNPJ]]", modelEmbarqueCerto.isBID.GetValueOrDefault(false) ? modelEmbarqueCerto.CNPJ : Convert.ToUInt64(proposta?.CNPJ).ToString(@"00\.000\.000\/0000\-00") ?? modelEmbarqueCerto.CNPJ)
                        .Replace("[[EnderecoCompleto]]", $"{modelEmbarqueCerto.Logadouro}, {modelEmbarqueCerto.Bairro} - {modelEmbarqueCerto.Cidade}/{modelEmbarqueCerto.Estado} {modelEmbarqueCerto.CEP}")
                        .Replace("[[NomeNavio]]", modelEmbarqueCerto.IsSemanal.GetValueOrDefault(false) && modelEmbarqueCerto.ReservarEspaco.GetValueOrDefault(false) ? BLConfiguracao.Pastas.ModuloTakeOrPayNomeNavio() : listaContainers[0].NavioViagem)
                        .Replace("[[UnidadesTotais]]", unidadesTotais.ToString())
                        .Replace("[[RIG]]", unidadesRIG.ToString())
                        .Replace("[[IBB]]", unidadesIBB.ToString())
                        .Replace("[[IOA]]", unidadesIOA.ToString())
                        .Replace("[[SSZ]]", unidadesSSZ.ToString())
                        .Replace("[[SPB]]", unidadesSPB.ToString())
                        .Replace("[[VIX]]", unidadesVIX.ToString())
                        .Replace("[[SSA]]", unidadesSSA.ToString())
                        .Replace("[[SUA]]", unidadesSUA.ToString())
                        .Replace("[[PEC]]", unidadesPEC.ToString())
                        .Replace("[[TextoPropostaComercial]]", modelEmbarqueCerto.isBID.GetValueOrDefault(false) ? BLConfiguracao.Pastas.ModuloTakeOrPayTextoPropostaComercial() : string.Format(BLConfiguracao.Pastas.ModuloTakeOrPayTextoPropostaComercialPropostas(), propostas))
                        .Replace("[[Semanal/Mensal]]", modelEmbarqueCerto.IsSemanal.GetValueOrDefault(false) ? "semanal" : "mensal")
                        .Replace("[[Dia]]", DateTime.Now.Day.ToString())
                        .Replace("[[Mes]]", culture.DateTimeFormat.GetMonthName(DateTime.Now.Month));
                }

                return termos;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return string.Empty;
            }
        }
        #endregion

        #region FormarOption
        /// <summary>
        /// FormarOption
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<string> FormarOption(List<string> list = null)
        {
            var retorno = new List<string> { "<option value=\"\" selected>Selecione</option>" };

            if (list == null || list.Count == 0)
                return retorno;

            retorno.AddRange(list.Select(x => $"<option value=\"{x}\">{x}</option>").Distinct(StringComparer.CurrentCultureIgnoreCase));

            return retorno;
        }
        #endregion
    }
}
