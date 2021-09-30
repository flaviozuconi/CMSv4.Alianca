using System;
using System.Data;
using Framework.Model;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CMSv4.Model
{
    /// <summary>
    /// ENQUETE
    /// </summary>
    [Serializable]
    [Table("MOD_ENQ_ENQUETE")]
    [Auditing("/cms/enqueteadmin", "CodigoPortal")]
    public class MLEnquete
    {
        [DataField("POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("ENQ_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required]
        [DataField("ENQ_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }
                
        [DataField("ENQ_D_INICIO", SqlDbType.DateTime)]
        public DateTime? DataInicio { get; set; }
                
        [DataField("ENQ_D_TERMINO", SqlDbType.DateTime)]
        public DateTime? DataTermino { get; set; }

        [DataField("ENQ_B_ATIVO", SqlDbType.Bit)]
        public bool? Ativo { get; set; }


        public bool IsFechada
        {
            get
            {
                if (DataInicio.HasValue && DataTermino.HasValue)
                    return !(DateTime.Today >= DataInicio.Value && DateTime.Today <= DataTermino.Value.AddDays(1));
                else if (DataInicio.HasValue)                
                    return !(DateTime.Today >= DataInicio.Value);                        
                else if(DataTermino.HasValue)
                    return !(DateTime.Today <= DataTermino.Value.AddDays(1));

                return false;
            }
        }
    }

    public class MLEnqueteResultado
    {
        public MLEnqueteResultado()
        {
            Resultados = new List<MLEnqueteOpcaoResultado>();
        }

        [DataField("ENQ_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? CodigoEnquete { get; set; }

        [DataField("ENQ_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        public decimal? Total { get; set; }

        [DataField("ENQ_D_INICIO", SqlDbType.DateTime)]
        public DateTime? DataInicio { get; set; }

        [DataField("ENQ_D_TERMINO", SqlDbType.DateTime)]
        public DateTime? DataTermino { get; set; }

        public bool IsFechada
        {
            get
            {
                if (DataInicio.HasValue && DataTermino.HasValue)
                    return !(DateTime.Today >= DataInicio.Value && DateTime.Today <= DataTermino.Value.AddDays(1));
                else if (DataInicio.HasValue)
                    return !(DateTime.Today >= DataInicio.Value);
                else if (DataTermino.HasValue)
                    return !(DateTime.Today <= DataTermino.Value.AddDays(1));

                return false;
            }
        }

        public int Repositorio { get; set; }
        public decimal CodigoPagina { get; set; }

        public bool? VotarRestrito { get; set; }
        public bool? ResultadoRestrito { get; set; }

        public bool? IsVotou { get; set; }

        public List<MLEnqueteOpcaoResultado> Resultados { get; set; }

    }

    public class MLEnquetePublico
    {
        [DataField("ENQ_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("ENQ_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("ROWNUMBER", SqlDbType.BigInt)]
        public Int64? RowNumber { get; set; }

        public List<MLEnqueteOpcaoResultado> Resultados { get; set; }
    }
}