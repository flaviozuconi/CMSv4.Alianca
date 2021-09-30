using Framework.Utilities;
using System;
using System.Linq;

namespace CMSv4.BusinessLayer
{
    /// <summary>
    /// BL para os módulos que não precisam de uma BL específica para fazer os métodos padrões
    /// </summary>
    public class BLModuloComum<Tedicao, Tpublicado, Thistorico> : BLModuloBase
    {

        #region Publicar

        /// <summary>
        /// PUBLICAR
        /// </summary>
        public new static string Publicar(string urlModulo, decimal codigoPagina, int repositorio, Guid? codigoHistorico)
        {
            var criterio = Activator.CreateInstance<Tedicao>();
            SetProperty(criterio, "CodigoPagina", codigoPagina);
            SetProperty(criterio, "Repositorio", repositorio);

            var conteudo = CRUD.Obter<Tedicao>(criterio);

            if (conteudo != null)
            {
                // GERAR HISTORICO
                if (codigoHistorico.HasValue)
                {
                    var criterioPublicado = Activator.CreateInstance<Tpublicado>();
                    SetProperty(criterioPublicado, "CodigoPagina", codigoPagina);
                    SetProperty(criterioPublicado, "Repositorio", repositorio);

                    var publicado = CRUD.Obter<Tpublicado>(criterioPublicado);
                    if (publicado != null)
                    {
                        var historico = CRUD.CopiarValores(publicado, Activator.CreateInstance<Thistorico>());
                        SetProperty(historico, "CodigoHistorico", codigoHistorico);
                        CRUD.Salvar(historico);
                    }
                }

                // PUBLICAR
                var publicacao = CRUD.CopiarValores(conteudo, Activator.CreateInstance<Tpublicado>());
                var usuario = BLUsuario.ObterLogado();

                if (usuario != null && usuario.Codigo.HasValue)
                {
                    SetProperty(publicacao, "CodigoUsuario", BLUsuario.ObterLogado().Codigo);
                }

                SetProperty(publicacao, "DataRegistro", DateTime.Now);
                CRUD.Salvar(publicacao);

                //CRUD.Excluir<Tedicao>(codigoPagina, repositorio);
            }

            return string.Format(@"
                        @{{ Html.RenderAction(""index"", ""{0}"", new {{ area=""Modulo"", codigoPagina = ""{1}"", repositorio = ""{2}"" }}); }}",
                        urlModulo,
                        codigoPagina,
                        repositorio
                        );
        }

        #endregion

        #region Mover

        /// <summary>
        /// MOVER
        /// </summary>
        public static void Mover(decimal codigoPagina, int repositorioOrigem, int repositorioDestino, bool IsPreenchido, bool IsMesmoModulo, string connectionstring)
        {
            //Obtem a Origem
            var criterio = Activator.CreateInstance<Tedicao>();
            SetProperty(criterio, "CodigoPagina", codigoPagina);
            SetProperty(criterio, "Repositorio", repositorioOrigem);

            var model = CRUD.Obter<Tedicao>(criterio, connectionstring);
            SetProperty(model, "Repositorio", repositorioDestino);
            CRUD.Excluir<Tedicao>(codigoPagina, repositorioOrigem, connectionstring);
            if (IsPreenchido)
            {
                if (IsMesmoModulo)
                {
                    var criterioDes = Activator.CreateInstance<Tedicao>();
                    SetProperty(criterioDes, "CodigoPagina", codigoPagina);
                    SetProperty(criterioDes, "Repositorio", repositorioDestino);
                    var modelDes = CRUD.Obter<Tedicao>(criterioDes, connectionstring);
                    SetProperty(modelDes, "Repositorio", repositorioOrigem);

                    CRUD.Salvar(modelDes, connectionstring);
                }
                else
                {
                    //CRUD.Excluir<Tedicao>(codigoPagina, repositorioOrigem);
                }
            }


            CRUD.Salvar(model, connectionstring);
        }

        #endregion

        #region CarregarConteudo

        /// <summary>
        /// Carrega Conteudo Publicado para Edição
        /// </summary>
        public new static void CarregarConteudo(decimal codigoPagina, int repositorio)
        {
            var criterio = Activator.CreateInstance<Tpublicado>();
            SetProperty(criterio, "CodigoPagina", codigoPagina);
            SetProperty(criterio, "Repositorio", repositorio);

            var publicado = CRUD.Obter<Tpublicado>(criterio);

            if (publicado != null)
            {
                var conteudo = CRUD.CopiarValores(publicado, Activator.CreateInstance<Tedicao>());
                SetProperty(conteudo, "CodigoUsuario", BLUsuario.ObterLogado().Codigo);
                SetProperty(conteudo, "DataRegistro", DateTime.Now);

                CRUD.Salvar(conteudo);
            }
        }

        #endregion

        #region CarregarHistorico

        /// <summary>
        /// Carregar Historico
        /// </summary>
        public new static void CarregarHistorico(decimal codigoPagina, int repositorio, Guid codigoHistorico)
        {
            var criterio = Activator.CreateInstance<Thistorico>();
            SetProperty(criterio, "CodigoPagina", codigoPagina);
            SetProperty(criterio, "Repositorio", repositorio);
            SetProperty(criterio, "CodigoHistorico", codigoHistorico);

            var historico = CRUD.Obter<Thistorico>(criterio);

            if (historico != null)
            {
                var conteudo = CRUD.CopiarValores(historico, Activator.CreateInstance<Tedicao>());
                SetProperty(conteudo, "CodigoUsuario", BLUsuario.ObterLogado().Codigo);
                SetProperty(conteudo, "DataRegistro", DateTime.Now);

                CRUD.Salvar(conteudo);
            }
        }

        #endregion

        #region DuplicarConteudo

        /// <summary>
        /// Duplicar Conteudo
        /// </summary>
        public new static void DuplicarConteudo(decimal codigoPagina, decimal codigoNovaPagina, int repositorio, bool publicado)
        {
            if (publicado)
            {
                var criterio = Activator.CreateInstance<Tpublicado>();
                SetProperty(criterio, "CodigoPagina", codigoPagina);
                SetProperty(criterio, "Repositorio", repositorio);

                var historico = CRUD.Obter<Tpublicado>(criterio);
                SalvarNovoConteudo(historico, codigoNovaPagina);
            }
            else
            {
                var criterio = Activator.CreateInstance<Tedicao>();
                SetProperty(criterio, "CodigoPagina", codigoPagina);
                SetProperty(criterio, "Repositorio", repositorio);

                var historico = CRUD.Obter<Tedicao>(criterio);
                SalvarNovoConteudo(historico, codigoNovaPagina);
            }
        }

        private static void SalvarNovoConteudo<TipoModel>(TipoModel historico, decimal codigoNovaPagina)
        {
            if (historico != null)
            {
                var conteudo = CRUD.CopiarValores(historico, Activator.CreateInstance<Tedicao>());
                SetProperty(conteudo, "CodigoPagina", codigoNovaPagina);
                SetProperty(conteudo, "CodigoUsuario", BLUsuario.ObterLogado().Codigo);
                SetProperty(conteudo, "DataRegistro", DateTime.Now);

                CRUD.Salvar(conteudo);
            }
        }

        #endregion

        private static void SetProperty<T>(T instancia, string propertyName, object value)
        {
            try
            {
                var propriedades = typeof(T).GetProperties();
                propriedades.FirstOrDefault(o => o.Name == propertyName).SetValue(instancia, value, null);
            }
            catch { }
        }

    }
}
