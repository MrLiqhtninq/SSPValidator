'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.1008
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Namespace Properties


	<System.Runtime.CompilerServices.CompilerGeneratedAttribute> _
	<System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")> _
	Public NotInheritable Partial Class Settings
		Inherits Global.System.Configuration.ApplicationSettingsBase

		Private Shared defaultInstance As Settings = DirectCast(Global.System.Configuration.ApplicationSettingsBase.Synchronized(New Settings()), Settings)

		Public Shared ReadOnly Property [Default]() As Settings
			Get
				Return defaultInstance
			End Get
		End Property

		<System.Configuration.UserScopedSettingAttribute> _
		<System.Diagnostics.DebuggerNonUserCodeAttribute> _
		<System.Configuration.DefaultSettingValueAttribute("COM1")> _
		Public Property ComPort() As String
			Get
				Return DirectCast(Me("ComPort"), String)
			End Get
			Set
				Me("ComPort") = value
			End Set
		End Property

		<System.Configuration.UserScopedSettingAttribute> _
		<System.Diagnostics.DebuggerNonUserCodeAttribute> _
		<System.Configuration.DefaultSettingValueAttribute("True")> _
		Public Property CommWindow() As Boolean
			Get
				Return CBool(Me("CommWindow"))
			End Get
			Set
				Me("CommWindow") = value
			End Set
		End Property
	End Class
End Namespace
