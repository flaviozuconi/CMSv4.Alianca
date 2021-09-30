using System;
using System.Data;
using Framework.Model;
using System.ComponentModel.DataAnnotations;

namespace CMSv4.Model
{
    /// <summary>
    /// Módulo de Youtube
    /// </summary>
    [Serializable]
    [Table("MOD_YOU_YOUTUBE_EDICAO")]
    public class MLModuloYoutubeEdicao
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        //[DataField("POR_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        //public decimal? CodigoPortal { get; set; }

        //[DataField("YOU_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        //public decimal? Codigo { get; set; }

        [DataField("YOU_C_TITULO", SqlDbType.VarChar, 100)]
        public string Titulo { get; set; }

        [Required]
        [DataField("YOU_C_LINK", SqlDbType.VarChar, 300)]
        public string Link { get; set; }

        [DataField("YOU_N_ALTURA", SqlDbType.Decimal, 18)]
        public decimal? Altura { get; set; }

        [DataField("YOU_N_LARGURA", SqlDbType.Decimal, 18)]
        public decimal? Largura { get; set; }

    }

    [Table("MOD_YOU_YOUTUBE_PUBLICADO")]
    public class MLModuloYoutubePublicado : MLModuloYoutubeEdicao { }

    [Table("MOD_YOU_YOUTUBE_HISTORICO")]
    public class MLModuloYoutubeHistorico : MLModuloYoutubeEdicao
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }
    }
}
