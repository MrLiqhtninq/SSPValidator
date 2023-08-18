Partial Class CCommsWindow
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
		Me.checkBox1 = New System.Windows.Forms.CheckBox()
		Me.logWindowText = New System.Windows.Forms.TextBox()
		Me.SuspendLayout()
		' 
		' checkBox1
		' 
		Me.checkBox1.AutoSize = True
		Me.checkBox1.Location = New System.Drawing.Point(17, 497)
		Me.checkBox1.Margin = New System.Windows.Forms.Padding(4)
		Me.checkBox1.Name = "checkBox1"
		Me.checkBox1.Size = New System.Drawing.Size(198, 21)
		Me.checkBox1.TabIndex = 1
		Me.checkBox1.Text = "Record Encrypted Packets"
		Me.checkBox1.UseVisualStyleBackColor = True
		' 
		' logWindowText
		' 
		Me.logWindowText.BackColor = System.Drawing.SystemColors.ButtonHighlight
		Me.logWindowText.Font = New System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CByte(0))
		Me.logWindowText.Location = New System.Drawing.Point(17, 16)
		Me.logWindowText.Margin = New System.Windows.Forms.Padding(4)
		Me.logWindowText.Multiline = True
		Me.logWindowText.Name = "logWindowText"
		Me.logWindowText.[ReadOnly] = True
		Me.logWindowText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
		Me.logWindowText.Size = New System.Drawing.Size(607, 473)
		Me.logWindowText.TabIndex = 2
		' 
		' CCommsWindow
		' 
		Me.AutoScaleDimensions = New System.Drawing.SizeF(8F, 16F)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(644, 533)
		Me.Controls.Add(Me.logWindowText)
		Me.Controls.Add(Me.checkBox1)
		Me.Margin = New System.Windows.Forms.Padding(4)
		Me.MaximizeBox = False
		Me.Name = "CCommsWindow"
		Me.ShowInTaskbar = False
		Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
		Me.Text = "CommsWindow"
		AddHandler Me.Load, New System.EventHandler(AddressOf Me.CommsWindow_Load)
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	#End Region

	Private checkBox1 As System.Windows.Forms.CheckBox
	Private logWindowText As System.Windows.Forms.TextBox
End Class
