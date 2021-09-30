using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace CMSv4.BusinessLayer
{
    public class BLArquivoCategoria : BLCRUD<MLArquivoCategoria>
    {
        #region Excluir

        public override int Excluir(List<string> ids, string connectionString = "")
        {
            try
            {
                foreach (var item in ids)
                {
                    var model = base.Obter(Convert.ToDecimal(item));
                    Directory.Delete(model.PastaFisica(PortalAtual.Diretorio), true);
                    BLReplicar.ExcluirDiretoriosReplicados(model.PastaFisica(PortalAtual.Diretorio));
                }

                return base.Excluir(ids);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Listar

        public override List<MLArquivoCategoria> Listar(MLArquivoCategoria criterios, string connectionString = "")
        {
            criterios.CodigoPortal = PortalAtual.Codigo;

            var usuario = BLUsuario.ObterLogado();
            var lista = new List<MLArquivoCategoria>();

            foreach (var grp in usuario.Grupos)
            {
                if (grp.Ativo ?? false)
                {
                    ///Administrador pode ver todas as pastas
                    if ((grp.CodigoGrupo ?? -1) == 1)
                    {
                        lista.Clear();
                        lista = CRUD.Listar(criterios, PortalAtual.ConnectionString);
                        break;
                    }
                    else
                    {
                        criterios.CodigoGrupoEditor = grp.CodigoGrupo;

                        var lstAux = CRUD.Listar(criterios, PortalAtual.ConnectionString);
                        lista.AddRange(lstAux);
                    }
                }
            }

            return lista;
        }

        #endregion

        #region Obter

        public override MLArquivoCategoria Obter(decimal Codigo, string connectionString = "")
        {
            try
            {
                var portal = PortalAtual.Obter;
                var model = new MLArquivoCategoria();
                var usuario = BLUsuario.ObterLogado();

                if (Codigo > 0)
                {
                    model = CRUD.Obter(new MLArquivoCategoria { Codigo = Codigo, CodigoPortal = portal.Codigo }, portal.ConnectionString);
                    var grpLeitura = CRUD.Listar(new MLArquivoCategoriaGrupoCliente() { CodigoArquivoCategoria = Codigo }, portal.ConnectionString);

                    foreach (var grp in grpLeitura)
                    {
                        if (grp.CodigoGrupoLeitura.HasValue)
                            model.CodigosGrupoLeitura.Add(grp.CodigoGrupoLeitura.Value);
                    }
                }

                ///Grupos permissão escrita
                IEnumerable<SelectListItem> itensPermissaoEscrita = null;

                ///Se o usuário logado pertence ao grupo de Administradores
                ///ele poderá então adicionar pastas para todos ou demais grupos
                if (usuario.Grupos.Find(x => (x.Ativo ?? false) && ((x.CodigoGrupo ?? -1) == 1)) != null)
                {
                    var grupos = CRUD.Listar(new MLGrupo() { Ativo = true });
                    itensPermissaoEscrita = grupos.Select(c => new SelectListItem
                    {
                        Value = c.Codigo.ToString(),
                        Text = c.Nome
                    });
                }
                else
                {
                    var grupos = usuario.Grupos.FindAll(grp => (grp.Ativo ?? false));
                    itensPermissaoEscrita = grupos.Select(c => new SelectListItem
                    {
                        Value = c.CodigoGrupo.ToString(),
                        Text = c.Nome
                    });
                }

                model.ItensPermissaoEscrita = itensPermissaoEscrita;

                ///Grupos permissão leitura
                var grpsPermissaoLeitura = CRUD.Listar(new MLGrupoCliente { CodigoPortal = portal.Codigo, Ativo = true }, portal.ConnectionString);
                IEnumerable<SelectListItem> itensPermissaoLeitura = grpsPermissaoLeitura.Select(c => new SelectListItem
                {
                    Value = c.Codigo.ToString(),
                    Text = c.Nome
                });

                model.ItensPermissaoLeitura = itensPermissaoLeitura;

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Salvar

        public override decimal Salvar(MLArquivoCategoria model, string connectionString = "")
        {
            var portal = PortalAtual.Obter;
            var oldFolder = string.Empty;

            try
            {
                using (var scope = new TransactionScope(portal.ConnectionString))
                {
                    ///Guarda a pasta anterior caso altere o nome da pasta
                    if (model.Codigo.HasValue)
                    {
                        var oldModel = CRUD.Obter<MLArquivoCategoria>(model.Codigo.Value);
                        oldFolder = oldModel.PastaFisica(PortalAtual.Diretorio);
                        model.LogDataCadastro = oldModel.LogDataCadastro;
                        model.LogUsuarioCadastro = oldModel.LogUsuarioCadastro;
                    }

                    model.CodigoPortal = portal.Codigo;
                    model.LogPreencher(model.Codigo.HasValue);
                    model.Codigo = CRUD.Salvar(model, portal.ConnectionString);

                    ///Grava os grupos de clientes que poderão ter acesso a essa pasta
                    CRUD.Excluir(new MLArquivoCategoriaGrupoCliente() { CodigoArquivoCategoria = model.Codigo }, portal.ConnectionString);

                    if (model.CodigosGrupoLeitura != null)
                    {
                        foreach (var grp in model.CodigosGrupoLeitura)
                        {
                            if (grp == 0)
                                continue;

                            var aux = new MLArquivoCategoriaGrupoCliente()
                            {
                                CodigoArquivoCategoria = model.Codigo,
                                CodigoGrupoLeitura = grp
                            };

                            CRUD.Salvar(aux, portal.ConnectionString);
                        }
                    }

                    ///Move os arquivos da pasta antiga para a nova
                    if (!string.IsNullOrEmpty(oldFolder) && (oldFolder != model.PastaFisica(PortalAtual.Diretorio)))
                    {
                        Directory.Move(oldFolder, model.PastaFisica(PortalAtual.Diretorio));
                        BLReplicar.MoverDiretorio(oldFolder, model.PastaFisica(PortalAtual.Diretorio));
                    }

                    else
                    {
                        Directory.CreateDirectory(model.PastaFisica(PortalAtual.Diretorio));
                        BLReplicar.Diretorio(model.PastaFisica(PortalAtual.Diretorio));
                    }

                    scope.Complete();
                }

                return model.Codigo.GetValueOrDefault(0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
