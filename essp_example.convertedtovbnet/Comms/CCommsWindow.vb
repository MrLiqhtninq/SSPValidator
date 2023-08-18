Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports System.IO
Imports ITLlib

Public Partial Class CCommsWindow
	Inherits Form
	' Variables
	Private m_PacketCounter As Integer
	Private m_bLogging As Boolean
	Private LogText As StringBuilder
	Private m_LogText As String
	Private m_SW As StreamWriter
	Private m_FileName As String
	Private Delegate Sub WriteToLog(text As String)
	Const SSP_POLL_CODE As Integer = 7
	Private SspLookup As New CSspLookup()

	' Variable access
	Public Property Log() As String
		Get
			Return m_LogText
		End Get
		Set
			m_LogText = value
		End Set
	End Property

	' Constructor
	Public Sub New(fileName As String)
		InitializeComponent()
		m_PacketCounter = 1
		m_bLogging = True
		' create persistent log
		Try
			' if the log dir doesn't exist, create it
			Dim logDir As String = "Logs\" & DateTime.Now.ToLongDateString()
			If Not Directory.Exists(logDir) Then
				Directory.CreateDirectory(logDir)
			End If
			' name this log as current time with name of validator at start
			m_FileName = fileName & "_" & DateTime.Now.Hour.ToString() & "h" & DateTime.Now.Minute.ToString() & "m" & DateTime.Now.Second.ToString() & "s.txt"
			' create/open the file
			m_SW = File.AppendText(logDir & "\" & m_FileName)
		Catch ex As Exception
			MessageBox.Show(ex.ToString(), "EXCEPTION")
			MyBase.Dispose()
		End Try
	End Sub

	' On loading, start logging and turn off the cross
	Private Sub CommsWindow_Load(sender As Object, e As EventArgs)
		Me.ControlBox = False
	End Sub

	' This function should be called in a loop, it monitors the SSP_COMMAND_INFO parameter
	' and writes the info to a text box in a readable format. If the failedCommand bool
	' is set true then it will not write a response.
	Public Sub UpdateLog(info As SSP_COMMAND_INFO, Optional failedCommand As Boolean = False)
		If m_bLogging Then
			Dim byteStr As String
			Dim len As Byte
			Dim pollResponse As Byte()

			' NON-ENCRPYTED
			' transmission
			LogText = New StringBuilder(500)
			LogText.AppendLine()
			LogText.AppendLine("No Encryption")
			LogText.Append("Sent Packet #")
			LogText.AppendLine(m_PacketCounter.ToString())
			len = info.PreEncryptedTransmit.PacketData(2)
			LogText.Append("Length: ")
			LogText.AppendLine(len.ToString())
			LogText.Append("Sync: ")
			LogText.AppendLine((info.PreEncryptedTransmit.PacketData(1) >> 7).ToString())
			LogText.Append("Command: ")
			LogText.AppendLine(SspLookup.GetCommandName(info.PreEncryptedTransmit.PacketData(3)))
			LogText.Append("Data: ")
			byteStr = BitConverter.ToString(info.PreEncryptedTransmit.PacketData, 3, len)
			LogText.AppendLine(FormatByteString(byteStr))
			' received

			If Not failedCommand Then
				LogText.AppendLine()
				LogText.Append("Received Packet #")
				LogText.AppendLine(m_PacketCounter.ToString())
				len = info.PreEncryptedRecieve.PacketData(2)
				LogText.Append("Length: ")
				LogText.AppendLine(len.ToString())
				LogText.Append("Sync: ")
				LogText.AppendLine((info.PreEncryptedRecieve.PacketData(1) >> 7).ToString())
				LogText.Append("Response: ")
				LogText.AppendLine(SspLookup.GetGenericResponseName(info.PreEncryptedRecieve.PacketData(3)))
				If info.PreEncryptedTransmit.PacketData(3) = SSP_POLL_CODE AndAlso len > 1 Then
					pollResponse = New Byte(len - 1) {}
					Array.Copy(info.PreEncryptedRecieve.PacketData, 3, pollResponse, 0, len)
					LogText.Append("Poll Response: ")
					LogText.Append(SspLookup.GetPollResponse(pollResponse))
				End If
				byteStr = BitConverter.ToString(info.PreEncryptedRecieve.PacketData, 3, len)
				LogText.Append("Data: ")
				LogText.AppendLine(FormatByteString(byteStr))
			Else
				LogText.AppendLine("No response...")
			End If

			If checkBox1.Checked = True Then
				' ENCRYPTED
				' transmission
				LogText.AppendLine()
				LogText.AppendLine("Encryption")
				LogText.Append("Sent Packet #")
				LogText.AppendLine(m_PacketCounter.ToString())
				len = info.Transmit.PacketData(2)
				LogText.Append("Length: ")
				LogText.AppendLine(len.ToString())
				LogText.Append("Sync: ")
				LogText.AppendLine((info.Transmit.PacketData(1) >> 7).ToString())
				byteStr = BitConverter.ToString(info.Transmit.PacketData, 3, len)
				LogText.Append("Data: ")
				LogText.AppendLine(FormatByteString(byteStr))

				' received
				If Not failedCommand Then
					LogText.AppendLine()
					LogText.Append("Received Packet #")
					LogText.AppendLine(m_PacketCounter.ToString())
					len = info.Receive.PacketData(2)
					LogText.Append("Length: ")
					LogText.AppendLine(len.ToString())
					LogText.Append("Sync: ")
					LogText.AppendLine((info.Receive.PacketData(1) >> 7).ToString())
					byteStr = BitConverter.ToString(info.Receive.PacketData, 3, len)
					LogText.Append("Data: ")
					LogText.AppendLine(FormatByteString(byteStr))
				Else
					LogText.AppendLine("No response...")
				End If
			End If

			m_LogText = LogText.ToString()

			If logWindowText.InvokeRequired Then
				Dim l As New WriteToLog(AddressOf AppendToWindow)
				logWindowText.BeginInvoke(l, New Object() {m_LogText})
			Else
				logWindowText.AppendText(m_LogText)
				logWindowText.SelectionStart = logWindowText.TextLength
			End If

			AppendToLog(m_LogText)
			m_PacketCounter += 1
		End If
	End Sub

	Private Function FormatByteString(s As String) As String
		Dim formatted As String = s
		Dim sArr As String()
		sArr = formatted.Split("-"C)
		formatted = ""
		For i As Integer = 0 To sArr.Length - 1
			formatted += sArr(i)
			formatted += " "
		Next
		Return formatted
	End Function

	Private Sub AppendToWindow(stringToAppend As String)
		logWindowText.AppendText(stringToAppend)
		logWindowText.SelectionStart = logWindowText.TextLength
	End Sub

	Private Sub AppendToLog(stringToAppend As String)
		m_SW.Write(stringToAppend)
	End Sub
End Class
