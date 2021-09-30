using CMSv4.BusinessLayer;
using Framework.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Transactions;
using System.Web;
using System.Web.SessionState;
using static Google.Apis.Pagespeedonline.v5.PagespeedapiResource.RunpagespeedRequest;

namespace CMSv4.UnitTest
{
    [TestClass]
    public abstract class IntegrationTest<TipoModel> : BaseMock
    {
        [TestMethod]
        public void CRUD()
        {
            var model = this.BindModel();

            using (var scpope = new TransactionScope())
            {
                var codigoNovoRegistro = this.Salvar(model);
                Assert.AreNotEqual(0, codigoNovoRegistro);

                //Obter
                var modelObter = this.Obter(codigoNovoRegistro);
                //Assert.IsTrue(modelObter != null && modelObter.Codigo.HasValue && modelObter.Codigo.Value > 0);
                Assert.IsTrue(modelObter != null);

                //Listar
                var modelList = this.Listar();
                Assert.AreNotEqual(0, modelList.Count);

                //Excluir
                var excluido = this.Excluir(codigoNovoRegistro);
                Assert.AreNotEqual(0, excluido);
            }
        }

        #region abstract methods

        public abstract List<TipoModel> Listar();

        public abstract decimal Salvar(TipoModel model);

        public abstract TipoModel Obter(decimal Codigo);

        public abstract int Excluir(decimal Codigo);

        /// <summary>
        /// Model com as propriedades preenchidas para execução dos testes
        /// </summary>
        /// <returns></returns>
        public abstract TipoModel BindModel();

        #endregion
    }

    [TestClass]
    public abstract class Teste<TipoModel> : BaseMock, ICrud
    {        
        [TestMethod]
        public void Listar()
        {
            MockListar();
        }

        [TestMethod]
        public void Salvar()
        {
            using (var scpope = new TransactionScope())
            {
                var retorno = MockSalvar();
                Assert.AreNotEqual(0, retorno);
            }
        }

        [TestMethod]
        public void Obter()
        {
            Assert.IsNotNull(MockObter());
        }

        [TestMethod]
        public void Excluir()
        {
            int retorno = 0;

            using (var scpope = new TransactionScope())
            {
                retorno = MockExcluir();
            }
        }

        public abstract List<TipoModel> MockListar();

        public abstract decimal MockSalvar();

        public abstract TipoModel MockObter();

        public abstract int MockExcluir();
    }

    [TestClass]
    public class BaseMock
    {
        private Mock<HttpContextBase> _contextBaseMock;
        private Mock<HttpRequestBase> _requestBaseMock;

        [TestInitialize]
        public void SetupTests()
        {
            var con = GetMockedHttpContext();

            BusinessLayer.HttpContextFactory.SetCurrentContext(con);
            Framework.Utilities.HttpContextFactory.SetCurrentContext(con);
        }

        public void MockFile(List<string> NomesArquivos)
        {
            var files = new Mock<HttpFileCollectionBase>();

            for (int i = 0; i < NomesArquivos.Count; i++)
            {
                //Criar o arquivo disponível
                var file = new Mock<HttpPostedFileBase>();
                var mStream = CriarImagemNaMemoria();

                file.Setup(d => d.FileName).Returns(NomesArquivos[i]);
                file.Setup(d => d.InputStream).Returns(mStream);
                file.Setup(x => x.ContentLength).Returns((int)mStream.Length);
                file.Setup(x => x.ContentType).Returns("image/jpg");

                files.Setup(x => x.Get(i).InputStream).Returns(file.Object.InputStream);
                _requestBaseMock.Setup(o => o.Files[i]).Returns(file.Object);
            }

            _requestBaseMock.Setup(x => x.Files).Returns(files.Object);

            //Adicionar o arquivo no request
            _requestBaseMock.Setup(o => o.Files.Count).Returns(NomesArquivos.Count);
        }

        public MemoryStream CriarImagemNaMemoria()
        {
            var memoryStream = new MemoryStream();
            var bmp = new Bitmap(128, 128);

            using (Graphics graphics = Graphics.FromImage(bmp))
            using (SolidBrush brush = new SolidBrush(Color.Yellow))
                graphics.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height);

            bmp.Save(memoryStream, ImageFormat.Jpeg);

            return memoryStream;
        }

        private HttpContextBase GetMockedHttpContext()
        {
            _requestBaseMock = new Mock<HttpRequestBase>();
            _contextBaseMock = new Mock<HttpContextBase>();

            var session = new Mock<HttpSessionStateBase>();
            var items = new Mock<Dictionary<string, object>>();
            var form = new Mock<NameValueCollection>();
            var serverMock = new Mock<HttpServerUtilityBase>(MockBehavior.Loose);            
            var application = new Mock<HttpApplicationStateBase>(MockBehavior.Default);

            var projectBaseDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, @"..\..\..\CMSv4"));

            application.Setup(x => x["Portais"]).Returns(CRUD.ListarCache(new MLPortal(), 4));

            //Rquest.Url
            _requestBaseMock.Setup(x => x.Url).Returns(new Uri("http://localhost:55056/Principal"));

            //Definir como autenticado, sempre que verificar se está autenticado retorna true
            _requestBaseMock.Setup(o => o.IsAuthenticated).Returns(true);

            //Inicia a propriedade Form dentro do request, para as action que utilizam Request.Form
            _requestBaseMock.Setup(o => o.Form).Returns(form.Object);

            //Definir IP padrão que será retornado ao chamado UserHostAdress no mocked Request.
            _requestBaseMock.Setup(x => x.UserHostAddress).Returns("127.0.0.1");

            //_contextBaseMock.Setup(x => x.ApplicationInstance.Co).Returns(application.Object);

            //Definir o usuário que está logado no sistema admin, abaixo, autentica o usuário com código 1
            _contextBaseMock.Setup(x => x.User).Returns(new GenericPrincipal(new GenericIdentity("USU1"), null));

            //Iniciar a propriedade de cache para evitar exception null
            _contextBaseMock.Setup(x => x.Cache).Returns(new System.Web.Caching.Cache());

            //Iniciar a propriedade de session para evitar exception null
            _contextBaseMock.Setup(x => x.Session).Returns(session.Object);

            //Iniciar a propriedade Items para evitar exception null
            _contextBaseMock.Setup(x => x.Items).Returns(items.Object);

            //Configurar o objeto SERVER para poder utilizar mock no diretório VM2.UI
            serverMock.Setup(i => i.MapPath(It.IsAny<String>())).Returns((String a) => projectBaseDir + a.Replace("/", "\\"));

            //Definir caminho padrão para Server Mappath no Context mock
            _contextBaseMock.Setup(x => x.Server).Returns(serverMock.Object);

            //Inicializar o Request dentro do context
            _contextBaseMock.Setup(x => x.Request).Returns(_requestBaseMock.Object);

            //Mock para utilização dos portais do cms
            _contextBaseMock.Setup(x => x.Application).Returns(application.Object);

            return _contextBaseMock.Object;
        }
    }

    [TestClass]
    public class Base
    {
        /// <summary>
        /// Configuração inicial a ser executada antes de qualquer TestMethod
        /// </summary>
        [TestInitialize]
        public void SetupTests()
        {
            //HttpContextFactory.SetCurrentContext(GetMockedHttpContext());

            var httpContext = new HttpContext(new HttpRequest(string.Empty, @"http://localhost:55056", string.Empty), new HttpResponse(new StringWriter()));
            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                     new HttpStaticObjectsCollection(), 10, true,
                                                     HttpCookieMode.AutoDetect,
                                                     SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                                                     BindingFlags.NonPublic | BindingFlags.Instance,
                                                     null, CallingConventions.Standard,
                                                     new[] { typeof(HttpSessionStateContainer) },
                                                     null)
                                                .Invoke(new object[] { sessionContainer });

            HttpContext.Current = httpContext;
        }
    }
}
