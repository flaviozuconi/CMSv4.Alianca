using Framework.Model;
using System;
using System.Data;
using VM2.PageSpeed.Model;

namespace CMSv4.Model
{
    [Table("CMS_PAG_PAGINA_PAGESPEED")]
    public class MLPaginaPageSpeed : MLPageSpeedViewModel
    {
        [DataField("PSP_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("PSP_PAG_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPagina { get; set; }

        [DataField("PSP_C_RESULT", SqlDbType.VarChar, -1)]
        public string JsonResult { get; set; }

        [DataField("PSP_D_ULTIMA_AVALIACAO", SqlDbType.DateTime)]
        public DateTime? DataUltimaAvaliacao { get; set; }
    }
}
