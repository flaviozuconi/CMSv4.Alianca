using CMSApp.Helpers;
using Framework.Utilities;

namespace System.Web.Mvc
{
    public class DataTableHandleErrorAttribute : HandleErrorTemplateAttribute
    {
        public override void GetResult(ExceptionContext errorContext)
        {
            //Retornar no result um json no padrão para datatable
            errorContext.Result = new JsonResult()
            {
                Data = new
                {
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    ExpirouSessao = true,
                    Mensagem = new BLTraducao().ObterAdm("Não foi possível listar as informações, verifique o log de erro."),
                    data = new System.Collections.Generic.List<string>()
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}