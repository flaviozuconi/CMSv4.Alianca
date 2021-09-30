using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Model;

namespace Framework.Utilities
{
    public class MLCookie
    {
        public MLCookie()
        {
            Valores = new List<MLCookieValores>();
        }
        //Nome que será salvo o cookie na maquina
        public string Nome { get; set; }

        //Data de expiração do coockie
        public DateTime Expires { get; set; }

        //Lista com os itens do cookie
        public List<MLCookieValores> Valores { get; set; }
    }

    //Chaves e valores que serão adicionados no cookie
    //ex: "IP", "127.0.0.1"
    public class MLCookieValores
    {
        [DataField("Chave", SqlDbType.VarChar, 250)]
        public string Chave { get; set; }

        [DataField("Valor", SqlDbType.VarChar, 250)]
        public string Valor { get; set; }
    }
}
