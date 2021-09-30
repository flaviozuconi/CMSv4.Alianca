using CMSv4.Model;
using Framework.Utilities;
using System;
using System.Collections.Generic;

namespace CMSv4.BusinessLayer
{
    public class BLEditorArquivoPermissao
    {
        public static void Salvar(string pasta, bool Restrito, List<int> grupos)
        {
            var infoDir = CRUD.Obter(new MLPasta() { Caminho = pasta }, PortalAtual.ConnectionString) ?? new MLPasta();

            infoDir.Caminho = pasta;
            infoDir.Restrito = Restrito;
            infoDir.Codigo = CRUD.Salvar(infoDir, PortalAtual.ConnectionString);

            CRUD.Excluir(new MLPastaPermissao() { CodigoDiretorio = infoDir.Codigo }, PortalAtual.ConnectionString);

            var permissoes = new List<MLPastaPermissao>();

            if (grupos != null)
            {
                foreach (var item in grupos)
                    permissoes.Add(new MLPastaPermissao() { CodigoDiretorio = infoDir.Codigo, CodigoGrupo = Convert.ToDecimal(item) });

                CRUD.Salvar(permissoes, PortalAtual.ConnectionString);
            }
        }
    }
}
