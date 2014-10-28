Imports System.Text
Imports NDde.Client

Module Program

    Sub Main()

        ' Wait for the user to press ENTER before proceding.
        Console.WriteLine("The Server sample must be running before the client can connect.")
        Console.WriteLine("Press ENTER to continue...")
        Console.ReadLine()

        Try
            ' Create a client that connects to 'myapp|mytopic'. 
            Using client As DdeClient = New DdeClient("myapp", "mytopic")

                ' Subscribe to the Disconnected event.  This event will notify the application when a conversation has been terminated.
                AddHandler client.Disconnected, AddressOf OnDisconnected

                ' Connect to the server.  It must be running or an exception will be thrown.
                client.Connect()

                ' Synchronous Execute Operation
                client.Execute("mycommand", 60000)

                ' Synchronous Poke Operation
                client.Poke("myitem", DateTime.Now.ToString(), 60000)

                ' Syncronous Request Operation
                Console.WriteLine("Request: " + client.Request("myitem", 60000))

                ' Asynchronous Execute Operation
                client.BeginExecute("mycommand", AddressOf OnExecuteComplete, client)

                ' Asynchronous Poke Operation
                client.BeginPoke("myitem", Encoding.ASCII.GetBytes(DateTime.Now.ToString() + Convert.ToChar(0)), 1, AddressOf OnPokeComplete, client)

                ' Asynchronous Request Operation
                client.BeginRequest("myitem", 1, AddressOf OnRequestComplete, client)

                ' Advise Loop
                client.StartAdvise("myitem", 1, True, 60000)
                AddHandler client.Advise, AddressOf OnAdvise

                ' Wait for the user to press ENTER before proceding.
                Console.WriteLine("Press ENTER to quit...")
                Console.ReadLine()

            End Using

        Catch e As Exception

            Console.WriteLine(e.ToString())
            Console.WriteLine("Press ENTER to quit...")
            Console.ReadLine()

        End Try

    End Sub

    Private Sub OnExecuteComplete(ByVal ar As IAsyncResult)
        Try
            Dim client As DdeClient = DirectCast(ar.AsyncState, DdeClient)
            client.EndExecute(ar)
            Console.WriteLine("OnExecuteComplete")
        Catch e As Exception
            Console.WriteLine("OnExecuteComplete: " + e.Message)
        End Try
    End Sub

    Private Sub OnPokeComplete(ByVal ar As IAsyncResult)
        Try
            Dim client As DdeClient = DirectCast(ar.AsyncState, DdeClient)
            client.EndPoke(ar)
            Console.WriteLine("OnPokeComplete")
        Catch e As Exception
            Console.WriteLine("OnPokeComplete: " + e.Message)
        End Try
    End Sub

    Private Sub OnRequestComplete(ByVal ar As IAsyncResult)
        Try
            Dim client As DdeClient = DirectCast(ar.AsyncState, DdeClient)
            Dim data() As Byte = client.EndRequest(ar)
            Console.WriteLine("OnRequestComplete: " + Encoding.ASCII.GetString(data))
        Catch e As Exception
            Console.WriteLine("OnRequestComplete: " + e.Message)
        End Try
    End Sub

    Private Sub OnStartAdviseComplete(ByVal ar As IAsyncResult)
        Try
            Dim client As DdeClient = DirectCast(ar.AsyncState, DdeClient)
            client.EndStartAdvise(ar)
            Console.WriteLine("OnStartAdviseComplete")
        Catch e As Exception
            Console.WriteLine("OnStartAdviseComplete: " + e.Message)
        End Try
    End Sub

    Private Sub OnStopAdviseComplete(ByVal ar As IAsyncResult)
        Try
            Dim client As DdeClient = DirectCast(ar.AsyncState, DdeClient)
            client.EndStopAdvise(ar)
            Console.WriteLine("OnStopAdviseComplete")
        Catch e As Exception
            Console.WriteLine("OnStopAdviseComplete: " + e.Message)
        End Try
    End Sub

    Private Sub OnAdvise(ByVal sender As Object, ByVal args As DdeAdviseEventArgs)
        Console.WriteLine("OnAdvise: " + args.Text)
    End Sub

    Private Sub OnDisconnected(ByVal sender As Object, ByVal args As DdeDisconnectedEventArgs)
        Console.WriteLine( _
         "OnDisconnected: " + _
         "IsServerInitiated=" + args.IsServerInitiated.ToString() + " " + _
         "IsDisposed=" + args.IsDisposed.ToString())
    End Sub

End Module
