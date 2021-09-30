using Framework.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Framework.Utilities
{
    [Serializable]
    [Table("ADM_POI_PORTAL_INSTALACAO")]
    public class MLPortalInstalacao
    {
        public enum ETAPAS
        {
            CRIARBD,
            CRIARESTRUTURA,
            //GRUPOSDEFAULT,
            MENUSDEFAULT,
            CONTEUDO
        }
                
        [DataField("POI_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("POI_POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("POI_N_ETAPA", SqlDbType.Int)]
        public int? Etapa { get; set; }

        [DataField("POI_B_STATUS", SqlDbType.Bit)]
        public bool? Status { get; set; }

        [DataField("POI_C_MENSAGEM", SqlDbType.VarChar, -1)]
        public string Mensagem { get; set; }

        [DataField("POI_D_TERMINO", SqlDbType.DateTime)]
        public DateTime? DataTermino { get; set; }
    }

    public class MLPortalFile
    {
        [DataField("filename", SqlDbType.VarChar)]
        public string FileName { get; set; }

        [DataField("filegroup", SqlDbType.VarChar)]
        public string FileGroup { get; set; }

    }
}
