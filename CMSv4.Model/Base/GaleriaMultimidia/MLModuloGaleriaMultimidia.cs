using System;
using System.Data;
using Framework.Model;
using System.Collections.Generic;

namespace CMSv4.Model
{
    /// <summary> 
    /// Model da Entidade
    /// </summary>
    public class MLModuloGaleriaMultimidia
    {
        [DataField("PAG_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoPagina { get; set; }

        [DataField("REP_N_NUMERO", SqlDbType.Int, PrimaryKey = true)]
        public int? Repositorio { get; set; }

        [DataField("GAL_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoGaleria { get; set; }

        [DataField("GAL_C_VIEW", SqlDbType.VarChar, 100)]
        public string NomeView { get; set; }

        [DataField("GAL_C_TITULO", SqlDbType.VarChar, 250)]
        public string Titulo { get; set; }

        [DataField("GAL_B_DESTAQUES", SqlDbType.Bit)]
        public bool? Destaques { get; set; }

        [DataField("GAL_N_QUANTIDADE", SqlDbType.Int)]
        public int? Quantidade { get; set; }

        [DataField("GAL_C_URL_LISTA", SqlDbType.VarChar, 250)]
        public string UrlLista { get; set; }

        [DataField("GAL_C_URL_DETALHE", SqlDbType.VarChar, 250)]
        public string UrlDetalhe { get; set; }

        [DataField("GAL_USU_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("GAL_D_REGISTRO", SqlDbType.DateTime)]
        public DateTime? DataRegistro { get; set; }

        public List<MLGaleriaMultimidia> Galerias { get; set; }

        public List<MLGaleriaMultimidiaArquivo> ArquivosGaleria { get; set; }

    }

    [Table("MOD_GAL_GALERIA_LISTA_EDICAO")]
    public class MLModuloGaleriaMultimidiaEdicao : MLModuloGaleriaMultimidia { }

    [Table("MOD_GAL_GALERIA_LISTA_PUBLICADO")]
    public class MLModuloGaleriaMultimidiaPublicado : MLModuloGaleriaMultimidia { }

    [Table("MOD_GAL_GALERIA_LISTA_HISTORICO")]
    public class MLModuloGaleriaMultimidiaHistorico : MLModuloGaleriaMultimidia
    {
        [DataField("HIS_GUID", SqlDbType.UniqueIdentifier, PrimaryKey = true)]
        public Guid? CodigoHistorico { get; set; }
    }
}
