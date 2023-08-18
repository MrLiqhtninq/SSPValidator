Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports System.IO.Ports

Public Partial Class frmOpenMenu
	Inherits Form
	Private m_ComPorts As String()
	Private m_Parent As Form1

	Public Sub New(frm As Form1)
		InitializeComponent()
		m_Parent = frm
		If SearchForComPorts() > 0 Then
			cbComPort.Items.AddRange(m_ComPorts)
		End If
		cbComPort.Text = Properties.Settings.[Default].ComPort
		ControlBox = False
	End Sub

	Public Function SearchForComPorts() As Integer
		m_ComPorts = SerialPort.GetPortNames()
		Return m_ComPorts.Length
	End Function

	Private Sub btnSearch_Click(sender As Object, e As EventArgs)
		Try
			If tbSSPAddress.Text <> "" Then
				[Global].ComPort = cbComPort.SelectedItem.ToString()
				[Global].SSPAddress = [Byte].Parse(tbSSPAddress.Text)
				m_Parent.Show()
				Me.Close()
			End If
		Catch ex As Exception
			MessageBox.Show(ex.ToString(), "EXCEPTION")
		End Try
	End Sub

	Private Sub tbSSPAddress_KeyDown(sender As Object, e As KeyEventArgs)
		If e.KeyCode = Keys.[Return] Then
			btnSearch_Click(sender, e)
		End If
	End Sub
End Class
