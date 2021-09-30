using System.Data;
using Framework.Model;
using System;
using System.Collections.Generic;

namespace Framework.Utilities
{
    /// <summary> 
    /// Model da Entidade Menu 
    /// </summary> 
    [Table("FWK_MEN_MENU")]
    public class MLMenu 
    {
        [DataField("MEN_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey=true, AutoNumber=true)]
        public decimal? Codigo { get; set; }

        [DataField("MEN_POR_N_CODIGO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? CodigoPortal { get; set; }

        [DataField("MEN_C_NOME", SqlDbType.VarChar, 50)]
        public string Nome { get; set; }

        [DataField("MEN_B_STATUS", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        /// <summary>
        /// Diretório onde serão gerados os arquivos físicos das views do menu
        /// </summary>
        public static class Pasta
        {
            public const string Views = "MenuAdmin";
        }

        public static class Config
        {
            public const string CacheKey = "USP_FWK_S_MENU_COMPLETO_";
            //public const string View = "@model%20MLMenuCompleto%0A@%7B%0A%20%20%20%20var%20portal%20%3D%20BLPortal.Atual%3B%0A%0A%20%20%20%20if%20%28string.IsNullOrEmpty%28ViewBag.Classe%29%29%0A%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20ViewBag.Classe%20%3D%20%22menu-principal%22%3B%0A%20%20%20%20%7D%0A%7D%0A%0A%3C%21--%20begin%20SIDE%20NAVIGATION%20--%3E%0A%3Cnav%20class%3D%22navbar-side%22%20role%3D%22navigation%22%3E%0A%20%20%20%20%3Cdiv%20class%3D%22navbar-collapse%20sidebar-collapse%20collapse%22%3E%0A%20%20%20%20%20%20%20%20%3Cul%20id%3D%22side%22%20class%3D%22nav%20navbar-nav%20side-nav%22%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%3C%21--%20begin%20SIDE%20NAV%20SEARCH%20--%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%3Cli%20class%3D%22nav-search%22%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3Cinput%20type%3D%22search%22%20class%3D%22form-control%20txt-search%22%20placeholder%3D%22@TAdm%28%22Procurar...%22%29%22%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3Cbutton%20class%3D%22btn%22%20onclick%3D%22return%20false%3B%22%20style%3D%22cursor%3A%20default%3B%22%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3Ci%20class%3D%22fa%20fa-search%22%3E%3C/i%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3C/button%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%3C/li%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%3C%21--%20end%20SIDE%20NAV%20SEARCH%20--%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%3C%21--%20begin%20menu%20CMS%20%20--%3E%0A%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20@foreach%20%28var%20item%20in%20Model.ItensMenu%29%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20if%20%28item.Filhos%20%21%3D%20null%20%26%26%20item.Filhos.Count%20%3E%200%29%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3Cli%20class%3D%22panel%22%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3Ca%20id%3D%22Menu_@item.Codigo%22%20class%3D%22accordion-toggle%22%20data-parent%3D%22%23side%22%20data-toggle%3D%22collapse%22%20data-target%3D%22%23@item.Codigo%22%20href%3D%22javascript%3A%3B%22%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3Cimg%20src%3D%22@item.Icone%22%20/%3E%20@TAdm%28item.Nome%29%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3Ci%20class%3D%22fa%20fa-caret-down%22%3E%3C/i%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3C/a%3E%0A%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3Cul%20class%3D%22collapse%20nav%20nav-menu%22%20id%3D%22@item.Codigo%22%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20@Item%28item%29%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3C/ul%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3C/li%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%7D%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20else%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3Cli%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3Ca%20data-url%3D%22@item.Href%22%20onclick%3D%22return%20newTab%28this%2C%20%27@item.Codigo%27%29%3B%22%20href%3D%22javascript%3A%3B%22%3E%3Cimg%20src%3D%22@item.Icone%22%20/%3E%20@TAdm%28item.Nome%29%3C/a%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3C/li%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%7D%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%7D%0A%20%20%20%20%20%20%20%20%3C/ul%3E%0A%20%20%20%20%20%20%20%20%3C%21--%20/.side-nav%20--%3E%0A%20%20%20%20%3C/div%3E%0A%20%20%20%20%3C%21--%20/.navbar-collapse%20--%3E%0A%3C/nav%3E%0A%3C%21--%20/.navbar-side%20--%3E%0A%0A@helper%20Item%28MLMenuItemModulo%20model%29%0A%7B%0Aforeach%20%28var%20item%20in%20model.Filhos%29%0A%7B%0A%20%20%20%20if%20%28item.Filhos.Count%20%3E%200%29%0A%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20if%20%28item.Css%20%3D%3D%20%22fa-angle-double-right%22%29%0A%20%20%20%20%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%20%20%20%20%3Cli%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3Clabel%3E%3Ci%20class%3D%22fa%20fa-angle-double-right%22%3E%3C/i%3E%20@item.Nome%3C/label%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%3C/li%3E%0A%0A%20%20%20%20%20%20%20%20%20%20%20%20%3Cul%20class%3D%22nav%22%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20@Item%28item%29%0A%20%20%20%20%20%20%20%20%20%20%20%20%3C/ul%3E%0A%20%20%20%20%20%20%20%20%7D%0A%20%20%20%20%20%20%20%20else%0A%20%20%20%20%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%20%20%20%20%3Cli%20class%3D%22tree%22%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3Cul%20class%3D%22nav%22%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3Cli%20class%3D%22parent%22%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3Ca%3E%3Cimg%20src%3D%22@item.Icone%22%3E%20@T%28item.Nome%29%3C/a%3E%0A%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3Cul%20class%3D%22nav%22%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20@Item%28item%29%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3C/ul%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3C/li%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%3C/ul%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%3C/li%3E%0A%20%20%20%20%20%20%20%20%7D%0A%20%20%20%20%7D%0A%20%20%20%20else%0A%20%20%20%20%7B%0A%20%20%20%20%20%20%20%20%3Cli%3E%0A%20%20%20%20%20%20%20%20%20%20%20%20%3Ca%20data-url%3D%22@item.Href%22%20onclick%3D%22return%20newTab%28this%2C%20%27@item.Codigo%27%29%3B%22%20href%3D%22javascript%3A%3B%22%3E%3Cimg%20src%3D%22@item.Icone%22%3E%20@TAdm%28item.Nome%29%3C/a%3E%0A%20%20%20%20%20%20%20%20%3C/li%3E%0A%20%20%20%20%7D%0A%7D%0A%7D";
            //public const string Menu = "{'ItensMenu':[{'Codigo':1,'CodigoMenu':1,'CodigoFuncionalidade':58,'Nome':'Página','CodigoPai':0,'Ativo':null,'CaminhoPagina':'/cms/Principal/pagina','Imagem':'/iconpack.axd/16/application_view_gallery','Ordem':0,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/cms/pagina','Css':null,'sbItens':[],'Url':'/cms/Principal/pagina'},{'Codigo':11,'CodigoMenu':1,'CodigoFuncionalidade':57,'Nome':'Seções','CodigoPai':3,'Ativo':null,'CaminhoPagina':'/cms/Principal/secao','Imagem':'/iconpack.axd/16/bricks','Ordem':0,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/cms/secao','Css':null,'sbItens':[],'Url':'/cms/Principal/secao'},{'Codigo':12,'CodigoMenu':1,'CodigoFuncionalidade':70,'Nome':'Arquivos','CodigoPai':7,'Ativo':null,'CaminhoPagina':'/cms/Principal/arquivoadmin','Imagem':'/iconpack.axd/16/file_extension_zip','Ordem':0,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/cms/arquivoadmin','Css':null,'sbItens':[],'Url':'/cms/Principal/arquivoadmin'},{'Codigo':15,'CodigoMenu':1,'CodigoFuncionalidade':3,'Nome':'Usuários','CodigoPai':5,'Ativo':null,'CaminhoPagina':'/admin/usuario','Imagem':'/iconpack.axd/16/user','Ordem':0,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/admin/usuario','Css':null,'sbItens':[],'Url':'/admin/usuario'},{'Codigo':27,'CodigoMenu':1,'CodigoFuncionalidade':68,'Nome':'Menu','CodigoPai':10,'Ativo':null,'CaminhoPagina':'/cms/Principal/MenuAdmin','Imagem':'/iconpack.axd/16/menu','Ordem':0,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/cms/menuadmin','Css':null,'sbItens':[],'Url':'/cms/Principal/MenuAdmin'},{'Codigo':34,'CodigoMenu':1,'CodigoFuncionalidade':59,'Nome':'Layouts','CodigoPai':4,'Ativo':null,'CaminhoPagina':'/cms/Principal/layout','Imagem':'/iconpack.axd/16/layout','Ordem':0,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/cms/layout','Css':null,'sbItens':[],'Url':'/cms/Principal/layout'},{'Codigo':41,'CodigoMenu':1,'CodigoFuncionalidade':96,'Nome':'Grupos','CodigoPai':16,'Ativo':null,'CaminhoPagina':'/admin/grupo','Imagem':'/iconpack.axd/16/group','Ordem':0,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/cms/grupocliente','Css':null,'sbItens':[],'Url':'/admin/grupo'},{'Codigo':42,'CodigoMenu':1,'CodigoFuncionalidade':4,'Nome':'Funcionalidades','CodigoPai':16,'Ativo':null,'CaminhoPagina':'/admin/funcionalidade','Imagem':'/iconpack.axd/16/group_key','Ordem':1,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/admin/funcionalidade','Css':null,'sbItens':[],'Url':'/admin/funcionalidade'},{'Codigo':38,'CodigoMenu':1,'CodigoFuncionalidade':55,'Nome':'Portais','CodigoPai':5,'Ativo':null,'CaminhoPagina':'/admin/portal','Imagem':'/iconpack.axd/16/globe_network','Ordem':1,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/cms/portal','Css':null,'sbItens':[],'Url':'/admin/portal'},{'Codigo':35,'CodigoMenu':1,'CodigoFuncionalidade':56,'Nome':'Templates','CodigoPai':4,'Ativo':null,'CaminhoPagina':'/cms/Principal/template','Imagem':'/iconpack.axd/16/layout_content','Ordem':1,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/cms/template','Css':null,'sbItens':[],'Url':'/cms/Principal/template'},{'Codigo':28,'CodigoMenu':1,'CodigoFuncionalidade':105,'Nome':'Busca','CodigoPai':10,'Ativo':null,'CaminhoPagina':'/cms/Principal/BuscaAdmin','Imagem':'/iconpack.axd/16/find','Ordem':1,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/cms/buscaadmin','Css':null,'sbItens':[],'Url':'/cms/Principal/BuscaAdmin'},{'Codigo':29,'CodigoMenu':1,'CodigoFuncionalidade':96,'Nome':'Grupo Cliente','CodigoPai':3,'Ativo':null,'CaminhoPagina':'/cms/Principal/grupocliente','Imagem':'/iconpack.axd/16/group','Ordem':1,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/cms/grupocliente','Css':null,'sbItens':[],'Url':'/cms/Principal/grupocliente'},{'Codigo':2,'CodigoMenu':1,'CodigoFuncionalidade':139,'Nome':'Módulos','CodigoPai':0,'Ativo':null,'CaminhoPagina':'Módulos','Imagem':'/iconpack.axd/16/application_view_tile','Ordem':1,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/cms/modulo','Css':null,'sbItens':[],'Url':'Módulos'},{'Codigo':3,'CodigoMenu':1,'CodigoFuncionalidade':null,'Nome':'Gestor do Portal','CodigoPai':0,'Ativo':null,'CaminhoPagina':null,'Imagem':'/iconpack.axd/16/user_edit','Ordem':2,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':null,'Css':null,'sbItens':[],'Url':null},{'Codigo':30,'CodigoMenu':1,'CodigoFuncionalidade':97,'Nome':'Clientes','CodigoPai':3,'Ativo':null,'CaminhoPagina':'/cms/Principal/cliente','Imagem':'/iconpack.axd/16/client_account_template','Ordem':2,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/cms/cliente','Css':null,'sbItens':[],'Url':'/cms/Principal/cliente'},{'Codigo':36,'CodigoMenu':1,'CodigoFuncionalidade':67,'Nome':'Editor de Arquivos','CodigoPai':4,'Ativo':null,'CaminhoPagina':'/cms/Principal/editorarquivos','Imagem':'/iconpack.axd/16/file_manager','Ordem':2,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/cms/editorarquivos','Css':null,'sbItens':[],'Url':'/cms/Principal/editorarquivos'},{'Codigo':39,'CodigoMenu':1,'CodigoFuncionalidade':100,'Nome':'Auditoria','CodigoPai':5,'Ativo':null,'CaminhoPagina':'/admin/auditoria/completo','Imagem':'/iconpack.axd/16/database_error','Ordem':2,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/admin/auditoria','Css':null,'sbItens':[],'Url':'/admin/auditoria/completo'},{'Codigo':43,'CodigoMenu':1,'CodigoFuncionalidade':97,'Nome':'Clientes','CodigoPai':16,'Ativo':null,'CaminhoPagina':'/admin/clienteadm','Imagem':'/iconpack.axd/16/client_account_template','Ordem':2,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/cms/cliente','Css':null,'sbItens':[],'Url':'/admin/clienteadm'},{'Codigo':40,'CodigoMenu':1,'CodigoFuncionalidade':113,'Nome':'Log de Erros','CodigoPai':5,'Ativo':null,'CaminhoPagina':'/admin/logerro','Imagem':'/iconpack.axd/16/bug','Ordem':3,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/admin/logerro','Css':null,'sbItens':[],'Url':'/admin/logerro'},{'Codigo':37,'CodigoMenu':1,'CodigoFuncionalidade':127,'Nome':'Limpar Cache','CodigoPai':4,'Ativo':null,'CaminhoPagina':'/cms/Principal/LimparCache','Imagem':'/iconpack.axd/16/broom','Ordem':3,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/cms/LimparCache','Css':null,'sbItens':[],'Url':'/cms/Principal/LimparCache'},{'Codigo':31,'CodigoMenu':1,'CodigoFuncionalidade':71,'Nome':'Tradução','CodigoPai':3,'Ativo':null,'CaminhoPagina':'/cms/Principal/traduzir','Imagem':'/iconpack.axd/16/application_form_edit','Ordem':3,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/cms/traduzir','Css':null,'sbItens':[],'Url':'/cms/Principal/traduzir'},{'Codigo':4,'CodigoMenu':1,'CodigoFuncionalidade':null,'Nome':'Desenvolvedor','CodigoPai':0,'Ativo':null,'CaminhoPagina':null,'Imagem':'/iconpack.axd/16/user_ninja','Ordem':3,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':null,'Css':null,'sbItens':[],'Url':null},{'Codigo':5,'CodigoMenu':1,'CodigoFuncionalidade':null,'Nome':'Administrador do Sistema','CodigoPai':0,'Ativo':null,'CaminhoPagina':null,'Imagem':'/iconpack.axd/16/user_suit','Ordem':4,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':null,'Css':null,'sbItens':[],'Url':null},{'Codigo':16,'CodigoMenu':1,'CodigoFuncionalidade':null,'Nome':'Segurança','CodigoPai':5,'Ativo':null,'CaminhoPagina':null,'Imagem':null,'Ordem':4,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':null,'Css':'fa-angle-double-right','sbItens':[],'Url':null},{'Codigo':32,'CodigoMenu':1,'CodigoFuncionalidade':71,'Nome':'Tradução Admin','CodigoPai':3,'Ativo':null,'CaminhoPagina':'/cms/Principal/traduzir? adm = 1','Imagem':'/iconpack.axd/16/application_form_edit','Ordem':4,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/cms/traduzir','Css':null,'sbItens':[],'Url':'/cms/Principal/traduzir? adm = 1'},{'Codigo':33,'CodigoMenu':1,'CodigoFuncionalidade':95,'Nome':'Lista Config','CodigoPai':3,'Ativo':null,'CaminhoPagina':'/cms/Principal/ListaAdmin/ListaConfig','Imagem':'/iconpack.axd/16/application_view_tile','Ordem':5,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':'/cms/listaadmin','Css':null,'sbItens':[],'Url':'/cms/Principal/ListaAdmin/ListaConfig'},{'Codigo':6,'CodigoMenu':1,'CodigoFuncionalidade':null,'Nome':'Treinamento','CodigoPai':0,'Ativo':null,'CaminhoPagina':null,'Imagem':'/iconpack.axd/16/books','Ordem':5,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':null,'Css':null,'sbItens':[],'Url':null},{'Codigo':10,'CodigoMenu':1,'CodigoFuncionalidade':null,'Nome':'Navegação','CodigoPai':2,'Ativo':null,'CaminhoPagina':null,'Imagem':null,'Ordem':6,'AbrirNovaPagina':false,'PadraoParaPortal':true,'UrlFuncionalidade':null,'Css':'fa-angle-double-right','sbItens':[],'Url':null}],'Codigo':1,'CodigoPortal':1,'Nome':'Sidebar','Ativo':true}";
        }
    }

    /// <summary>
    ///     Model que extende a model de Menu e contempla as listas
    /// </summary>
    [Serializable]
    [Table("FWK_MEN_MENU")]
    public class MLMenuCompleto : MLMenu
    {
        /// <summary>
        ///     Construtor da classe, inicializa as variáveis
        /// </summary>
        public MLMenuCompleto()
        {
            ItensMenu = new List<MLMenuItem>();
        }

        public List<MLMenuItem> ItensMenu { get; set; }

    }

    /// <summary> 
    /// Model da Entidade MenuItem 
    /// </summary> 
    [Serializable]
    [Table("FWK_MNI_MENU_ITEM")]
    public class MLMenuItem : IComparable<MLMenuItem>
    {
        public MLMenuItem()
        {
            Filhos = new List<MLMenuItem>();
        }

        [DataField("MNI_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("MNI_MEN_N_CODIGO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? CodigoMenu { get; set; }

        [DataField("MNI_FUN_N_CODIGO", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? CodigoFuncionalidade { get; set; }

        [DataField("MNI_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [DataField("MNI_N_CODIGO_PAI", SqlDbType.Decimal, 18, IgnoreEmpty = true)]
        public decimal? CodigoPai { get; set; }

        [DataField("MNI_B_STATUS", SqlDbType.Bit)]
        public bool? Ativo { get; set; }

        [DataField("MNI_C_CAMINHO_PAGINA", SqlDbType.VarChar, 300)]
        public string CaminhoPagina { get; set; }

        [DataField("MNI_C_IMAGEM", SqlDbType.VarChar, 300)]
        public string Imagem { get; set; }

        [DataField("MNI_N_ORDEM", SqlDbType.Int, IgnoreEmpty = true)]
        public int? Ordem { get; set; }

        [DataField("MNI_B_NOVA_PAGINA", SqlDbType.Bit)]
        public bool? AbrirNovaPagina { get; set; }

        [DataField("MNI_B_PADRAO", SqlDbType.Bit)]
        public bool? PadraoParaPortal { get; set; }

        [DataField("MNI_B_EXIBIR_MODULOS", SqlDbType.Bit)]
        public bool? ExibirModulosLista { get; set; }

        [DataField("FUN_C_URL", SqlDbType.VarChar, 100, IgnoreEmpty = true)]
        public string UrlFuncionalidade { get; set; }

        [DataField("MNI_C_CSS", SqlDbType.VarChar, 100)]
        public string Css { get; set; }

        public List<MLMenuItem> Filhos { get; set; }

        public string Url(string diretorioPortal)
        {
            if (!string.IsNullOrEmpty(CaminhoPagina))
                return CaminhoPagina.Replace("{portal}", diretorioPortal);
            return UrlFuncionalidade;
        }

        /// <summary>
        /// Ordenação dos itens
        /// </summary>
        int IComparable<MLMenuItem>.CompareTo(MLMenuItem other)
        {
            if (this.Ordem.HasValue && !other.Ordem.HasValue) return 1;
            if (!this.Ordem.HasValue && other.Ordem.HasValue) return -1;

            if (this.Ordem == other.Ordem) return 0;

            if (this.Ordem > other.Ordem) return 1;
            if (this.Ordem < other.Ordem) return -1;

            return 0;
        }
    }

    /// <summary> 
    /// Model da Entidade Portais
    /// </summary> 
    [Serializable]
    [Table("FWK_MNI_MENU_ITEM")]
    public class MLMenuItemOrdem
    {
        [DataField("MNI_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("MNI_N_CODIGO_PAI", SqlDbType.Decimal, 18)]
        public decimal? CodigoPai { get; set; }

        [DataField("MNI_N_ORDEM", SqlDbType.Int, IgnoreEmpty = true)]
        public int? Ordem { get; set; }
    }

    [Table("FWK_MEN_MENU_VIEW")]
    public class MLMenuView
    {
        [DataField("MNV_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("MVC_POR_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoPortal { get; set; }

        [DataField("MNV_C_NOME", SqlDbType.VarChar, 200)]
        public string Nome { get; set; }

        [DataField("MNV_C_VIEW", SqlDbType.VarChar, -1)]
        public string View { get; set; }

        [DataField("MNV_C_SCRIPTS", SqlDbType.VarChar, -1)]
        public string Script { get; set; }
    }

    [Serializable]
    public class MLMenuItemAdmin
    {
        public decimal? CodigoAtual { get; set; }
        public List<MLMenuItem> Itens { get; set; }
    }
}


