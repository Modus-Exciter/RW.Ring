﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1022
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Notung {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class LogSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static LogSettings defaultInstance = ((LogSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new LogSettings())));
        
        public static LogSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("[{Date:dd.MM.yyyy HH:mm:ss}] [{Level}] {Source}: {Message}")]
        public string MessageTemplate {
            get {
                return ((string)(this["MessageTemplate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("===============================================")]
        public string Separator {
            get {
                return ((string)(this["Separator"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("65536")]
        public uint LogFileSize {
            get {
                return ((uint)(this["LogFileSize"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<<- - - - - - ->>")]
        public string DefaultLogger {
            get {
                return ((string)(this["DefaultLogger"]));
            }
        }
    }
}