﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NitroxServer.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0")]
    internal sealed partial class ServerSessionSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static ServerSessionSettings defaultInstance = ((ServerSessionSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new ServerSessionSettings())));
        
        public static ServerSessionSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        public string SaveInterval {
            get {
                return ((string)(this["SaveInterval"]));
            }
            set {
                this["SaveInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("yourpassword")]
        public string ServerAdminPassword {
            get {
                return ((string)(this["ServerAdminPassword"]));
            }
            set {
                this["ServerAdminPassword"] = value;
            }
        }
    }
}
