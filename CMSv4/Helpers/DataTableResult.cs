using CMSv4.BusinessLayer;
using Framework.Utilities;
using System.Collections.Specialized;
using System.Web.Mvc;

public class DataTableResult : JsonResult
{
    public int TotalRows { private get; set; }

    public DataTableResult()
    {
        JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        MaxJsonLength = 50000000;
    }

    public override void ExecuteResult(ControllerContext context)
    {
        object dataBackup = Data;

        Data = new
        {
            recordsTotal = TotalRows,
            recordsFiltered = TotalRows,
            data = dataBackup
        };

        base.ExecuteResult(context);
    }
}

public class DataTable
{
    public static JsonResult Listar<TipoModel>(TipoModel criterios, NameValueCollection request, string connectionString = "")
    {
        if (string.IsNullOrEmpty(connectionString))
            connectionString = ApplicationSettings.ConnectionStrings.Default;

        double total;
        var lista = new BLCRUD<TipoModel>().Listar(criterios, request, out total, connectionString);

        // Retorna os resultados
        return new JsonResult()
        {
            Data = new
            {
                recordsTotal = total,
                recordsFiltered = total,
                data = lista
            },
            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
            MaxJsonLength = 50000000
        };
    }
}

