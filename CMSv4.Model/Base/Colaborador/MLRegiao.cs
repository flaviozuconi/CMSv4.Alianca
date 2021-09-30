using System;
using System.Data;
using Framework.Model;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade Regiao 
    /// </summary> 
    [Table("MOD_BRK_PRO_REGIAO")]
    public class MLRegiao
    {
        [DataField("REG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("REG_N_CODIGO_BASE", SqlDbType.Decimal, 18)]
        public decimal? CodigoBase { get; set; }

        [DataField("REG_IDI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoIdioma { get; set; }

        [DataField("REG_POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("REG_C_DESCRICAO", SqlDbType.VarChar, 200)]
        public string Descricao { get; set; }

        [DataField("REG_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("LOG_D_CADASTRO", SqlDbType.DateTime)]
        public DateTime? DataCriacao { get; set; }

        [DataField("LOG_USU_N_CODIGO_CADASTRO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuarioCriacao { get; set; }

        [DataField("LOG_D_ALTERACAO", SqlDbType.DateTime)]
        public DateTime? DataModificacao { get; set; }

        [DataField("LOG_USU_N_CODIGO_ALTERACAO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuarioModificacao { get; set; }

        [DataField("IDIOMAS_DISPONIVEIS", SqlDbType.VarChar, -1, IgnoreEmpty = true)]
        [SubSelectField("IDIOMAS_DISPONIVEIS", @"SELECT CAST(IDI.IDI_N_CODIGO AS VARCHAR) + ':' + IDI.IDI_C_SIGLA + ':' + ISNULL((SELECT CAST(REG_N_CODIGO AS VARCHAR) FROM MOD_BRK_PRO_REGIAO 
			                        WHERE REG_IDI_N_CODIGO = IDI.IDI_N_CODIGO AND 
			                        (REG_N_CODIGO = tabela.REG_N_CODIGO OR REG_N_CODIGO_BASE = tabela.REG_N_CODIGO OR REG_N_CODIGO = tabela.REG_N_CODIGO_BASE OR REG_N_CODIGO_BASE = tabela.REG_N_CODIGO_BASE)), '') + ','
	                        FROM FWK_IDI_IDIOMA IDI
	                        WHERE IDI.IDI_B_ATIVO = 1
                            AND IDI.IDI_N_CODIGO <> tabela.REG_IDI_N_CODIGO
	                        FOR XML PATH('')")]
        public string IdiomasCadastrados { get; set; }
    }
}