using System;
using Framework.Model;
using System.Data;

namespace CMSv4.Model
{
    [Table("MOD_BRK_DIR_DIRETORIO")]
    public class MLPasta
    {
        [DataField("DIR_N_CODIGO", SqlDbType.Decimal, 18, AutoNumber = true, PrimaryKey = true)]
        public decimal? Codigo { get; set; }

        [DataField("DIR_C_CAMINHO", SqlDbType.VarChar, -1)]
        public string Caminho { get; set; }

        [DataField("DIR_B_RESTRITO", SqlDbType.Bit)]
        public bool? Restrito { get; set; }
    }

    [Table("MOD_BRK_DIR_DIRETORIO_X_GRUPO")]
    public class MLPastaPermissao
    {
        [DataField("DIP_DIR_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoDiretorio { get; set; }

        [DataField("DIP_GPR_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true)]
        public decimal? CodigoGrupo { get; set; }
    }

    public class MLPastaArquivo
    {   
        public string Nome { get; set; }
        public string Caminho { get; set; }
        public string ID { get; set; }
        public bool Atual { get; set; }
    }

    public class MLGaleriaArquivo
    {
        private const double SIZE_BYTE = 1024;
        private const double SIZE_KB = 1048576;
        private const double SIZE_MB = 1073741824;
        private const double SIZE_GB = 1099511627776;

        [DataField("Name", SqlDbType.VarChar, 255)]
        public string Nome { get; set; }

        [DataField("Name", SqlDbType.VarChar)]
        public string Caminho { get; set; }

        [DataField("CreationTime", SqlDbType.DateTime)]
        public DateTime? DataCriacao { get; set; }

        [DataField("LastAccessTime", SqlDbType.DateTime)]
        public DateTime? UltimoAcesso { get; set; }

        [DataField("LastWriteTime", SqlDbType.DateTime)]
        public DateTime? UltimaAlteracao { get; set; }

        [DataField("Length", SqlDbType.BigInt)]
        public double? Tamanho { get; set; }

        [DataField("IconUrl", SqlDbType.VarChar)]
        public string IconUrl { get; set; }

        // Campos Formatados

        public string CaminhoCompleto
        {
            get
            {
                if (string.IsNullOrEmpty(Caminho)) return Nome;
                return (Caminho + "/" + Nome).Replace("//", "/");
            }
        }

        [DataField("Length", SqlDbType.VarChar)]
        public string TamanhoEmTexto
        {
            get
            {
                if (Tamanho.HasValue)
                {
                    if (Tamanho < SIZE_BYTE) return Tamanho + " bytes";

                    if (Tamanho < SIZE_KB) return Math.Truncate((double)(Tamanho / SIZE_BYTE)) + " KB";

                    if (Tamanho < SIZE_MB) return Math.Round((double)(Tamanho / SIZE_KB), 2).ToString("n2") + " MB";

                    if (Tamanho < SIZE_GB) return Math.Round((double)(Tamanho / SIZE_MB), 2).ToString("n2") + " GB";
                }

                return "";
            }
        }

        [DataField("CreationTime", SqlDbType.VarChar)]
        public string DataCriacaoEmTexto
        {
            get
            {
                if (DataCriacao.HasValue)
                {
                    return DataCriacao.Value.ToString("dd/MM/yyyy HH:mm");
                }

                return "";
            }
        }

        [DataField("LastWriteTime", SqlDbType.VarChar)]
        public string UltimaAlteracaoEmTexto
        {
            get
            {
                if (UltimaAlteracao.HasValue)
                {
                    return UltimaAlteracao.Value.ToString("dd/MM/yyyy HH:mm");
                }

                return "";
            }
        }

        public string Extensao { get; set; }
        public string NomeEncriptacao { get; set; }
    }
}
