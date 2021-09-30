using System;
using System.Data;
using Framework.Model;
using System.ComponentModel.DataAnnotations;

namespace CMSv4.Model
{
    /// <summary>
    /// Evento
    /// </summary>
    [Serializable]
    [Table("MOD_EVE_EVENTO_PARTICIPANTE")]
    public class MLEventoParticipante{

        [DataField("EVP_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required]
        [DataField("EVP_EVE_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoEvento { get; set; }

        [Required]
        [DataField("EVP_C_NOME", SqlDbType.VarChar, 200)]
        public string Nome { get; set; }

        [Required]
        [DataField("EVP_C_EMAIL", SqlDbType.VarChar, 100)]
        public string Email { get; set; }

        [DataField("EVP_C_TELEFONE", SqlDbType.VarChar, 50)]
        public string Telefone { get; set; }

        [DataField("EVP_C_IP", SqlDbType.VarChar, 20)]
        public string IP { get; set; }

        [Required]
        [DataField("EVP_D_CADASTRO", SqlDbType.DateTime)]
        public DateTime? DataCadastro { get; set; }

        [DataField("EVP_N_STATUS", SqlDbType.Int)]
        public int? Status { get; set; }

        public bool IsPresente
        {
            get
            {
                if (Status.HasValue && Status == 1)
                    return true;

                return false;
            }
        }

    }
}