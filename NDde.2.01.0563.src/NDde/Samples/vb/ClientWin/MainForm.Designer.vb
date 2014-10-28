<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.displayTextBox = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'displayTextBox
        '
        Me.displayTextBox.Location = New System.Drawing.Point(11, 11)
        Me.displayTextBox.Name = "displayTextBox"
        Me.displayTextBox.ReadOnly = True
        Me.displayTextBox.Size = New System.Drawing.Size(270, 20)
        Me.displayTextBox.TabIndex = 0
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(292, 41)
        Me.Controls.Add(Me.displayTextBox)
        Me.Name = "MainForm"
        Me.Padding = New System.Windows.Forms.Padding(8)
        Me.Text = "DDE Sample Application"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents displayTextBox As System.Windows.Forms.TextBox

End Class
