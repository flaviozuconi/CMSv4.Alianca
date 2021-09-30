using Framework.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace CMSv4.Model
{
    [Serializable]
    [Table("MOD_PRG_PAIS_REGIAO")]
    [Auditing("/cms/paisregiaoadmin", "CodigoPortal")]
    public class MLPaisRegiao
    {
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("PRG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("PRG_C_DESCRICAO", SqlDbType.VarChar, 300)]
        public string Descricao { get; set; }

        [DataField("PRG_REG_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoRegiao { get; set; }

        [DataField("PRG_C_IMAGEM", SqlDbType.VarChar, 5)]
        public string Imagem { get; set; }

        [JoinField("PRG_REG_N_CODIGO", "MOD_BRK_PRO_REGIAO", "REG_N_CODIGO", "REG_C_DESCRICAO")]
        [DataField("REG_C_DESCRICAO", SqlDbType.VarChar, 50)]
        public string NomeRegiao { get; set; }

        [DataField("PRG_B_ATIVO", SqlDbType.Bit)]
        public bool? IsAtivo { get; set; }

        public bool IsImagemDeletar { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MLPaisRegiaoContato
    {

        public MLPaisRegiaoContato()
        {
            PaisRegiao = new List<MLPaisRegiao>();
            SubRegiao = new List<MLSubRegiao>();
            Telefone = new List<MLTelefone>();
        }

        public List<MLPaisRegiao> PaisRegiao { get; set; }
        public List<MLSubRegiao> SubRegiao { get; set; }
        public List<MLTelefone> Telefone { get; set; }
    }
    
}
