Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Public Class ChannelData
	Public Value As Integer
	Public Channel As Byte
	Public Currency As Char()
	Public Level As Integer
	Public Recycling As Boolean
	Public Sub New()
		Value = 0
		Channel = 0
		Currency = New Char(2) {}
		Level = 0
		Recycling = False
	End Sub
End Class

Public Class CHelpers
	' Helper function to convert 4 bytes to an int 32 from a specified array and index.
	Public Shared Function ConvertBytesToInt32(b As Byte(), index As Integer) As Integer
		Return BitConverter.ToInt32(b, index)
	End Function

	' 2 bytes to int 16
	Public Shared Function ConvertBytesToInt16(b As Byte(), index As Integer) As Integer
		Return BitConverter.ToInt16(b, index)
	End Function

	' Convert int32 to byte array
	Public Shared Function ConvertIntToBytes(n As Integer) As Byte()
		Return BitConverter.GetBytes(n)
	End Function

	' Convert int16 to byte array
	Public Shared Function ConvertIntToBytes(n As Short) As Byte()
		Return BitConverter.GetBytes(n)
	End Function

	' Convert int8 to byte
	Public Shared Function ConvertIntToBytes(n As Char) As Byte()
		Return BitConverter.GetBytes(n)
	End Function

	' Helper uses value of channel and adds a decimal point and two zeroes, also adds current
	' currency.
	Public Shared Function FormatToCurrency(unformattedNumber As Integer) As String
		Dim f As Single = unformattedNumber * 0.01F
		Return f.ToString("0.00")
	End Function

	' This helper takes a byte and returns the command/response name as a string.
	Public Shared Function ConvertByteToName(b As Byte) As String
		Select Case b
			Case &H1
				Return "RESET COMMAND"
			Case &H11
				Return "SYNC COMMAND"
			Case &H4a
				Return "SET GENERATOR COMMAND"
			Case &H4b
				Return "SET MODULUS COMMAND"
			Case &H4c
				Return "KEY EXCHANGE COMMAND"
			Case &H2
				Return "SET INHIBITS COMMAND"
			Case &Ha
				Return "ENABLE COMMAND"
			Case &H9
				Return "DISABLE COMMAND"
			Case &H7
				Return "POLL COMMAND"
			Case &H5
				Return "SETUP REQUEST COMMAND"
			Case &H3
				Return "DISPLAY ON COMMAND"
			Case &H4
				Return "DISPLAY OFF COMMAND"
			Case &H5c
				Return "ENABLE PAYOUT COMMAND"
			Case &H5b
				Return "DISABLE PAYOUT COMMAND"
			Case &H3b
				Return "SET ROUTING COMMAND"
			Case &H45
				Return "SET VALUE REPORTING TYPE COMMAND"
			Case &H42
				Return "PAYOUT LAST NOTE COMMAND"
			Case &H3f
				Return "EMPTY COMMAND"
			Case &H41
				Return "GET NOTE POSITIONS COMMAND"
			Case &H43
				Return "STACK LAST NOTE COMMAND"
			Case &Hf1
				Return "RESET RESPONSE"
			Case &Hef
				Return "NOTE READ RESPONSE"
			Case &Hee
				Return "CREDIT RESPONSE"
			Case &Hed
				Return "REJECTING RESPONSE"
			Case &Hec
				Return "REJECTED RESPONSE"
			Case &Hcc
				Return "STACKING RESPONSE"
			Case &Heb
				Return "STACKED RESPONSE"
			Case &Hea
				Return "SAFE JAM RESPONSE"
			Case &He9
				Return "UNSAFE JAM RESPONSE"
			Case &He8
				Return "DISABLED RESPONSE"
			Case &He6
				Return "FRAUD ATTEMPT RESPONSE"
			Case &He7
				Return "STACKER FULL RESPONSE"
			Case &He1
				Return "NOTE CLEARED FROM FRONT RESPONSE"
			Case &He2
				Return "NOTE CLEARED TO CASHBOX RESPONSE"
			Case &He3
				Return "CASHBOX REMOVED RESPONSE"
			Case &He4
				Return "CASHBOX REPLACED RESPONSE"
			Case &Hdb
				Return "NOTE STORED RESPONSE"
			Case &Hda
				Return "NOTE DISPENSING RESPONSE"
			Case &Hd2
				Return "NOTE DISPENSED RESPONSE"
			Case &Hc9
				Return "NOTE TRANSFERRED TO STACKER RESPONSE"
			Case &Hf0
				Return "OK RESPONSE"
			Case &Hf2
				Return "UNKNOWN RESPONSE"
			Case &Hf3
				Return "WRONG PARAMS RESPONSE"
			Case &Hf4
				Return "PARAM OUT OF RANGE RESPONSE"
			Case &Hf5
				Return "CANNOT PROCESS RESPONSE"
			Case &Hf6
				Return "SOFTWARE ERROR RESPONSE"
			Case &Hf8
				Return "FAIL RESPONSE"
			Case &Hfa
				Return "KEY NOT SET RESPONSE"
			Case Else
				Return "Byte command name unsupported"
		End Select
	End Function
End Class
