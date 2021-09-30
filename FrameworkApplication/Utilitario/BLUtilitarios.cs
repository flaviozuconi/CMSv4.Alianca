using Framework.Model;
using Humanizer;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Globalization;

namespace Framework.Utilities
{
    /// <summary>
    /// Funcoes Gerais
    /// </summary>
    public static class BLUtilitarios
    {
        public static class PageSpeed
        {
            public static int ConvertScore(object score)
            {
                return Convert.ToInt16(Convert.ToDouble(score ?? 0) * 100);
            }

            public static string UrlToShortText(string url)
            {
                var arrayPaths = url.Split('/');

                if (url.Length <= 2)
                    return url;

                return "../" + arrayPaths[arrayPaths.Length - 2] + "/" + arrayPaths[arrayPaths.Length - 1];
            }

            public static string MillisecondsToReadibleTime(string milliseconds)
            {
                return TimeSpan.FromMilliseconds(Convert.ToDouble(milliseconds)).Humanize();
            }

            public static string BytesToReadibleSize(object value)
            {
                return Convert.ToDouble(value)
                    .Bytes()
                    .Humanize("#");
            }
        }

        #region ObterConteudoArquivo

        /// <summary>
        /// Retorna o conteudo de um arquivo solicitado em formato de texto
        /// </summary>
        public static string ObterConteudoArquivo(string caminhoVirtual)
        {
            return ObterConteudoArquivo(caminhoVirtual, null);
        }

        public static string ObterConteudoArquivo(string caminhoVirtual, HttpContext context)
        {
            var con = context;

            if (con == null)
                con = HttpContext.Current;
            try
            {
                if (string.IsNullOrEmpty(caminhoVirtual)) return "";

                string conteudo = "";
                var arquivo = con.Server.MapPath(caminhoVirtual);

                //if (!File.Exists(arquivo))
                //{
                //    var nomeArquivo = System.IO.Path.GetFileName(arquivo);

                //    var databaseArquivo = CRUD.Obter(new MLFaleConoscoArquivo() { Nome = nomeArquivo }, BLPortal.Atual.ConnectionString);

                //    if(databaseArquivo != null && databaseArquivo.Codigo.HasValue)
                //    {
                //        using (var novoArquivo = new StreamWriter(arquivo))
                //        {
                //            novoArquivo.Write(databaseArquivo.Conteudo);
                //            novoArquivo.Close();
                //        }
                //    }

                //    return databaseArquivo.Conteudo;
                //}

                if (!File.Exists(arquivo))
                    return conteudo;

                using (var reader = new StreamReader(arquivo))
                {
                    conteudo = reader.ReadToEnd();

                    reader.Close();
                    reader.Dispose();
                }

                return conteudo;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }
        #endregion

        #region ValidarEmail

        /// <summary>
        /// Validar Formato do Email
        /// </summary>
        public static bool ValidarEmail(string email)
        {
            try
            {
                var emailValido = new System.Net.Mail.MailAddress(email);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region Download

        /// <summary>
        /// Realiza o download de arquivos na internet para o servidor local
        /// </summary>
        /// <returns>
        /// Total de bytes processados pelo download
        /// </returns>
        public static int Download(string remoteFilename, string localFilename)
        {
            // Function will return the number of bytes processed
            // to the caller. Initialize to 0 here.
            int bytesProcessed = 0;

            Stream remoteStream = null;
            Stream localStream = null;
            WebResponse response = null;

            try
            {
                WebRequest request = WebRequest.Create(remoteFilename);

                request.Proxy.Credentials = new NetworkCredential("cveber", "veber041");

                if (request != null)
                {
                    response = request.GetResponse();
                    if (response != null)
                    {
                        remoteStream = response.GetResponseStream();
                        localStream = File.Create(localFilename);

                        byte[] buffer = new byte[1024];
                        int bytesRead;

                        do
                        {
                            bytesRead = remoteStream.Read(buffer, 0, buffer.Length);
                            localStream.Write(buffer, 0, bytesRead);
                            bytesProcessed += bytesRead;

                        } while (bytesRead > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
            finally
            {
                if (response != null) response.Close();
                if (remoteStream != null) remoteStream.Close();
                if (localStream != null) localStream.Close();
            }

            return bytesProcessed;
        }


        #endregion

        #region CSV

        /// <summary>
        /// Transforma uma lista de MODEL em CSV para exportar para excel
        /// </summary>
        public static string ToCSV<TipoModel>(List<TipoModel> lista, string delimitador, Dictionary<string, string> replace = null)
        {
            var sb = new StringBuilder();

            try
            {
                var propriedades = typeof(TipoModel).GetProperties();

                // header
                foreach (var p in propriedades)
                {
                    sb.Append(p.Name);
                    sb.Append(delimitador);
                }
                sb.Remove(sb.Length - delimitador.Length, delimitador.Length);
                sb.AppendLine();

                // conteudo
                foreach (var item in lista)
                {
                    foreach (var p in propriedades)
                    {
                        var conteudo = Convert.ToString(p.GetValue(item, null));

                        //pesquisa em 'conteudo' em 'replace', realiza troca de valor caso seja encontrado
                        if (replace != null && !string.IsNullOrEmpty(conteudo) && replace.ContainsKey(conteudo))
                        {
                            conteudo = replace[conteudo];
                        }

                        sb.Append(conteudo.Replace(delimitador, ""));
                        sb.Append(delimitador);
                    }

                    sb.Remove(sb.Length - delimitador.Length, delimitador.Length);
                    sb.AppendLine();

                }

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Ler o arquivo CSC e preencher a model de acordo com o atributo.
        /// Utilize o CsvField para definir o Index em que esta o valor do campo.
        /// Ex:
        /// [CsvField(0)]
        /// [DataField("TAB_C_NOME", SqlDbType.VarChar, 150)]
        /// public string Nome { get; set; }
        /// 
        /// Nesse exemplo, o valor da primeira coluna será atribuído na propriedade nome.
        /// </summary>
        /// <typeparam name="TipoModel"></typeparam>
        /// <param name="file"></param>
        /// <returns></returns>
        public static List<TipoModel> FromCSV<TipoModel>(HttpPostedFileBase file)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                    return null;

                var propriedades = typeof(TipoModel).GetProperties();
                var lstRetorno = new List<TipoModel>();

                using (var reader = new StreamReader(file.InputStream, Encoding.GetEncoding("ISO-8859-1")))
                {
                    var first = true;

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        if (first)
                        {
                            first = false;
                            continue;
                        }

                        var values = line.Split(';');
                        var modelPreenchida = Activator.CreateInstance<TipoModel>();

                        // Procura as informações em [row] conforme as propriedades da [model]
                        foreach (var item in propriedades)
                        {
                            var csvField = item.GetCustomAttributes(typeof(CsvField), false);

                            if (csvField == null || csvField.Length <= 0) continue;
                            var indexField = ((CsvField)csvField[0]).Index;

                            if (values.Length >= indexField && !string.IsNullOrWhiteSpace(values[indexField]))
                            {
                                var typeRowItem = values[indexField].GetType();

                                if (!item.PropertyType.Equals(typeRowItem))
                                {
                                    var newValue = Convert.ChangeType(values[indexField], Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType);
                                    item.SetValue(modelPreenchida, newValue, null);
                                }
                                else
                                    item.SetValue(modelPreenchida, values[indexField], null);
                            }
                        }

                        lstRetorno.Add(modelPreenchida);
                    }
                }

                return lstRetorno;

            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return null;
            }
        }

        #endregion

        #region ObterMensagensErro

        /// <summary>
        /// Retorna as mensagens de erro do Model State
        /// </summary>
        public static string ObterMensagensErro(this ModelStateDictionary state)
        {
            try
            {
                var msg = "";
                var enumerator = state.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Value.Errors.Count > 0)
                    {
                        msg += enumerator.Current.Value.Errors[0].ErrorMessage + "\n";
                    }
                }

                return msg;
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                throw;
            }
        }

        #endregion

        #region DescompactarZip

        /// <summary>
        /// Método usado para descompactar
        /// arquivos zip
        /// </summary>
        /// <param name="arquivo"></param>
        /// <param name="diretorio"></param>
        /// <returns></returns>
        public static bool DescompactarZip(string arquivo, string diretorio)
        {
            try
            {
                using (var _zip = ZipFile.OpenRead(arquivo))
                {
                    _zip.ExtractToDirectory(diretorio);
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return false;
            }

            return true;
        }

        #endregion

        #region Criador de Thumb

        /// <summary>
        /// Redimensiona imagem mantendo proporcoes da imagem
        /// </summary>
        /// <remarks>
        /// http://kossovsky.net/index.php/2009/06/image-resizing/
        /// </remarks>
        public static Rectangle EnsureAspectRatio(Image Image, int Width, float? Height)
        {
            if (!Height.HasValue) Height = (int)Math.Ceiling(Image.Height * ((float)Width / (float)Image.Width));

            float AspectRatio = Width / (float)Height;
            float CalculatedWidth = Width, CalculatedHeight = (float)Height;

            if (Width >= Height)
            {
                if (Width > Height)
                {
                    CalculatedHeight = Width / AspectRatio;
                    if (CalculatedHeight > Height)
                    {
                        CalculatedHeight = Height.Value;
                        CalculatedWidth = CalculatedHeight * AspectRatio;
                    }
                }
                else
                {
                    CalculatedWidth = Height.Value * AspectRatio;
                    if (CalculatedWidth > Width)
                    {
                        CalculatedWidth = Width;
                        CalculatedHeight = CalculatedWidth / AspectRatio;
                    }
                }
            }
            else
            {
                if (Width < Height)
                {
                    CalculatedHeight = Width / AspectRatio;
                    if (CalculatedHeight > Height)
                    {
                        CalculatedHeight = Height.Value;
                        CalculatedWidth = CalculatedHeight * AspectRatio;
                    }
                }
                else
                {
                    CalculatedWidth = Height.Value * AspectRatio;
                    if (CalculatedWidth > Width)
                    {
                        CalculatedWidth = Width;
                        CalculatedHeight = CalculatedWidth / AspectRatio;
                    }
                }
            }
            return Rectangle.Ceiling(new RectangleF((Image.Width - CalculatedWidth) / 2, 0, CalculatedWidth, CalculatedHeight));
        }

        public static void CompressAndSaveImage(Image img, string fileName, long quality)
        {
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            img.Save(fileName, GetCodecInfo("image/jpeg"), parameters);
        }

        public static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            foreach (ImageCodecInfo encoder in ImageCodecInfo.GetImageEncoders())
                if (encoder.MimeType == mimeType)
                    return encoder;
            throw new ArgumentOutOfRangeException(
                string.Format("'{0}' not supported", mimeType));
        }

        #endregion

        #region CriarNovaSenha

        #region GetNewPassword

        /// <summary>
        /// Randomizar um novo Password
        /// </summary>
        /// <returns>String Password</returns>
        /// <user>mcarlos</user>
        public static string GetNewPassword()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(2, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }

        #endregion

        #region RandomNumber

        /// <summary>
        /// Randonizar um numero
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>Número</returns>
        /// <user>mazevedo</user>
        private static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        #endregion

        #region RandomString

        /// <summary>
        /// Randonizar um caracter
        /// </summary>
        /// <param name="size"></param>
        /// <param name="lowerCase"></param>
        /// <returns>Letra</returns>
        /// <user>mazevedo</user>
        private static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        #endregion

        #endregion

        #region GEOIP

        /// <summary>
        /// Converter IP_number em ip
        /// </summary>
        /// <param name="longIP"></param>
        /// <returns></returns>

        public static string LongToIP(long longIP)
        {
            string ip = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                int num = (int)(longIP / Math.Pow(256, (3 - i)));
                longIP = longIP - (long)(num * Math.Pow(256, (3 - i)));
                if (i == 0)
                    ip = num.ToString();
                else
                    ip = ip + "." + num.ToString();
            }
            return ip;
        }
        /// <summary>
        /// Converter ip em IP_number
        /// </summary>
        /// <param name="longIP"></param>
        /// <returns></returns>


        public static long IP2Long(string ip)
        {
            try
            {
                string[] ipBytes;
                double num = 0;
                if (!string.IsNullOrEmpty(ip))
                {
                    ipBytes = ip.Split('.');
                    for (int i = ipBytes.Length - 1; i >= 0; i--)
                    {
                        num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
                    }
                }
                return (long)num;
            }
            catch
            {
                return 0;
            }
        }
        #endregion

        #region Email
        /// <summary>
        /// Envia um email com as configurações do sistema(Porte, email origem)
        /// </summary>
        /// <param name="strEmailDestino">Email destino</param>
        /// <param name="strAssunto">Assunto do Email</param>
        /// <param name="strBody">Corpo do Email</param>
        /// <returns>Flag indicando o sucesso no envio do email</returns>
        public static bool EnviarEmail( string strEmailDestino, string strAssunto, string strCorpo )
        {
            try
            {
                string strEmail = BLConfiguracao.UsuarioEmail;
                string strSenha = BLConfiguracao.SenhaEmail;
                string strHost = BLConfiguracao.ServidorEmail;
                int intPorta = BLConfiguracao.PortaEmail;

                SmtpClient stcEmail = new SmtpClient( );
                stcEmail.UseDefaultCredentials = false;
                stcEmail.Credentials = new System.Net.NetworkCredential( strEmail, strSenha );

                // Porta adicionada para testar o usuário app
                stcEmail.Port = BLConfiguracao.PortaEmail;

                //from, recipients, subject, body
                stcEmail.Host = strHost;
                stcEmail.Send( strEmail, strEmailDestino, strAssunto, strCorpo );
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion
        
        #region LerExcel

        /// <summary>
        /// Ler um excel da folha especificada para model generica
        /// </summary>
        /// <typeparam name="TipoModel"></typeparam>
        /// <param name="sheetName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<TipoModel> LerExcel<TipoModel>(string sheetName, string path)
        {
            var datatable = LerExcel(sheetName, path);

            var propriedades = typeof(TipoModel).GetProperties();

            // Cria uma lista genérica conforme o tipo informado
            var lstRetorno = new List<TipoModel>();

            foreach (DataRow row in datatable.Rows)
            {
                var modelPreenchida = Activator.CreateInstance<TipoModel>();

                // Procura as informações em [row] conforme as propriedades da [model]
                foreach (var item in propriedades)
                {
                    var dbField = item.GetCustomAttributes(typeof(DataField), false);

                    if (dbField == null || dbField.Length <= 0) continue;
                    var fieldName = ((DataField)dbField[0]).Name;

                    if (row.Table.Columns.Contains(fieldName) && row[fieldName] != System.DBNull.Value)
                    {
                        var typeRowItem = row[fieldName].GetType();

                        if (!item.PropertyType.Equals(typeRowItem))
                        {
                            var newValue = Convert.ChangeType(row[fieldName], Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType);
                            item.SetValue(modelPreenchida, newValue, null);
                        }
                        else
                        {
                            item.SetValue(modelPreenchida, row[fieldName], null);
                        }
                    }

                }

                lstRetorno.Add(modelPreenchida);
            }

            return lstRetorno;
        }

        /// <summary>
        /// Ler uma página do excel e retornar em DataTable
        /// </summary>
        /// <param name="sheetName">Nome da página do excel</param>
        /// <param name="path">Caminho físico para o </param>
        /// <returns>DataTable</returns>
        public static DataTable LerExcel(string sheetName, string path)
        {
            DataTable dt = new DataTable();

            using (OleDbConnection conn = new OleDbConnection())
            {
                string Import_FileName = path;
                string fileExtension = Path.GetExtension(Import_FileName);

                if (fileExtension == ".xls")
                    conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Import_FileName + ";" + "Extended Properties='Excel 8.0;HDR=YES;'";
                if (fileExtension == ".xlsx")
                    conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Import_FileName + ";" + "Extended Properties='Excel 12.0 Xml;HDR=YES;'";

                if (String.IsNullOrWhiteSpace(sheetName))
                {
                    //Obter uma lista com as folhas do excel (abas)
                    conn.Open();
                    DataTable dbSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    if (dbSchema != null && dbSchema.Rows.Count > 0)
                        sheetName = dbSchema.Rows[0]["TABLE_NAME"].ToString().Replace("$", "");
                }

                if (!String.IsNullOrWhiteSpace(sheetName))
                {
                    using (OleDbCommand comm = new OleDbCommand())
                    {
                        comm.CommandText = "Select * from [" + sheetName + "$]";
                        comm.Connection = conn;

                        using (OleDbDataAdapter da = new OleDbDataAdapter())
                        {
                            da.SelectCommand = comm;
                            da.Fill(dt);
                        }
                    }
                }
            }

            return dt;
        }

        #region EPPlus

        public class EPPlus
        {
            #region Exportar 

            /// <summary>
            /// Gerar um arquivo excel com o conteúdo de List<TipoModel>
            /// </summary>
            /// <typeparam name="TipoModel"></typeparam>
            /// <param name="lista"></param>
            /// <param name="namefile"></param>
            /// <returns></returns>
            public static MemoryStream Exportar<TipoModel>(List<TipoModel> lista)
            {
                MemoryStream stream = new MemoryStream();

                /*Obtém apenas as propriedades que possuem "DescriptionField" na Model*/
                var propriedades = (from i in typeof(TipoModel).GetProperties()
                                    where i.IsDefined(typeof(CsvField), false)
                                    select i).OrderBy(x => ((CsvField)(x.GetCustomAttributes(typeof(CsvField), true)[0])).Order).ToArray();

                using (ExcelPackage excel = new ExcelPackage(stream))
                {
                    //Cria a planilha no arquivo.
                    ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("Planilha 1");
                    ExcelRange cells = worksheet.Cells;
                    int row = 1;

                    /*Formatação Generica*/
                    cells.Style.Font.Name = "Arial";
                    cells.Style.Font.Size = 10;

                    /*Cabeçalho*/
                    for (int i = 0; i < propriedades.Count(); i++)
                    {
                        cells[row, (i + 1)].Value = ((CsvField)(propriedades[i].GetCustomAttributes(typeof(CsvField), true)[0])).Name;
                        cells[row, (i + 1)].Style.Font.Bold = true;
                        cells[row, (i + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cells[row, (i + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    foreach (TipoModel item in lista)
                    {
                        row++;

                        for (int i = 0; i < propriedades.Count(); i++)
                        {
                            cells[row, (i + 1)].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            //Encontrar o atributo excelField na propriedade da model
                            var excelField = (CsvField)propriedades[i].GetCustomAttributes(typeof(CsvField), true)[0];

                            //Criar uma instancia do tipo de formatador definido no ExcelField
                            // var excelFormater = (ICsvFormat)Activor.CreateInstance(excelField.Type);

                            //Obter o valor formatado de acordo com o tipo da propriedade da model
                            cells[row, (i + 1)].Value = propriedades[i].GetValue(item, null); //excelFormater.GetValue(excelField, propriedades[i].GetValue(item, null));

                            if (!string.IsNullOrEmpty(excelField.Format))
                                cells[row, (i + 1)].Style.Numberformat.Format = excelField.Format;

                        }
                    }
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    excel.Save();
                }
                return stream;
            }

            #endregion

            public static List<TipoModel> LerExcel<TipoModel>(string path)
            {
                var datatable = LerExcel(path);

                var propriedades = typeof(TipoModel).GetProperties();

                // Cria uma lista genérica conforme o tipo informado
                var lstRetorno = new List<TipoModel>();

                foreach (DataRow row in datatable.Rows)
                {
                    var modelPreenchida = Activator.CreateInstance<TipoModel>();

                    // Procura as informações em [row] conforme as propriedades da [model]
                    foreach (var item in propriedades)
                    {
                        var dbField = item.GetCustomAttributes(typeof(DataField), false);

                        if (dbField == null || dbField.Length <= 0) continue;
                        var fieldName = ((DataField)dbField[0]).Name;

                        if (row.Table.Columns.Contains(fieldName) && row[fieldName] != System.DBNull.Value)
                        {
                            var typeRowItem = row[fieldName].GetType();

                            if (!item.PropertyType.Equals(typeRowItem))
                            {
                                var newValue = Convert.ChangeType(row[fieldName], Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType);
                                item.SetValue(modelPreenchida, newValue, null);
                            }
                            else
                            {
                                item.SetValue(modelPreenchida, row[fieldName], null);
                            }
                        }

                    }

                    lstRetorno.Add(modelPreenchida);
                }

                return lstRetorno;
            }

            public static DataTable LerExcel(string path, bool hasHeader = true)
            {
                using (var pck = new OfficeOpenXml.ExcelPackage())
                {
                    using (var stream = File.OpenRead(path))
                    {
                        pck.Load(stream);
                    }
                    var ws = pck.Workbook.Worksheets[1];

                    DataTable tbl = new DataTable();

                    foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                    {
                        tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                    }

                    var startRow = hasHeader ? 2 : 1;

                    for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        DataRow row = tbl.Rows.Add();
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Text;
                        }
                    }
                    return tbl;
                }
            }

            public static List<TipoModel> LerExcel<TipoModel>(HttpPostedFileBase file, bool hasHeader = true)
            {
                var datatable = LerExcel(file, hasHeader);

                return LerExcel<TipoModel>(datatable);
            }

            private static List<TipoModel> LerExcel<TipoModel>(DataTable datatable)
            {
                var propriedades = typeof(TipoModel).GetProperties();

                // Cria uma lista genérica conforme o tipo informado
                var lstRetorno = new List<TipoModel>();

                foreach (DataRow row in datatable.Rows)
                {
                    var valido = false;
                    var modelPreenchida = Activator.CreateInstance<TipoModel>();

                    // Procura as informações em [row] conforme as propriedades da [model]
                    foreach (var item in propriedades)
                    {
                        try
                        {
                            var dbField = item.GetCustomAttributes(typeof(CsvField), false);

                            if (dbField == null || dbField.Length <= 0) continue;
                            var fieldName = ((CsvField)dbField[0]).Name;

                            if (row.Table.Columns.Contains(fieldName) && row[fieldName] != System.DBNull.Value && row[fieldName] != null && Convert.ToString(row[fieldName]) != "")
                            {
                                valido = true;
                                var typeRowItem = row[fieldName].GetType();

                                if (!item.PropertyType.Equals(typeRowItem))
                                {
                                    try
                                    {
                                        var newValue = Convert.ChangeType(row[fieldName], Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType, new CultureInfo("pt-BR"));
                                        item.SetValue(modelPreenchida, newValue, null);
                                    }
                                    catch
                                    {
                                        if (item.PropertyType.ToString().Contains("System.DateTime"))
                                        {
                                            var newValue = Convert.ToDateTime(row[fieldName].ToString(), CultureInfo.InvariantCulture);
                                            item.SetValue(modelPreenchida, newValue, null);
                                        }
                                    }
                                }
                                else
                                {
                                    item.SetValue(modelPreenchida, row[fieldName], null);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ApplicationLog.ErrorLog(ex);
                            valido = false;
                        }
                    }

                    if (valido)
                        lstRetorno.Add(modelPreenchida);
                }

                return lstRetorno;
            }

            public static DataTable LerExcel(HttpPostedFileBase file, bool hasHeader = true)
            {
                using (var pck = new OfficeOpenXml.ExcelPackage())
                {
                    pck.Load(file.InputStream);

                    return LerExcel(pck, hasHeader);
                }
            }

            private static DataTable LerExcel(OfficeOpenXml.ExcelPackage pck, bool hasHeader = true)
            {
                DataTable tbl = new DataTable();

                var ws = pck.Workbook.Worksheets[1];

                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text.Trim() : string.Format("Column {0}", firstRowCell.Start.Column));
                }

                var startRow = hasHeader ? 2 : 1;

                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();

                    foreach (var cell in wsRow)
                    {
                        try
                        {
                            if (cell != null && cell.Start != null && row != null && row.ItemArray.Length > cell.Start.Column - 1)
                                row[cell.Start.Column - 1] = cell.Text;
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                return tbl;
            }
        }

        #endregion

        #endregion

        #region CarregarXmlParaObjeto

        /// <summary>
        /// Carregar arquivo de configuração no Obj
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CarregarXmlParaObjeto<T>(string texto)
        {
            var obj = Activator.CreateInstance(typeof(T));

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                xmlDoc.LoadXml(texto);
                obj = (T)xmlSerializer.Deserialize(new StringReader(xmlDoc.OuterXml));
            }
            catch
            {
                throw;
            }

            return (T)obj;
        }

        #endregion

        

        public static string ToString(this bool? val, string textTrue, string textFalse, string textNull = "")
        {
            if (val.HasValue)
            {
                if (val.Value)
                {
                    return textTrue;
                }
                else
                {
                    return textFalse;
                }
            }

            if (!string.IsNullOrWhiteSpace(textNull))
            {
                return textNull;
            }

            return textFalse;
        }
    }
}