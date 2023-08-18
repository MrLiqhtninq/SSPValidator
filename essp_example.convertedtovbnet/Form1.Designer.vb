Partial Class Form1
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
		Me.components = New System.ComponentModel.Container()
		Me.btnRun = New System.Windows.Forms.Button()
		Me.btnHalt = New System.Windows.Forms.Button()
		Me.timer1 = New System.Windows.Forms.Timer(Me.components)
		Me.textBox1 = New System.Windows.Forms.TextBox()
		Me.resetValidatorBtn = New System.Windows.Forms.Button()
		Me.logTickBox = New System.Windows.Forms.CheckBox()
		Me.tbNumNotes = New System.Windows.Forms.TextBox()
		Me.label1 = New System.Windows.Forms.Label()
		Me.btnClear = New System.Windows.Forms.Button()
		Me.SuspendLayout()
		' 
		' btnRun
		' 
		Me.btnRun.Location = New System.Drawing.Point(18, 465)
		Me.btnRun.Margin = New System.Windows.Forms.Padding(4)
		Me.btnRun.Name = "btnRun"
		Me.btnRun.Size = New System.Drawing.Size(100, 28)
		Me.btnRun.TabIndex = 1
		Me.btnRun.Text = "&Run"
		Me.btnRun.UseVisualStyleBackColor = True
		AddHandler Me.btnRun.Click, New System.EventHandler(AddressOf Me.btnRun_Click)
		' 
		' btnHalt
		' 
		Me.btnHalt.Location = New System.Drawing.Point(142, 465)
		Me.btnHalt.Margin = New System.Windows.Forms.Padding(4)
		Me.btnHalt.Name = "btnHalt"
		Me.btnHalt.Size = New System.Drawing.Size(100, 28)
		Me.btnHalt.TabIndex = 2
		Me.btnHalt.Text = "&Halt"
		Me.btnHalt.UseVisualStyleBackColor = True
		AddHandler Me.btnHalt.Click, New System.EventHandler(AddressOf Me.btnHalt_Click)
		' 
		' timer1
		' 
		Me.timer1.Interval = 250
		AddHandler Me.timer1.Tick, New System.EventHandler(AddressOf Me.timer1_Tick)
		' 
		' textBox1
		' 
		Me.textBox1.BackColor = System.Drawing.SystemColors.ButtonHighlight
		Me.textBox1.Location = New System.Drawing.Point(16, 33)
		Me.textBox1.Margin = New System.Windows.Forms.Padding(4)
		Me.textBox1.Multiline = True
		Me.textBox1.Name = "textBox1"
		Me.textBox1.[ReadOnly] = True
		Me.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
		Me.textBox1.Size = New System.Drawing.Size(372, 334)
		Me.textBox1.TabIndex = 4
		' 
		' resetValidatorBtn
		' 
		Me.resetValidatorBtn.Location = New System.Drawing.Point(18, 415)
		Me.resetValidatorBtn.Margin = New System.Windows.Forms.Padding(4)
		Me.resetValidatorBtn.Name = "resetValidatorBtn"
		Me.resetValidatorBtn.Size = New System.Drawing.Size(188, 28)
		Me.resetValidatorBtn.TabIndex = 15
		Me.resetValidatorBtn.Text = "R&eset Validator"
		Me.resetValidatorBtn.UseVisualStyleBackColor = True
		AddHandler Me.resetValidatorBtn.Click, New System.EventHandler(AddressOf Me.resetValidatorBtn_Click)
		' 
		' logTickBox
		' 
		Me.logTickBox.AutoSize = True
		Me.logTickBox.Checked = True
		Me.logTickBox.CheckState = System.Windows.Forms.CheckState.Checked
		Me.logTickBox.Location = New System.Drawing.Point(285, 470)
		Me.logTickBox.Margin = New System.Windows.Forms.Padding(4)
		Me.logTickBox.Name = "logTickBox"
		Me.logTickBox.Size = New System.Drawing.Size(104, 21)
		Me.logTickBox.TabIndex = 16
		Me.logTickBox.Text = "Comms Log"
		Me.logTickBox.UseVisualStyleBackColor = True
		AddHandler Me.logTickBox.CheckedChanged, New System.EventHandler(AddressOf Me.logTickBox_CheckedChanged)
		' 
		' tbNumNotes
		' 
		Me.tbNumNotes.Location = New System.Drawing.Point(215, 375)
		Me.tbNumNotes.Margin = New System.Windows.Forms.Padding(4)
		Me.tbNumNotes.Name = "tbNumNotes"
		Me.tbNumNotes.Size = New System.Drawing.Size(132, 22)
		Me.tbNumNotes.TabIndex = 17
		' 
		' label1
		' 
		Me.label1.AutoSize = True
		Me.label1.Location = New System.Drawing.Point(25, 378)
		Me.label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.label1.Name = "label1"
		Me.label1.Size = New System.Drawing.Size(182, 17)
		Me.label1.TabIndex = 19
		Me.label1.Text = "Number of Notes Accepted:"
		' 
		' btnClear
		' 
		Me.btnClear.Location = New System.Drawing.Point(214, 415)
		Me.btnClear.Margin = New System.Windows.Forms.Padding(4)
		Me.btnClear.Name = "btnClear"
		Me.btnClear.Size = New System.Drawing.Size(133, 28)
		Me.btnClear.TabIndex = 21
		Me.btnClear.Text = "Clear Totals"
		Me.btnClear.UseVisualStyleBackColor = True
		AddHandler Me.btnClear.Click, New System.EventHandler(AddressOf Me.btnClear_Click)
		' 
		' Form1
		' 
		Me.AutoScaleDimensions = New System.Drawing.SizeF(8F, 16F)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(401, 504)
		Me.Controls.Add(Me.btnClear)
		Me.Controls.Add(Me.label1)
		Me.Controls.Add(Me.tbNumNotes)
		Me.Controls.Add(Me.logTickBox)
		Me.Controls.Add(Me.resetValidatorBtn)
		Me.Controls.Add(Me.textBox1)
		Me.Controls.Add(Me.btnHalt)
		Me.Controls.Add(Me.btnRun)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
		Me.Margin = New System.Windows.Forms.Padding(4)
		Me.Name = "Form1"
		Me.Text = "Validator eSSP C# example"
		AddHandler Me.FormClosing, New System.Windows.Forms.FormClosingEventHandler(AddressOf Me.Form1_FormClosing)
		AddHandler Me.Load, New System.EventHandler(AddressOf Me.Form1_Load)
		AddHandler Me.Shown, New System.EventHandler(AddressOf Me.Form1_Shown)
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	#End Region

	Private btnRun As System.Windows.Forms.Button
	Private btnHalt As System.Windows.Forms.Button
	Private timer1 As System.Windows.Forms.Timer
	Private textBox1 As System.Windows.Forms.TextBox
	Private resetValidatorBtn As System.Windows.Forms.Button
	Private logTickBox As System.Windows.Forms.CheckBox
	Private tbNumNotes As System.Windows.Forms.TextBox
	Private label1 As System.Windows.Forms.Label
	Private btnClear As System.Windows.Forms.Button

End Class

