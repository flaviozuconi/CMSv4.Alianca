using CMSApp.Helpers;
using Framework.Utilities;

namespace System.Web.Mvc
{
    public class JsonHandleError : HandleErrorTemplateAttribute
    {
        public override void GetResult(ExceptionContext errorContext)
        {
            //Retornar no result um json no padrão para datatable
            errorContext.Result = new JsonResult()
            {
                Data = new
                {
                    Sucesso = false,
                    Mensagem = new BLTraducao().ObterAdm("Não foi possível concluir sua solicitação, verifique o log de erro!")
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}