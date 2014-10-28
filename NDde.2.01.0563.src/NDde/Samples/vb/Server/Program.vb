Imports NDde.Server

Module Program

    Sub Main()

        Try

            ' Create a server that will register the service name 'myapp'.
            Using server As DdeServer = New MyServer("myapp")

                ' Register the service name.
                server.Register()

                ' Wait for the user to press ENTER before proceding.
                Console.WriteLine("Press ENTER to quit...")
                Console.ReadLine()

            End Using

        Catch e As Exception

            Console.WriteLine(e)
            Console.WriteLine("Press ENTER to quit...")
            Console.ReadLine()

        End Try

    End Sub

    Private Class MyServer
        Inherits DdeServer

        Private WithEvents theTimer As System.Timers.Timer = New System.Timers.Timer()

        Public Sub New(ByVal service As String)
            MyBase.New(service)
            ' Create a timer that will be used to advise clients of new data.
            theTimer.Interval = 1000
            theTimer.SynchronizingObject = Me.Context
        End Sub

        Private Sub theTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles theTimer.Elapsed
            Me.Advise("*", "*")
        End Sub

        Public Overrides Sub Register()
            MyBase.Register()
            theTimer.Start()
        End Sub

        Public Overrides Sub Unregister()
            theTimer.Stop()
            MyBase.Unregister()
        End Sub

        Protected Overrides Function OnBeforeConnect(ByVal topic As String) As Boolean
            Console.WriteLine("OnBeforeConnect:".PadRight(16) _
             + " Service='" + MyBase.Service + "'" _
             + " Topic='" + topic + "'")

            Return True
        End Function

        Protected Overrides Sub OnAfterConnect(ByVal conversation As DdeConversation)
            Console.WriteLine("OnAfterConnect:".PadRight(16) _
             + " Service='" + conversation.Service + "'" _
             + " Topic='" + conversation.Topic + "'" _
             + " Handle=" + conversation.Handle.ToString())
        End Sub

        Protected Overrides Sub OnDisconnect(ByVal conversation As DdeConversation)
            Console.WriteLine("OnDisconnect:".PadRight(16) _
             + " Service='" + conversation.Service + "'" _
             + " Topic='" + conversation.Topic + "'" _
             + " Handle=" + conversation.Handle.ToString())
        End Sub

        Protected Overrides Function OnStartAdvise(ByVal conversation As DdeConversation, ByVal item As String, ByVal format As Integer) As Boolean
            Console.WriteLine("OnStartAdvise:".PadRight(16) _
             + " Service='" + conversation.Service + "'" _
             + " Topic='" + conversation.Topic + "'" _
             + " Handle=" + conversation.Handle.ToString() _
             + " Item='" + item + "'" _
             + " Format=" + format.ToString())

            ' Initiate the advisory loop only if the format is CF_TEXT.
            Return format = 1
        End Function

        Protected Overrides Sub OnStopAdvise(ByVal conversation As DdeConversation, ByVal item As String)
            Console.WriteLine("OnStopAdvise:".PadRight(16) _
              + " Service='" + conversation.Service + "'" _
              + " Topic='" + conversation.Topic + "'" _
              + " Handle=" + conversation.Handle.ToString() _
              + " Item='" + item + "'")
        End Sub

        Protected Overrides Function OnExecute(ByVal conversation As DdeConversation, ByVal command As String) As ExecuteResult
            Console.WriteLine("OnExecute:".PadRight(16) _
             + " Service='" + conversation.Service + "'" _
             + " Topic='" + conversation.Topic + "'" _
             + " Handle=" + conversation.Handle.ToString() _
             + " Command='" + command + "'")

            ' Tell the client that the command was processed.
            Return ExecuteResult.Processed
        End Function

        Protected Overrides Function OnPoke(ByVal conversation As DdeConversation, ByVal item As String, ByVal data As Byte(), ByVal format As Integer) As PokeResult
            Console.WriteLine("OnPoke:".PadRight(16) _
             + " Service='" + conversation.Service + "'" _
             + " Topic='" + conversation.Topic + "'" _
             + " Handle=" + conversation.Handle.ToString() _
             + " Item='" + item + "'" _
             + " Data=" + data.Length.ToString() _
             + " Format=" + format.ToString())

            ' Tell the client that the data was processed.
            Return PokeResult.Processed
        End Function

        Protected Overrides Function OnRequest(ByVal conversation As DdeConversation, ByVal item As String, ByVal format As Integer) As RequestResult
            Console.WriteLine("OnRequest:".PadRight(16) _
             + " Service='" + conversation.Service + "'" _
             + " Topic='" + conversation.Topic + "'" _
             + " Handle=" + conversation.Handle.ToString() _
             + " Item='" + item + "'" _
             + " Format=" + format.ToString())

            ' Return data to the client only if the format is CF_TEXT.
            If format = 1 Then
                Return New RequestResult(System.Text.Encoding.ASCII.GetBytes("Time=" + DateTime.Now.ToString() + Convert.ToChar(0)))
            End If
            Return RequestResult.NotProcessed
        End Function

        Protected Overrides Function OnAdvise(ByVal topic As String, ByVal item As String, ByVal format As Integer) As Byte()
            Console.WriteLine("OnAdvise:".PadRight(16) _
             + " Service='" + Me.Service + "'" _
             + " Topic='" + topic + "'" _
             + " Item='" + item + "'" _
             + " Format=" + format.ToString())

            ' Send data to the client only if the format is CF_TEXT.
            If format = 1 Then
                Return System.Text.Encoding.ASCII.GetBytes("Time=" + DateTime.Now.ToString() + Convert.ToChar(0))
            End If
            Return Nothing
        End Function

    End Class

End Module
