Imports System.IO


Public Class CFileIO
	Public Shared Sub WriteTextFile(FileNameStub As String, FileContents As String)
		Dim sw As StreamWriter
		Dim fileName As String
		Dim logDir As String = "TEBS_Logs\" & DateTime.Now.ToLongDateString()
		' if the log dir doesn't exist, create it 
		If Not Directory.Exists(logDir) Then
			Directory.CreateDirectory(logDir)
		End If

		' name this log as current time with name of validator at start
		fileName = FileNameStub & " at " & DateTime.Now.ToString("HH mm ss") & ".txt"
		' create/open the file
		sw = File.AppendText(logDir & "\" & fileName)
		sw.Write(FileContents)
		sw.Close()
	End Sub
End Class
