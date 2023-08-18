Partial Class frmOpenMenu
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
		Me.tbSSPAddress = New System.Windows.Forms.TextBox()
		Me.btnSearch = New System.Windows.Forms.Button()
		Me.label1 = New System.Windows.Forms.Label()
		Me.label2 = New System.Windows.Forms.Label()
		Me.SuspendLayout()
		' 
		' cbComPort
		' 
		Me.cbComPort.FormattingEnabled = True
		Me.cbComPort.Location = New System.Drawing.Point(95, 20)
		Me.cbComPort.Name = "cbComPort"
		Me.cbComPort.Size = New System.Drawing.Size(121, 21)
		Me.cbComPort.TabIndex = 0
		' 
		' tbSSPAddress
		' 
		Me.tbSSPAddress.Location = New System.Drawing.Point(95, 47)
		Me.tbSSPAddress.Name = "tbSSPAddress"
		Me.tbSSPAddress.Size = New System.Drawing.Size(121, 20)
		Me.tbSSPAddress.TabIndex = 1
		Me.tbSSPAddress.Text = "0"
		AddHandler Me.tbSSPAddress.KeyDown, New System.Windows.Forms.KeyEventHandler(AddressOf Me.tbSSPAddress_KeyDown)
		' 
		' btnSearch
		' 
		Me.btnSearch.Location = New System.Drawing.Point(95, 73)
		Me.btnSearch.Name = "btnSearch"
		Me.btnSearch.Size = New System.Drawing.Size(121, 23)
		Me.btnSearch.TabIndex = 2
		Me.btnSearch.Text = "&Connect"
		Me.btnSearch.UseVisualStyleBackColor = True
		AddHandler Me.btnSearch.Click, New System.EventHandler(AddressOf Me.btnSearch_Click)
		' 
		' label1
		' 
		Me.label1.AutoSize = True
		Me.label1.Location = New System.Drawing.Point(39, 23)
		Me.label1.Name = "label1"
		Me.label1.Size = New System.Drawing.Size(50, 13)
		Me.label1.TabIndex = 3
		Me.label1.Text = "Com Port"
		' 
		' label2
		' 
		Me.label2.AutoSize = True
		Me.label2.Location = New System.Drawing.Point(20, 50)
		Me.label2.Name = "label2"
		Me.label2.Size = New System.Drawing.Size(69, 13)
		Me.label2.TabIndex = 4
		Me.label2.Text = "SSP Address"
		' 
		' frmOpenMenu
		' 
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(249, 120)
		Me.Controls.Add(Me.label2)
		Me.Controls.Add(Me.label1)
		Me.Controls.Add(Me.btnSearch)
		Me.Controls.Add(Me.tbSSPAddress)
		Me.Controls.Add(Me.cbComPort)
		Me.Name = "frmOpenMenu"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "Example Software"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	#End Region

	Private cbComPort As System.Windows.Forms.ComboBox
	Private tbSSPAddress As System.Windows.Forms.TextBox
	Private btnSearch As System.Windows.Forms.Button
	Private label1 As System.Windows.Forms.Label
	Private label2 As System.Windows.Forms.Label
End Class
