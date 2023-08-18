Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports System.IO.Ports

Public Partial Class frmSettings
	Inherits Form
	Public Sub New()
		InitializeComponent()
	End Sub

	Private Sub frmSettings_Load(sender As Object, e As EventArgs)
		Dim ports As String() = SerialPort.GetPortNames()
		If (ports Is Nothing) OrElse (ports.Length = 0) Then
			MessageBox.Show("No Serial Port found!", "ERROR")
			MyBase.Dispose()
		Else
			' first add current com port
			cbComPort.Items.Add([Global].ComPort)

			' add others if they are different
			Array.Sort(Of String)(ports)
			For i As Integer = 0 To ports.Length - 1
				If ports(i) <> [Global].ComPort Then
					cbComPort.Items.Add(ports(i))
				End If
			Next
			Me.cbComPort.SelectedItem = cbComPort.Items(0)
			tbSSPAddress.Text = [Global].SSPAddress.ToString()
		End If
	End Sub

	Private Sub btnCancel_Click(sender As Object, e As EventArgs)
		MyBase.Dispose()
	End Sub

	Private Sub btnOK_Click(sender As Object, e As EventArgs)
		[Global].ComPort = cbComPort.Text
		Try
			If tbSSPAddress.Text <> "" Then
				[Global].SSPAddress = [Byte].Parse(tbSSPAddress.Text)
			End If
		Catch ex As Exception
			MessageBox.Show(ex.ToString(), "EXCEPTION")
		End Try
		MyBase.Dispose()
	End Sub
End Class
