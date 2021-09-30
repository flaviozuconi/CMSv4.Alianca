using System;
using System.IO;
using System.Web.Mvc;
using Framework.Utilities;
using CMSv4.Model;

namespace CMSApp.Areas.CMS.Controllers
{
    public class UtilitariosController : SecureController
    {
        //
        // GET: /CMS/Utilitarios/
        [CheckPermission(global::Permissao.Publico)]
        public ActionResult Index(string qsUrlPastaOrigem, string qsPastaDestino, decimal qsCodigoLista)
        {

            ViewBag.Retono = String.Empty;

            #region Importar imagem lista            

            if (!String.IsNullOrWhiteSpace(qsUrlPastaOrigem) && !String.IsNullOrWhiteSpace(qsPastaDestino) && qsCodigoLista > 0)
            {
                try
                {
                    //Diretorio
                    qsUrlPastaOrigem = System.Web.HttpContext.Current.Server.MapPath(qsUrlPastaOrigem);
                    qsPastaDestino = System.Web.HttpContext.Current.Server.MapPath(qsPastaDestino);                   

                    //Buscar registro
                    var imagens = CRUD.Listar<MLListaConteudo>(new MLListaConteudo {CodigoLista = qsCodigoLista }).FindAll(o=>!String.IsNullOrWhiteSpace(o.Imagem));
                    
                    //Salvar os arquivos na nova pasta
                    foreach (var imagem in imagens)
                    {
                        string strArquivoOrigem = qsUrlPastaOrigem + "/" + imagem.CodigoReferencia + imagem.Imagem;

                        if (System.IO.File.Exists(strArquivoOrigem))
                        {
                            string strArquivoDestino = qsPastaDestino + "/" + imagem.Codigo;
                            var diDestino = new DirectoryInfo(strArquivoDestino);

                            if (!diDestino.Exists)
                                diDestino.Create();

                            System.IO.File.Move(strArquivoOrigem, strArquivoDestino + "/capa" + imagem.Imagem);
                        }
                    }

                    ViewBag.Retono = "Arquivos copiados com sucesso!";
                }
                catch (Exception ex)
                {
                    ViewBag.Retono = ex.Message;
                    throw;
                }


            }

            #endregion

            return View();
        }


    }
}

