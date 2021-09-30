using CMSApp.Helpers;

namespace System.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CustomViewHandleErrorAttribute : HandleErrorTemplateAttribute
    {
        private readonly string _DefaultCustomView = "~/Areas/ModuloAdmin/Views/Shared/_ErrorMessage.cshtml";

        /// <summary>
        /// Por padrão a view de retorno é "Error", se não for passado a view, redefinir a view de erro.
        /// </summary>
        public CustomViewHandleErrorAttribute() : base()
        {
            this.View = _DefaultCustomView;
        }

        public override void GetResult(ExceptionContext errorContext)
        {
            //Retornar no result um json no padrão para datatable
            errorContext.Result = new ViewResult()
            {
                ViewName = this.View
            };
        }
    }
}