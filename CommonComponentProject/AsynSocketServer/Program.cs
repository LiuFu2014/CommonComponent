using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AsynSocketServer
{
    class Program
    {
        // Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        //public AsynchronousSocketListener() { }
        public static void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];
            // Establish the local endpoint for the socket.
            // The DNS name of the computer
            // running the listener is "host.contoso.com".

            string host = "192.168.1.8";
            IPAddress ip = IPAddress.Parse(host);
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ip, 2112);
            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                while (true)
                {
                    // Set the event to nonsignaled state.（通知allDone.WaitOne()阻塞，如果不调用，则只会阻塞初始的一次）
                    allDone.Reset();
                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);


                    //Socket handler = listener.Accept();
                    //// Create the state object.
                    //StateObject state = new StateObject();
                    //state.workSocket = handler;
                    //handler.BeginReceive(state.auffer2, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                    // Test
                    //Console.WriteLine("测试在accept之后是否还会继续流程。");
                    //// Wait until a connection is made before continuing.（allDone.Set()发出信号）
                    allDone.WaitOne();
                    //Console.WriteLine("测试在 allDone.WaitOne();之后是否还会继续流程。");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }
        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();
            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            // Create the state object.
            StateObject state = new StateObject();
            //这里将返回的socket赋值到自定义类，调用委托的时候可以
            state.workSocket = handler;
            handler.BeginReceive(state.auffer2, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);

        }
        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);
            if (bytesRead > 0)
            {
                // There    might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(state.auffer2, 0, bytesRead));
                // Check for end-of-file tag. If it is not there, read
                // more data.
                content = state.sb.ToString();
                //Console.WriteLine("from" + handler.RemoteEndPoint.ToString() + " mes:" + content);
                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the
                    // client. Display it on the console.
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                    content.Length, content);
                    // Echo the data back to the client.
                    Send(handler, "Server return :" + content);
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.auffer2, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }
        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), handler);
        }
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public static int Main(String[] args)
        {
            StartListening();
            return 0;
        }

    }

    /// <summary>
    /// 这里类到底是怎么获取数据的？
    /// </summary>
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        // 为什么我定义的这个auff2就接收不到传递的数据呢？
        public byte[] auffer2 = new byte[BufferSize];
        //public byte[] buffer3 = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();

    }
}
