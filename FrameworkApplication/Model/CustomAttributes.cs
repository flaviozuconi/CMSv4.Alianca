using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Model
{
    [System.AttributeUsage(AttributeTargets.Property)]
    public class SubSelectField : Attribute
    {
        public string Coluna { get; set; }

        public string QuerySql { get; set; }

        public SubSelectField(string coluna, string querySql)
        {
            Coluna = coluna;
            QuerySql = querySql;
        }
    }

    [System.AttributeUsage(AttributeTargets.Property)]
    public class WhereField : Attribute
    {
        public WhereCondition Condition { get; set; }

        public WhereField(WhereCondition condition = WhereCondition.EqualsOrLike)
        {
            Condition = condition;
        }
    }

    public enum WhereCondition
    {
        EqualsOrLike, // = OU LIKE
        Equals, // = SOMENTE
        Like, // LIKE SOMENTE
        In, // COLUNA IN 'LISTA'
        ListInList, // SELECT ITEM FROM DBO.SPLITTABLE(COLUNA) IN SELECT ITEM FROM DBO.SPLITTABLE('LISTA')
        ListNotInList, // SELECT ITEM FROM DBO.SPLITTABLE(COLUNA) NOT IN SELECT ITEM FROM DBO.SPLITTABLE('LISTA')
        NotIn // COLUNA NOT IN 'LISTA'
    }

    [System.AttributeUsage(AttributeTargets.Property)]
    public class DataList : Attribute
    {
    }
}
