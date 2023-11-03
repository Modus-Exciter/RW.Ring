using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Notung.Feuerzauber.Configuration
{

    /// <summary>
    /// Базовый класс для реализации пунктов выпадающего списка
    /// </summary>
    public abstract class ListItemBase 
    {
      
        public abstract IEnumerable<object> LoadingItems();
        public BindingList<string> GetList() {

            return new BindingList<string>(LoadingItems().ToList().ConvertAll(x => x.ToString())); ;
        }
    }
}
