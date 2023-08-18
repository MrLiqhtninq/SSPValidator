Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports ITLlib

Public Partial Class Form1
	Inherits Form
	' Events handling section 

	Private Sub Form1_Load(sender As Object, e As EventArgs)
		' create an instance of the validator info class
		Validator = New CValidator()
		btnHalt.Enabled = False

		' Position comms window
		Dim p As New Point(Location.X, Location.Y)
		p.X += Me.Width
		Validator.CommsLog.Location = p

		If Properties.Settings.[Default].CommWindow Then
			Validator.CommsLog.Show()
			logTickBox.Checked = True
		Else
			logTickBox.Checked = False
		End If
	End Sub

	Private Sub Form1_Shown(sender As Object, e As EventArgs)
		' hide this and show opening menu
		Hide()
		Dim menu As New frmOpenMenu(Me)
		menu.Show()
	End Sub

	Private Sub btnRun_Click(sender As Object, e As EventArgs)
		MainLoop()
	End Sub

	Private Sub exitToolStripMenuItem_Click(sender As Object, e As EventArgs)
		Application.[Exit]()
	End Sub

	Private Sub settingsToolStripMenuItem_Click(sender As Object, e As EventArgs)
		Dim formSettings As Form = New frmSettings()
		formSettings.ShowDialog()
		Running = False
	End Sub

	Private Sub timer1_Tick(sender As Object, e As EventArgs)
		timer1.Enabled = False
	End Sub

	Private Sub btnHalt_Click(sender As Object, e As EventArgs)
		textBox1.AppendText("Poll loop stopped" & vbCr & vbLf)
		Running = False
	End Sub

	Private Sub resetValidatorBtn_Click(sender As Object, e As EventArgs)
		If Validator IsNot Nothing Then
			Validator.Reset(textBox1)
		End If
	End Sub

	Private Sub logTickBox_CheckedChanged(sender As Object, e As EventArgs)
		If logTickBox.Checked Then
			Validator.CommsLog.Show()
		Else
			Validator.CommsLog.Hide()
		End If
	End Sub

	Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs)
		Running = False
		Properties.Settings.[Default].CommWindow = logTickBox.Checked
		Properties.Settings.[Default].ComPort = [Global].ComPort
		Properties.Settings.[Default].Save()
	End Sub

	Private Sub optionsToolStripMenuItem_Click(sender As Object, e As EventArgs)
		Dim f As New frmSettings()
		f.Show()
	End Sub

	Private Sub btnClear_Click(sender As Object, e As EventArgs)
		Validator.NumberOfNotesStacked = 0
	End Sub

	Private Sub reconnectionTimer_Tick(sender As Object, e As EventArgs)
		reconnectionTimer.Enabled = False
	End Sub
End Class
