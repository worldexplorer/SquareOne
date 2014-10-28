Imports System.Text
Imports NDde.Client

Public Class MainForm
    Private WithEvents client As New DdeClient("myapp", "mytopic", Me)

    Private Sub MainForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' Connect to the server.  It must be running or an exception will be thrown.
            client.Connect()

            ' Advise Loop
            client.StartAdvise("myitem", 1, True, 60000)
        Catch ex As Exception
            displayTextBox.Text = ex.Message
        End Try
    End Sub

    Private Sub MainForm_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        displayTextBox.Left = Me.DisplayRectangle.Left
        displayTextBox.Width = Me.DisplayRectangle.Width
        displayTextBox.Top = Me.DisplayRectangle.Top
        displayTextBox.Height = Me.DisplayRectangle.Height
    End Sub

    Private Sub client_Advise(ByVal sender As Object, ByVal e As NDde.Client.DdeAdviseEventArgs) Handles client.Advise
        displayTextBox.Text = "OnAdvise: " + e.Text
    End Sub

    Private Sub client_Disconnected(ByVal sender As Object, ByVal e As NDde.Client.DdeDisconnectedEventArgs) Handles client.Disconnected
        displayTextBox.Text = _
         "OnDisconnected: " + _
         "IsServerInitiated=" + e.IsServerInitiated.ToString() + " " + _
         "IsDisposed=" + e.IsDisposed.ToString()
    End Sub
End Class
