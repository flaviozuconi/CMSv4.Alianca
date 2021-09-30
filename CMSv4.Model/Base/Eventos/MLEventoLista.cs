using System;
using System.Data;
using Framework.Model;
using System.ComponentModel.DataAnnotations;
using Framework.Utilities;

namespace CMSv4.Model
{
    /// <summary>
    /// Evento
    /// </summary>
    [Serializable]
    [Table("MOD_EVE_EVENTOS")]
    public class MLEventoLista
    {
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("EVE_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required]
        [DataField("EVE_C_URL", SqlDbType.VarChar, 100)]
        public string Url { get; set; }

        [Required]
        [DataField("EVE_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("EVE_C_CHAMADA", SqlDbType.VarChar, -1)]
        public string Chamada { get; set; }

        [DataField("EVE_C_CONTEUDO", SqlDbType.VarChar, -1)]
        public string Conteudo { get; set; }

        [Required]
        [DataField("EVE_D_INICIO", SqlDbType.DateTime)]
        public DateTime? DataInicio { get; set; }

        [DataField("EVE_D_TERMINO", SqlDbType.DateTime)]
        public DateTime? DataTermino { get; set; }

        [DataField("EVE_C_IMAGEM", SqlDbType.VarChar, 250)]
        public string Imagem { get; set; }

        [DataField("EVE_C_TAGS", SqlDbType.VarChar, -1)]
        public string Tags { get; set; }

        [DataField("EVE_C_LOCAL", SqlDbType.VarChar, 100)]
        public string Local { get; set; }

        [DataField("EVE_B_INSCRICAO", SqlDbType.Bit)]
        public bool? IsInscricao { get; set; }

        [DataField("EVE_D_INICIO_INSCRICAO", SqlDbType.DateTime)]
        public DateTime? DataInicioInscricao { get; set; }

        [DataField("EVE_D_TERMINO_INSCRICAO", SqlDbType.DateTime)]
        public DateTime? DataTerminoInscricao { get; set; }

        [DataField("EVE_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }


        [DataField("ROWNUMBER", SqlDbType.BigInt)]
        public Int64? RowNumber { get; set; }

        [DataField("TOTAL_ROWS", SqlDbType.Int)]
        public Int32? TotalRows { get; set; }


        public DateTime? DataAtual
        {
            get
            {
                if (DataInicio < DateTime.Today)
                    return DateTime.Today;

                return DataInicio.Value;
            }
        }

        public string DataString
        {
            get
            {
                if (DataInicio == DataTermino)
                    return DataInicio.Value.ToString(BLTraducao.T("dd/MM/yyyy"));

                return DataInicio.Value.ToString(BLTraducao.T("dd/MM/yyyy")) + " - " + DataTermino.Value.ToString(BLTraducao.T("dd/MM/yyyy"));

            }
        }

    }
}
