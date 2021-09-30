using Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace CMSv4.BusinessLayer
{
    public class BLCRUD<TipoModel> 
    {
        #region Excluir

        /// <summary>
        /// Excluir registro por PK
        /// </summary>
        /// <param name="codigo">Codigo do registro (PK)</param>
        /// <param name="connectionString">String de conexao</param>
        /// <returns>int</returns>
        public virtual int Excluir(decimal codigo, string connectionString = "")
        {
            return CRUD.Excluir<TipoModel>(codigo, connectionString);
        }

        /// <summary>
        /// Excluir todos os registros por PK
        /// </summary>
        /// <param name="ids">Lista de codigos dos registros (PK)</param>
        /// <param name="connectionString">String de conexao</param>
        /// <returns>int</returns>
        public virtual int Excluir(List<string> ids, string connectionString = "")
        {
            return CRUD.Excluir<TipoModel>(ids, connectionString);
        }

        /// <summary>
        /// Excluir registro por propriedades
        /// </summary>
        /// <param name="criterios">propriedades da model</param>
        /// <param name="connectionString">String de conexao</param>
        /// <returns>int</returns>
        public virtual int Excluir(TipoModel criterios, string connectionString = "")
        {
            return CRUD.Excluir(criterios, connectionString);
        }

        #endregion

        #region Listar

        /// <summary>
        /// Listar itens de acordo com os valores presentes na model criterio
        /// </summary>
        /// <param name="criterios">Model com paremetros de filtro</param>
        /// <param name="connectionString">String de conexao</param>
        /// <returns>Lista de model</returns>
        public virtual List<TipoModel> Listar(TipoModel criterios, string connectionString = "")
        {
            return CRUD.Listar(criterios, connectionString);
        }

        /// <summary>
        /// Listar itens de acordo com os valores presentes na model criterio e controle de ordenacao
        /// </summary>
        /// <param name="criterios">Model com paremetros de filtro</param>
        /// <param name="orderBy">Nome da propriedade da model que ser utilizada para ordenacao do retorno</param>
        /// <param name="sortOrder">ASC ou DESC</param>
        /// <param name="connectionString">String de conexao</param>
        /// <returns>Lista de model</returns>
        public virtual List<TipoModel> Listar(TipoModel criterios, string orderBy, string sortOrder, string connectionString = "")
        {
            return CRUD.Listar(criterios, orderBy, sortOrder, connectionString);
        }

        /// <summary>
        /// Listar itens de acordo com os valores presentes na model criterio e controle de ordenacao 
        /// e a quantidade de registros que serao retornados
        /// </summary>
        /// <param name="criterios">Model com paremetros de filtro</param>
        /// <param name="top">Quantidade de registros que serao retornados</param>
        /// <param name="orderBy">Nome da propriedade da model que ser utilizada para ordenacao do retorno</param>
        /// <param name="sortOrder">ASC ou DESC</param>
        /// <param name="connectionString">String de conexao</param>
        /// <param name="useLikeOnSearch">Define se sera utilizado like '%<param>%' ou = '<param>'</param>
        /// <returns>Lista de model</returns>
        public virtual List<TipoModel> Listar(TipoModel criterios, int? top, string orderBy, string sortOrder, string connectionString = "", bool useLikeOnSearch = true)
        {
            return CRUD.Listar(criterios, top, orderBy, sortOrder, connectionString, useLikeOnSearch);
        }

        /// <summary>
        /// Listar para Datatable
        /// </summary>
        /// <param name="criterios"></param>
        /// <param name="request"></param>
        /// <param name="total"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public virtual List<TipoModel> Listar(TipoModel criterios, NameValueCollection request, out double total, string connectionString = "")
        {
            return CRUD.Listar(criterios, request, out total, connectionString);
        }

        #endregion

        #region Obter

        /// <summary>
        /// Obter um registro por codigo (PK) da model
        /// </summary>
        /// <param name="Codigo">Codigo do registro</param>
        /// <param name="connectionString">String de conexao</param>
        /// <returns>Model</returns>
        public virtual TipoModel Obter(decimal Codigo, string connectionString = "")
        {
            return CRUD.Obter<TipoModel>(Codigo, connectionString);
        }

        /// <summary>
        /// Obter o nome que será utilizado no arquivo excel
        /// </summary>
        /// <returns></returns>
        //public virtual string ObterNomeExcel()
        //{
        //    var dbField = typeof(TipoModel).GetCustomAttribute<Framework.Model.Table>(false);

        //    if (dbField == null)
        //        return "";

        //    return dbField.ExcelName;

        //}

        /// <summary>
        /// Obter um registro por codigo (PK) da model
        /// </summary>
        /// <param name="Codigo">Codigo do registro</param>
        /// <param name="connectionString">String de conexao</param>
        /// <returns>Model</returns>
        public virtual TipoModel Obter(string Codigo, string connectionString = "")
        {
            return CRUD.Obter<TipoModel>(Codigo, connectionString);
        }

        /// <summary>
        /// Obter um registro de acordo com os criterios dos valores preenchidos,
        /// nas propriedades da model.
        /// </summary>
        /// <param name="criterio">Model com os parametros de filtro</param>
        /// <param name="connectionString">String de conexao</param>
        /// <returns>Model</returns>
        public virtual TipoModel Obter(TipoModel criterio, string connectionString = "")
        {
            return CRUD.Obter(criterio, connectionString);
        }

        #endregion

        #region Salvar

        /// <summary>
        /// Salvar ou atualizar os valores da model (Nao ignora campos vazios)
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="connectionString">String de conexao</param>
        /// <returns>Codigo do registro</returns>
        public virtual decimal Salvar(TipoModel model, string connectionString = "")
        {
            return CRUD.Salvar(model, connectionString);
        }

        /// <summary>
        /// Salvar ou atualizar os valores da model (Ignora campos vazios)
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="connectionString">String de conexao</param>
        /// <returns>Codigo do registro</returns>
        public virtual decimal SalvarParcial(TipoModel model, string connectionString = "")
        {
            return CRUD.SalvarParcial(model, connectionString);
        }

        #endregion

        #region ValidarExistente

        /// <summary>
        /// Verificar se ja existe um registro com o valor especificado
        /// </summary>
        /// <param name="id">Codigo do registro que esta sendo editado, null para novo registro</param>
        /// <param name="propertyFilter">Nome da propriedade que sera utilizada para busca</param>
        /// <param name="propertyPk">Nome da propriedade que representa o id (Chave Primaria)</param>
        /// <param name="value">Valor para busca no propertyFilter</param>
        /// <returns>bool</returns>
        public virtual bool ValidarExistente(decimal? id, string propertyFilter, string propertyPk, string value)
        {
            var criterio = Activator.CreateInstance<TipoModel>();
            var prop = criterio.GetType().GetProperty(propertyFilter);

            if (prop != null)
                prop.SetValue(criterio, value, null);

            var model = Obter(criterio);

            if (model == null)
                return true;

            var propIdValue = model.GetType().GetProperty(propertyPk).GetValue(model, null);

            if (model != null && propIdValue.ToString() != id?.ToString())
                return false;

            return true;
        }

        #endregion
    }
}
