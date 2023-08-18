Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports System.Timers
Imports ITLlib

Public Class CValidator
    ' ssp library variables
    Private m_eSSP As SSPComms
    Private m_cmd As SSP_COMMAND
    Private keys As SSP_KEYS
    Private sspKey As SSP_FULL_KEY
    Private info As SSP_COMMAND_INFO

    ' variable declarations

    ' The comms window class, used to log everything sent to the validator visually and to file
    Private m_Comms As CCommsWindow

    ' The protocol version this validator is using, set in setup request
    Private m_ProtocolVersion As Integer

    ' A variable to hold the type of validator, this variable is initialised using the setup request command
    Private m_UnitType As Char

    ' Variable to hold the number of notes accepted by the validator.
    Private m_NumStackedNotes As Integer

    ' Variable to hold the number of channels in the validator dataset
    Private m_NumberOfChannels As Integer

    ' The multiplier by which the channel values are multiplied to give their
    ' real penny value. E.g. £5.00 on channel 1, the value would be 5 and the multiplier
    ' 100.
    Private m_ValueMultiplier As Integer

    ' A list of dataset data, sorted by value. Holds the info on channel number, value, currency,
    ' level and whether it is being recycled.
    Private m_UnitDataList As List(Of ChannelData)

    ' constructor
    Public Sub New()
        m_eSSP = New SSPComms()
        m_cmd = New SSP_COMMAND()
        keys = New SSP_KEYS()
        sspKey = New SSP_FULL_KEY()
        info = New SSP_COMMAND_INFO()

        m_Comms = New CCommsWindow("NoteValidator")
        m_NumberOfChannels = 0
        m_ValueMultiplier = 1
        m_UnitType = Chr(&HFF)
        m_UnitDataList = New List(Of ChannelData)()
    End Sub

    ' Variable Access 


    ' access to ssp variables
    ' the pointer which gives access to library functions such as open com port, send command etc
    Public Property SSPComms() As SSPComms
        Get
            Return m_eSSP
        End Get
        Set(value As SSPComms)
            m_eSSP = Value
        End Set
    End Property

    ' a pointer to the command structure, this struct is filled with info and then compiled into
    ' a packet by the library and sent to the validator
    Public Property CommandStructure() As SSP_COMMAND
        Get
            Return m_cmd
        End Get
        Set(value As SSP_COMMAND)
            m_cmd = Value
        End Set
    End Property

    ' pointer to an information structure which accompanies the command structure
    Public Property InfoStructure() As SSP_COMMAND_INFO
        Get
            Return info
        End Get
        Set(value As SSP_COMMAND_INFO)
            info = Value
        End Set
    End Property

    ' access to the comms log for recording new log messages
    Public Property CommsLog() As CCommsWindow
        Get
            Return m_Comms
        End Get
        Set(value As CCommsWindow)
            m_Comms = Value
        End Set
    End Property

    ' access to the type of unit, this will only be valid after the setup request
    Public ReadOnly Property UnitType() As Char
        Get
            Return m_UnitType
        End Get
    End Property

    ' access to number of channels being used by the validator
    Public Property NumberOfChannels() As Integer
        Get
            Return m_NumberOfChannels
        End Get
        Set(value As Integer)
            m_NumberOfChannels = Value
        End Set
    End Property

    ' access to number of notes stacked
    Public Property NumberOfNotesStacked() As Integer
        Get
            Return m_NumStackedNotes
        End Get
        Set(value As Integer)
            m_NumStackedNotes = Value
        End Set
    End Property

    ' access to value multiplier
    Public Property Multiplier() As Integer
        Get
            Return m_ValueMultiplier
        End Get
        Set(value As Integer)
            m_ValueMultiplier = Value
        End Set
    End Property

    ' get a channel value
    Public Function GetChannelValue(channelNum As Integer) As Integer
        If channelNum >= 1 AndAlso channelNum <= m_NumberOfChannels Then
            For Each d As ChannelData In m_UnitDataList
                If d.Channel = channelNum Then
                    Return d.Value
                End If
            Next
        End If
        Return -1
    End Function

    ' get a channel currency
    Public Function GetChannelCurrency(channelNum As Integer) As String
        If channelNum >= 1 AndAlso channelNum <= m_NumberOfChannels Then
            For Each d As ChannelData In m_UnitDataList
                If d.Channel = channelNum Then
                    Return New String(d.Currency)
                End If
            Next
        End If
        Return ""
    End Function

    ' Command functions 


    ' The enable command allows the validator to receive and act on commands sent to it.
    Public Sub EnableValidator(Optional log As TextBox = Nothing)
        m_cmd.CommandData(0) = CCommands.SSP_CMD_ENABLE
        m_cmd.CommandDataLength = 1

        If Not SendCommand(log) Then
            Return
        End If
        ' check response
        If CheckGenericResponses(log) AndAlso log IsNot Nothing Then
            log.AppendText("Unit enabled" & vbCr & vbLf)
        End If
    End Sub

    ' Disable command stops the validator from acting on commands.
    Public Sub DisableValidator(Optional log As TextBox = Nothing)
        m_cmd.CommandData(0) = CCommands.SSP_CMD_DISABLE
        m_cmd.CommandDataLength = 1

        If Not SendCommand(log) Then
            Return
        End If
        ' check response
        If CheckGenericResponses(log) AndAlso log IsNot Nothing Then
            log.AppendText("Unit disabled" & vbCr & vbLf)
        End If
    End Sub

    ' The reset command instructs the validator to restart (same effect as switching on and off)
    Public Sub Reset(Optional log As TextBox = Nothing)
        m_cmd.CommandData(0) = CCommands.SSP_CMD_RESET
        m_cmd.CommandDataLength = 1
        If Not SendCommand(log) Then
            Return
        End If

        If CheckGenericResponses(log) Then
            log.AppendText("Resetting unit" & vbCr & vbLf)
        End If
    End Sub

    ' This command just sends a sync command to the validator
    Public Function SendSync(Optional log As TextBox = Nothing) As Boolean
        m_cmd.CommandData(0) = CCommands.SSP_CMD_SYNC
        m_cmd.CommandDataLength = 1
        If Not SendCommand(log) Then
            Return False
        End If

        If CheckGenericResponses(log) Then
            log.AppendText("Successfully sent sync" & vbCr & vbLf)
        End If
        Return True
    End Function

    ' This function sets the protocol version in the validator to the version passed across. Whoever calls
    ' this needs to check the response to make sure the version is supported.
    Public Sub SetProtocolVersion(pVersion As Byte, Optional log As TextBox = Nothing)
        m_cmd.CommandData(0) = CCommands.SSP_CMD_HOST_PROTOCOL_VERSION
        m_cmd.CommandData(1) = pVersion
        m_cmd.CommandDataLength = 2
        If Not SendCommand(log) Then
            Return
        End If
    End Sub

    ' This function sends the command LAST REJECT CODE which gives info about why a note has been rejected. It then
    ' outputs the info to a passed across textbox.
    Public Sub QueryRejection(log As TextBox)
        m_cmd.CommandData(0) = CCommands.SSP_CMD_LAST_REJECT_CODE
        m_cmd.CommandDataLength = 1
        If Not SendCommand(log) Then
            Return
        End If

        If CheckGenericResponses(log) Then
            If log Is Nothing Then
                Return
            End If
            Select Case m_cmd.ResponseData(1)
                Case &H0
                    log.AppendText("Note accepted" & vbCr & vbLf)
                    Exit Select
                Case &H1
                    log.AppendText("Note length incorrect" & vbCr & vbLf)
                    Exit Select
                Case &H2
                    log.AppendText("Invalid note" & vbCr & vbLf)
                    Exit Select
                Case &H3
                    log.AppendText("Invalid note" & vbCr & vbLf)
                    Exit Select
                Case &H4
                    log.AppendText("Invalid note" & vbCr & vbLf)
                    Exit Select
                Case &H5
                    log.AppendText("Invalid note" & vbCr & vbLf)
                    Exit Select
                Case &H6
                    log.AppendText("Channel inhibited" & vbCr & vbLf)
                    Exit Select
                Case &H7
                    log.AppendText("Second note inserted during read" & vbCr & vbLf)
                    Exit Select
                Case &H8
                    log.AppendText("Host rejected note" & vbCr & vbLf)
                    Exit Select
                Case &H9
                    log.AppendText("Invalid note" & vbCr & vbLf)
                    Exit Select
                Case &HA
                    log.AppendText("Invalid note read" & vbCr & vbLf)
                    Exit Select
                Case &HB
                    log.AppendText("Note too long" & vbCr & vbLf)
                    Exit Select
                Case &HC
                    log.AppendText("Validator disabled" & vbCr & vbLf)
                    Exit Select
                Case &HD
                    log.AppendText("Mechanism slow/stalled" & vbCr & vbLf)
                    Exit Select
                Case &HE
                    log.AppendText("Strim attempt" & vbCr & vbLf)
                    Exit Select
                Case &HF
                    log.AppendText("Fraud channel reject" & vbCr & vbLf)
                    Exit Select
                Case &H10
                    log.AppendText("No notes inserted" & vbCr & vbLf)
                    Exit Select
                Case &H11
                    log.AppendText("Invalid note read" & vbCr & vbLf)
                    Exit Select
                Case &H12
                    log.AppendText("Twisted note detected" & vbCr & vbLf)
                    Exit Select
                Case &H13
                    log.AppendText("Escrow time-out" & vbCr & vbLf)
                    Exit Select
                Case &H14
                    log.AppendText("Bar code scan fail" & vbCr & vbLf)
                    Exit Select
                Case &H15
                    log.AppendText("Invalid note read" & vbCr & vbLf)
                    Exit Select
                Case &H16
                    log.AppendText("Invalid note read" & vbCr & vbLf)
                    Exit Select
                Case &H17
                    log.AppendText("Invalid note read" & vbCr & vbLf)
                    Exit Select
                Case &H18
                    log.AppendText("Invalid note read" & vbCr & vbLf)
                    Exit Select
                Case &H19
                    log.AppendText("Incorrect note width" & vbCr & vbLf)
                    Exit Select
                Case &H1A
                    log.AppendText("Note too short" & vbCr & vbLf)
                    Exit Select
            End Select
        End If
    End Sub

    ' This function performs a number of commands in order to setup the encryption between the host and the validator.
    Public Function NegotiateKeys(Optional log As TextBox = Nothing) As Boolean
        Dim i As Byte

        ' make sure encryption is off
        m_cmd.EncryptionStatus = False

        ' send sync
        If log IsNot Nothing Then
            log.AppendText("Syncing... ")
        End If
        m_cmd.CommandData(0) = CCommands.SSP_CMD_SYNC
        m_cmd.CommandDataLength = 1

        If Not SendCommand(log) Then
            Return False
        End If
        If log IsNot Nothing Then
            log.AppendText("Success")
        End If

        m_eSSP.InitiateSSPHostKeys(keys, m_cmd)

        ' send generator
        m_cmd.CommandData(0) = CCommands.SSP_CMD_SET_GENERATOR
        m_cmd.CommandDataLength = 9
        If log IsNot Nothing Then
            log.AppendText("Setting generator... ")
        End If

        ' Convert generator to bytes and add to command data.
        BitConverter.GetBytes(keys.Generator).CopyTo(m_cmd.CommandData, 1)

        If Not SendCommand(log) Then
            Return False
        End If
        If log IsNot Nothing Then
            log.AppendText("Success" & vbCr & vbLf)
        End If

        ' send modulus
        m_cmd.CommandData(0) = CCommands.SSP_CMD_SET_MODULUS
        m_cmd.CommandDataLength = 9
        If log IsNot Nothing Then
            log.AppendText("Sending modulus... ")
        End If

        ' Convert modulus to bytes and add to command data.
        BitConverter.GetBytes(keys.Modulus).CopyTo(m_cmd.CommandData, 1)

        If Not SendCommand(log) Then
            Return False
        End If
        If log IsNot Nothing Then
            log.AppendText("Success" & vbCr & vbLf)
        End If

        ' send key exchange
        m_cmd.CommandData(0) = CCommands.SSP_CMD_REQUEST_KEY_EXCHANGE
        m_cmd.CommandDataLength = 9
        If log IsNot Nothing Then
            log.AppendText("Exchanging keys... ")
        End If

        ' Convert host intermediate key to bytes and add to command data.
        BitConverter.GetBytes(keys.HostInter).CopyTo(m_cmd.CommandData, 1)

        If Not SendCommand(log) Then
            Return False
        End If
        If log IsNot Nothing Then
            log.AppendText("Success" & vbCr & vbLf)
        End If

        ' Read slave intermediate key.
        keys.SlaveInterKey = BitConverter.ToUInt64(m_cmd.ResponseData, 1)

        m_eSSP.CreateSSPHostEncryptionKey(keys)

        ' get full encryption key
        m_cmd.Key.FixedKey = &H123456701234567L
        m_cmd.Key.VariableKey = keys.KeyHost

        If log IsNot Nothing Then
            log.AppendText("Keys successfully negotiated" & vbCr & vbLf)
        End If

        Return True
    End Function

    ' This function uses the setup request command to get all the information about the validator.
    Public Sub ValidatorSetupRequest(Optional log As TextBox = Nothing)
        Dim sbDisplay As New StringBuilder(1000)

        ' send setup request
        m_cmd.CommandData(0) = CCommands.SSP_CMD_SETUP_REQUEST
        m_cmd.CommandDataLength = 1

        If Not SendCommand(log) Then
            Return
        End If

        ' display setup request


        ' unit type
        Dim index As Integer = 1
        sbDisplay.Append("Unit Type: ")
        m_UnitType = Chr(m_cmd.ResponseData(index))
        index += 1
        Select Case m_UnitType
            Case Chr(&H0)
                sbDisplay.Append("Validator")
                Exit Select
            Case Chr(&H3)
                sbDisplay.Append("SMART Hopper")
                Exit Select
            Case Chr(&H6)
                sbDisplay.Append("SMART Payout")
                Exit Select
            Case Chr(&H7)
                sbDisplay.Append("NV11")
                Exit Select
            Case Chr(&HD)
                sbDisplay.Append("TEBS")
                Exit Select
            Case Else
                sbDisplay.Append("Unknown Type")
                Exit Select
        End Select

        ' firmware
        sbDisplay.AppendLine()
        sbDisplay.Append("Firmware: ")

        sbDisplay.Append(Chr(m_cmd.ResponseData(index)))
        index += 1
        sbDisplay.Append(Chr(m_cmd.ResponseData(index)))
        index += 1
        sbDisplay.Append(".")
        sbDisplay.Append(Chr(m_cmd.ResponseData(index)))
        index += 1
        sbDisplay.Append(Chr(m_cmd.ResponseData(index)))
        index += 1

        sbDisplay.AppendLine()
        ' country code.
        ' legacy code so skip it.
        index += 3

        ' value multiplier.
        ' legacy code so skip it.
        index += 3

        ' Number of channels
        sbDisplay.AppendLine()
        sbDisplay.Append("Number of Channels: ")
        m_NumberOfChannels = m_cmd.ResponseData(index)
        index += 1
        sbDisplay.Append(m_NumberOfChannels)
        sbDisplay.AppendLine()

        ' channel values.
        ' legacy code so skip it.
        index += m_NumberOfChannels
        ' Skip channel values
        ' channel security
        ' legacy code so skip it.
        index += m_NumberOfChannels

        ' real value multiplier
        ' (big endian)
        sbDisplay.Append("Real Value Multiplier: ")
        m_ValueMultiplier = m_cmd.ResponseData(index + 2)
        m_ValueMultiplier += m_cmd.ResponseData(index + 1) << 8
        m_ValueMultiplier += m_cmd.ResponseData(index) << 16
        sbDisplay.Append(m_ValueMultiplier)
        sbDisplay.AppendLine()
        index += 3


        ' protocol version
        sbDisplay.Append("Protocol Version: ")
        m_ProtocolVersion = m_cmd.ResponseData(index)
        index += 1
        sbDisplay.Append(m_ProtocolVersion)
        sbDisplay.AppendLine()

        ' Add channel data to list then display.
        ' Clear list.
        m_UnitDataList.Clear()
        ' Loop through all channels.

        For i As Byte = 0 To CByte(m_NumberOfChannels - 1)
            Dim loopChannelData As New ChannelData()
            ' Channel number.
            loopChannelData.Channel = CByte(i + 1)

            ' Channel value.
            loopChannelData.Value = BitConverter.ToInt32(m_cmd.ResponseData, index + (m_NumberOfChannels * 3) + (i * 4)) * m_ValueMultiplier

            ' Channel Currency
            loopChannelData.Currency(0) = Chr(m_cmd.ResponseData(index + (i * 3)))
            loopChannelData.Currency(1) = Chr(m_cmd.ResponseData((index + 1) + (i * 3)))
            loopChannelData.Currency(2) = Chr(m_cmd.ResponseData((index + 2) + (i * 3)))

            ' Channel level.
            loopChannelData.Level = 0

            ' Channel recycling
            loopChannelData.Recycling = False

            ' Add data to list.
            m_UnitDataList.Add(loopChannelData)

            'Display data
            sbDisplay.Append("Channel ")
            sbDisplay.Append(loopChannelData.Channel)
            sbDisplay.Append(": ")
            sbDisplay.Append(loopChannelData.Value \ m_ValueMultiplier)
            sbDisplay.Append(" ")
            sbDisplay.Append(loopChannelData.Currency)
            sbDisplay.AppendLine()
        Next

        ' Sort the list by .Value.
        m_UnitDataList.Sort(Function(d1, d2) d1.Value.CompareTo(d2.Value))

        If log IsNot Nothing Then
            log.AppendText(sbDisplay.ToString())
        End If
    End Sub

    ' This function sends the set inhibits command to set the inhibits on the validator. An additional two
    ' bytes are sent along with the command byte to indicate the status of the inhibits on the channels.
    ' For example 0xFF and 0xFF in binary is 11111111 11111111. This indicates all 16 channels supported by
    ' the validator are uninhibited. If a user wants to inhibit channels 8-16, they would send 0x00 and 0xFF.
    Public Sub SetInhibits(Optional log As TextBox = Nothing)
        ' set inhibits
        m_cmd.CommandData(0) = CCommands.SSP_CMD_SET_CHANNEL_INHIBITS
        m_cmd.CommandData(1) = &HFF
        m_cmd.CommandData(2) = &HFF
        m_cmd.CommandDataLength = 3

        If Not SendCommand(log) Then
            Return
        End If
        If CheckGenericResponses(log) AndAlso log IsNot Nothing Then
            log.AppendText("Inhibits set" & vbCr & vbLf)
        End If
    End Sub

    ' The poll function is called repeatedly to poll to validator for information, it returns as
    ' a response in the command structure what events are currently happening.
    Public Function DoPoll(log As TextBox) As Boolean
        Dim i As Byte

        'send poll
        m_cmd.CommandData(0) = CCommands.SSP_CMD_POLL
        m_cmd.CommandDataLength = 1

        If Not SendCommand(log) Then
            Return False
        End If

        'parse poll response
        Dim noteVal As Integer = 0
        For i = 1 To m_cmd.ResponseDataLength - CByte(1)
            Select Case m_cmd.ResponseData(i)
                ' This response indicates that the unit was reset and this is the first time a poll
                ' has been called since the reset.
                Case CCommands.SSP_POLL_SLAVE_RESET
                    log.AppendText("Unit reset" & vbCr & vbLf)
                    Exit Select
                    ' A note is currently being read by the validator sensors. The second byte of this response
                    ' is zero until the note's type has been determined, it then changes to the channel of the 
                    ' scanned note.
                Case CCommands.SSP_POLL_READ_NOTE
                    If m_cmd.ResponseData(i + 1) > 0 Then
                        noteVal = GetChannelValue(m_cmd.ResponseData(i + 1))
                        log.AppendText("Note in escrow, amount: " & CHelpers.FormatToCurrency(noteVal) & " " & GetChannelCurrency(m_cmd.ResponseData(i + 1)) & vbCr & vbLf)
                    Else
                        log.AppendText("Reading note..." & vbCr & vbLf)
                    End If
                    i += CByte(1)
                    Exit Select
                    ' A credit event has been detected, this is when the validator has accepted a note as legal currency.
                Case CCommands.SSP_POLL_CREDIT_NOTE
                    noteVal = GetChannelValue(m_cmd.ResponseData(i + 1))
                    log.AppendText("Credit " & CHelpers.FormatToCurrency(noteVal) & " " & GetChannelCurrency(m_cmd.ResponseData(i + 1)) & vbCr & vbLf)
                    m_NumStackedNotes += 1
                    i += CByte(1)
                    Exit Select
                    ' A note is being rejected from the validator. This will carry on polling while the note is in transit.
                Case CCommands.SSP_POLL_NOTE_REJECTING
                    log.AppendText("Rejecting note..." & vbCr & vbLf)
                    Exit Select
                    ' A note has been rejected from the validator, the note will be resting in the bezel. This response only
                    ' appears once.
                Case CCommands.SSP_POLL_NOTE_REJECTED
                    log.AppendText("Note rejected" & vbCr & vbLf)
                    QueryRejection(log)
                    Exit Select
                    ' A note is in transit to the cashbox.
                Case CCommands.SSP_POLL_NOTE_STACKING
                    log.AppendText("Stacking note..." & vbCr & vbLf)
                    Exit Select
                    ' A note has reached the cashbox.
                Case CCommands.SSP_POLL_NOTE_STACKED
                    log.AppendText("Note stacked" & vbCr & vbLf)
                    Exit Select
                    ' A safe jam has been detected. This is where the user has inserted a note and the note
                    ' is jammed somewhere that the user cannot reach.
                Case CCommands.SSP_POLL_SAFE_NOTE_JAM
                    log.AppendText("Safe jam" & vbCr & vbLf)
                    Exit Select
                    ' An unsafe jam has been detected. This is where a user has inserted a note and the note
                    ' is jammed somewhere that the user can potentially recover the note from.
                Case CCommands.SSP_POLL_UNSAFE_NOTE_JAM
                    log.AppendText("Unsafe jam" & vbCr & vbLf)
                    Exit Select
                    ' The validator is disabled, it will not execute any commands or do any actions until enabled.
                Case CCommands.SSP_POLL_DISABLED
                    Exit Select
                    ' A fraud attempt has been detected. The second byte indicates the channel of the note that a fraud
                    ' has been attempted on.
                Case CCommands.SSP_POLL_FRAUD_ATTEMPT
                    log.AppendText("Fraud attempt, note type: " & GetChannelValue(m_cmd.ResponseData(i + 1)) & vbCr & vbLf)
                    i += CByte(1)
                    Exit Select
                    ' The stacker (cashbox) is full. 
                Case CCommands.SSP_POLL_STACKER_FULL
                    log.AppendText("Stacker full" & vbCr & vbLf)
                    Exit Select
                    ' A note was detected somewhere inside the validator on startup and was rejected from the front of the
                    ' unit.
                Case CCommands.SSP_POLL_NOTE_CLEARED_FROM_FRONT
                    log.AppendText(GetChannelValue(m_cmd.ResponseData(i + 1)) & " note cleared from front at reset." & vbCr & vbLf)
                    i += CByte(1)
                    Exit Select
                    ' A note was detected somewhere inside the validator on startup and was cleared into the cashbox.
                Case CCommands.SSP_POLL_NOTE_CLEARED_TO_CASHBOX
                    log.AppendText(GetChannelValue(m_cmd.ResponseData(i + 1)) & " note cleared to stacker at reset." & vbCr & vbLf)
                    i += CByte(1)
                    Exit Select
                    ' The cashbox has been removed from the unit. This will continue to poll until the cashbox is replaced.
                Case CCommands.SSP_POLL_CASHBOX_REMOVED
                    log.AppendText("Cashbox removed..." & vbCr & vbLf)
                    Exit Select
                    ' The cashbox has been replaced, this will only display on a poll once.
                Case CCommands.SSP_POLL_CASHBOX_REPLACED
                    log.AppendText("Cashbox replaced" & vbCr & vbLf)
                    Exit Select
                    ' A bar code ticket has been detected and validated. The ticket is in escrow, continuing to poll will accept
                    ' the ticket, sending a reject command will reject the ticket.
                    'case CCommands.SSP_POLL_BAR_CODE_VALIDATED:
                    '    log.AppendText("Bar code ticket validated\r\n");
                    '    break;
                    '''/ A bar code ticket has been accepted (equivalent to note credit).
                    'case CCommands.SSP_POLL_BAR_CODE_ACK:
                    '    log.AppendText("Bar code ticket accepted\r\n");
                    '    break;
                    ' The validator has detected its note path is open. The unit is disabled while the note path is open.
                    ' Only available in protocol versions 6 and above.
                Case CCommands.SSP_POLL_NOTE_PATH_OPEN
                    log.AppendText("Note path open" & vbCr & vbLf)
                    Exit Select
                    ' All channels on the validator have been inhibited so the validator is disabled. Only available on protocol
                    ' versions 7 and above.
                Case CCommands.SSP_POLL_CHANNEL_DISABLE
                    log.AppendText("All channels inhibited, unit disabled" & vbCr & vbLf)
                    Exit Select
                Case Else
                    log.AppendText("Unrecognised poll response detected " & CInt(m_cmd.ResponseData(i)) & vbCr & vbLf)
                    Exit Select
            End Select
        Next
        Return True
    End Function

    ' Non-Command functions 


    ' This function calls the open com port function of the SSP library.
    Public Function OpenComPort(Optional log As TextBox = Nothing) As Boolean
        If log IsNot Nothing Then
            log.AppendText("Opening com port" & vbCr & vbLf)
        End If
        If Not m_eSSP.OpenSSPComPort(m_cmd) Then
            Return False
        End If
        Return True
    End Function

    ' Exception and Error Handling 


    ' This is used for generic response error catching, it outputs the info in a
    ' meaningful way.
    Private Function CheckGenericResponses(log As TextBox) As Boolean
        If m_cmd.ResponseData(0) = CCommands.SSP_RESPONSE_OK Then
            Return True
        Else
            If log IsNot Nothing Then
                Select Case m_cmd.ResponseData(0)
                    Case CCommands.SSP_RESPONSE_COMMAND_CANNOT_BE_PROCESSED
                        If m_cmd.ResponseData(1) = &H3 Then
                            log.AppendText("Validator has responded with ""Busy"", command cannot be processed at this time" & vbCr & vbLf)
                        Else
                            log.AppendText("Command response is CANNOT PROCESS COMMAND, error code - 0x" & BitConverter.ToString(m_cmd.ResponseData, 1, 1) & vbCr & vbLf)
                        End If
                        Return False
                    Case CCommands.SSP_RESPONSE_FAIL
                        log.AppendText("Command response is FAIL" & vbCr & vbLf)
                        Return False
                    Case CCommands.SSP_RESPONSE_KEY_NOT_SET
                        log.AppendText("Command response is KEY NOT SET, Validator requires encryption on this command or there is" & "a problem with the encryption on this request" & vbCr & vbLf)
                        Return False
                    Case CCommands.SSP_RESPONSE_PARAMETER_OUT_OF_RANGE
                        log.AppendText("Command response is PARAM OUT OF RANGE" & vbCr & vbLf)
                        Return False
                    Case CCommands.SSP_RESPONSE_SOFTWARE_ERROR
                        log.AppendText("Command response is SOFTWARE ERROR" & vbCr & vbLf)
                        Return False
                    Case CCommands.SSP_RESPONSE_COMMAND_NOT_KNOWN
                        log.AppendText("Command response is UNKNOWN" & vbCr & vbLf)
                        Return False
                    Case CCommands.SSP_RESPONSE_WRONG_NO_PARAMETERS
                        log.AppendText("Command response is WRONG PARAMETERS" & vbCr & vbLf)
                        Return False
                    Case Else
                        Return False
                End Select
            Else
                Return False
            End If
        End If
    End Function

    Public Function SendCommand(log As TextBox) As Boolean
        ' Backup data and length in case we need to retry
        Dim backup As Byte() = New Byte(254) {}
        m_cmd.CommandData.CopyTo(backup, 0)
        Dim length As Byte = m_cmd.CommandDataLength

        ' attempt to send the command
        If m_eSSP.SSPSendCommand(m_cmd, info) = False Then
            m_eSSP.CloseComPort()
            m_Comms.UpdateLog(info, True)
            ' update the log on fail as well
            If log IsNot Nothing Then
                log.AppendText("Sending command failed" & vbCr & vbLf & "Port status: " & m_cmd.ResponseStatus.ToString() & vbCr & vbLf)
            End If
            Return False
        End If

        ' update the log after every command
        m_Comms.UpdateLog(info)

        Return True
    End Function
End Class
