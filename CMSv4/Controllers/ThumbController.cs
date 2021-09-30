using CMSv4.BusinessLayer;
using System.Web.Mvc;

namespace CMSApp.Controllers
{
    public class ThumbController : Controller
    {
        /// <summary>
        /// Cria um thumb da imagem informada, usando a largura desejada, mas mantendo as proporçoes do arquivo
        /// </summary>
        /// <param name="diretorioPortal">Nome do diretório do portal7</param>
        /// <param name="modulo">Nome do Módulo</param>
        /// <param name="codigoRegistro">Código do Registro</param>
        /// <param name="width">Largura</param>
        /// <param name="heigth">Altura</param>
        /// <param name="imagem">Nome da Imagem com Extensão</param>
        public ActionResult Index(string diretorioPortal, string modulo, string codigoRegistro, string width, string heigth, string imagem, bool? crop)
        {
            var thumb = new Thumb(diretorioPortal, modulo, codigoRegistro, width, heigth, imagem, crop);

            var result = thumb.Generate();

            return File(result.File, result.ContentType);
        }

    }
}
