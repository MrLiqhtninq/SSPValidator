Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports System.Threading
Imports ITLlib

Public Partial Class Form1
	Inherits Form
	' Variables used by this program.

    Private Running As Boolean = False ' Indicates the status of the main poll loop
    Private pollTimer As Integer = 250 ' Timer in ms between polls
    Private reconnectionAttempts As Integer = 10, reconnectionInterval As Integer = 3 ' Connection info to deal with retrying connection to validator
    Private Connected As Boolean = False
    Private ConnectionFail As Boolean = False ' Threading bools to indicate status of connection with validator
    Private Validator As CValidator ' The main validator class - used to send commands to the unit
    Private FormSetup As Boolean = False ' Boolean so the form will only be setup once
    Private reconnectionTimer As New System.Windows.Forms.Timer() ' Timer used to give a delay between reconnect attempts
    Private ConnectionThread As Thread ' Thread used to connect to the validator

    ' Constructor
	Public Sub New()
		InitializeComponent()
		timer1.Interval = pollTimer
		AddHandler reconnectionTimer.Tick, New EventHandler(AddressOf reconnectionTimer_Tick)
	End Sub

	' This updates UI variables such as textboxes etc.
	Private Sub UpdateUI()
		' update text boxes
		tbNumNotes.Text = Validator.NumberOfNotesStacked.ToString()
	End Sub

	' The main program loop, this is to control the validator, it polls at
	' a value set in this class (pollTimer).
	Private Sub MainLoop()
		btnRun.Enabled = False
		Validator.CommandStructure.ComPort = [Global].ComPort
		Validator.CommandStructure.SSPAddress = [Global].SSPAddress
		Validator.CommandStructure.Timeout = 3000

		' connect to the validator
		If ConnectToValidator() Then
			Running = True
			textBox1.AppendText(vbCr & vbLf & "Poll Loop" & vbCr & vbLf & "*********************************" & vbCr & vbLf)
			btnHalt.Enabled = True
		End If

		While Running
			' if the poll fails, try to reconnect
			If Not Validator.DoPoll(textBox1) Then
				textBox1.AppendText("Poll failed, attempting to reconnect..." & vbCr & vbLf)
				Connected = False
				ConnectionThread = New Thread(AddressOf ConnectToValidatorThreaded)
				ConnectionThread.Start()

                While Not Connected
                    Threading.Thread.MemoryBarrier()
                    If ConnectionFail Then
                        Threading.Thread.MemoryBarrier()
                        textBox1.AppendText("Failed to reconnect to validator" & vbCr & vbLf)
                        Return
                    End If
                    Application.DoEvents()
                End While
				textBox1.AppendText("Reconnected successfully" & vbCr & vbLf)
			End If

			timer1.Enabled = True
			' update form
			UpdateUI()
			' setup dynamic elements of win form once
			If Not FormSetup Then
				SetupFormLayout()
				FormSetup = True
			End If
			While timer1.Enabled
				Application.DoEvents()
					' Yield to free up CPU
				Thread.Sleep(1)
			End While
		End While

		'close com port and threads
		Validator.SSPComms.CloseComPort()

		btnRun.Enabled = True
		btnHalt.Enabled = False
	End Sub

	' This is a one off function that is called the first time the MainLoop()
	' function runs, it just sets up a few of the UI elements that only need
	' updating once.
	Private Sub SetupFormLayout()
		' need validator class instance
		If Validator Is Nothing Then
			MessageBox.Show("Validator class is null.", "ERROR")
			Return
		End If
	End Sub

	' This function opens the com port and attempts to connect with the validator. It then negotiates
	' the keys for encryption and performs some other setup commands.
	Private Function ConnectToValidator() As Boolean
		' setup the timer
		reconnectionTimer.Interval = reconnectionInterval * 1000
		' for ms
		' run for number of attempts specified
		For i As Integer = 0 To reconnectionAttempts - 1
			' reset timer
			reconnectionTimer.Enabled = True

			' close com port in case it was open
			Validator.SSPComms.CloseComPort()

			' turn encryption off for first stage
			Validator.CommandStructure.EncryptionStatus = False

			' open com port and negotiate keys
			If Validator.OpenComPort(textBox1) AndAlso Validator.NegotiateKeys(textBox1) Then
				Validator.CommandStructure.EncryptionStatus = True
				' now encrypting
				' find the max protocol version this validator supports
				Dim maxPVersion As Byte = FindMaxProtocolVersion()
				If maxPVersion > 6 Then
					Validator.SetProtocolVersion(maxPVersion, textBox1)
				Else
					MessageBox.Show("This program does not support units under protocol version 6, update firmware.", "ERROR")
					Return False
				End If
				' get info from the validator and store useful vars
				Validator.ValidatorSetupRequest(textBox1)
				' check this unit is supported by this program
				If Not IsUnitTypeSupported(Validator.UnitType) Then
					MessageBox.Show("Unsupported unit type, this SDK supports the BV series and the NV series (excluding the NV11)")
					Application.[Exit]()
					Return False
				End If
				' inhibits, this sets which channels can receive notes
				Validator.SetInhibits(textBox1)
				' enable, this allows the validator to receive and act on commands
				Validator.EnableValidator(textBox1)

				Return True
			End If
			While reconnectionTimer.Enabled
				Application.DoEvents()
				' wait for reconnectionTimer to tick
			End While
		Next
		Return False
	End Function

	' This is the same as the above function but set up differently for threading.
	Private Sub ConnectToValidatorThreaded()
		' setup the timer
		reconnectionTimer.Interval = reconnectionInterval * 1000
		' for ms
		' run for number of attempts specified
		For i As Integer = 0 To reconnectionAttempts - 1
			' reset timer
			reconnectionTimer.Enabled = True

			' close com port in case it was open
			Validator.SSPComms.CloseComPort()

			' turn encryption off for first stage
			Validator.CommandStructure.EncryptionStatus = False

			' open com port and negotiate keys
			If Validator.OpenComPort() AndAlso Validator.NegotiateKeys() Then
				Validator.CommandStructure.EncryptionStatus = True
				' now encrypting
				' find the max protocol version this validator supports
				Dim maxPVersion As Byte = FindMaxProtocolVersion()
				If maxPVersion > 6 Then
					Validator.SetProtocolVersion(maxPVersion)
				Else
					MessageBox.Show("This program does not support units under protocol version 6, update firmware.", "ERROR")
					Connected = False
					Return
				End If
				' get info from the validator and store useful vars
				Validator.ValidatorSetupRequest()
				' inhibits, this sets which channels can receive notes
				Validator.SetInhibits()
				' enable, this allows the validator to operate
				Validator.EnableValidator()
                Threading.Thread.MemoryBarrier()
				Connected = True
				Return
			End If
			While reconnectionTimer.Enabled
				Application.DoEvents()
				' wait for reconnectionTimer to tick
			End While
		Next

        Threading.Thread.MemoryBarrier()
        Connected = False

        Threading.Thread.MemoryBarrier()
        ConnectionFail = True
	End Sub

	' This function finds the maximum protocol version that a validator supports. To do this
	' it attempts to set a protocol version starting at 6 in this case, and then increments the
	' version until error 0xF8 is returned from the validator which indicates that it has failed
	' to set it. The function then returns the version number one less than the failed version.
	Private Function FindMaxProtocolVersion() As Byte
		' not dealing with protocol under level 6
		' attempt to set in validator
		Dim b As Byte = &H6
		While True
			Validator.SetProtocolVersion(b)
			If Validator.CommandStructure.ResponseData(0) = CCommands.SSP_RESPONSE_FAIL Then
                Return b - CByte(1)
			End If
            b += CByte(1)
			If b > 20 Then
				Return &H6
				' return default if protocol 'runs away'
			End If
        End While
        Return &H6
	End Function

	' This function checks whether the type of validator is supported by this program. This program only
	' supports Note Validators so any other type should be rejected.
	Private Function IsUnitTypeSupported(type As Char) As Boolean
        If type = Chr(&H0) Then
            Return True
        End If
		Return False
	End Function
End Class
