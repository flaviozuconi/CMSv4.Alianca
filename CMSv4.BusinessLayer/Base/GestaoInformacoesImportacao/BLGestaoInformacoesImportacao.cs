using CMSv4.Model.Base.GestaoInformacoesImportacao;
using Framework.DataLayer;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Web;

namespace CMSv4.BusinessLayer.Base.GestaoInformacoesImportacao
{
    public class BLGestaoInformacoesImportacao : BLCRUD<MLGestaoInformacoesImportacao>
    {
        #region Validacao
        /// <summary>
        /// Valida se já existe o registro
        /// </summary>
        /// <param name="id"></param>
        /// <param name="proposta"></param>
        /// <param name="booking"></param>
        /// <param name="numeroBL"></param>
        /// <returns></returns>
        public static bool IsValid(decimal? id, string proposta, string booking, string numeroBL)
        {
            if (booking.Replace(" ", "") == "") booking = null;

            if (proposta.Replace(" ", "") == "") proposta = null;
            
            if (numeroBL.Replace(" ", "") == "") numeroBL = null;

            var model = CRUD.Obter(new MLGestaoInformacoesImportacao() { Codigo = id, PropostaComercial = proposta, NumeroBooking = booking, NumeroBL = numeroBL });

            if (model != null && model.Codigo.HasValue && model.Codigo.Value > 0 && id != model.Codigo)
                return false;

            return true;
        }
        #endregion

        #region Importacao
        /// <summary>
        /// Importa os dados da planilha de Gestao de Informações de Importação
        /// </summary>
        /// <param name="file"></param>
        /// <param name="excluir"></param>
        /// <param name="importacao"></param>
        /// <returns></returns>
        public static string Importacao(HttpPostedFileBase file, bool? excluir, MLGestaoInformacoesImportacaoHistorico importacao) 
        {
            var lstModel = BLUtilitarios.EPPlus.LerExcel<MLGestaoInformacoesImportacao>(file);

            string erros = ValidaPlanilha(lstModel);

            if (!string.IsNullOrEmpty(erros))
            {
                importacao.Sucesso = false;
                importacao.Finalizado = true;
                new BLCRUD<MLGestaoInformacoesImportacaoHistorico>().SalvarParcial(importacao);

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
        /// Valida os dados da planilha de Gestao de Informações e Importação
        /// </summary>
        /// <param name="lista"></param>
        /// <returns></returns>
        public static string ValidaPlanilha(List<MLGestaoInformacoesImportacao> lista)
        {
            StringBuilder erros = new StringBuilder();
            int linha = 2;

            List<int> errPropostaComercial = new List<int>();
            List<int> errNumeroBooking = new List<int>();
            List<int> errNumeroBL = new List<int>();

            if (lista.Count <= 0)
                return TAdm("A planilha está vazia.");

            foreach (var item in lista)
            {
                if (string.IsNullOrEmpty(item.PropostaComercial))
                    errPropostaComercial.Add(linha);

                if (string.IsNullOrEmpty(item.NumeroBooking))
                    errNumeroBooking.Add(linha);

                if (string.IsNullOrEmpty(item.NumeroBL))
                    errNumeroBL.Add(linha);

                linha++;
            }

            if (errPropostaComercial.Count > 0)
                erros.AppendLine("O campo \"Proposta Comercial\" é obrigatório na(s) linha(s): " + string.Join(",", errPropostaComercial) + "<br>");

            if (errNumeroBooking.Count > 0)
                erros.AppendLine("O campo \"Numero do Booking\" é obrigatório na(s) linha(s): " + string.Join(",", errNumeroBooking) + "<br>");

            if (errNumeroBL.Count > 0)
                erros.AppendLine("O campo \"Numero do BL\" é obrigatório na(s) linha(s): " + string.Join(",", errNumeroBL) + "<br>");

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
        public static void InserirPlanilha(List<MLGestaoInformacoesImportacao> lista, MLGestaoInformacoesImportacaoHistorico importacao, bool? excluir, decimal? codigoUsuario)
        {
            try
            {
                if (excluir.HasValue && excluir.Value)
                {
                    Database.ExecuteNonQuery(new SqlCommand("TRUNCATE TABLE MOD_GII_GESTAO_INFORMACOES_IMPORTACAO"));
                }

                var listaImport = CRUD.Listar<MLGestaoInformacoesImportacao>();

                foreach (var item in lista)
                {
                    item.CodigoUsuario = codigoUsuario;
                    item.DataAtualizacao = DateTime.Now;

                    var existe = listaImport.Find(x => x.NumeroBooking == item.NumeroBooking && 
                    x.PropostaComercial == item.PropostaComercial && x.NumeroBL == item.NumeroBL);

                    if (existe != null && existe.Codigo != null)
                    {
                        item.Codigo = existe.Codigo;
                    }

                    new BLCRUD<MLGestaoInformacoesImportacao>().SalvarParcial(item);
                }

                importacao.Sucesso = true;
                importacao.Finalizado = true;

                new BLCRUD<MLGestaoInformacoesImportacaoHistorico>().SalvarParcial(importacao);

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                importacao.Sucesso = false;
                importacao.Finalizado = true;
                new BLCRUD<MLGestaoInformacoesImportacaoHistorico>().SalvarParcial(importacao);
            }
        }
        #endregion

    }
}
