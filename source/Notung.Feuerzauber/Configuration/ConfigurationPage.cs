using Notung.Configuration;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Notung.Feuerzauber.Configuration
{

    public abstract class ConfigurationPage
    {
        public ConfigurationPage()
        {
            Sections = new List<ConfigurationSection>();
        }

        /// <summary>
        /// Секции
        /// </summary>
        public  List<ConfigurationSection> Sections { get; set; }
        /// <summary>
        /// Устанавливает изменена страница
        /// </summary>
        public bool PageСhanged { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationSections"></param>

        public abstract void InitializationSections(List<Type> configurationSections);

        /// <summary>
        /// Заголовок страницы
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Картинка в списке настроек
        /// </summary>
        public abstract Image Image { get; }

        /// <summary>
        /// Выполнять ли проверку настроек в потоке пользовательского интерфейса
        /// </summary>
        public abstract bool UIThreadValidation { get; }




    }
}
