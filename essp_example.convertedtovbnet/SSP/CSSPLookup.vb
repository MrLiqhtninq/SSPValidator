
Imports System.Collections.Generic
Imports System.Xml


'Lookup for SSP commands, generic responses and poll responses.
Class CSspLookup
	'Command struct.  If command has fixed length, that length will be held in .Length
	'If command has variable length, .Length will be 0 and byte at position given by .LengthByte
	'should be read, multiplied by .Multiply and have .Add added to give the length of the
	'command. Variable length commands facility is not used at the moment.  
	Private Structure SspCommand
		Public CommandName As String
		Public Length As Integer
		Public LengthByte As Integer
		Public Multiply As Integer
		Public Add As Integer
	End Structure
	'Poll response struct.  If poll response has fixed length, that length will be held in .Length
	'If poll response has variable length, .Length will be 0 and byte at position in data block given by .LengthByte
	'should be read, multiplied by .Multiply and have .Add added to give the length of the
	'poll response. 
	Private Structure SspPollResponse
		Public ResponseName As String
		Public Length As Integer
		Public LengthByte As Integer
		Public Multiply As Integer
		Public Add As Integer
	End Structure
	'Generic response struct.  Struct used rather than a string for possible future expansion.
	Private Structure SspGenericResponse
		Public ResponseName As String
	End Structure

	'Dictionaries to contain data read from XML files.
	Private commandsDictionary As Dictionary(Of Integer, SspCommand)
	Private pollResponsesDictionary As Dictionary(Of Integer, SspPollResponse)
	Private genericResponsesDictionary As Dictionary(Of Integer, SspGenericResponse)

	'Constructor
	Public Sub New()
		CSspReadCommandsData()
		CSspReadGenericResponsesData()
		CSspReadPollResponsesData()
	End Sub

	'Takes commandCode and returns CommandName from dictionary
	Public Function GetCommandName(commandCode As Integer) As String
		Dim SspCom As New SspCommand()
		If commandsDictionary.ContainsKey(commandCode) Then
			SspCom = commandsDictionary(commandCode)
			Return SspCom.CommandName
		End If
		Return "Unknown Command"
	End Function

	'Takes responseCode and returns ResponseName from dictionary
	Public Function GetGenericResponseName(responseCode As Integer) As String
		Dim SspGenRep As New SspGenericResponse()
		If genericResponsesDictionary.ContainsKey(responseCode) Then
			SspGenRep = genericResponsesDictionary(responseCode)
			Return SspGenRep.ResponseName
		End If
		Return "Unknown Response"
	End Function

	'Takes response to poll command as a byte array, parses out individual responses and returns
	'a string containing the response names
	Public Function GetPollResponse(response As Byte()) As String
		Dim ReturnString As String = ""
		Dim ResponseCode As Integer = 0
		Dim Increment As Integer = 0
		Dim SspPolResp As New SspPollResponse()
		Try
			'Start at first byte
			Dim ByteCounter As Integer = 1
			'Loop until end of byte array is reached
			While ByteCounter < response.Length
				'Add the ResponseName for the ResponseCode to the return string.
				ResponseCode = response(ByteCounter)
				SspPolResp = pollResponsesDictionary(ResponseCode)
				ReturnString += SspPolResp.ResponseName

				'If response is fixed length
				If SspPolResp.Length <> 0 Then
					'Increment byte counter by fixed amount
					Increment = SspPolResp.Length
				Else
					'Else calculate length of response from data and increment byte counter
					Increment = response((ByteCounter + SspPolResp.LengthByte))
					Increment *= SspPolResp.Multiply
					Increment += SspPolResp.Add
					Increment += 1
				End If

				ByteCounter += Increment

				If ByteCounter < response.Length Then
					ReturnString += ", "
				End If
			End While
		Catch e As Exception
			ReturnString = e.Message
		End Try
		ReturnString += vbCr & vbLf
		Return ReturnString
	End Function

	'Reads data from Resources/Commands.xml
	Private Sub CSspReadCommandsData()
		commandsDictionary = New Dictionary(Of Integer, SspCommand)()
		Dim document As New XmlDocument()
		Dim SspCom As New SspCommand()
		Dim i As Integer = 0
		document.Load("Resources/Commands.xml")

		Dim nodeList As XmlNodeList = document.DocumentElement.SelectNodes("/Root/CommandInfo")

		'Loop through all CommandInfo nodes in /Root.
		For Each node As XmlNode In nodeList
			'The xml document contains a CommandInfo node for every possible (0x00 to 0xFF) value of CommandCode.
			'Only process nodes witha a <"CommandName"/> child
			If node.SelectSingleNode("CommandName") IsNot Nothing Then
				i = Int32.Parse(node.SelectSingleNode("CommandCode").InnerText)

				'Put CommandName into struct 
				SspCom.CommandName = node.SelectSingleNode("CommandName").InnerText

				'Add any other elements, if present in xml
				If node.SelectSingleNode("Length") IsNot Nothing Then
					SspCom.Length = Int32.Parse(node.SelectSingleNode("Length").InnerText)
				Else
					SspCom.Length = 0
				End If

				If node.SelectSingleNode("LengthByte") IsNot Nothing Then
					SspCom.LengthByte = Int32.Parse(node.SelectSingleNode("LengthByte").InnerText)
				Else
					SspCom.LengthByte = 0
				End If

				If node.SelectSingleNode("Multiply") IsNot Nothing Then
					SspCom.Multiply = Int32.Parse(node.SelectSingleNode("Multiply").InnerText)
				Else
					SspCom.Multiply = 0
				End If

				If node.SelectSingleNode("Add") IsNot Nothing Then
					SspCom.Add = Int32.Parse(node.SelectSingleNode("Add").InnerText)
				Else
					SspCom.Add = 0
				End If
				'Add entry to dictionary
				commandsDictionary.Add(i, SspCom)
			End If
		Next
	End Sub
	'Reads data from Resources/GenericResponses.xml
	Private Sub CSspReadGenericResponsesData()
		genericResponsesDictionary = New Dictionary(Of Integer, SspGenericResponse)()
		Dim document As New XmlDocument()
		Dim SspGenericResponse As New SspGenericResponse()
		Dim i As Integer = 0
		document.Load("Resources/GenericResponses.xml")

		Dim nodeList As XmlNodeList = document.DocumentElement.SelectNodes("/Root/GenericResponseInfo")
		'Loop through all GenericResponseInfo nodes in /Root.
		For Each node As XmlNode In nodeList
			'The xml document contains a GenericResponseInfo node for every possible (0x00 to 0xFF) value of GenericResponseCode.
			'Only process nodes witha a <"GenericResponseName"/> child
			If node.SelectSingleNode("GenericResponseName") IsNot Nothing Then
				'Add GenericResponseCode and GenericResponseName into the dictionary.
				i = Int32.Parse(node.SelectSingleNode("GenericResponseCode").InnerText)
				SspGenericResponse.ResponseName = node.SelectSingleNode("GenericResponseName").InnerText
				genericResponsesDictionary.Add(i, SspGenericResponse)
			End If
		Next
	End Sub

	'Reads data from Resources/PollResponses.xml.
	Private Sub CSspReadPollResponsesData()
		pollResponsesDictionary = New Dictionary(Of Integer, SspPollResponse)()
		Dim document As New XmlDocument()
		Dim SspPoll As New SspPollResponse()
		Dim i As Integer = 0
		document.Load("Resources/PollResponses.xml")

		Dim nodeList As XmlNodeList = document.DocumentElement.SelectNodes("/Root/PollResponseInfo")

		'Loop through all PollResponseInfo nodes in /Root.
		For Each node As XmlNode In nodeList

			If node.SelectSingleNode("PollResponseName") IsNot Nothing Then
				i = Int32.Parse(node.SelectSingleNode("PollResponseCode").InnerText)

				'Put PollResponseName into struct 
				SspPoll.ResponseName = node.SelectSingleNode("PollResponseName").InnerText

				'Add any other elements, if present in xml
				If node.SelectSingleNode("Length") IsNot Nothing Then
					SspPoll.Length = Int32.Parse(node.SelectSingleNode("Length").InnerText)
				Else
					SspPoll.Length = 0
				End If

				If node.SelectSingleNode("LengthByte") IsNot Nothing Then
					SspPoll.LengthByte = Int32.Parse(node.SelectSingleNode("LengthByte").InnerText)
				Else
					SspPoll.LengthByte = 0
				End If

				If node.SelectSingleNode("Multiply") IsNot Nothing Then
					SspPoll.Multiply = Int32.Parse(node.SelectSingleNode("Multiply").InnerText)
				Else
					SspPoll.Multiply = 0
				End If

				If node.SelectSingleNode("Add") IsNot Nothing Then
					SspPoll.Add = Int32.Parse(node.SelectSingleNode("Add").InnerText)
				Else
					SspPoll.Add = 0
				End If
				'Add entry to dictionary
				pollResponsesDictionary.Add(i, SspPoll)
			End If
		Next
	End Sub
End Class
