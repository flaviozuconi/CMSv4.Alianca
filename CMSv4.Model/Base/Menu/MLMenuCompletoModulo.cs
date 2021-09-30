using System;
using System.Collections.Generic;
using Framework.Model;

namespace CMSv4.Model
{
    /// <summary>
    ///     Model que extende a model de Menu e contempla as listas
    /// </summary>
    [Serializable]
    [Table("MOD_MEN_MENU")]
    public class MLMenuCompletoModulo : MLMenuModulo
    {
        /// <summary>
        ///     Construtor da classe, inicializa as variáveis
        /// </summary>
        public MLMenuCompletoModulo()
        {
            ItensMenu = new List<MLMenuItemModulo>();
        }

        public List<MLMenuItemModulo> ItensMenu { get; set; }
    }
}
