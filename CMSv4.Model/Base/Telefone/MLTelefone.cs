using Framework.Model;
using System;
using System.Data;

namespace CMSv4.Model
{
    [Serializable]
    [Table("MOD_TEL_TELEFONES")]
    [Auditing("/cms/telefoneadmin","CodigoPortal")]
    public class MLTelefone
    {
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("TEL_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("TEL_C_NOME", SqlDbType.VarChar, 200)]
        public string Nome { get; set; }

        [DataField("TEL_C_CARGO", SqlDbType.VarChar, 200)]
        public string Cargo { get; set; }

        [DataField("TEL_C_FONE", SqlDbType.VarChar, 20)]
        public string Fone { get; set; }

        [DataField("TEL_C_RAMAL", SqlDbType.VarChar, 5)]
        public string Ramal { get; set; }

        public string Filtro { get; set; }

        [DataField("TEL_SRE_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoSubRegiao { get; set; }

        public string CaminhoImagem { get; set; }
    }

    public class MLTelefoneCompleto
    {

        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("TEL_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("TEL_C_NOME", SqlDbType.VarChar, 200)]
        public string Nome { get; set; }

        [DataField("TEL_C_CARGO", SqlDbType.VarChar, 200)]
        public string Cargo { get; set; }

        [DataField("TEL_C_FONE", SqlDbType.VarChar, 20)]
        public string Fone { get; set; }

        [DataField("TEL_C_RAMAL", SqlDbType.VarChar, 5)]
        public string Ramal { get; set; }

        [DataField("TEL_SRE_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoSubRegiao { get; set; }

         [DataField("REG_C_DESCRICAO", SqlDbType.VarChar, 200)]
        public string Regiao { get; set; }
    
        [DataField("PRG_C_DESCRICAO", SqlDbType.VarChar, 300)]
        public string PaisRegiao { get; set; }
    
        [DataField("SRE_C_DESCRICAO", SqlDbType.VarChar, 300)]
        public string SubRegiao { get; set; }
  
    }

}
