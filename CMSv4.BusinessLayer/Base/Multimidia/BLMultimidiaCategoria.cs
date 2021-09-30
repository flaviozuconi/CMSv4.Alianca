using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;

namespace CMSv4.BusinessLayer
{
    public class BLMultimidiaCategoria : BLCRUD<MLMultimidiaCategoria>
    {
        #region Salvar

        public decimal Salvar(MLMultimidiaCategoria model)
        {
            var portal = PortalAtual.Obter;
            var oldFolder = string.Empty;

            using (var scope = new TransactionScope(portal.ConnectionString))
            {
                ///Guarda a pasta anterior caso altere o nome da pasta
                if (model.Codigo.HasValue)
                {
                    var oldModel = base.Obter(model.Codigo.Value);
                    oldFolder = oldModel.PastaFisica(PortalAtual.Diretorio);
                    model.LogDataCadastro = oldModel.LogDataCadastro;
                    model.LogUsuarioCadastro = oldModel.LogUsuarioCadastro;
                }

                model.CodigoPortal = portal.Codigo;
                model.LogPreencher(model.Codigo.HasValue);
                model.Codigo = base.Salvar(model, portal.ConnectionString);

                ///Grava os grupos de clientes que poderão ter acesso a essa pasta
                CRUD.Excluir(new MLMultimidiaCategoriaGrupoCliente() { CodigoArquivoCategoria = model.Codigo }, portal.ConnectionString);

                if (model.CodigosGrupoLeitura != null)
                {
                    foreach (var grp in model.CodigosGrupoLeitura)
                    {
                        if (grp == 0)
                            continue;

                        var aux = new MLMultimidiaCategoriaGrupoCliente()
                        {
                            CodigoArquivoCategoria = model.Codigo,
                            CodigoGrupoLeitura = grp
                        };

                        CRUD.Salvar(aux, portal.ConnectionString);
                    }
                }

                ///Move os arquivos da pasta antiga para a nova
                if (!string.IsNullOrEmpty(oldFolder) && (oldFolder != model.PastaFisica(PortalAtual.Diretorio)))
                    Directory.Move(oldFolder, model.PastaFisica(PortalAtual.Diretorio));
                else
                    Directory.CreateDirectory(model.PastaFisica(PortalAtual.Diretorio));

                scope.Complete();
            }

            return model.Codigo.GetValueOrDefault();
        }

        #endregion

        #region ListGrid

        public List<MLMultimidiaCategoria> ListGrid(MLMultimidiaCategoria model)
        {
            model.CodigoPortal = PortalAtual.Codigo;

            var usuario = BLUsuario.ObterLogado();
            var lista = new List<MLMultimidiaCategoria>();

            foreach (var grp in usuario.Grupos)
            {
                if (grp.Ativo ?? false)
                {
                    ///Administrador pode ver todas as pastas
                    if ((grp.CodigoGrupo ?? -1) == 1)
                    {
                        lista.Clear();
                        lista = base.Listar(model, PortalAtual.ConnectionString);
                        break;
                    }
                    else
                    {
                        model.CodigoGrupoEditor = grp.CodigoGrupo;

                        var lstAux = base.Listar(model, PortalAtual.ConnectionString);
                        lista.AddRange(lstAux);
                    }
                }
            }
            return lista;
        }

        #endregion

        #region ObterDadosParaEdicao

        public MLMultimidiaCategoria ObterDadosParaEdicao(decimal? id, ref List<MLGrupo> Escrita, ref List<MLGrupoCliente> Leitura)
        {

            var portal = PortalAtual.Obter;
            var model = new MLMultimidiaCategoria();
            var usuario = BLUsuario.ObterLogado();

            if (id.HasValue)
            {
                model = CRUD.Obter(new MLMultimidiaCategoria { Codigo = id, CodigoPortal = portal.Codigo }, portal.ConnectionString);
                var grpLeitura = CRUD.Listar(new MLMultimidiaCategoriaGrupoCliente() { CodigoArquivoCategoria = id }, portal.ConnectionString);

                foreach (var grp in grpLeitura)
                {
                    if (grp.CodigoGrupoLeitura.HasValue)
                        model.CodigosGrupoLeitura.Add(grp.CodigoGrupoLeitura.Value);
                }
            }

            ///Se o usuário logado pertence ao grupo de Administradores
            ///ele poderá então adicionar pastas para todos ou demais grupos
            if (usuario.Grupos.Find(x => (x.Ativo ?? false) && ((x.CodigoGrupo ?? -1) == 1)) != null)
                Escrita = CRUD.Listar(new MLGrupo() { Ativo = true });
            else
                Escrita = usuario.Grupos.FindAll(grp => grp.Ativo.GetValueOrDefault()).Select(c => new MLGrupo
                {
                    Codigo = c.CodigoGrupo,
                    Nome = c.Nome
                }).ToList();


            Leitura = CRUD.Listar(new MLGrupoCliente { CodigoPortal = PortalAtual.Codigo, Ativo = true }, portal.ConnectionString);

            return model;
        }
        #endregion

        #region Excluir
        public override int Excluir(List<string> ids, string connectionString = "")
        {
            int qtdeExcluidos = 0;

            foreach (var item in ids)
            {
                var model = base.Obter(Convert.ToDecimal(item));
                qtdeExcluidos += base.Excluir(Convert.ToDecimal(item), connectionString);

                if (!String.IsNullOrWhiteSpace(model.Nome))
                    Directory.Delete(model.PastaFisica(PortalAtual.Diretorio),true);
            }

            return qtdeExcluidos; 
        }
        #endregion
    }
}
