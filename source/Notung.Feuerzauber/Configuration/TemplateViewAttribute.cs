using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notung.Feuerzauber.Configuration
{
   
    public class TemplateViewAttribute : Attribute
    {
        public TemplatesView Template { get; set; }
    }

    public enum TemplatesView
    {
        None = 0,
        TextBox = 1,
        CheckBox = 2,
        ComboBox = 3
    }
}
