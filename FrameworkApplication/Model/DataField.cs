using System;
using System.Data;

namespace Framework.Model
{
    [System.AttributeUsage(AttributeTargets.Property)]
    public class CsvField : Attribute
    {
        #region Propriedades

        public int Index { get; set; }

        #endregion

        #region Construtores

        public CsvField(int index)
        {
            Index = index;
        }

        public CsvField(string name, int order)
        {
            Name = name;
            Order = order;
        }

        #endregion

        public string Name { get; set; }
        public int Order { get; set; }
        public Type Type { get; set; }
        public string Format { get; set; }
}

    /// <summary>
    /// Atributo para os campos da model se integrarem com os 
    /// campos no banco de dados SQL Server
    /// </summary>
    /// <remarks>
    /// Exemplos:
    /// 
    /// [DataField("MEU_C_CAMPO", System.Data.SqlDbType.VarChar, 50)] 
    /// public string MeuCampo {get;set;}
    /// 
    /// [DataField("MEU_N_NUMERO", System.Data.SqlDbType.Decimal, 18, 2)]
    /// public string MeuNumero {get;set;}
    /// </remarks>
    [System.AttributeUsage(AttributeTargets.Property)]
    public class DataField : Attribute
    {
        #region Propriedades

        /// <summary>Nome do campo na base de dados</summary>
        public string Name { get; set; }

        /// <summary>Tipo do campo</summary>
        public SqlDbType Type { get; set; }

        /// <summary>Tamanho do campo</summary>
        public int Size { get; set; }

        /// <summary>Quantidade de casas decimais</summary>
        public byte Scale { get; set; }

        /// <summary>Indica se o campo é chave primária</summary>
        public bool PrimaryKey { get; set; }

        /// <summary>Indica se o campo se´ra utilizado como FK na consulta de lista</summary>
        public bool ForeignKey { get; set; }

        /// <summary>Indica se o campo é auto numeração</summary>
        public bool AutoNumber { get; set; }

        /// <summary>
        /// Se o campo não estiver preenchido na model, ele é ignorado no INSERT e no UPDATE,
        /// caso esse atributo não esteja definido, ou seja FALSE, o sistema insere valor NULL 
        /// </summary>
        public bool IgnoreEmpty { get; set; }

        /// <summary>
        /// Determina se o campo será utilizado na busca
        /// </summary>
        public bool Busca
        {
            get { return _Busca; }
            set { _Busca = value; }
        }
        private bool _Busca = true; //Definir valor padrão

        /// <summary>
        /// Indica se o campo será utilizado ao gerar um cookie da model
        /// </summary>
        public bool IsCookie { get; set; }

        /// <summary>
        /// Determina se a propriedade será preenchida com resultado da base de dados
        /// </summary>
        public bool FillModel {
            get { return _FillModel; }
            set { _FillModel = value; }
        }
        private bool _FillModel = true; //Definir valor padrão

        #endregion

        #region Construtores

        public DataField(string name, SqlDbType dbType)
        {
            Name = name;
            Type = dbType;
        }

        public DataField(string name, SqlDbType dbType, bool primaryKey)
        {
            Name = name;
            Type = dbType;
            PrimaryKey = primaryKey;
        }

        public DataField(string name, SqlDbType dbType, int size)
        {
            Name = name;
            Type = dbType;
            Size = size;
        }

        public DataField(string name, SqlDbType dbType, int size, bool primaryKey)
        {
            Name = name;
            Type = dbType;
            Size = size;
            PrimaryKey = primaryKey;
        }

        public DataField(string name, SqlDbType dbType, int size, byte scale)
        {
            Name = name;
            Type = dbType;
            Size = size;
            Scale = scale;
        }

        public DataField(string name, SqlDbType dbType, int size, byte scale, bool primaryKey)
        {
            Name = name;
            Type = dbType;
            Size = size;
            Scale = scale;
            PrimaryKey = primaryKey;
        }

        public DataField(string name, SqlDbType dbType, int size, byte scale, bool primaryKey, bool autoNumber)
        {
            Name = name;
            Type = dbType;
            Size = size;
            Scale = scale;
            PrimaryKey = primaryKey;
            AutoNumber = autoNumber; 
        }

        public DataField(string name, SqlDbType dbType, int size, byte scale, bool primaryKey, bool autoNumber, bool isCookie)
        {
            Name = name;
            Type = dbType;
            Size = size;
            Scale = scale;
            PrimaryKey = primaryKey;
            AutoNumber = autoNumber;
            IsCookie = isCookie;
        }

        public DataField(string name, SqlDbType dbType, int size, byte scale, bool primaryKey, bool autoNumber, bool isCookie, bool foreignKye)
        {
            Name = name;
            Type = dbType;
            Size = size;
            Scale = scale;
            PrimaryKey = primaryKey;
            AutoNumber = autoNumber;
            IsCookie = isCookie;
            ForeignKey = foreignKye;
        }

        #endregion
    }
}
