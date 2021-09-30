using CMSv4.Model;
using Framework.Utilities;
using System;
using System.IO;
using System.IO.Compression;

namespace CMSv4.BusinessLayer
{
    public class BLReplicar
    {
        public static string ReverseMapPath(string path)
        {
            string appPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string res = string.Format("~{0}", path.Replace(appPath, "").Replace("\\", "/"));
            return res;
        }

        #region ReplicarArquivo

        #region Arquivo

        public static void Arquivo(string arquivo)
        {
            Arquivo(arquivo, ReverseMapPath(Path.GetDirectoryName(arquivo)));
        }

        /// <summary>
        /// Replica o arquivo se houver locais
        /// </summary>
        /// <param name="arquivo">Arquivo com caminho completo (Map Path)</param>
        /// <param name="strDiretorioVirtual">Diretório Virtual onde o arquivo será salvo</param>
        /// <user>rvissontai</user>
        public static void Arquivo(string arquivo, string strDiretorioVirtual)
        {
            var stream = new MemoryStream(System.IO.File.ReadAllBytes(arquivo));
            byte[] arrByte = new byte[stream.Length];
            stream.Read(arrByte, 0, Convert.ToInt32(stream.Length));

            var blnErro = false;
            var strLocalAtual = string.Empty;
            var blnEnvioEmail = false;
            var nomeArquivo = Path.GetFileName(arquivo);
            string[] lstLocaisReplicacao = BLConfiguracao.LocaisReplicacao;

            if (strDiretorioVirtual.Substring(strDiretorioVirtual.Length - 1, 1) != @"\")
                strDiretorioVirtual += @"\";

            if (lstLocaisReplicacao != null && lstLocaisReplicacao.Length > 0)
            {
                foreach (string strItem in lstLocaisReplicacao)
                {
                    strLocalAtual = strItem;

                    if (!string.IsNullOrEmpty(strItem))
                    {
                        try
                        {
                            string strDiretorioCompleto = strItem + Path.GetDirectoryName(strDiretorioVirtual).Replace("~", "");
                            if (!Directory.Exists(strDiretorioCompleto))
                            {
                                Directory.CreateDirectory(strDiretorioCompleto);
                            }

                            if (File.Exists(strDiretorioCompleto + @"\" + nomeArquivo))
                                File.Delete(strDiretorioCompleto + @"\" + nomeArquivo);

                            using (var fs = new FileStream(strDiretorioCompleto + @"\" + nomeArquivo, FileMode.Create, FileAccess.Write))
                            {
                                fs.Write(arrByte, 0, arrByte.Length);
                                fs.Flush();
                            }
                        }
                        catch (Exception ex)
                        {
                            blnErro = true;
                            var model = new MLHistoricoReplicacao();

                            try
                            {
                                // Gerar Historico de Replicação Inválida
                                model.Arquivo = strDiretorioVirtual + @"\" + arquivo.Replace("//", "").Replace("\\", "");
                                model.Local = strLocalAtual;
                                model.Resultado = ex.Message;
                                model.IsReplicado = false;
                                model.Data = DateTime.Now;

                                CRUD.Salvar(model);

                                if (!blnEnvioEmail)
                                {
                                    if (!string.IsNullOrEmpty(BLConfiguracao.EmailErroReplicacao))
                                    {
                                        var objMLPortal = BLPortal.Portais.Find(a => a.Diretorio.Equals(BLPortal.Url, StringComparison.InvariantCultureIgnoreCase));
                                        string strPortal = string.Empty;

                                        if (objMLPortal != null)
                                            strPortal = objMLPortal.Nome;

                                        BLUtilitarios.EnviarEmail(BLConfiguracao.EmailErroReplicacao, "Ocorreu um erro no CMS do Portal: " + strPortal + ". No Diretorio: " + strItem + " - " + DateTime.Now.ToString("HH:mm:ss"), "Erro na replicação de arquivo");
                                    }
                                }
                            }
                            catch
                            {
                                throw;
                            }
                        }
                    }
                }

                if (blnErro)
                    throw new Exception();
            }
        }

        #endregion

        #region Descompactar

        public static void DescompactarArquivo(string arquivo, string diretorio)
        {
            string[] lstLocaisReplicacao = BLConfiguracao.LocaisReplicacao;
            var strLocalAtual = string.Empty;

            if (diretorio.Substring(diretorio.Length - 1, 1) != @"\")
                diretorio += @"\";

            if (lstLocaisReplicacao != null && lstLocaisReplicacao.Length > 0)
            {
                foreach (string strItem in lstLocaisReplicacao)
                {
                    strLocalAtual = strItem;

                    if (!string.IsNullOrEmpty(strItem))
                    {
                        string strDiretorioCompleto = strItem + Path.GetDirectoryName(diretorio).Replace("~", "");

                        if (!Directory.Exists(strDiretorioCompleto))
                            Directory.CreateDirectory(strDiretorioCompleto);

                        ZipFile.ExtractToDirectory(arquivo, strDiretorioCompleto);
                    }
                }
            }
        }

        #endregion

        #region ExcluirArquivosReplicados

        public static void ExcluirArquivosReplicados(string pstrNomeArquivo)
        {
            ExcluirArquivosReplicados(ReverseMapPath(Path.GetDirectoryName(pstrNomeArquivo)), pstrNomeArquivo);
        }

        /// <summary>
        /// Exclui os arquivos replicados
        /// </summary>
        /// <param name="strDiretorio">Diretório Auxiliar</param>
        /// <param name="pstrNomeArquivo">Nome do Arquivo</param>
        public static void ExcluirArquivosReplicados(string strDiretorioVirtual, string pstrNomeArquivo)
        {
            string[] lstLocaisReplicacao = BLConfiguracao.LocaisReplicacao;

            if (lstLocaisReplicacao == null || lstLocaisReplicacao.Length == 0)
                return;

            if (!strDiretorioVirtual.EndsWith(@"\"))
                strDiretorioVirtual += @"\";

            for (int i = 0; i < lstLocaisReplicacao.Length; i++)
            {
                if (string.IsNullOrEmpty(lstLocaisReplicacao[i]))
                    continue;

                try
                {
                    var arquivoParaExclusao = new FileInfo(string.Concat(lstLocaisReplicacao[i], Path.GetDirectoryName(strDiretorioVirtual).Replace("~", ""), @"\", Path.GetFileName(pstrNomeArquivo)));
                    if (arquivoParaExclusao.Exists)
                        arquivoParaExclusao.Delete();
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                }
            }
        }

        #endregion

        #region Mover Arquivo

        /// <summary>
        /// Mover o arquivo do diretório de origem para o diretório de destino
        /// </summary>
        /// <param name="file">Caminho completo para o arquivo original</param>
        /// <param name="fileDest">Caminho completo para o destino</param>
        public static void MoverArquivo(string file, string fileDest)
        {
            string[] lstLocaisReplicacao = BLConfiguracao.LocaisReplicacao;
            var diretorioVirtualOrigem = ReverseMapPath(Path.GetDirectoryName(file));
            var diretorioVirtualDestino = ReverseMapPath(Path.GetDirectoryName(fileDest));

            if (lstLocaisReplicacao != null && lstLocaisReplicacao.Length > 0)
            {
                foreach (string strItem in lstLocaisReplicacao)
                {
                    if (!string.IsNullOrEmpty(strItem))
                    {
                        try
                        {
                            string strDiretorioCompletoOrigem = strItem + diretorioVirtualOrigem.Replace("~", "");
                            string strDiretorioCompletoDestino = strItem + diretorioVirtualDestino.Replace("~", "");

                            if (File.Exists(strDiretorioCompletoOrigem + @"\" + Path.GetFileName(file)))
                                new FileInfo(Path.Combine(strDiretorioCompletoOrigem, Path.GetFileName(file))).MoveTo(Path.Combine(strDiretorioCompletoDestino, Path.GetFileName(fileDest)));
                        }
                        catch (Exception) { }
                    }
                }
            }
        }

        #endregion

        #region Renomear

        /// <summary>
        /// Renovar arquivo nos servidores
        /// </summary>
        /// <param name="file">Nome do arquivo com caminho completo</param>
        /// <param name="newName">Novo nome para o arquivo (file)</param>
        public static void RenomearArquivo(string file, string newName)
        {
            string[] lstLocaisReplicacao = BLConfiguracao.LocaisReplicacao;
            var diretorioVirtual = ReverseMapPath(Path.GetDirectoryName(file));

            if (lstLocaisReplicacao != null && lstLocaisReplicacao.Length > 0)
            {
                foreach (string strItem in lstLocaisReplicacao)
                {
                    if (!string.IsNullOrEmpty(strItem))
                    {
                        try
                        {
                            string strDiretorioCompleto = strItem + diretorioVirtual.Replace("~", "");
                            if (!Directory.Exists(strDiretorioCompleto))
                                Directory.CreateDirectory(strDiretorioCompleto);

                            if (File.Exists(strDiretorioCompleto + @"\" + Path.GetFileName(file)))
                                new FileInfo(Path.Combine(strDiretorioCompleto, Path.GetFileName(file))).MoveTo(Path.Combine(strDiretorioCompleto, newName));
                        }
                        catch (Exception) { }
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Replicar Diretório

        #region Diretório

        /// <summary>
        /// Replicar diretório se houver locais
        /// </summary>
        /// <param name="dir"></param>
        public static void Diretorio(string strDiretorioVirtual)
        {
            string[] lstLocaisReplicacao = BLConfiguracao.LocaisReplicacao;
            var strLocalAtual = string.Empty;

            if (lstLocaisReplicacao != null && lstLocaisReplicacao.Length > 0)
            {
                foreach (string strItem in lstLocaisReplicacao)
                {
                    strLocalAtual = strItem;

                    if (!string.IsNullOrEmpty(strItem))
                    {
                        try
                        {
                            string strDiretorioCompleto = strItem + strDiretorioVirtual.Replace("~", "");

                            if (!Directory.Exists(strDiretorioCompleto))
                                Directory.CreateDirectory(strDiretorioCompleto);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
            }
        }

        #endregion

        #region ExcluirDiretoriosReplicados

        public static void ExcluirDiretoriosReplicados(string strDiretorioVirtual)
        {
            string[] lstLocaisReplicacao = BLConfiguracao.LocaisReplicacao;

            if (lstLocaisReplicacao == null || lstLocaisReplicacao.Length == 0)
                return;

            if (!strDiretorioVirtual.EndsWith(@"\"))
                strDiretorioVirtual += @"\";

            for (int i = 0; i < lstLocaisReplicacao.Length; i++)
            {
                if (string.IsNullOrEmpty(lstLocaisReplicacao[i]))
                    continue;

                try
                {
                    var dirExcluir = new DirectoryInfo(string.Concat(lstLocaisReplicacao[i], Path.GetDirectoryName(strDiretorioVirtual).Replace("~", "")));
                    if (dirExcluir.Exists)
                        dirExcluir.Delete();
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                }
            }
        }

        #endregion

        #region MoverDiretorio

        /// <summary>
        /// Mover o diretório para a localização de destino
        /// </summary>
        /// <param name="dir">Caminho completo do diretório</param>
        /// <param name="dirDest">Caminho completo para o destino</param>
        public static void MoverDiretorio(string dir, string dirDest)
        {
            string[] lstLocaisReplicacao = BLConfiguracao.LocaisReplicacao;
            var diretorioVirtualOrigem = ReverseMapPath(dir);
            var diretorioVirtualDestino = ReverseMapPath(dirDest);

            if (lstLocaisReplicacao != null && lstLocaisReplicacao.Length > 0)
            {
                foreach (string strItem in lstLocaisReplicacao)
                {
                    if (!string.IsNullOrEmpty(strItem))
                    {
                        try
                        {
                            string strDiretorioCompletoOrigem = strItem + diretorioVirtualOrigem.Replace("~", "");
                            string strDiretorioCompletoDestino = strItem + diretorioVirtualDestino.Replace("~", "");

                            if (Directory.Exists(strDiretorioCompletoOrigem))
                                new DirectoryInfo(strDiretorioCompletoOrigem).MoveTo(strDiretorioCompletoDestino);
                        }
                        catch (Exception) { }
                    }
                }
            }
        }

        #endregion

        #region Renomear

        public static void RenomearDiretorio(string diretorioAtual, string diretorioNovo)
        {
            string[] lstLocaisReplicacao = BLConfiguracao.LocaisReplicacao;

            if (lstLocaisReplicacao == null || lstLocaisReplicacao.Length == 0)
                return;

            if (!diretorioAtual.EndsWith(@"\"))
                diretorioAtual += @"\";

            for (int i = 0; i < lstLocaisReplicacao.Length; i++)
            {
                if (string.IsNullOrEmpty(lstLocaisReplicacao[i]))
                    continue;

                try
                {
                    var dir = new DirectoryInfo(string.Concat(lstLocaisReplicacao[i], diretorioAtual.Replace("~", "")));
                    if (dir.Exists)
                        dir.MoveTo(lstLocaisReplicacao[i] + diretorioNovo.Replace("~", ""));
                }
                catch (Exception ex)
                {
                    ApplicationLog.ErrorLog(ex);
                }

            }
        }

        #endregion

        #endregion
    }
}
