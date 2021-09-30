using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using Framework.Utilities;

namespace Framework.Utilities
{
    /// <summary>
    /// Funcoes de Encriptacao
    /// </summary>
    public static class BLEncriptacao
    {
        #region Variáveis Privadas

        private static string gstrChaveEncriptacao = "!#$a54?3";
        private static readonly byte[] garrRangeBytes = { 10, 20, 30, 40, 50, 60, 70, 80 };
                
        /// <summary>     
        /// Vetor de bytes utilizados para a criptografia (Chave Externa)     
        /// </summary>     
        private static readonly byte[] bIVAES = { 0x50, 0x08, 0xF1, 0xDD, 0xDE, 0x3C, 0xF2, 0x18, 0x44, 0x74, 0x19, 0x2C, 0x53, 0x49, 0xAB, 0xBC };

        /// <summary>     
        /// Representação de valor em base 64 (Chave Interna)            
        /// </summary>     
        private const string gstrChaveEncritacaoAES = "SkMgTWVnYSBTdG9yZSBDcmlwdG9ncmFmaWEgLyBBRVM=";

        #endregion

        #region Encriptar Senha

        /// <summary>
        ///     Encripta uma senha
        /// </summary>
        /// <param name="pstrSenha">Senha a ser criptografada</param>
        /// <returns>Senha cifrada</returns>
        public static string EncriptarSenha(string pstrSenha)
        {
            return GetSwcSHA1(pstrSenha);
        }

        private static string GetSwcSHA1(string value)
        {
            SHA1 algorithm = SHA1.Create();
            byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
            string sh1 = "";
            for (int i = 0; i < data.Length; i++)
            {
                sh1 += data[i].ToString("x2").ToUpperInvariant();
            }
            return sh1;
        }

        #endregion

        #region Query String

        #region Encriptar Query String

        /// <summary>
        ///     Criptografa valores para serem usados na querystring
        /// </summary>
        /// <param name="pstrValor">Valor a ser criptografado</param>
        /// <returns>String criptografada</returns>
        public static string EncriptarQueryString(string pstrValor)
        {
            if (string.IsNullOrWhiteSpace(pstrValor))
                return pstrValor;

            byte[] arrChave = { };
            byte[] arrBytesValor;
            MemoryStream msmStreamCriptografia = null;
            CryptoStream csmCriptografia = null;
            DESCryptoServiceProvider dscProvider = new DESCryptoServiceProvider();

            try
            {
                arrChave = Encoding.UTF8.GetBytes(gstrChaveEncriptacao.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                arrBytesValor = Encoding.UTF8.GetBytes(pstrValor);
                msmStreamCriptografia = new MemoryStream();
                csmCriptografia = new CryptoStream(msmStreamCriptografia, dscProvider.CreateEncryptor(arrChave, garrRangeBytes), CryptoStreamMode.Write);
                csmCriptografia.Write(arrBytesValor, 0, arrBytesValor.Length);
                csmCriptografia.FlushFinalBlock();
                return Convert.ToBase64String(msmStreamCriptografia.ToArray());
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return pstrValor;
            }

        }

        #endregion

        #region Desencriptar QueryString

        /// <summary>
        ///     Desencripta um valor string
        /// </summary>
        /// <param name="pstrValor">Valor encriptado</param>
        /// <returns>Valor descriptado</returns>
        public static string DesencriptarQueryString(string pstrValor)
        {

            byte[] arrChave = { };
            byte[] arrValor = new byte[pstrValor.Length];
            DESCryptoServiceProvider dscProvider = new DESCryptoServiceProvider();
            MemoryStream msmStreamCriptografia = null;
            CryptoStream csmCriptografia = null;

            try
            {
                arrChave = Encoding.UTF8.GetBytes(gstrChaveEncriptacao.Substring(0, 8));
                pstrValor = pstrValor.Replace(" ", "+");
                arrValor = Convert.FromBase64String(pstrValor);
                msmStreamCriptografia = new MemoryStream();
                csmCriptografia = new CryptoStream(msmStreamCriptografia, dscProvider.CreateDecryptor(arrChave, garrRangeBytes), CryptoStreamMode.Write);
                csmCriptografia.Write(arrValor, 0, arrValor.Length);
                csmCriptografia.FlushFinalBlock();

                Encoding encoding = Encoding.UTF8;
                return encoding.GetString(msmStreamCriptografia.ToArray());
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return pstrValor;
            }

        }

        #endregion

        #endregion

        #region EncriptarMD5
        
        /// <summary>
        ///  Cifra o conteúdo em MD5, criando uma HASH que não é descriptografável
        /// </summary>
        public static string EncriptarMD5(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        #endregion

        #region AES - Senha

        /// <summary>
        /// Encriptar o Valor
        /// </summary>        
        public static string EncriptarAes(string valor)
        {
            try
            {
                // Se a string não está vazia, executa a criptografia
                if (!string.IsNullOrEmpty(valor))
                {
                    // Cria instancias de vetores de bytes com as chaves                
                    byte[] bKey = Convert.FromBase64String(gstrChaveEncritacaoAES);
                    byte[] bText = new UTF8Encoding().GetBytes(valor);

                    // Instancia a classe de criptografia Rijndael
                    Rijndael rijndael = new RijndaelManaged
                    {

                        // Define o tamanho da chave "256 = 8 * 32"                
                        // Lembre-se: chaves possíves:                
                        // 128 (16 caracteres), 192 (24 caracteres) e 256 (32 caracteres)                
                        KeySize = 256
                    };

                    // Cria o espaço de memória para guardar o valor criptografado:                
                    MemoryStream mStream = new MemoryStream();
                    // Instancia o encriptador                 
                    CryptoStream encryptor = new CryptoStream(
                        mStream,
                        rijndael.CreateEncryptor(bKey, bIVAES),
                        CryptoStreamMode.Write);

                    // Faz a escrita dos dados criptografados no espaço de memória
                    encryptor.Write(bText, 0, bText.Length);
                    // Despeja toda a memória.                
                    encryptor.FlushFinalBlock();
                    // Pega o vetor de bytes da memória e gera a string criptografada                
                    return Convert.ToBase64String(mStream.ToArray());
                }
                else
                {
                    // Se a string for vazia retorna nulo                
                    return null;
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Desencritar o valor () 
        /// </summary>        
        public static string DesencriptarAes(string valor)
        {
            try
            {
                // Se a string não está vazia, executa a criptografia           
                if (!string.IsNullOrEmpty(valor))
                {
                    // Cria instancias de vetores de bytes com as chaves                
                    byte[] bKey = Convert.FromBase64String(gstrChaveEncritacaoAES);
                    byte[] bText = Convert.FromBase64String(valor);

                    // Instancia a classe de criptografia Rijndael                
                    Rijndael rijndael = new RijndaelManaged
                    {

                        // Define o tamanho da chave "256 = 8 * 32"                
                        // Lembre-se: chaves possíves:                
                        // 128 (16 caracteres), 192 (24 caracteres) e 256 (32 caracteres)                
                        KeySize = 256
                    };

                    // Cria o espaço de memória para guardar o valor DEScriptografado:               
                    MemoryStream mStream = new MemoryStream();

                    // Instancia o Decriptador                 
                    CryptoStream decryptor = new CryptoStream(
                        mStream,
                        rijndael.CreateDecryptor(bKey, bIVAES),
                        CryptoStreamMode.Write);

                    // Faz a escrita dos dados criptografados no espaço de memória   
                    decryptor.Write(bText, 0, bText.Length);
                    // Despeja toda a memória.                
                    decryptor.FlushFinalBlock();
                    // Instancia a classe de codificação para que a string venha de forma correta         
                    UTF8Encoding utf8 = new UTF8Encoding();
                    // Com o vetor de bytes da memória, gera a string descritografada em UTF8       
                    return utf8.GetString(mStream.ToArray());
                }
                else
                {
                    // Se a string for vazia retorna nulo                
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.ErrorLog(ex);
                return string.Empty;
            }
        }

        #endregion

    }
}
