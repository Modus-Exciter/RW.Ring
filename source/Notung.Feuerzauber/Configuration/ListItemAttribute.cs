using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Notung.Feuerzauber.Configuration
{

    /// <summary>
    /// Атрибут для связвание с выплывающим списком
    /// </summary>
    public class ListItemAttribute : Attribute
    {
       
        public Type TypeList { get; set; }
    }
}
