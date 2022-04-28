using CMSv4.Model.Base.GestaoInformacoesExportacao;
using Framework.DataLayer;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Web;

namespace CMSv4.BusinessLayer.Base.GestaoInformacoesExportacao
{
    public class BLGestaoInformacoesExportacao : BLCRUD<MLGestaoInformacoesExportacao>
    {
        #region Importacao
        /// <summary>
        /// Importa os dados da planilha de Gestao de Informações e Exportação
        /// </summary>
        /// <param name="file"></param>
        /// <param name="excluir"></param>
        /// <param name="importacao"></param>
        /// <returns></returns>
        public static string Importacao(HttpPostedFileBase file, bool? excluir, MLGestaoInformacoesExportacaoHistorico importacao)
        {
            var lstModel = BLUtilitarios.EPPlus.LerExcel<MLGestaoInformacoesExportacao>(file);

            string erros = ValidaPlanilha(lstModel);

            if (!string.IsNullOrEmpty(erros))
            {
                importacao.Sucesso = false;
                importacao.Finalizado = true;
                new BLCRUD<MLGestaoInformacoesExportacaoHistorico>().SalvarParcial(importacao);

                return erros;
            }

            var codigoUsuario = BLUsuario.ObterLogado()?.Codigo;

            Thread thread = new Thread(new ThreadStart(() =>
            {
                InserirPlanilha(lstModel, importacao, excluir, codigoUsuario);

            }));

            thread.Start();

            return erros;
        }
        #endregion

        #region TAdm
        /// <summary>
        /// Tradução
        /// </summary>
        /// <param name="texto"></param>
        /// <returns></returns>
        public static string TAdm(string texto)
        {
            var T = new BLTraducao();
            return T.ObterAdm(texto);
        }
        #endregion

        #region ValidaPlanilha
        /// <summary>
        /// Valida os dados da planilha de Gestao de Informações e Exportação
        /// </summary>
        /// <param name="lista"></param>
        /// <returns></returns>
        public static string ValidaPlanilha(List<MLGestaoInformacoesExportacao> lista)
        {
            StringBuilder erros = new StringBuilder();
            int linha = 2;

            List<int> errPropostaComercial = new List<int>();
            List<int> errNumeroBooking = new List<int>();

            if (lista.Count <= 0)
                return TAdm("A planilha está vazia.");

            foreach (var item in lista)
            {
                if (string.IsNullOrEmpty(item.PropostaComercial))
                    errPropostaComercial.Add(linha);

                if (string.IsNullOrEmpty(item.NumeroBooking))
                    errNumeroBooking.Add(linha);

                linha++;
            }

            if (errPropostaComercial.Count > 0)
                erros.AppendLine("O campo \"Proposta Comercial\" é obrigatório na(s) linha(s): " + string.Join(",", errPropostaComercial) + "<br>");

            if (errNumeroBooking.Count > 0)
                erros.AppendLine("O campo \"Numero do Booking\" é obrigatório na(s) linha(s): " + string.Join(",", errNumeroBooking) + "<br>");

            return erros.ToString();
        }
        #endregion

        #region Inserir Planilha
        /// <summary>
        /// Insere os registros da planilha no banco
        /// </summary>
        /// <param name="lista"></param>
        /// <param name="importacao"></param>
        /// <param name="excluir"></param>
        /// <param name="codigoUsuario"></param>
        public static void InserirPlanilha(List<MLGestaoInformacoesExportacao> lista,
            MLGestaoInformacoesExportacaoHistorico importacao, bool? excluir, decimal? codigoUsuario)
        {

            try
            {
                if (excluir.HasValue && excluir.Value)
                {
                    Database.ExecuteNonQuery(new SqlCommand("TRUNCATE TABLE MOD_GIE_GESTAO_INFORMACOES_EXPORTACAO"));
                }

                foreach (var item in lista)
                {
                    item.CodigoUsuario = codigoUsuario;
                    item.DataAtualizacao = DateTime.Now;

                    new BLCRUD<MLGestaoInformacoesExportacao>().Salvar(item);
                }

                importacao.Sucesso = true;
                importacao.Finalizado = true;

                new BLCRUD<MLGestaoInformacoesExportacaoHistorico>().SalvarParcial(importacao);

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                importacao.Sucesso = false;
                importacao.Finalizado = true;
                new BLCRUD<MLGestaoInformacoesExportacaoHistorico>().SalvarParcial(importacao);
            }
        }
        #endregion
    }
}
