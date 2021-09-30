using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Framework.Model;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace Framework.Utilities
{
    /// <summary> 
    /// Model da Entidade Usuario
    /// </summary> 
    [Serializable]
    [Table("FWK_USU_USUARIO")]
    [Auditing("/admin/usuario")]
    public class MLUsuario
    {
        public MLUsuario()
        {
            Grupos = new List<MLUsuarioItemGrupo>();
            Funcionalidades = new List<MLUsuarioItemFuncionalidade>();
            Portais = new List<MLUsuarioItemPortal>();
        }

        [DataField("USU_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [Required, StringLength(100)]
        [DataField("USU_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [Required, StringLength(200)]
        [DataField("USU_C_LOGIN", SqlDbType.VarChar, 200)]
        public string Login { get; set; }

        [StringLength(50), DataType(DataType.Password)]
        [DataField("USU_C_SENHA", SqlDbType.VarChar, 50, IgnoreEmpty = true)]
        public string Senha { get; set; }

        [Required, StringLength(200)]
        [DataField("USU_C_EMAIL", SqlDbType.VarChar, 200)]
        public string Email { get; set; }

        [DataField("USU_D_CADASTRO", SqlDbType.DateTime, IgnoreEmpty = true)]
        public DateTime? DataCadastro { get; set; }

        [DataField("USU_B_STATUS", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("USU_B_ALTERARSENHA", SqlDbType.Bit, IgnoreEmpty = true)]
        public bool? AlterarSenha { get; set; }

        [DataField("USU_B_PUBLICO", SqlDbType.Bit, IgnoreEmpty = true)]
        public bool? Publico { get; set; }

        [DataField("USU_C_TOKEN_NOVA_SENHA", SqlDbType.VarChar, 36)]
        public string TokenNovaSenha { get; set; }


        public List<MLUsuarioItemGrupo> Grupos { get; set; }

        public List<MLUsuarioItemFuncionalidade> Funcionalidades { get; set; }

        public List<MLUsuarioItemPortal> Portais { get; set; }


        // Verifica as Permissões do usuário na Área Administrativa do sistema

        #region Check Permissao

        public bool CheckPortal(decimal? codigoPortal)
        {
            if (!codigoPortal.HasValue) return false;

            var portal = BLPortal.Portais.Find(o => o.Codigo == codigoPortal);

            if (portal == null) return false;

            if (!portal.Restrito.HasValue) return true;

            if (portal.Restrito == false) return true;

            if (Portais == null) return false;

            if (Portais.Find(o => o.CodigoPortal == codigoPortal) == null) return false;

            return true;
        }

        public bool CheckPermissao(Permissao permissao, string url)
        {
            return CheckPermissao(permissao, url, false);
        }

        public bool CheckPermissao(Permissao permissao, string url, bool? needPortal)
        {
            if (!needPortal.HasValue) needPortal = false;

            // poderia usar referência ao UI, mas isso quebraria a arquitetura
            string PORTAL_PREFIX = "/cms";

            if (Funcionalidades == null) return false;
            if (Funcionalidades.Count == 0) return false;

            url = url.ToLowerInvariant();
            if (!url.EndsWith("//")) url = url + "/";

            // Portal
            if (needPortal.Value && !url.StartsWith(PORTAL_PREFIX)) return false;

            if (url.StartsWith(PORTAL_PREFIX))
            {
                var diretorioPortal = url.Split('/');

                if (diretorioPortal.Length > 1)
                {
                    if (Portais.Count == 0 && needPortal.Value) return false;

                    var portalEncontrado = this.Portais.Find(o => o.Diretorio.ToLowerInvariant() == diretorioPortal[2]);

                    if (portalEncontrado != null)
                    {
                        // Validar a permissão do usuário no Portal
                        if (needPortal.Value && Portais.Find(o => o.CodigoPortal == portalEncontrado.CodigoPortal) == null) return false;

                        // e remover o portal da URL para continuar verificação
                        url = url.Replace("/" + diretorioPortal[2], "");
                    }
                    else
                        return false;

                }
                else return false;
            }

            MLUsuarioItemFuncionalidade item = null;

            item = Funcionalidades.Find(o => url.ToLowerInvariant().Equals((o.Url.ToLowerInvariant() + "/").Replace("////", "//")));
            
            if (item == null) //Caso não encontre a funcionalidade exatamente com o mesmo nome ou se a url possuir codigo de registro
            {
                //.Find obtém o primeiro item da lista que atende a solicitação de .StartsWith de url
                //.OrderByDescending para caso de funcionalidades que diferem apenas pela action (ex: moduloadmin e moduloadmin/action)
                item = Funcionalidades.OrderByDescending(a => a.Url).ToList().Find(o => url.ToLowerInvariant().StartsWith((o.Url.ToLowerInvariant() + "/").Replace("////", "//")));
            }

            if (item == null) return false;
            if (permissao == Permissao.Visualizar && item.Visualizar.HasValue && item.Visualizar == true) return true;
            if (permissao == Permissao.Modificar && item.Modificar.HasValue && item.Modificar == true) return true;
            if (permissao == Permissao.Excluir && item.Excluir.HasValue && item.Excluir == true) return true;

            return false;
        }

        public MLAuditoria GetAuditoria(string codigoref, string url, decimal? codigoportal)
        {

            var codigoFuncionalidade = GetCodigoFuncionalidade(url, !codigoportal.HasValue);

            if (codigoFuncionalidade.HasValue)            
                return CRUD.Obter<MLAuditoria>(new MLAuditoria { CodigoReferencia = codigoref, CodigoFuncionalidade = codigoFuncionalidade, CodigoPortal = codigoportal });
            
            return null;
        }

        public decimal? GetCodigoFuncionalidade(string url)
        {
            return GetCodigoFuncionalidade(url, false);
        }
        public decimal? GetCodigoFuncionalidade(string url,bool pularPortal)
        {
            // poderia usar referência ao UI, mas isso quebraria a arquitetura
            string PORTAL_PREFIX = "/cms";

            if (Funcionalidades == null) return null;
            if (Funcionalidades.Count == 0) return null;

            url = url.ToLowerInvariant();
            if (!url.EndsWith("//")) url = url + "/";

            if (url.StartsWith(PORTAL_PREFIX) && !pularPortal)
            {
                var diretorioPortal = url.Split('/');
                if (diretorioPortal.Length > 1)
                {
                    url = url.Replace("/" + diretorioPortal[2], "");
                }
            }

            var item = Funcionalidades.Find(o => url.ToLowerInvariant().StartsWith((o.Url.ToLowerInvariant() + "/").Replace("////", "//")));
            if (item != null && item.CodigoFuncionalidade.HasValue)
                return item.CodigoFuncionalidade;

            return null;
        }

        #endregion


        /// <summary>
        /// Retorna a lista completa com os grupos do usuario separado por virgula
        /// </summary>
        public string GruposToString()
        {
            if (Grupos == null || Grupos.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(",", Grupos.Select(a => a.CodigoGrupo));
        }
    }

    [Table("FWK_USU_USUARIO")]
    public class MLUsuarioGrid : MLUsuario
    {
        [DataField("PORTAIS_ASSOCIADOS", SqlDbType.VarChar, -1, IgnoreEmpty = true)]
        [SubSelectField("PORTAIS_ASSOCIADOS", @"SELECT REVERSE(STUFF(REVERSE((
            SELECT POR_C_NOME + ', ' FROM ADM_POR_PORTAL
            INNER JOIN ADM_PXU_PORTAL_X_USUARIO ON PXU_POR_N_CODIGO = POR_N_CODIGO
            WHERE PXU_USU_N_CODIGO = TABELA.USU_N_CODIGO
            FOR XML PATH(''))), 1, 2, ''))")]
        public string PortaisAssociados { get; set; }
    }

    #region MLUsuarioAlterarSenha

    /// <summary> 
    /// Model da Entidade Usuario
    /// </summary> 
    [Serializable]
    [Table("FWK_USU_USUARIO")]
    public class MLUsuarioAlterarSenha
    {
        [DataField("USU_N_CODIGO", SqlDbType.Decimal, 18, true)]
        public decimal? Codigo { get; set; }

        [Required, StringLength(50), DataType(DataType.Password)]
        [DataField("USU_C_SENHA", SqlDbType.VarChar, 50)]
        public string Senha { get; set; }

        [Required, StringLength(50), DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.CompareAttribute("Senha", ErrorMessage = "Informe uma senha e digite novamente para confirmar")]
        public string ConfirmarSenha { get; set; }

        [DataField("USU_B_ALTERARSENHA", SqlDbType.Bit, IgnoreEmpty = true)]
        public bool? AlterarSenha { get; set; }

        [DataField("USU_C_TOKEN_NOVA_SENHA", SqlDbType.VarChar, 36)]
        public string TokenNovaSenha { get; set; }
    }

    #endregion

    #region MLUsuarioItemGrupo

    /// <summary> 
    /// Model
    /// </summary> 
    [Serializable]
    [Table("FWK_GXU_GRUPO_USUARIO")]
    public class MLUsuarioItemGrupo
    {
        [DataField("GXU_USU_N_CODIGO", SqlDbType.Decimal, 18, true)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("GRP_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoGrupo { get; set; }

        [DataField("GRP_C_NOME", SqlDbType.VarChar, 50)]
        public string Nome { get; set; }

        [DataField("GRP_B_STATUS", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("USUARIO_ASSOCIADO", SqlDbType.Bit)]
        public bool? UsuarioAssociado { get; set; }
    }

    #endregion

    #region MLUsuarioItemFuncionalidade

    /// <summary> 
    /// Model
    /// </summary> 
    [Serializable]
    public class MLUsuarioItemFuncionalidade
    {
        [DataField("FUN_N_CODIGO", SqlDbType.Decimal, 18, true)]
        public decimal? CodigoFuncionalidade { get; set; }

        [DataField("FUN_C_URL", SqlDbType.VarChar, 100)]
        public string Url { get; set; }

        [DataField("GPE_B_VISUALIZAR", SqlDbType.Bit)]
        public bool? Visualizar { get; set; }

        [DataField("GPE_B_MODIFICAR", SqlDbType.Bit)]
        public bool? Modificar { get; set; }

        [DataField("GPE_B_EXCLUIR", SqlDbType.Bit)]
        public bool? Excluir { get; set; }
    }

    #endregion

    #region MLUsuarioItemPortal

    /// <summary> 
    /// Model da Entidade Portais
    /// </summary> 
    [Serializable]
    [Table("ADM_PXU_PORTAL_X_USUARIO")]
    public class MLUsuarioItemPortal
    {
        [DataField("PXU_USU_N_CODIGO", SqlDbType.Decimal, 18, true)]
        public decimal? CodigoUsuario { get; set; }

        [DataField("PXU_POR_N_CODIGO", SqlDbType.Decimal, 18, true)]
        public decimal? CodigoPortal { get; set; }

        [JoinField("PXU_POR_N_CODIGO", "ADM_POR_PORTAL", "POR_N_CODIGO", "POR_C_NOME")]
        [DataField("POR_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [JoinField("PXU_POR_N_CODIGO", "ADM_POR_PORTAL", "POR_N_CODIGO", "POR_C_DIRETORIO")]
        [DataField("POR_C_DIRETORIO", SqlDbType.VarChar, 20)]
        public string Diretorio { get; set; }

        [DataField("USUARIO_ASSOCIADO", SqlDbType.Bit, IgnoreEmpty = true)]
        public bool? UsuarioAssociado { get; set; }
    }

    #endregion

}
