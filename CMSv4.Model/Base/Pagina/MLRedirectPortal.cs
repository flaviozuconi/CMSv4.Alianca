using Framework.Model;
using System;
using System.Data;

namespace CMSv4.Model
{
    [Serializable]
    [Table("CMS_RED_REDIRECT")]
    public class MLRedirect
    {      
        [DataField("RED_C_URL_DE", SqlDbType.VarChar,200)]
        public string UrlDe { get; set; }

        [DataField("RED_C_URL_PARA", SqlDbType.VarChar, 200)]
        public string UrlPara { get; set; }

        [DataField("RED_N_OPCAO", SqlDbType.Int)]
        public int? Opcao { get; set; }

        [DataField("RED_C_TIPO",SqlDbType.VarChar,50)]
        public string Tipo { get; set; }
    }
}
