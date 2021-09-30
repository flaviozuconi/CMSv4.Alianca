using CMSv4.Model;
using Framework.Utilities;
using System;
using System.IO;
using System.Transactions;
using System.Web;

namespace CMSv4.BusinessLayer.Pagina
{
    public class BLPaginaPublicar
    {
        private string _PastaFisica { get; set; }
        private string _PastaConteudo { get; set; }
        private string _ConnectionString { get; set; }
        private string _PortalDiretorio { get; set; }

        private MLPaginaPublicada _ModelPaginaPublicada { get; set; }
        private MLPaginaAdmin _ModelPaginaAdmin { get; set; }

        private string _Template { get; set; }
        private Guid? _CodigoHistorico { get; set; }

        public BLPaginaPublicar(string connectionString, string portalDiretorio)
        {
            _ConnectionString = connectionString;
            _PortalDiretorio = portalDiretorio;

            BindDirectories();
        }

        private void BindDirectories()
        {
            _PastaConteudo = string.Format(PaginaPublicarHelper.PastaConteudo, _PortalDiretorio);
            _PastaFisica = HttpContext.Current.Server.MapPath(_PastaConteudo);

            if (!Directory.Exists(_PastaFisica))
                Directory.CreateDirectory(_PastaFisica);
        }

        public MLPaginaAdmin Start(decimal id)
        {
            try
            {
                _ModelPaginaAdmin = new BLPagina(_ConnectionString).ObterPaginaAdmin(id);

                MLPaginaPublicada model;

                if (_ModelPaginaAdmin.PaginaEdicao.Codigo.HasValue)
                {
                    CopyModelFromEditionToPublished();

                    CopyModulesFromEditionToPublished();

                    _ModelPaginaAdmin.PaginaEdicao = null;
                    _ModelPaginaAdmin.PaginaPublicada = _ModelPaginaPublicada;
                }
                else if (_ModelPaginaAdmin.PaginaPublicada.Codigo.HasValue)
                {
                    model = _ModelPaginaAdmin.PaginaPublicada;
                }
                else
                {
                    return null;
                }

                Publish();

                return _ModelPaginaAdmin;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        private void CopyModelFromEditionToPublished()
        {
            _ModelPaginaPublicada = new MLPaginaPublicada()
            {
                ApresentarNaBusca = _ModelPaginaAdmin.PaginaEdicao.ApresentarNaBusca,
                Codigo  = _ModelPaginaAdmin.PaginaEdicao.Codigo,
                DataEdicao = DateTime.Now,
                Descricao = _ModelPaginaAdmin.PaginaEdicao.Descricao,
                Tags = _ModelPaginaAdmin.PaginaEdicao.Tags,
                TemplateCustomizado = _ModelPaginaAdmin.PaginaEdicao.TemplateCustomizado,
                NomeLayout = _ModelPaginaAdmin.PaginaEdicao.NomeLayout,
                NomeTemplate = _ModelPaginaAdmin.PaginaEdicao.NomeTemplate,
                Scripts = _ModelPaginaAdmin.PaginaEdicao.Scripts,
                Css = _ModelPaginaAdmin.PaginaEdicao.Css,
                Titulo = _ModelPaginaAdmin.PaginaEdicao.Titulo,
                UsuarioEditor = BLUsuario.ObterLogado().Codigo,
                UrlLogin = _ModelPaginaAdmin.PaginaEdicao.UrlLogin ?? ""
            };
        }

        private void CopyModulesFromEditionToPublished()
        {
            foreach (var item in _ModelPaginaAdmin.PaginaEdicao.Modulos)
            {
                _ModelPaginaPublicada.Modulos.Add(new MLPaginaModuloPublicado()
                {
                    CodigoModulo = item.CodigoModulo,
                    CodigoPagina = item.CodigoPagina,
                    Repositorio = item.Repositorio,
                    UrlModulo = item.UrlModulo
                });
            }
        }

        private void Publish()
        {
            try
            {
                GetTemplateContent();

                InsertCustomCssOnThePage();

                InsertCustonJsOnThePage();

                SavePageInDataBase();

                ReplaceTemplateRepositoriesWithModules();

                DeleteAlreadySavedModules();

                RemoveEmptyRepositories();

                WritePublishedPageOnDisk();

                WriteOrDeleteCustomCssFromDisk();

                WriteOrDeleteCustomJsFromDisk();
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        private void GetTemplateContent()
        {
            if (string.IsNullOrEmpty(_ModelPaginaPublicada.TemplateCustomizado))
            {
                if (!string.IsNullOrEmpty(_ModelPaginaPublicada.NomeTemplate))
                    _Template = BLTemplate.CarregarArquivo(BLPortal.Atual, _ModelPaginaPublicada.NomeTemplate);
                else
                    _Template = "";
            }
            else
                _Template = _ModelPaginaPublicada.TemplateCustomizado;

            _Template = string.Concat(_Template, Environment.NewLine, PaginaPublicarHelper.Head);
        }

        private void InsertCustomCssOnThePage()
        {
            var style = $"<link rel=\"stylesheet\" href=\"{_PastaConteudo}/custom_style_{_ModelPaginaPublicada.Codigo}.css\">";

            if (string.IsNullOrWhiteSpace(_ModelPaginaPublicada.Css))
                style = string.Empty;

            ReplaceTagWithContent("#STYLE_PAGINA#", style);
        }

        private void InsertCustonJsOnThePage()
        {
            var script = $"<script type=\"text/javascript\" src=\"{_PastaConteudo}/custom_script_{_ModelPaginaPublicada.Codigo}.js\"></script>";

            if (string.IsNullOrWhiteSpace(_ModelPaginaPublicada.Scripts))
                script = string.Empty;

            ReplaceTagWithContent("#SCRIPT_PAGINA#", script);
        }

        private void ReplaceTagWithContent(string tagToBeReplaced, string contentToInsert)
        {
            if (!string.IsNullOrEmpty(contentToInsert))
                _Template = _Template.Replace(tagToBeReplaced, contentToInsert.Unescape());
            else
                _Template = _Template.Replace(tagToBeReplaced, string.Empty);
        }

        private void ReplaceTemplateRepositoriesWithModules()
        {
            var modulos = BLModulo.Listar(_ConnectionString);

            CRUD.Excluir(new MLPaginaModuloPublicado() { CodigoPagina = _ModelPaginaPublicada.Codigo.Value });

            foreach (var item in _ModelPaginaPublicada.Modulos)
            {
                CRUD.Salvar(item);

                System.Reflection.Assembly assembly;
                try
                {
                    var moduloEncontrado = modulos.Find(o => o.Codigo == item.CodigoModulo);
                    if (moduloEncontrado == null
                        || string.IsNullOrEmpty(moduloEncontrado.NomeAssembly)
                        || string.IsNullOrEmpty(moduloEncontrado.NomeBusinesLayer)) continue;

                    assembly = System.Reflection.Assembly.Load(moduloEncontrado.NomeAssembly);
                    var moduloType = assembly.GetType(moduloEncontrado.NomeBusinesLayer);

                    var method = moduloType.GetMethod("Publicar");

                    _Template = _Template.Replace("{R" + item.Repositorio.Value.ToString("00") + "}",
                        Convert.ToString(method.Invoke(null, new object[] { item.UrlModulo, item.CodigoPagina, item.Repositorio, _CodigoHistorico })));
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                }
                finally
                {
                    assembly = null;
                }
            }
        }

        private void SavePageInDataBase()
        {
            using (var scope = new TransactionScope(_ConnectionString))
            {
                var objBLPagina = new BLPagina(_ConnectionString);

                _CodigoHistorico = objBLPagina.GerarHistorico(_ModelPaginaPublicada.Codigo.Value);

                CRUD.Salvar(_ModelPaginaPublicada);

                scope.Complete();
            }
        }

        private void DeleteAlreadySavedModules()
        {
            CRUD.Excluir(new MLPaginaModuloEdicao() { CodigoPagina = _ModelPaginaPublicada.Codigo.Value });

            CRUD.Excluir<MLPaginaEdicao>(_ModelPaginaPublicada.Codigo.Value, _ConnectionString);
        }

        private void RemoveEmptyRepositories()
        {
            for (var i = 1; i <= 99; i++)
            {
                var rep = "{R" + i.ToString("00") + "}";
                _Template = _Template.Replace(rep, "");
            }
        }

        private void WritePublishedPageOnDisk()
        {
            var fileNameWithExtension = string.Format("p{0}", _ModelPaginaPublicada.Codigo) + ".cshtml";

            WriteFileOnDisk(fileNameWithExtension, _Template);
        }

        private void WriteOrDeleteCustomCssFromDisk()
        {
            var fileNameWithExtension = $"custom_style_{_ModelPaginaPublicada.Codigo}.css";
            var hasCustomStyle = !string.IsNullOrWhiteSpace(_ModelPaginaPublicada.Css);

            if(hasCustomStyle)
            {
                WriteFileOnDisk(fileNameWithExtension, _ModelPaginaPublicada.Css);
            }
            else
            {
                DeleteFileFromDisk(fileNameWithExtension);
            }
        }

        private void WriteOrDeleteCustomJsFromDisk()
        {
            var fileNameWithExtension = $"custom_script_{_ModelPaginaPublicada.Codigo}.js";
            var hasCustomScript = !string.IsNullOrWhiteSpace(_ModelPaginaPublicada.Scripts);

            if (hasCustomScript)
            {
                WriteFileOnDisk(fileNameWithExtension, _ModelPaginaPublicada.Scripts);
            }
            else
            {
                DeleteFileFromDisk(fileNameWithExtension);
            }
        }

        private void WriteFileOnDisk(string fileNameWithExtension, string fileContent)
        {
            var fileWithFullPath = Path.Combine(_PastaFisica, fileNameWithExtension);

            File.WriteAllText(fileWithFullPath, fileContent.Unescape());
            BLReplicar.Arquivo(fileWithFullPath);
        }

        private void DeleteFileFromDisk(string fileNameWithExtension)
        {
            var fileWithFullPath = Path.Combine(_PastaFisica, fileNameWithExtension);

            try
            {
                var fileDoesNotExist = !File.Exists(fileWithFullPath);

                if (fileDoesNotExist)
                    return;

                File.Delete(fileWithFullPath);
                BLReplicar.ExcluirArquivosReplicados(fileWithFullPath);
            }
            catch {}
        }
    }
}
