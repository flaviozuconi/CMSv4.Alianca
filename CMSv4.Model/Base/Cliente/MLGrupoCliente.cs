using Framework.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Runtime.Serialization;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade Grupo de Cliente
    /// </summary>
    [DataContract]
    [Serializable]
    [Table("CMS_GCL_GRUPO_CLIENTE")]
    [Auditing("/cms/grupocliente","CodigoPortal")]
    public class MLGrupoCliente
    {
        [DataMember]
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataMember]
        [DataField("GCL_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataMember]
        [Required, StringLength(50)]
        [DataField("GCL_C_NOME", SqlDbType.VarChar, 50)]
        public string Nome { get; set; }

        [DataField("GCL_B_STATUS", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        /// <summary>
        /// Define se os clientes que realizarem o cadastro na área pública farão parte desse grupo automaticamente.
        /// </summary>
        [DataField("GCL_B_DEFAULT_CADASTRO_PUBLICO", SqlDbType.Bit)]
        public bool? DefaultCadastroPublico { get; set; }
    }
}
