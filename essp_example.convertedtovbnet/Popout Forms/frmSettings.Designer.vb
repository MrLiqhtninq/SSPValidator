Partial Class frmSettings
	''' <summary>
	''' Required designer variable.
	''' </summary>
	Private components As System.ComponentModel.IContainer = Nothing

	''' <summary>
	''' Clean up any resources being used.
	''' </summary>
	''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	Protected Overrides Sub Dispose(disposing As Boolean)
		If disposing AndAlso (components IsNot Nothing) Then
			components.Dispose()
		End If
		MyBase.Dispose(disposing)
	End Sub

	#Region "Windows Form Designer generated code"

	''' <summary>
	''' Required method for Designer support - do not modify
	''' the contents of this method with the code editor.
	''' </summary>
	Private Sub InitializeComponent()
		Me.cbComPort = New System.Windows.Forms.ComboBox()
		Me.label1 = New System.Windows.Forms.Label()
		Me.btnOK = New System.Windows.Forms.Button()
		Me.btnCancel = New System.Windows.Forms.Button()
		Me.tbSSPAddress = New System.Windows.Forms.TextBox()
		Me.label2 = New System.Windows.Forms.Label()
		Me.SuspendLayout()
		' 
		' cbComPort
		' 
		Me.cbComPort.FormattingEnabled = True
		Me.cbComPort.Location = New System.Drawing.Point(85, 25)
		Me.cbComPort.Name = "cbComPort"
		Me.cbComPort.Size = New System.Drawing.Size(113, 21)
		Me.cbComPort.TabIndex = 0
		' 
		' label1
		' 
		Me.label1.AutoSize = True
		Me.label1.Location = New System.Drawing.Point(29, 28)
		Me.label1.Name = "label1"
		Me.label1.Size = New System.Drawing.Size(50, 13)
		Me.label1.TabIndex = 1
		Me.label1.Text = "Com Port"
		' 
		' btnOK
		' 
		Me.btnOK.Location = New System.Drawing.Point(18, 97)
		Me.btnOK.Name = "btnOK"
		Me.btnOK.Size = New System.Drawing.Size(75, 23)
		Me.btnOK.TabIndex = 2
		Me.btnOK.Text = "&Ok"
		Me.btnOK.UseVisualStyleBackColor = True
		AddHandler Me.btnOK.Click, New System.EventHandler(AddressOf Me.btnOK_Click)
		' 
		' btnCancel
		' 
		Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.btnCancel.Location = New System.Drawing.Point(112, 97)
		Me.btnCancel.Name = "btnCancel"
		Me.btnCancel.Size = New System.Drawing.Size(75, 23)
		Me.btnCancel.TabIndex = 3
		Me.btnCancel.Text = "&Cancel"
		Me.btnCancel.UseVisualStyleBackColor = True
		AddHandler Me.btnCancel.Click, New System.EventHandler(AddressOf Me.btnCancel_Click)
		' 
		' tbSSPAddress
		' 
		Me.tbSSPAddress.Location = New System.Drawing.Point(85, 52)
		Me.tbSSPAddress.Name = "tbSSPAddress"
		Me.tbSSPAddress.Size = New System.Drawing.Size(113, 20)
		Me.tbSSPAddress.TabIndex = 4
		' 
		' label2
		' 
		Me.label2.AutoSize = True
		Me.label2.Location = New System.Drawing.Point(10, 55)
		Me.label2.Name = "label2"
		Me.label2.Size = New System.Drawing.Size(69, 13)
		Me.label2.TabIndex = 5
		Me.label2.Text = "SSP Address"
		' 
		' frmSettings
		' 
		Me.AcceptButton = Me.btnOK
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.CancelButton = Me.btnCancel
		Me.ClientSize = New System.Drawing.Size(210, 141)
		Me.Controls.Add(Me.label2)
		Me.Controls.Add(Me.tbSSPAddress)
		Me.Controls.Add(Me.btnCancel)
		Me.Controls.Add(Me.btnOK)
		Me.Controls.Add(Me.label1)
		Me.Controls.Add(Me.cbComPort)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
		Me.Name = "frmSettings"
		Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
		Me.Text = "Settings"
		AddHandler Me.Load, New System.EventHandler(AddressOf Me.frmSettings_Load)
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	#End Region

	Private cbComPort As System.Windows.Forms.ComboBox
	Private label1 As System.Windows.Forms.Label
	Private btnOK As System.Windows.Forms.Button
	Private btnCancel As System.Windows.Forms.Button
	Private tbSSPAddress As System.Windows.Forms.TextBox
	Private label2 As System.Windows.Forms.Label
End Class
