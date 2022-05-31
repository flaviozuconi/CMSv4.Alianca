using System.Configuration;
using System.ComponentModel;
using System.Collections.Generic;
using System;
using System.Web;

namespace Framework.Utilities
{
    public static partial class BLConfiguracao
    {
        #region Geral 

        #region ArquivoXML

        /// <summary> 
        ///     Caminho dos Arquivo XML de configuração do envio de email automatico
        /// </summary>
        public static string ArquivoXML
        {
            get
            {
                if (ConfigurationManager.AppSettings["VM2.CMS.Conteudo.Produto.DiretorioArquivos.XML"] != null)
                {
                    return ConfigurationManager.AppSettings["VM2.CMS.Conteudo.Produto.DiretorioArquivos.XML"].Replace("%%PORTAL%%", BLPortal.Atual.Diretorio);
                }
                else
                {
                    return "~/Portal/%%PORTAL%%/Produtos/XML/".Replace("%%PORTAL%%", BLPortal.Atual.Diretorio);
                }
            }
        }

        #endregion

        #region Codigo Grupo usuario Exclusivo - Arquivo Restrito

        public static decimal CodigoUsuarioRestrito
        {
            get
            {
                if (ConfigurationManager.AppSettings["VM2.CMS.Modulo.Arquivo.Restrito"] != null)
                    return Convert.ToDecimal(ConfigurationManager.AppSettings["VM2.CMS.Modulo.Arquivo.Restrito"]);
                else
                    return 24;
            }
        }

        #endregion

        #region Codigo Idioma Brasil

        public static decimal CodigoIdiomaBrasil
        {
            get
            {
                if (ConfigurationManager.AppSettings["VM2.IDIOMA.BRASIL"] != null)
                    return Convert.ToDecimal(ConfigurationManager.AppSettings["VM2.IDIOMA.BRASIL"]);
                else
                    return 1;
            }
        }

        #endregion

        #region UrlSSL

        public static string UrlSSL
        {
            get
            {
                if (ConfigurationManager.AppSettings["CMS.Configuracoes.Url.Ssl"] != null)
                    return Convert.ToString(ConfigurationManager.AppSettings["CMS.Configuracoes.Url.Ssl"]);
                else
                    return String.Empty;
            }
        }

        #endregion

        #region Obter

        /// <summary>
        /// Procura o valor no AppSettings converte para o tipo escolhido e retorna o valor.
        /// Caso não exista a propriedade, ou ocorra erro na conversão, retorna valor padrão.
        /// </summary>
        public static Tipo Obter<Tipo>(string chave, Tipo defaultValue)
        {
            try
            {
                if (ConfigurationManager.AppSettings[chave] != null)
                    return (Tipo)Convert.ChangeType(ConfigurationManager.AppSettings[chave], typeof(Tipo));
            }
            catch
            {
            }

            return defaultValue;
        }

        #endregion

        #region Email

        #region Usuário

        /// <summary>
        ///     Usuario para envio de Email
        /// </summary>
        /// <user>mazevedo</user>
        public static string UsuarioEmail
        {
            get
            {
                if( ConfigurationManager.AppSettings[ "VM2.Framework.Email.Usuario" ] != null )
                {
                    return ConfigurationManager.AppSettings[ "VM2.Framework.Email.Usuario" ].ToString( );
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Senha

        /// <summary>
        ///     Senha para envio de Email
        /// </summary>
        /// <user>mazevedo</user>
        public static string SenhaEmail
        {
            get
            {
                if( ConfigurationManager.AppSettings[ "VM2.Framework.Email.Senha" ] != null )
                {
                    return ConfigurationManager.AppSettings[ "VM2.Framework.Email.Senha" ].ToString( );
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Servidor

        /// <summary>
        ///     Servidor para envio de Email
        /// </summary>
        /// <user>mazevedo</user>
        public static string ServidorEmail
        {
            get
            {
                if( ConfigurationManager.AppSettings[ "VM2.Framework.Email.Servidor" ] != null )
                {
                    return ConfigurationManager.AppSettings[ "VM2.Framework.Email.Servidor" ].ToString( );
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Porta

        /// <summary>
        ///     Porta para envio de Email
        /// </summary>
        /// <user>mazevedo</user>
        public static int PortaEmail
        {
            get
            {
                if( ConfigurationManager.AppSettings[ "VM2.Framework.Email.Porta" ] != null )
                {
                    return Convert.ToInt32( ConfigurationManager.AppSettings[ "VM2.Framework.Email.Porta" ] );
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        #endregion

        #region Locais Replicacao

        /// <summary>
        ///     Locais Replicacao
        /// </summary>
        /// <user>vnarcizo</user>
        public static string[] LocaisReplicacao
        {
            get
            {
                if (ConfigurationManager.AppSettings["VM2.CMS.Replicacao.Locais"] != null)
                    return ConfigurationManager.AppSettings["VM2.CMS.Replicacao.Locais"].Split(';');

                return null;
            }
        }

        #region Email Erro replicação arquivo

        /// <summary>
        ///     Email para enviar o erro na replicação
        /// </summary>
        /// <user>rvissontai</user>
        public static string EmailErroReplicacao
        {
            get
            {
                if (ConfigurationManager.AppSettings["VM2.CMS.Replicacao.EmailErro"] != null)
                {
                    return ConfigurationManager.AppSettings["VM2.CMS.Replicacao.EmailErro"];
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #endregion

        #region UrlIntegracaoPerson
        /// <summary>
        /// UrlIntegracaoPerson
        /// </summary>
        public static string UrlIntegracaoPerson
        {
            get
            {
                if (ConfigurationManager.AppSettings["URL.Integracao.Movidesk.Person"] != null)
                    return Convert.ToString(ConfigurationManager.AppSettings["URL.Integracao.Movidesk.Person"]);
                else
                    return "https://api.movidesk.com/public/v1/persons?";
            }
        }

        #endregion

        #region UrlIntegracaoToken
        /// <summary>
        /// UrlIntegracaoToken
        /// </summary>
        public static string UrlIntegracaoToken
        {
            get
            {
                if (ConfigurationManager.AppSettings["URL.Integracao.Movidesk.Token"] != null)
                    return Convert.ToString(ConfigurationManager.AppSettings["URL.Integracao.Movidesk.Token"]);
                else
                    return "35f58971-abb0-40b4-8254-c2ebdf3b05a6";
            }
        }

        #endregion

        #region UrlIntegracaoTicket
        /// <summary>
        /// UrlIntegracaoPerson
        /// </summary>
        public static string UrlIntegracaoTicket
        {
            get
            {
                if (ConfigurationManager.AppSettings["URL.Integracao.Movidesk.Ticket"] != null)
                    return Convert.ToString(ConfigurationManager.AppSettings["URL.Integracao.Movidesk.Ticket"]);
                else
                    return "https://api.movidesk.com/public/v1/tickets?";
            }
        }

        #endregion


        #endregion

        #region Admin
        public static class Admin
        {
            #region TamanhoPaginaGrid

            /// <summary>
            /// Tamanho da Página do Grid
            /// </summary>
            public static int TamanhoPaginaGrid
            {
                get
                {
                    return Obter<int>("Framework.Admin.TamanhoPaginaGrid", 50);
                }
            }

            #endregion

            #region Views

            /// <summary>
            /// Tipos de views disponíveis para um modulo
            /// </summary>
            public static List<string> Views
            {
                get
                {
                    return new List<string>() { "detalhe", "destaque", "lista" };
                }
            }

            #endregion
        }
        #endregion

        #region Pastas

        public static class Pastas
        {
            #region ModuloGenerico

            /// <summary>
            ///  Modulo Arquivos do Portal
            /// </summary>
            public static string ModuloGenerico(string pastaBasePortal, string nomeModulo)
            {
                return string.Format("/Portal/{0}/arquivos/{1}/", pastaBasePortal, nomeModulo);
            }

            #endregion

            #region LayoutsPortal

            /// <summary>
            /// Layouts do Portal
            /// </summary>
            public static string LayoutsPortal(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.Pastas.Layouts", "/portal/{0}/layouts"), pastaBasePortal);
            }

            #endregion

            #region TemplatesPortal

            /// <summary>
            /// Templates do Portal
            /// </summary>
            public static string TemplatesPortal(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.Pastas.Templates", "/portal/{0}/templates"), pastaBasePortal);
            }

            #endregion

            #region ContentPortal

            /// <summary>
            /// Content do Portal
            /// </summary>
            public static string ContentPortal(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.Pastas.Content", "/portal/{0}/content"), pastaBasePortal);
            }

            #endregion

            #region ImagensPortal

            /// <summary>
            /// Imagens do Portal
            /// </summary>
            public static string ImagensPortal(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.Pastas.Imagens", "/portal/{0}/arquivos/imagens"), pastaBasePortal);
            }

            #endregion

            #region FlashPortal

            /// <summary>
            /// Flash do Portal
            /// </summary>
            public static string FlashPortal(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.Pastas.Flash", "/portal/{0}/arquivos/flash"), pastaBasePortal);
            }

            #endregion

            #region DocumentosPortal

            /// <summary>
            /// Documentos do Portal
            /// </summary>
            public static string DocumentosPortal(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.Pastas.Documentos", "/portal/{0}/arquivos/docs"), pastaBasePortal);
            }

            #endregion

            #region ClientesPortal
            public static string ClientesPortal(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.Pastas.Clientes", "/Portal/{0}/arquivos/clientes/"), pastaBasePortal);
            }
            #endregion

            #region ModuloFaleConosco

            /// <summary>
            ///  Modulo Arquivos do Portal
            /// </summary>
            public static string ModuloFaleConoscoEmail(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.Pastas.FaleConosco.ModeloEmail", "/Portal/{0}/arquivos/faleconosco/modeloemail/"), pastaBasePortal);
            }

            public static string ModuloFaleConoscoForm(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.Pastas.FaleConosco.Formularios", "/Portal/{0}/arquivos/faleconosco/formularios/"), pastaBasePortal);
            }

            #endregion

            #region ModuloArquivos

            /// <summary>
            /// Modulo Arquivos do Portal
            /// </summary>
            public static string ModuloArquivos(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.ModuloArquivos.PastaBase", "/portal/{0}/arquivos"), pastaBasePortal);
            }

            /// <summary>
            /// Modulo ConteudoApp
            /// </summary>
            public static string  ModuloConteudoApp(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.ModuloConteudoApp.PastaImagem", "/portal/{0}/arquivos/ConteudoApp"), pastaBasePortal);
            }

            /// <summary>
            /// Modulo Arquivos do Portal
            /// </summary>
            public static string ModuloArquivosThumbImagens(string pastaBasePortal)
            {
                return Obter<string>("CMS.ModuloArquivos.PastaThumbImagens", ModuloArquivos(pastaBasePortal) + "/thumbArquivos");
            }

            /// <summary>
            /// Modulo Arquivos do Portal - Pasta das Imagens dos Arquivos
            /// </summary>
            public static string ModuloArquivosImagens(string pastaBasePortal, decimal CodArquivo)
            {
                return Obter<string>("CMS.ModuloArquivos.PastaImagens", ModuloArquivos(pastaBasePortal) + "/ImagemArquivos/"+ CodArquivo.ToString());
            }

            /// <summary>
            /// Modulo Arquivos do Portal - Pasta das Imagens dos Arquivos
            /// </summary>
            public static string ModuloArquivosImagens(string pastaBasePortal)
            {
                return Obter<string>("CMS.ModuloArquivos.PastaImagens", ModuloArquivos(pastaBasePortal) + "/ImagemArquivos");
            }

            #endregion

            #region ModuloMultimidia

            /// <summary>
            /// Modulo Multimidia do Portal
            /// </summary>
            public static string ModuloMultimidia(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.ModuloMultimidia.PastaBase", "/portal/{0}/Multimidia"), pastaBasePortal);
            }

            /// <summary>
            /// Modulo Multimidia do Portal
            /// </summary>
            public static string ModuloMultimidiaThumbImagens(string pastaBasePortal)
            {
                return Obter<string>("CMS.ModuloMultimidia.PastaThumbImagens", ModuloMultimidia(pastaBasePortal) + "/thumbMultimidia");
            }

            /// <summary>
            /// Modulo Multimidia do Portal - Pasta das Imagens dos Multimidia
            /// </summary>
            public static string ModuloMultimidiaImagens(string pastaBasePortal, decimal CodArquivo)
            {
                return Obter<string>("CMS.ModuloMultimidia.PastaImagens", ModuloMultimidia(pastaBasePortal) + "/ImagemMultimidia/" + CodArquivo.ToString());
            }

            /// <summary>
            /// Modulo Multimidia do Portal - Pasta das Imagens dos Multimidia
            /// </summary>
            public static string ModuloMultimidiaImagens(string pastaBasePortal)
            {
                return Obter<string>("CMS.ModuloMultimidia.PastaImagens", ModuloMultimidia(pastaBasePortal) + "/ImagemMultimidia");
            }

            #endregion

            #region Modulo Arquivo Restrito

            /// <summary>
            /// Modulo Multimidia do Portal
            /// </summary>
            public static string ModuloArquivoRestritoArquivo(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.ModuloArquivoRestrito.PastaBase", "/portal/{0}/ArquivoRestrito/Arquivo"), pastaBasePortal);
            }

            /// <summary>
            /// Modulo Multimidia do Portal
            /// </summary>
            public static string ModuloArquivoRestritoImagem(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.ModuloArquivoRestrito.PastaBase", "/portal/{0}/ArquivoRestrito/Imagem"), pastaBasePortal);
            }

            #endregion  

            #region ModuloListas

            /// <summary>
            /// Modulo Noticias do Portal
            /// </summary>
            public static string ModuloListas(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.ModuloListas.PastaBase", "/portal/{0}/arquivos/listas"), pastaBasePortal);
            }

            #endregion

            #region ModuloRegiaoPais

            /// <summary>
            /// Modulo Noticias do Portal
            /// </summary>
            public static string ModuloRegiaoPais(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.ModuloRegiaoPais.PastaBase", "/portal/{0}/arquivos/RegiaoPais"), pastaBasePortal);
            }

            #endregion

            #region ModuloBanner

            /// <summary>
            /// Modulo Banners do Portal
            /// </summary>
            public static string ModuloBanner(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.ModuloBanner.PastaBase", "/portal/{0}/arquivos/banner"), pastaBasePortal);
            }

            /// <summary>
            /// Modulo Banners do Portal Hover
            /// </summary>
            public static string ModuloBannerHover(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.ModuloBanner.PastaBase.Hover", "/portal/{0}/arquivos/banner/Hover"), pastaBasePortal);
            }

            #endregion

            #region ModuloResultado

            /// <summary>
            /// Modulo  do Portal
            /// </summary>
            public static string ModuloResultado(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.ModuloResultado.PastaBase", "/portal/{0}/arquivos/resultado"), pastaBasePortal);
            }

         

            #endregion

            #region ModuloListaFotos

            /// <summary>
            /// Modulo Noticias do Portal
            /// </summary>
            public static string ModuloListaFotos(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.ModuloListas.PastaBase", "/portal/{0}/arquivos/listas/4656197cf2aa4add8d8d1a0ec721fc64/galeria/"), pastaBasePortal);
            }

            #endregion

            #region ModuloEvento

            /// <summary>
            /// Modulo Evento do Portal
            /// </summary>
            public static string ModuloEvento(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.ModuloEvento.PastaBase", "/portal/{0}/arquivos/Eventos"), pastaBasePortal);
            }

            #endregion

            #region ModuloMenu

            /// <summary>
            /// Modulo Menu do Portal
            /// </summary>
            public static string ModuloMenu(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.ModuloMenu.PastaBase", "/portal/{0}/arquivos/menu"), pastaBasePortal);
            }

            #endregion

            #region ModuloGaleria

            /// <summary>
            /// Modulo Evento do Portal
            /// </summary>
            public static string ModuloGaleria(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.ModuloGaleria.PastaBase", "/portal/{0}/arquivos/galeria"), pastaBasePortal);
            }

            #endregion

            #region ModuloTakeOrPay

            /// <summary>
            ///  Modulo Take Or Pay do Portal
            /// </summary>
            public static string ModuloTakeOrPayEmail(string pastaBasePortal)
            {
                return string.Format(Obter<string>("CMS.Pastas.TakeOrPay.ModeloEmail", "/Portal/{0}/arquivos/TakeOrPay/modeloemail/"), pastaBasePortal);
            }


            /// <summary>
            ///  Modulo Take Or Pay do Portal
            /// </summary>
            public static string ModuloTakeOrPayContainer20()
            {
                return Obter<string>("CMS.Pastas.TakeOrPay.Container20", "20'");
            }


            /// <summary>
            ///  Modulo Take Or Pay do Portal
            /// </summary>
            public static string ModuloTakeOrPayNomeNavio()
            {
                return Obter<string>("CMS.Pastas.TakeOrPay.NomeNavio", "disponibilizado pela CONTRATADA");
            }

            /// <summary>
            ///  Modulo Take Or Pay do Portal
            /// </summary>
            public static string ModuloTakeOrPayTextoPropostaComercial()
            {
                return Obter<string>("CMS.Pastas.TakeOrPay.TextoPropostaComercial", "na(s) Proposta(s) Comercial(is) referentes ao BID em vigência na data de assinatura do presente instrumento");
            }

            /// <summary>
            ///  Modulo Take Or Pay do Portal
            /// </summary>
            public static string ModuloTakeOrPayTextoPropostaComercialPropostas()
            {
                return Obter<string>("CMS.Pastas.TakeOrPay.TextoPropostaComercialPropostas", "na(s) Proposta(s) Comercial(s) n(s). {0} (Anexo I)");
            }

            #endregion
        }

        #endregion

        #region Instalacao

        public static class Instalacao
        {
            public static string Pasta
            {
                get { return "~/Portal/_Instalacao/"; }
            }

            public static string ScriptCreateDataBase
            {
                get { return string.Format("{0}SQL/CreateBase.sql", Pasta); }
            }

            public static string ScriptTables
            {
                get { return string.Format("{0}SQL/Estrutura-Tabelas.sql", Pasta); }
            }

            public static string ScriptProcedures
            {
                get { return string.Format("{0}/SQL/Estrutura-Procedures.sql", Pasta); }
            }

            public static string ScriptDados
            {
                get { return string.Format("{0}/SQL/Dados.sql", Pasta); }
            }

            public static string PastaConteudo
            {
                get { return string.Format("{0}/Conteudo/", Pasta); }
            }

            public static string ArquivoConteudoDefault
            {
                get { return string.Format("{0}Default.zip", PastaConteudo); }
            }

            public static List<MLPortalInstalacao.ETAPAS> Etapas(string tipo)
            {
                if (tipo == "A")
                    return EtapasBancoAtual;
                else if (tipo == "E")
                    return EtapasBancoExistente;
                else
                    return EtapasBancoNovo;
            }


            public static List<MLPortalInstalacao.ETAPAS> EtapasBancoAtual
            {
                get
                {
                    return new List<MLPortalInstalacao.ETAPAS> { 
                        MLPortalInstalacao.ETAPAS.MENUSDEFAULT, 
                        MLPortalInstalacao.ETAPAS.CONTEUDO 
                    };
                }
            }

            public static List<MLPortalInstalacao.ETAPAS> EtapasBancoExistente
            {
                get
                {
                    return new List<MLPortalInstalacao.ETAPAS> { 
                        MLPortalInstalacao.ETAPAS.CRIARESTRUTURA, 
                        MLPortalInstalacao.ETAPAS.MENUSDEFAULT,
                        MLPortalInstalacao.ETAPAS.CONTEUDO
                    };
                }
            }

            public static List<MLPortalInstalacao.ETAPAS> EtapasBancoNovo
            {
                get
                {
                    return new List<MLPortalInstalacao.ETAPAS> { 
                        MLPortalInstalacao.ETAPAS.CRIARBD,
                        MLPortalInstalacao.ETAPAS.CRIARESTRUTURA,
                        MLPortalInstalacao.ETAPAS.MENUSDEFAULT,
                        MLPortalInstalacao.ETAPAS.CONTEUDO
                    };
                }
            }

        }

        #endregion
    }
}
