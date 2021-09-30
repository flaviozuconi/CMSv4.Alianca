using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using Framework.Model;

namespace CMSv4.Model
{
    /// <summary>
    /// Classe Colaborador da YYamada
    /// </summary>
    [DataContract, Table("CMS_COL_CLIENTE_COLABORADOR")]
    public class MLColaborador
    {
        [DataField("COL_N_CODIGO", SqlDbType.Decimal, 18, PrimaryKey = true, AutoNumber = true)]
        public decimal? Codigo { get; set; }

        [DataField("COL_CLI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? CodigoCliente { get; set; }

        [DataMember]
        public decimal? CodigoGrupo { get; set; }

        [DataMember, DataField("COL_C_MATRICULA", SqlDbType.VarChar, 100)]
        public string Matricula { get; set; }

        [DataField("COL_C_TELEFONE", SqlDbType.VarChar, 100)]
        public string Telefone { get; set; }

        [DataMember, DataField("COL_C_AREA", SqlDbType.VarChar, 200)]
        public string Area { get; set; }

        [DataMember, DataField("COL_C_LOJA_LOTACAO", SqlDbType.VarChar, 100)]
        public string LojaLotacao { get; set; }

        [DataMember, DataField("COL_C_SUPERIOR_IMEDIATO", SqlDbType.VarChar, 100)]
        public string SuperiorImediato { get; set; }

        [DataMember, DataField("COL_C_CARGO", SqlDbType.VarChar, 100)]
        public string Cargo { get; set; }

        [DataMember, DataField("COL_C_SEXO", SqlDbType.VarChar, 1)]
        public string Sexo { get; set; }

        [DataMember, DataField("COL_C_CIDADE", SqlDbType.VarChar, 100)]
        public string Cidade { get; set; }

        [DataMember, DataField("COL_C_ESTADO", SqlDbType.VarChar, 2)]
        public string Estado { get; set; }

        [DataMember(IsRequired = true)]
        [DataField("CLI_C_NOME", SqlDbType.VarChar, 100, IgnoreEmpty = true)]
        [JoinField("COL_CLI_N_CODIGO", "CMS_CLI_CLIENTE", "CLI_N_CODIGO", "CLI_C_NOME")]
        public string Nome { get; set; }

        [DataField("CLI_C_FOTO", SqlDbType.VarChar, 5, IgnoreEmpty = true)]
        public string Foto { get; set; }

        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string Senha { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public bool? Status { get; set; }

        [DataMember]
        public DateTime? DataNascimento { get; set; }

        [DataMember]
        public List<string> Telefones { get; set; }

        public string SexoString
        {
            get
            {
                if (!String.IsNullOrEmpty(Sexo))
                {
                    if (Sexo == "M")
                    {
                        return "Sexo Masculino";
                    }

                    return "Sexo Feminino";
                }

                return String.Empty;
            }
        }

        public string CidadeEstadoString
        {
            get
            {
                if (!String.IsNullOrEmpty(Cidade) && !String.IsNullOrEmpty(Estado))
                {
                    return String.Concat(Cidade, "/", Estado);
                }
                else if (!String.IsNullOrEmpty(Cidade) && String.IsNullOrEmpty(Estado))
                {
                    return Cidade;
                }
                else if (String.IsNullOrEmpty(Cidade) && !String.IsNullOrEmpty(Estado))
                {
                    return Estado;
                }

                return String.Empty;
            }
        }

        public string NomeImagem
        {
            get
            {
                if (!String.IsNullOrEmpty(Foto))
                    return String.Concat(Codigo, Foto);

                return String.Empty;
            }
        }
    }

    public class MLColaboradorPublico
    {
        [DataField("CLI_N_CODIGO", SqlDbType.Decimal, 18)]
        public decimal? Codigo { get; set; }

        [DataField("CLI_C_NOME", SqlDbType.VarChar, 100)]
        public string Nome { get; set; }

        [DataField("CLI_D_ANIVERSARIO", SqlDbType.DateTime, IgnoreEmpty = true)]
        public DateTime? DataNascimento { get; set; }

        [DataField("CLI_C_FOTO", SqlDbType.VarChar, 5)]
        public string Foto { get; set; }

        [DataField("COL_C_CARGO", SqlDbType.VarChar, 100)]
        public string Cargo { get; set; }

        [DataField("POR_C_NOME", SqlDbType.VarChar, 200)]
        public string Empresa { get; set; }

        [DataField("COL_C_LOJA_LOTACAO", SqlDbType.VarChar, 200)]
        public string LojaLotacao { get; set; }

        [DataField("COL_C_AREA", SqlDbType.VarChar, 200)]
        public string Area { get; set; }

        [DataField("COL_C_SEXO", SqlDbType.VarChar, 1)]
        public string Sexo { get; set; }

        [DataField("COL_C_CIDADE", SqlDbType.VarChar, 100)]
        public string Cidade { get; set; }

        [DataField("COL_C_ESTADO", SqlDbType.VarChar, 2)]
        public string Estado { get; set; }

        [DataField("COL_C_SUPERIOR_IMEDIATO", SqlDbType.VarChar, 100)]
        public string SuperiorImediato { get; set; }

        public string NomeImagem
        {
            get
            {
                if (!string.IsNullOrEmpty(Foto))
                    return string.Concat(Codigo, Foto);

                return string.Empty;
            }
        }

        public string SexoString
        {
            get
            {
                if (!string.IsNullOrEmpty(Sexo))
                {
                    if (Sexo == "M")
                    {
                        return "Sexo Masculino";
                    }

                    return "Sexo Feminino";
                }

                return string.Empty;
            }
        }

        public string CidadeEstadoString
        {
            get
            {
                if (!string.IsNullOrEmpty(Cidade) && !string.IsNullOrEmpty(Estado))
                {
                    return string.Concat(Cidade, "/", Estado);
                }
                else if (!string.IsNullOrEmpty(Cidade) && string.IsNullOrEmpty(Estado))
                {
                    return Cidade;
                }
                else if (string.IsNullOrEmpty(Cidade) && !string.IsNullOrEmpty(Estado))
                {
                    return Estado;
                }

                return string.Empty;
            }
        }

        [DataField("CLI_C_EMAIL", SqlDbType.VarChar, 200)]
        public string Email { get; set; }

        [DataField("COL_C_TELEFONE", SqlDbType.VarChar, 50)]
        public string Telefone { get; set; }
    }
}
