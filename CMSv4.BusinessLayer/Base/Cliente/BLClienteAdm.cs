using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Transactions;
using System.Web;

namespace CMSv4.BusinessLayer
{
    public class BLClienteAdm : BLCRUD<MLCliente>
    {
        private MLCliente Model;
        private bool RemoverCapa;
        private string Senha;
        private string ConnectionString;
        private bool NovoRegistro = false;

        #region Obter

        public MLClienteViewModelItem ObterViewModel(decimal Codigo, string connectionString = "")
        {
            var model = new MLClienteViewModelItem();
            var query = "SELECT POR_N_CODIGO, GCL_N_CODIGO FROM CMS_CXG_CLIENTE_X_GRUPO INNER JOIN CMS_GCL_GRUPO_CLIENTE ON CXG_GCL_N_CODIGO = GCL_N_CODIGO AND CXG_CLI_N_CODIGO = " + Codigo.ToString();

            model.Cliente = base.Obter(Codigo, connectionString);
            model.Grupos = CRUD.ExecuteQuery<MLGrupoCliente>(query, connectionString);
            model.Cliente.Colaborador = new BLColaborador().Obter(new MLColaborador { CodigoCliente = Codigo }, connectionString);
            model.Portais = BLUsuario.ObterLogado().Portais;

            if (model.Cliente.Colaborador == null)
                model.Cliente.Colaborador = new MLColaborador();

            return model;
        }

        #endregion

        #region Excluir

        public override int Excluir(List<string> ids, string connectionString = "")
        {
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (var item in ids)
                {
                    CRUD.Excluir<MLCliente>(Convert.ToDecimal(item));
                    CRUD.Excluir<MLClienteItemGrupo>("CodigoCliente", Convert.ToDecimal(item), PortalAtual.ConnectionString);
                }

                scope.Complete();
            }

            return 1;
        }

        #endregion

        #region Salvar

        public decimal Salvar(MLCliente model, bool? removerCapa)
        {
            InicializarVariaveisGlobais(model, removerCapa);

            ConfigurarModelParaNovoRegistro();

            RemoverCapaSeCondicaoTrue();

            SalvarCliente();

            SalvarColaborador();

            SalvarImagemEAtulizarModelCliente();

            EnviarEmailParaNovoRegistro();

            return Model.Codigo.Value;
        }

        private void InicializarVariaveisGlobais(MLCliente model, bool? removerCapa)
        {
            Model = model;
            RemoverCapa = removerCapa.GetValueOrDefault(false);

            Senha = BLUtilitarios.GetNewPassword();
            ConnectionString = PortalAtual.ConnectionString;
            NovoRegistro = !Model.Codigo.HasValue;
        }

        private void ConfigurarModelParaNovoRegistro()
        {
            if (NovoRegistro)
            {
                Model.Senha = BLEncriptacao.EncriptarSenha(Senha);
                Model.DataCadastro = DateTime.Now;
                Model.AlterarSenha = true;
            }
        }

        private void RemoverCapaSeCondicaoTrue()
        {
            if (RemoverCapa && !string.IsNullOrEmpty(Model.Foto))
            {
                BLClienteReplicar.ExcluirArquivosReplicados(string.Concat(Model.Codigo, Model.Foto));
                Model.Foto = string.Empty;
            }
        }

        private void SalvarCliente()
        {
            Model.Ativo = Model.Ativo.GetValueOrDefault(false);
            Model.Codigo = CRUD.Salvar(Model, ConnectionString);
        }

        private void SalvarColaborador()
        {
            Model.Colaborador.CodigoCliente = Model.Codigo;
            Model.Colaborador.Codigo = CRUD.Salvar(Model.Colaborador, ConnectionString);
        }

        private void SalvarImagemEAtulizarModelCliente()
        {
            if(HttpContext.Current.Request.Files.Count > 0)
            {
                var imagem = HttpContext.Current.Request.Files[0];

                if (imagem != null && imagem.ContentLength > 0)
                {
                    Model.Foto = ".png";
                    BLClienteReplicar.ReplicarArquivo(imagem, Model.Codigo);
                    CRUD.SalvarParcial(Model, ConnectionString);
                }
            }
        }

        private void EnviarEmailParaNovoRegistro()
        {
            if (NovoRegistro)
            {
                var tradutor = new BLTraducao();
                var conteudoEmail = ObterModeloDeEmailParaNovaSenha();
                var emailBuilder = new BLEmailBuilder();

                emailBuilder
                    .Assunto(tradutor.ObterAdm("Nova Senha"))
                    .Destinatarios(Model.Email)
                    .Conteudo(conteudoEmail)
                    .Async(true)
                    .Enviar();
            }
        }

        private string ObterModeloDeEmailParaNovaSenha()
        {
            return BLEmail.ObterModelo(BLEmail.ModelosPadrao.NovaSenha)
                .Replace("[[nome]]", Model.Nome)
                .Replace("[[senha]]", Senha);
        }

        #endregion

        #region SalvarGruposPortal

        public void SalvarGruposPortal(decimal portal, decimal cliente, List<decimal> associados, List<decimal> nassociados)
        {
            var modelportal = CRUD.Obter<MLPortal>(portal);

            using (var scope = new TransactionScope(modelportal.ConnectionString))
            {
                if (associados != null)
                {
                    foreach (var item in associados)
                    {
                        CRUD.Salvar(new MLClienteItemGrupo
                        {
                            CodigoCliente = cliente,
                            CodigoGrupo = item
                        });
                    }
                }

                if (nassociados != null)
                    foreach (var item in nassociados)
                        CRUD.Excluir<MLClienteItemGrupo>(cliente, item);

                scope.Complete();
            }
        }

        #endregion
    }
}
