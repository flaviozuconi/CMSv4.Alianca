using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Framework.DataLayer;
using Framework.Utilities;
using System.IO;
using CMSv4.Model;

namespace CMSv4.BusinessLayer
{
    public enum TipoGaleria
    {
        Arquivos = 1,
        Imagens = 2,
        Flash = 3
    }

    public class BLGaleria
    {

        #region Diretorio Virtual Raiz

        private static string ObterDiretorioVirtualRaiz(TipoGaleria tipo, string pastaPortal)
        {
            switch (tipo)
            {
                case TipoGaleria.Arquivos: return BLConfiguracao.Pastas.DocumentosPortal(pastaPortal);
                case TipoGaleria.Imagens: return BLConfiguracao.Pastas.ImagensPortal(pastaPortal);
                case TipoGaleria.Flash: return BLConfiguracao.Pastas.FlashPortal(pastaPortal);
            }

            // Não deve acessar pastas que não sejam pastas de galeria

            throw new Exception("Tipo de galeria inválido");
        }

        #endregion

        // Pastas

        //#region ListarPastas

        /// <summary>
        /// Listar as pastas de um subdiretorio para área administrativa de galeria
        /// </summary>
        public static List<string> ListarDiretorioPortal()
        {
            try
            {

                List<string> lstRetorno = new List<string>();

                var diretorioVirtualRaiz = "/";

                var diretorioRaiz = HttpContext.Current.Server.MapPath(diretorioVirtualRaiz);
                if (!Directory.Exists(diretorioRaiz)) Directory.CreateDirectory(diretorioRaiz);

                var diretorioVirtual = (diretorioVirtualRaiz + "/portal" ).Replace("//", "/");
                string diretorioLista = HttpContext.Current.Server.MapPath(diretorioVirtual);

                if (!Directory.Exists(diretorioLista) || !diretorioLista.StartsWith(diretorioRaiz)) return null;

                var diretorios = Directory.GetDirectories(diretorioLista);

                //foreach (var diretorio in diretorios)
                //{
                //    var texto = Path.GetFileNameWithoutExtension(diretorio);

                //    if (texto == "_thumb") continue;

                //    var nodeItem = new Ext.Net.Node
                //    {
                //        Text = texto,
                //        DataPath = subDiretorio + "/" + texto
                //    };

                //    lista.Add(nodeItem);
                //}

                return lstRetorno;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        ///// <summary>
        ///// Listar as pastas de um subdiretorio para área administrativa de galeria
        ///// </summary>
        public static List<MLPasta> ListarPastas(string dataPath)
        {
            try
            {
                if (dataPath == null)
                    dataPath = string.Empty;

                var diretorioVirtualRaiz = String.Format("/portal/{0}/", BLPortal.Atual.Diretorio);
                var diretorioRaiz = HttpContext.Current.Server.MapPath(diretorioVirtualRaiz);
                var diretorioVirtual = diretorioVirtualRaiz + dataPath.Replace("|", "/");
                var dirInfoRaiz = new DirectoryInfo(diretorioRaiz);

                if (dirInfoRaiz.Exists)
                {
                    var diretorioLista = HttpContext.Current.Server.MapPath(diretorioVirtual);
                    var dInfo = new DirectoryInfo(diretorioLista);

                    if (dInfo.Exists)
                    {
                        //Todas as pastas do diretório em uma única string separada por vírgula
                        var aInfo = string.Join(",", dInfo.GetDirectories().Select(s => dataPath + "|" + s.Name).ToArray());

                        //Obter todas as pastas liberadas e as pastas que permitidas (por grupos do usuário).
                        var pastasPermitidas = ListarPastaPermissoes(aInfo);

                        var retorno = new List<MLPasta>();

                        foreach (var diretorio in dInfo.GetDirectories())
                        {
                            retorno.Add(new MLPasta()
                            {
                                Caminho = string.Concat(dataPath, "|", diretorio.Name),
                                Restrito = pastasPermitidas.Find(o => o.Caminho == dataPath + "|" + diretorio.Name) != null
                            });
                        }

                        return retorno;

                        //var pasta = CRUD.Obter(new MLPasta() { Caminho = dataPath }) ?? new MLPasta();
                        //var permissoes = CRUD.Listar(new MLPastaPermissao() { CodigoDiretorio = pasta?.Codigo }) ?? new List<MLPastaPermissao>();

                        //var codigosPermissaoPasta = permissoes.Select(s => s.CodigoGrupo);

                        //var liberado = BLUsuario.ObterLogado().Grupos.Select(o => o.CodigoGrupo).Intersect(codigosPermissaoPasta);

                        //return (from e in dInfo.GetDirectories()
                        //                  select new MLPasta()
                        //                  {
                        //                      Caminho = string.Concat(dataPath, "|", e.Name),
                        //                      Restrito = (!pasta.Restrito.HasValue || !pasta.Restrito.Value) || (liberado != null && liberado.Count() > 0)
                        //                    }
                        //                ).ToList();
                    }
                }

                return new List<MLPasta>();
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        public static List<MLPasta> ListarPastaPermissoes(string pastas)
        {
            var grupos = string.Join(",", BLUsuario.ObterLogado().Grupos.Select(s => s.CodigoGrupo).ToArray());

            using (var command = Database.NewCommand("USP_MOD_BPR_PRO_L_PASTA_PERMISSOES", BLPortal.Atual.ConnectionString))
            {
                // Parametros
                command.NewCriteriaParameter("@PASTAS", SqlDbType.VarChar, -1, pastas);
                command.NewCriteriaParameter("@GRUPOS_USER", SqlDbType.VarChar, -1, grupos);

                // Execucao
                return Database.ExecuteReader<MLPasta>(command);
            }
        }
        ///// <summary>
        ///// Listar as pastas de um subdiretorio para área administrativa de galeria
        ///// </summary>
        //public static NodeCollection ListarPastas(TipoGaleria tipo, string pastaPortal, string subDiretorio)
        //{
        //    try
        //    {
        //        var lista = new Ext.Net.NodeCollection(false);

        //        var diretorioVirtualRaiz = ObterDiretorioVirtualRaiz(tipo, pastaPortal);

        //        var diretorioRaiz = HttpContext.Current.Server.MapPath(diretorioVirtualRaiz);
        //        if (!Directory.Exists(diretorioRaiz)) Directory.CreateDirectory(diretorioRaiz);

        //        var diretorioVirtual = (diretorioVirtualRaiz + "/" + subDiretorio).Replace("//", "/");
        //        string diretorioLista = HttpContext.Current.Server.MapPath(diretorioVirtual);

        //        if (!Directory.Exists(diretorioLista) || !diretorioLista.StartsWith(diretorioRaiz)) return null;

        //        var diretorios = Directory.GetDirectories(diretorioLista);

        //        foreach (var diretorio in diretorios)
        //        {
        //            var texto = Path.GetFileNameWithoutExtension(diretorio);

        //            if (texto == "_thumb") continue;

        //            var nodeItem = new Ext.Net.Node
        //            {
        //                Text = texto,
        //                DataPath = subDiretorio + "/" + texto
        //            };

        //            lista.Add(nodeItem);
        //        }

        //        return lista;
        //    }
        //    catch (Exception ex)
        //    {
        //        ApplicationLog.ErrorLog(ex);
        //        throw;
        //    }
        //}

        //#endregion


        #region Nova Pasta

        /// <summary>
        /// Nova pasta
        /// </summary>
        /// <remarks>
        public static bool NovaPasta(TipoGaleria tipo, string pastaPortal, string subDiretorio, string novaPasta)
        {
            try
            {
                novaPasta = novaPasta.Replace(" ", "_");
                novaPasta = novaPasta.Replace("/", "_");
                novaPasta = novaPasta.Replace("*", "_");
                novaPasta = novaPasta.Replace(":", "_");
                novaPasta = novaPasta.Replace("%", "_");
                novaPasta = novaPasta.Replace("#", "_");
                novaPasta = novaPasta.Replace("@", "_");
                novaPasta = novaPasta.Replace("!", "_");
                novaPasta = novaPasta.Replace("&", "_");
                novaPasta = novaPasta.Replace("+", "_");
                novaPasta = novaPasta.Replace("=", "_");
                novaPasta = novaPasta.Replace("<", "_");
                novaPasta = novaPasta.Replace(">", "_");
                novaPasta = novaPasta.Replace("'", "_");
                novaPasta = novaPasta.Replace("\"", "_");

                var diretorioVirtualRaiz = ObterDiretorioVirtualRaiz(tipo, pastaPortal);

                string diretorioRaiz = HttpContext.Current.Server.MapPath(diretorioVirtualRaiz);

                if (!Directory.Exists(diretorioRaiz)) Directory.CreateDirectory(diretorioRaiz);

                var diretorioVirtual = (diretorioVirtualRaiz + "/" + subDiretorio + "/" + novaPasta).Replace("//", "/");
                string diretorioNovo = HttpContext.Current.Server.MapPath(diretorioVirtual);

                Directory.CreateDirectory(diretorioNovo);

                return true;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        public static bool NovaPasta(string diretorio, string novaPasta)
        {
            novaPasta = novaPasta.Replace(" ", "_");
            novaPasta = novaPasta.Replace("/", "_");
            novaPasta = novaPasta.Replace("*", "_");
            novaPasta = novaPasta.Replace(":", "_");
            novaPasta = novaPasta.Replace("%", "_");
            novaPasta = novaPasta.Replace("#", "_");
            novaPasta = novaPasta.Replace("@", "_");
            novaPasta = novaPasta.Replace("!", "_");
            novaPasta = novaPasta.Replace("&", "_");
            novaPasta = novaPasta.Replace("+", "_");
            novaPasta = novaPasta.Replace("=", "_");
            novaPasta = novaPasta.Replace("<", "_");
            novaPasta = novaPasta.Replace(">", "_");
            novaPasta = novaPasta.Replace("'", "_");
            novaPasta = novaPasta.Replace("\"", "_");

            var diretorioVirtualRaiz = String.Format("~/portal/{0}/", BLPortal.Atual.Diretorio);
            var diretorioRaiz = string.Empty;
            var diretorioCriacao = string.Empty;
            var diretorioVirtual = string.Empty;

            if (!string.IsNullOrEmpty(diretorio))
                diretorioVirtual = diretorioVirtualRaiz + diretorio.Replace("|", "/") + "/" + novaPasta;
            else
                diretorioVirtual = diretorioVirtualRaiz + novaPasta;

            diretorioCriacao = HttpContext.Current.Server.MapPath(diretorioVirtual);

            var di = new DirectoryInfo(diretorioCriacao);
            if (di.Exists)
            {
                return false;
            }
            else
            {
                di.Create();
                BLReplicar.Diretorio(diretorioVirtual);
                return true;
            }
        }

        #endregion

        #region Excluir Pasta

        /// <summary>
        /// Excluir pasta
        /// </summary>
        /// <remarks>
        public static bool ExcluirPasta(TipoGaleria tipo, string pastaPortal, string diretorioVirtual)
        {
            try
            {
                var diretorioVirtualRaiz = ObterDiretorioVirtualRaiz(tipo, pastaPortal);

                var diretorio = (diretorioVirtualRaiz + "/" + diretorioVirtual).Replace("//","/");

                return ExcluirPasta(diretorio);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        public static bool ExcluirPasta(string diretorioVirtual)
        {
            var diretorio = HttpContext.Current.Server.MapPath(String.Format("~/portal/{0}/{1}", BLPortal.Atual.Diretorio, diretorioVirtual.Replace("|", "/")));
            var dirInfo = new DirectoryInfo(diretorio);

            if (!dirInfo.Exists)
            {
                //o diretorio nao foi encontrado no servidor
                return false;
            }
            else
            {
                var files = dirInfo.GetFiles();
                var directories = dirInfo.GetDirectories();

                if (files.Count() > 0 || directories.Count() > 0)
                {
                    //não excluir diretorio se houver arquivos
                    return false;
                }
                else
                {
                    dirInfo.Delete();
                    BLReplicar.ExcluirDiretoriosReplicados(String.Format("~/portal/{0}/{1}", BLPortal.Atual.Diretorio, diretorioVirtual.Replace("|", "/")));

                    return true;
                }
            }
        }

        #endregion

        #region Renomear Pasta

        /// <summary>
        /// Renomear pasta
        /// </summary>
        public static bool RenomearPasta(TipoGaleria tipo, string pastaPortal, string dataPath, string novoNome)
        {
            try
            {
                novoNome = novoNome.Replace(" ", "_");
                novoNome = novoNome.Replace("/", "_");
                novoNome = novoNome.Replace("*", "_");
                novoNome = novoNome.Replace(":", "_");
                novoNome = novoNome.Replace("%", "_");
                novoNome = novoNome.Replace("#", "_");
                novoNome = novoNome.Replace("@", "_");
                novoNome = novoNome.Replace("!", "_");
                novoNome = novoNome.Replace("&", "_");
                novoNome = novoNome.Replace("+", "_");
                novoNome = novoNome.Replace("=", "_");
                novoNome = novoNome.Replace("<", "_");
                novoNome = novoNome.Replace(">", "_");
                novoNome = novoNome.Replace("'", "_");
                novoNome = novoNome.Replace("\"", "_");


                var diretorioVirtualRaiz = ObterDiretorioVirtualRaiz(tipo, pastaPortal);

                var diretorioRaiz = HttpContext.Current.Server.MapPath(diretorioVirtualRaiz);

                var diretorioVirtual = (diretorioVirtualRaiz + "/" + dataPath).Replace("//", "/");
                var diretorioVirtualNovo = Path.GetDirectoryName(diretorioVirtual).Replace("\\", "/") + "/" + novoNome;

                if (!Directory.Exists(diretorioRaiz)) Directory.CreateDirectory(diretorioRaiz);

                string diretorioNovo = HttpContext.Current.Server.MapPath(diretorioVirtualNovo);
                if (!diretorioNovo.StartsWith(diretorioRaiz)) return false;

                string diretorioAntigo = HttpContext.Current.Server.MapPath(diretorioVirtual);
                if (!diretorioAntigo.StartsWith(diretorioRaiz)) return false;

                Directory.Move(diretorioAntigo, diretorioNovo);
                BLReplicar.RenomearDiretorio(String.Format("~/portal/{0}/{1}", BLPortal.Atual.Diretorio, diretorioVirtual.Replace("|", "/")), String.Format("~/portal/{0}/{1}", BLPortal.Atual.Diretorio, diretorioVirtualNovo));

                return true;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        public static bool RenomearPasta(string diretorioVirtualAntigo, string novoNome)
        {
            novoNome = novoNome.Replace(" ", "_");
            novoNome = novoNome.Replace("/", "_");
            novoNome = novoNome.Replace("*", "_");
            novoNome = novoNome.Replace(":", "_");
            novoNome = novoNome.Replace("%", "_");
            novoNome = novoNome.Replace("#", "_");
            novoNome = novoNome.Replace("@", "_");
            novoNome = novoNome.Replace("!", "_");
            novoNome = novoNome.Replace("&", "_");
            novoNome = novoNome.Replace("+", "_");
            novoNome = novoNome.Replace("=", "_");
            novoNome = novoNome.Replace("<", "_");
            novoNome = novoNome.Replace(">", "_");
            novoNome = novoNome.Replace("'", "_");
            novoNome = novoNome.Replace("\"", "_");

            var dir = string.Concat(diretorioVirtualAntigo.Substring(0, diretorioVirtualAntigo.LastIndexOf("|")), "|", novoNome).Replace("|", "/");
            var diretorioVirtualAntes = HttpContext.Current.Server.MapPath(String.Format("~/portal/{0}/{1}", BLPortal.Atual.Diretorio, diretorioVirtualAntigo.Replace("|", "/")));
            var diretorioVirtualDepois = HttpContext.Current.Server.MapPath(String.Format("~/portal/{0}/{1}", BLPortal.Atual.Diretorio, dir));
            var dirInfo = new DirectoryInfo(diretorioVirtualAntes);

            if (dirInfo.Exists)
            {
                dirInfo.MoveTo(diretorioVirtualDepois);
                BLReplicar.RenomearDiretorio(String.Format("~/portal/{0}/{1}", BLPortal.Atual.Diretorio, diretorioVirtualAntigo.Replace("|", "/")), String.Format("~/portal/{0}/{1}", BLPortal.Atual.Diretorio, dir));
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        // Arquivos

        #region Listar Arquivos

        /// <summary>
        /// Listagem
        /// </summary>
        public static List<MLGaleriaArquivo> ListarArquivos(TipoGaleria tipo, string pastaPortal, string dataPath)
        {
            try
            {
                // Busca lista no banco de dados

                var lista = new List<MLGaleriaArquivo>();

                var diretorioVirtualRaiz = ObterDiretorioVirtualRaiz(tipo, pastaPortal);
                var diretorioRaiz = HttpContext.Current.Server.MapPath(diretorioVirtualRaiz);

                var diretorioVirtual = diretorioVirtualRaiz + dataPath;

                if (Directory.Exists(diretorioRaiz))
                {
                    string diretorioLista = HttpContext.Current.Server.MapPath(diretorioVirtual);
                    if (!Directory.Exists(diretorioLista) || !diretorioLista.StartsWith(diretorioRaiz)) return null;

                    var dInfo = new DirectoryInfo(diretorioLista);

                    var aInfo = dInfo.GetFiles();

                    foreach (var arquivo in aInfo)
                    {
                        lista.Add(new MLGaleriaArquivo
                        {
                            Nome = arquivo.Name,
                            Caminho = dataPath,
                            DataCriacao = arquivo.CreationTime,
                            UltimoAcesso = arquivo.LastAccessTime,
                            UltimaAlteracao = arquivo.LastWriteTime,
                            Tamanho = arquivo.Length,
                            IconUrl = GetFileIconUrl(arquivo.Extension)
                        }
                        );
                    }
                }

                // Retorna os resultados
                return lista;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Listar Tudo
        /// </summary>
        public static List<MLGaleriaArquivo> ListarArquivos(string dataPath)
        {
            try
            {
                // Busca lista no banco de dados

                var lista = new List<MLGaleriaArquivo>();

                var diretorioVirtualRaiz = String.Format("~/portal/{0}/", BLPortal.Atual.Diretorio);
                var diretorioRaiz = HttpContext.Current.Server.MapPath(diretorioVirtualRaiz);
                var diretorioVirtual = diretorioVirtualRaiz + dataPath.Replace("|","/");

                if (Directory.Exists(diretorioRaiz))
                {
                    string diretorioLista = HttpContext.Current.Server.MapPath(diretorioVirtual);
                    if (!Directory.Exists(diretorioLista) || !diretorioLista.StartsWith(diretorioRaiz)) return null;

                    var dInfo = new DirectoryInfo(diretorioLista);

                    var aInfo = dInfo.GetFiles();

                    foreach (var arquivo in aInfo)
                    {
                        lista.Add(new MLGaleriaArquivo
                        {
                            Nome = arquivo.Name,
                            Caminho = dataPath,
                            DataCriacao = arquivo.CreationTime,
                            UltimoAcesso = arquivo.LastAccessTime,
                            UltimaAlteracao = arquivo.LastWriteTime,
                            Tamanho = arquivo.Length,
                            IconUrl = GetFileIconUrl(arquivo.Extension),
                            Extensao = arquivo.Extension,
                            NomeEncriptacao = BLEncriptacao.EncriptarQueryString(diretorioVirtual + "/" + arquivo.Name)
                        }
                        );
                    }
                }

                // Retorna os resultados
                return lista;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Obtem a url do icone conforme extensao do arquivo
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private static string GetFileIconUrl(string extension)
        {
            switch (extension.ToLower().TrimStart('.'))
            {
                case "3gp": return IconPack.Small.file_extension_3gp;
                case "7z": return IconPack.Small.file_extension_7z;
                case "ace": return IconPack.Small.file_extension_ace;
                case "ai": return IconPack.Small.file_extension_ai;
                case "aif": return IconPack.Small.file_extension_aif;
                case "aiff": return IconPack.Small.file_extension_aiff;
                case "amr": return IconPack.Small.file_extension_amr;
                case "asf": return IconPack.Small.file_extension_asf;
                case "asx": return IconPack.Small.file_extension_asx;
                case "bat": return IconPack.Small.file_extension_bat;
                case "bin": return IconPack.Small.file_extension_bin;
                case "bmp": return IconPack.Small.file_extension_bmp;
                case "bup": return IconPack.Small.file_extension_bup;
                case "cab": return IconPack.Small.file_extension_cab;
                case "cbr": return IconPack.Small.file_extension_cbr;
                case "cda": return IconPack.Small.file_extension_cda;
                case "cdl": return IconPack.Small.file_extension_cdl;
                case "cdr": return IconPack.Small.file_extension_cdr;
                case "chm": return IconPack.Small.file_extension_chm;
                case "dat": return IconPack.Small.file_extension_dat;
                case "divx": return IconPack.Small.file_extension_divx;
                case "dll": return IconPack.Small.file_extension_dll;
                case "dmg": return IconPack.Small.file_extension_dmg;
                case "doc": return IconPack.Small.file_extension_doc;
                case "docx": return IconPack.Small.file_extension_doc;
                case "dss": return IconPack.Small.file_extension_dss;
                case "dvf": return IconPack.Small.file_extension_dvf;
                case "dwg": return IconPack.Small.file_extension_dwg;
                case "eml": return IconPack.Small.file_extension_eml;
                case "eps": return IconPack.Small.file_extension_eps;
                case "exe": return IconPack.Small.file_extension_exe;
                case "fla": return IconPack.Small.file_extension_fla;
                case "flv": return IconPack.Small.file_extension_flv;
                case "gif": return IconPack.Small.file_extension_gif;
                case "gz": return IconPack.Small.file_extension_gz;
                case "hqx": return IconPack.Small.file_extension_hqx;
                case "htm": return IconPack.Small.file_extension_htm;
                case "html": return IconPack.Small.file_extension_html;
                case "ifo": return IconPack.Small.file_extension_ifo;
                case "indd": return IconPack.Small.file_extension_indd;
                case "iso": return IconPack.Small.file_extension_iso;
                case "jar": return IconPack.Small.file_extension_jar;
                case "jpeg": return IconPack.Small.file_extension_jpeg;
                case "jpg": return IconPack.Small.file_extension_jpg;
                case "lnk": return IconPack.Small.file_extension_lnk;
                case "log": return IconPack.Small.file_extension_log;
                case "m4a": return IconPack.Small.file_extension_m4a;
                case "m4b": return IconPack.Small.file_extension_m4b;
                case "m4p": return IconPack.Small.file_extension_m4p;
                case "m4v": return IconPack.Small.file_extension_m4v;
                case "mcd": return IconPack.Small.file_extension_mcd;
                case "mdb": return IconPack.Small.file_extension_mdb;
                case "mid": return IconPack.Small.file_extension_mid;
                case "mov": return IconPack.Small.file_extension_mov;
                case "mp2": return IconPack.Small.file_extension_mp2;
                case "mp4": return IconPack.Small.file_extension_mp4;
                case "mpeg": return IconPack.Small.file_extension_mpeg;
                case "mpg": return IconPack.Small.file_extension_mpg;
                case "msi": return IconPack.Small.file_extension_msi;
                case "mswmm": return IconPack.Small.file_extension_mswmm;
                case "ogg": return IconPack.Small.file_extension_ogg;
                case "pdf": return IconPack.Small.file_extension_pdf;
                case "png": return IconPack.Small.file_extension_png;
                case "pps": return IconPack.Small.file_extension_pps;
                case "ps": return IconPack.Small.file_extension_ps;
                case "psd": return IconPack.Small.file_extension_psd;
                case "pst": return IconPack.Small.file_extension_pst;
                case "ptb": return IconPack.Small.file_extension_ptb;
                case "pub": return IconPack.Small.file_extension_pub;
                case "qbb": return IconPack.Small.file_extension_qbb;
                case "qbw": return IconPack.Small.file_extension_qbw;
                case "qxd": return IconPack.Small.file_extension_qxd;
                case "ram": return IconPack.Small.file_extension_ram;
                case "rar": return IconPack.Small.file_extension_rar;
                case "rm": return IconPack.Small.file_extension_rm;
                case "rmvb": return IconPack.Small.file_extension_rmvb;
                case "rtf": return IconPack.Small.file_extension_rtf;
                case "sea": return IconPack.Small.file_extension_sea;
                case "ses": return IconPack.Small.file_extension_ses;
                case "sit": return IconPack.Small.file_extension_sit;
                case "sitx": return IconPack.Small.file_extension_sitx;
                case "ss": return IconPack.Small.file_extension_ss;
                case "swf": return IconPack.Small.file_extension_swf;
                case "tgz": return IconPack.Small.file_extension_tgz;
                case "thm": return IconPack.Small.file_extension_thm;
                case "tif": return IconPack.Small.file_extension_tif;
                case "tmp": return IconPack.Small.file_extension_tmp;
                case "torrent": return IconPack.Small.file_extension_torrent;
                case "ttf": return IconPack.Small.file_extension_ttf;
                case "txt": return IconPack.Small.file_extension_txt;
                case "vcd": return IconPack.Small.file_extension_vcd;
                case "vob": return IconPack.Small.file_extension_vob;
                case "wav": return IconPack.Small.file_extension_wav;
                case "wma": return IconPack.Small.file_extension_wma;
                case "wmv": return IconPack.Small.file_extension_wmv;
                case "wps": return IconPack.Small.file_extension_wps;
                case "xls": return IconPack.Small.file_extension_xls;
                case "xlsx": return IconPack.Small.file_extension_xls;
                case "xpi": return IconPack.Small.file_extension_xpi;
                case "zip": return IconPack.Small.file_extension_zip;

                default: return IconPack.Small.document_empty;
            }
        }

        #endregion

        #region Excluir

        /// <summary>
        /// Excluir arquivo
        /// </summary>
        public static bool Excluir(TipoGaleria tipo, string pastaPortal, string diretorioVirtual)
        {
            try
            {
                var diretorioVirtualRaiz = ObterDiretorioVirtualRaiz(tipo, pastaPortal);

                var arquivo = (diretorioVirtualRaiz + "/" + diretorioVirtual).Replace("//", "/");

                return Excluir(arquivo);
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return false;
            }
        }

        /// <summary>
        /// Excluir arquivo
        /// </summary>
        public static bool Excluir(string arquivoDiretorioVirtual)
        {
            try
            {
                var arquivo = HttpContext.Current.Server.MapPath(arquivoDiretorioVirtual);
                var fileInfo = new FileInfo(arquivo);
                
                if (!fileInfo.Exists)
                    return false;

                if (fileInfo.IsReadOnly)
                {
                    fileInfo.IsReadOnly = false;
                    fileInfo.Refresh();
                }

                fileInfo.Delete();

                return true;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return false;
            }
        }

        #endregion

    }
}
