namespace CMSv4.BusinessLayer.Pagina
{
    public class PaginaPublicarHelper
    {
        public const string Head = @"
            @section head {
                @CMSv4.BusinessLayer.BLConteudo.GetTextForHead()
                @Html.Raw(CMSv4.BusinessLayer.BLConteudo.GetFreeTextForHead())
#STYLE_PAGINA#
            }
            @section scripts {
                @CMSv4.BusinessLayer.BLConteudo.GetTextForScriptsView()
                @CMSv4.BusinessLayer.BLConteudo.GetTextForScripts()
#SCRIPT_PAGINA#
            }
        ";

        public const string PastaConteudo = "~/portal/{0}/paginas/";
    }
}
