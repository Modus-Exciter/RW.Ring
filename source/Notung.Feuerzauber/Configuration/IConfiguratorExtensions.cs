using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Notung.Configuration;

namespace Notung.Feuerzauber.Configuration
{
     public static class IConfiguratorExtensions
    {
        /// <summary>
        /// Расширение добавляющий альтенативный метод получения Секции. Тип задается обьектом.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="sectionType"></param>
        /// <returns></returns>
        public static ConfigurationSection GetSection(this IConfigurator self, Type sectionType)
        {
            Type configurationType = self.GetType();
          
            MethodInfo getSectionMethod = configurationType.GetMethod("GetSection").MakeGenericMethod(sectionType);
           
            return getSectionMethod.Invoke(self, null) as ConfigurationSection;
        }
        /// <summary>
        /// Расширение добавляющий альтенативный метод сохранения Секции.  Тип задается обьектом.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="section"></param>
        public static void SaveSection<T>(this IConfigurator self, T section) where T: ConfigurationSection
        {
            Type configurationType = self.GetType();

            MethodInfo saveSectionMethod = configurationType.GetMethod("SaveSection").MakeGenericMethod(section.GetType());

            saveSectionMethod.Invoke(self, new[] { section });
        }
    }
}
