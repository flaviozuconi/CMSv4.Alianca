using Framework.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CMSv4.Model.Base
{
    [Table("MOD_TKP_TAKE_OR_PAY_PROGRAMACAO_PROPOSTA")]
    [Serializable]
    public class MLProgramacaoProposta
    {
       
        [DataField("PRP_N_CODIGO", SqlDbType.Decimal, 18, 0, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }
   
        [DataField("PRP_USU_N_CODIGO", SqlDbType.Decimal, 18, 0)]
        public decimal? CodigoUsuario { get; set; }
 
        [DataField("PRP_D_DATA_IMPORTACAO", SqlDbType.DateTime)]
        public DateTime? DataImportacao { get; set; }

        [CsvField("Nº Proposta", 0)]
        [DataField("PRP_N_PROPOSTA", SqlDbType.Decimal, 18, 0)]
        public decimal? NumeroProposta { get; set; }

        [CsvField("Cliente", 1)]
        [DataField("PRP_C_CLIENTE", SqlDbType.VarChar, 250)]
        public string Cliente { get; set; }

        [CsvField("CNPJ", 2)]
        [DataField("PRP_C_CNPJ", SqlDbType.VarChar, 20)]
        public string CNPJ { get; set; }

        [CsvField("IE", 3)]
        [DataField("PRP_C_IE", SqlDbType.VarChar, 25)]
        public string IE { get; set; }

        [CsvField("Tipo", 4)]
        [DataField("PRP_C_TIPO", SqlDbType.VarChar, 25)]
        public string Tipo { get; set; }

        [CsvField("Filial", 5)]
        [DataField("PRP_C_FILIAL", SqlDbType.VarChar, 150)]
        public string Filial { get; set; }

        [CsvField("Tipo Proposta", 6)]
        [DataField("PRP_C_TIPO_PROPOSTA", SqlDbType.VarChar, 150)]
        public string TipoProposta { get; set; }

        [CsvField("Vendedor", 7)]
        [DataField("PRP_C_VENDEDOR", SqlDbType.VarChar, 250)]
        public string Vendedor { get; set; }

        [CsvField("Serv Prestado", 8)]
        [DataField("PRP_C_SERVICO_PRESTADO", SqlDbType.VarChar, 250)]
        public string ServicoPrestado { get; set; }

        [CsvField("Tipo operação comercial", 9)]
        [DataField("PRP_C_TIPO_OPERACAO", SqlDbType.VarChar, 150)]
        public string TipoOperacao { get; set; }

        [CsvField("Pagamento", 10)]
        [DataField("PRP_C_PAGAMENTO", SqlDbType.VarChar, 50)]
        public string Pagamento { get; set; }

        [CsvField("Familia Mercadoria", 11)]
        [DataField("PRP_C_FAMILIA_MERCADORIA", SqlDbType.VarChar, 250)]
        public string FamiliaMercadoria { get; set; }

        [CsvField("Diferenciador", 12)]
        [DataField("PRP_C_DIFERENCIADOR", SqlDbType.VarChar, -1)]
        public string Diferenciador { get; set; }

        [CsvField("Modal", 13)]
        [DataField("PRP_C_MODAL", SqlDbType.VarChar, 50)]
        public string Modal { get; set; }

        [CsvField("Ova", 14)]
        [DataField("PRP_C_OVA", SqlDbType.VarChar, 50)]
        public string Ova { get; set; }

        [CsvField("Desova", 15)]
        [DataField("PRP_C_DESOVA", SqlDbType.VarChar, 50)]
        public string Desova { get; set; }

        [CsvField("Escolta Origem", 16)]
        [DataField("PRP_C_ESCOLTA_ORIGEM", SqlDbType.VarChar, 1)]
        public string EscoltaOrigem { get; set; }

        [CsvField("Escolta Destino", 17)]
        [DataField("PRP_C_ESCOLTA_DESTINO", SqlDbType.VarChar, 1)]
        public string EscoltaDestino { get; set; }

        [CsvField("Conferente Origem", 18)]
        [DataField("PRP_C_CONFERENTE_ORIGEM", SqlDbType.VarChar, 1)]
        public string ConferenteOrigem { get; set; }

        [CsvField("Conferente Destino", 19)]
        [DataField("PRP_C_CONFERENTE_DESTINO", SqlDbType.VarChar, 1)]
        public string ConferenteDestino { get; set; }

        [CsvField("Tipo Seguro", 20)]
        [DataField("PRP_C_TIPO_SEGURO", SqlDbType.VarChar, 50)]
        public string TipoSeguro { get; set; }

        [CsvField("Tipo DDR", 21)]
        [DataField("PRP_C_TIPO_DDR", SqlDbType.VarChar, 50)]
        public string TipoDDR { get; set; }

        [CsvField("Data validade DDR", 22)]
        [DataField("PRP_D_VALIDADE_DDR", SqlDbType.DateTime)]
        public DateTime? ValidadeDDR { get; set; }

        [CsvField("Valor Declarado", 23)]
        [DataField("PRP_N_VALOR_DECLARADO", SqlDbType.Decimal, 16, 2)]
        public decimal? ValorDeclarado { get; set; }

        [CsvField("Volume Médio Mensal", 24)]
        [DataField("PRP_N_VALOR_MEDIO_MENSAL", SqlDbType.Decimal, 16, 0)]
        public decimal? ValorMedioMensal  { get; set; }

        [CsvField("Shipper Own Container", 25)]
        [DataField("PRP_C_SHIPPER_OWN_CONTAINER", SqlDbType.VarChar, 1)]
        public string ShipperOwnContainer { get; set; }

        [CsvField("Carga Perigosa", 26)]
        [DataField("PRP_C_CARGA_PERIGOSA", SqlDbType.VarChar, 1)]
        public string CargaPerigosa { get; set; }

        [CsvField("OOG", 27)]
        [DataField("PRP_C_OOG", SqlDbType.VarChar, 1)]
        public string OOG { get; set; }

        [CsvField("Serviços Especiais", 28)]
        [DataField("PRP_C_SERVICOS_ESPECIAIS", SqlDbType.VarChar, 1)]
        public string ServicosEspeciais { get; set; }

        [CsvField("Porto Origem", 29)]
        [DataField("PRP_C_PORTO_ORIGEM", SqlDbType.VarChar, 150)]
        public string PortoOrigem { get; set; }

        [CsvField("Municipio Origem", 30)]
        [DataField("PRP_C_MUNICIPIO_ORIGEM", SqlDbType.VarChar, 150)]
        public string MunicipioOrigem { get; set; }

        [CsvField("UF Origem", 31)]
        [DataField("PRP_C_UF_ORIGEM", SqlDbType.VarChar, 2)]
        public string UFOrigem { get; set; }

        [CsvField("Porto Destino", 32)]
        [DataField("PRP_C_PORTO_DESTINO", SqlDbType.VarChar, 150)]
        public string PortoDestino { get; set; }

        [CsvField("Municipio Destino", 33)]
        [DataField("PRP_C_MUNICIPIO_DESTINO", SqlDbType.VarChar, 150)]
        public string MunicipioDestino { get; set; }

        [CsvField("UF Destino", 34)]
        [DataField("PRP_C_UF_DESTINO", SqlDbType.VarChar, 2)]
        public string UFDestino { get; set; }

        [CsvField("Tipo Container", 35)]
        [DataField("PRP_C_TIPO_CONTAINER", SqlDbType.VarChar, 15)]
        public string TipoContainer { get; set; }

        [CsvField("Qtde Entregas", 36)]
        [DataField("PRP_N_QUANTIDADE_ENTREGAS", SqlDbType.Decimal, 18, 0)]
        public decimal? QuantidadeEntregas  { get; set; }

        [CsvField("Peso Estimado", 37)]
        [DataField("PRP_N_PESO_ESTIMADO", SqlDbType.Decimal, 16, 2)]
        public decimal? PesoEstimado  { get; set; }

        [CsvField("Frete Acordado", 38)]
        [DataField("PRP_N_FRETE_ACORDADO", SqlDbType.Decimal, 16, 2)]
        public decimal? FreteAcordado { get; set; }

        [CsvField("CM Acordado", 39)]
        [DataField("PRP_N_CM_ACORDADO", SqlDbType.Decimal, 16, 0)]
        public decimal? CMAcordado { get; set; }

        [CsvField("Percentual AdValorem", 40)]
        [DataField("PRP_N_PERCENTUAL_ADVALOR", SqlDbType.Decimal, 16, 2)]
        public decimal? PercentualAdValor  { get; set; }

        [CsvField("Trem Balsa Origem", 41)]
        [DataField("PRP_C_TREM_BALSA_ORIGEM", SqlDbType.VarChar, 1)]
        public string TremBalsaOrigem { get; set; }

        [CsvField("Trem Balsa Destino", 42)]
        [DataField("PRP_C_TREM_BALSA_DESTINO", SqlDbType.VarChar, 1)]
        public string TremBalsaDestino { get; set; }

        [CsvField("Recebe ICT", 43)]
        [DataField("PRP_C_RECEBE_ICT", SqlDbType.VarChar, 1)]
        public string RecebeICT { get; set; }

        [CsvField("Entrega ICT", 44)]
        [DataField("PRP_C_ENTREGA_ICT", SqlDbType.VarChar, 1)]
        public string EntregaICT { get; set; }

        [CsvField("Sinal ajuste origem", 45)]
        [DataField("PRP_C_SINAL_AJUSTE_ORIGEM", SqlDbType.VarChar, 1)]
        public string SinalAjusteOrigem { get; set; }

        [CsvField("Valor ajuste origem", 46)]
        [DataField("PRP_N_VALOR_AJUSTE_ORIGEM", SqlDbType.Decimal, 16, 2)]
        public decimal? ValorAjusteOrigem { get; set; }

        [CsvField("Sinal ajuste destino", 47)]
        [DataField("PRP_C_SINAL_AJUSTE_DESTINO", SqlDbType.VarChar, 1)]
        public string SinalAjusteDesitno { get; set; }

        [CsvField("Valor ajuste destino", 48)]
        [DataField("PRP_N_VALOR_AJUSTE_DESTINO", SqlDbType.Decimal, 16, 2)]
        public decimal? ValorAjusteDestino { get; set; }

        [CsvField("Requerimento", 49)]
        [DataField("PRP_C_REQUERIMENTO", SqlDbType.VarChar, 50)]
        public string Requerimento { get; set; }

        [CsvField("Descrição", 50)]
        [DataField("PRP_C_DESCRICAO", SqlDbType.VarChar, -1)]
        public string Descricao { get; set; }

        [CsvField("Data Validade", 51)]
        [DataField("PRP_D_VALIDADE", SqlDbType.DateTime)]
        public DateTime? Validade  { get; set; }

        [CsvField("Data Fechamento", 52)]
        [DataField("PRP_D_FECHAMENTO", SqlDbType.DateTime)]
        public DateTime? Fechamento  { get; set; }

        [CsvField("Data Inclusão", 53)]
        [DataField("PRP_D_INCLUSAO", SqlDbType.DateTime)]
        public DateTime? Inclusao { get; set; }

        [CsvField("Status", 54)]
        [DataField("PRP_C_STATUS", SqlDbType.VarChar, 50)]
        public string Status { get; set; }

        [CsvField("Portal", 55)]
        [DataField("PRP_C_PORTAL", SqlDbType.VarChar, 1)]
        public string PortalProposta { get; set; }

        [CsvField("Data validade cotação", 56)]
        [DataField("PRP_D_VALIDADE_COTACAO", SqlDbType.DateTime)]
        public DateTime? ValidadeCotacao { get; set; }

        [CsvField("Data cotação aceita", 57)]
        [DataField("PRP_D_COTACAO_ACEITA", SqlDbType.DateTime)]
        public DateTime? CotacaoAceita { get; set; }

        [CsvField("Data cotação negada", 58)]
        [DataField("PRP_D_COTACAO_NEGADA", SqlDbType.DateTime)]
        public DateTime? CotacaoNegada { get; set; }

        [CsvField("Data cotação rejeitada", 59)]
        [DataField("PRP_D_COTACAO_REJEITADA", SqlDbType.DateTime)]
        public DateTime? CotacaoRejeitada { get; set; }

        [CsvField("Serviço Especial Executível", 60)]
        [DataField("PRP_C_SERVICO_ESPECIAL_EXECUTIVEL", SqlDbType.VarChar, 200)]
        public string ServicoEspecialExecutivel { get; set; }

        [CsvField("Validade Serviço Especial Executível", 61)]
        [DataField("PRP_C_VALIDADE_SERVICO_ESPECIAL_EXECUTIVEL", SqlDbType.VarChar, 200)]
        public string ValidadeServicoEspecialExecutivel { get; set; }

        [CsvField("Serviço Especial não executível", 62)]
        [DataField("PRP_C_SERVICO_ESPECIAL_NAO_EXECUTIVE", SqlDbType.VarChar, 200)]
        public string ServicoEspecialNaoExecutivel { get; set; }

        [CsvField("Validade Serviço Especial não executível", 63)]
        [DataField("PRP_C_VALIDADE_SERVICO_ESPECIAL_NAO_EXECUTIVEL", SqlDbType.VarChar, 200)]
        public string ValidadeServicoEspecialNaoExecutivel { get; set; }

        [CsvField("Empresa", 64)]
        [DataField("PRP_C_EMPRESA", SqlDbType.VarChar, 150)]
        public string Empresa { get; set; }

        [CsvField("Centro de Custo", 65)]
        [DataField("PRP_C_CENTRO_CUSTO", SqlDbType.VarChar, 150)]
        public string CentroCusto { get; set; }

        [JoinField("PRP_USU_N_CODIGO", "FWK_USU_USUARIO", "USU_N_CODIGO", "USU_C_NOME")]
        [DataField("USU_C_NOME", SqlDbType.VarChar, 100)]
        public string NomeUsuario { get; set; }
    }

    [Table("MOD_TKP_TAKE_OR_PAY_PROGRAMACAO_PROPOSTA")]
    [Serializable]
    public class MLProgramacaoPropostaDataTable
    {
        [Required]
        [DataField("PRP_N_CODIGO", SqlDbType.Decimal, 18, 0, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }
              
        [DataField("PRP_USU_N_CODIGO", SqlDbType.Decimal, 18, 0)]
        public decimal? CodigoUsuario { get; set; }
                
        [DataField("PRP_D_DATA_IMPORTACAO", SqlDbType.DateTime)]
        public DateTime? DataImportacao { get; set; }

        [DataField("PRP_N_PROPOSTA", SqlDbType.Decimal, 18, 0)]
        public decimal? NumeroProposta { get; set; }

        [DataField("PRP_C_CLIENTE", SqlDbType.VarChar, 250)]
        public string Cliente { get; set; }

        [DataField("PRP_C_CNPJ", SqlDbType.VarChar, 20)]
        public string CNPJ { get; set; }

        [DataField("USU_C_NOME", SqlDbType.VarChar, 100)]
        public string NomeUsuario { get; set; }

        [DataField("sql_total", SqlDbType.Int)]
        public int? Total { get; set; }
    }
}
