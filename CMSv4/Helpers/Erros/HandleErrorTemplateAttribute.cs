using Framework.Utilities;
using System.Web.Mvc;

namespace CMSApp.Helpers
{
    public abstract class HandleErrorTemplateAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext errorContext)
        {
            //Definir como true para evitar que a ação de tratamento padrão seja executada
            errorContext.ExceptionHandled = true;

            //Salvar log com a exceção da action
            ApplicationLog.ErrorLog(errorContext.Exception);

            GetResult(errorContext);
        }

        public abstract void GetResult(ExceptionContext errorContext);
    }
}