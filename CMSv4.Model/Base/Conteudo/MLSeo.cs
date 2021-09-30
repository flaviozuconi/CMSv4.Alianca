using Framework.Model;
using System;
using System.Data;

namespace CMSv4.Model
{
    [Serializable]
    public class MLConteudoSeo
    {
        [DataField("COS_C_SCHEMA", SqlDbType.VarChar, 400)]
        public string Schema { get; set; }

        [DataField("COS_C_IMAGE", SqlDbType.VarChar, 200)]
        public string Image { get; set; }

        [DataField("COS_C_COPYRIGHT", SqlDbType.VarChar, 200)]
        public string Copyright { get; set; }

        [DataField("COS_C_REVISITAFTER", SqlDbType.VarChar, 50)]
        public string Revisitafter { get; set; }

        [DataField("COS_C_AUTHOR", SqlDbType.VarChar, 100)]
        public string Author { get; set; }

        [DataField("COS_C_TWITTERCARD", SqlDbType.VarChar, 500)]
        public string Twittercard { get; set; }

        [DataField("COS_C_TWITTERSITE", SqlDbType.VarChar, 50)]
        public string Twittersite { get; set; }

        [DataField("COS_C_TWITTERTITLE", SqlDbType.VarChar, 200)]
        public string Twittertitle { get; set; }

        [DataField("COS_C_TWITTERDESCRIPTION", SqlDbType.VarChar, 500)]
        public string Twitterdescription { get; set; }

        [DataField("COS_C_TWITTERIMAGE", SqlDbType.VarChar, 200)]
        public string Twitterimage { get; set; }
             
        [DataField("COS_C_OGTOTITLE", SqlDbType.VarChar, 200)]
        public string Ogtotitle { get; set; }

        [DataField("COS_C_OGSITENAME", SqlDbType.VarChar, 100)]
        public string Ogsitename { get; set; }
            
        [DataField("COS_C_OGDESCRIPTION", SqlDbType.VarChar, 400)]
        public string Ogdescription { get; set; }

        [DataField("COS_C_OGIMAGE", SqlDbType.VarChar, 200)]
        public string Ogimage { get; set; }

        [DataField("COS_C_OGTYPE", SqlDbType.VarChar, 50)]
        public string Ogtype { get; set; }

        [DataField("COS_C_OGLOCALE", SqlDbType.VarChar, 50)]
        public string Oglocale { get; set; }

        [DataField("COS_OGAUTHOR", SqlDbType.VarChar, 100)]
        public string OgAuthor { get; set; }

        [DataField("COS_C_OGPUBLISHER", SqlDbType.VarChar, 100)]
        public string Ogpublisher { get; set; }

        [DataField("COS_C_SHORTCUTICON", SqlDbType.VarChar, 200)]
        public string Shortcuticon { get; set; }

        [DataField("COS_C_APPLETOUCHICON", SqlDbType.VarChar, 200)]
        public string Appletouchicon { get; set; }

        [DataField("COS_C_OUTROS", SqlDbType.VarChar, -1)]
        public string Outros { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }

        public string Tags { get; set; }

        public string Titulo { get; set; }

        public void SetPadroes(){
            Ogtotitle = Ogtotitle ?? Titulo;
            Ogdescription = Ogdescription ?? Description;
            Twittertitle = Twittertitle ?? Titulo;
            Twitterdescription = Twitterdescription ?? Description;
        }

    }
}
