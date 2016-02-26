using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsynSocketClient
{
    class Program
    {
        // The port number for the remote device.
        private const int port = 2112;
        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        // The response from the remote device.
        private static String response = String.Empty;

        /// <summary>
        /// 开始
        /// </summary>
        private static void StartClient()
        {
            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                // The name of the remote device is "host.contoso.com".
                //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                //IPAddress ipAddress = ipHostInfo.AddressList[1];
                IPAddress ipAddress = IPAddress.Parse("192.168.1.8");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.
                Socket client = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Send test data to the remote device.
                int i = int.MaxValue;
                Send(client, "This is a" + i.ToString() + " test<EOF>");
                sendDone.WaitOne();


                // Receive the response from the remote device.
                Receive(client);
                receiveDone.WaitOne();

                // Write the response to the console.
                Console.WriteLine("Response received : {0}", response);

                // Release the socket.
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// 客户端的连接回调函数
        /// </summary>
        /// <param name="ar"></param>
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // 这里是根据BeginConnect调用函数中的object参数进行强制转换的
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// 接收函数
        /// </summary>
        /// <param name="client"></param>
        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;
                //   state:
                //     一个用户定义对象，其中包含接收操作的相关信息。当操作完成时，此对象会被传递给 System.Net.Sockets.Socket.EndReceive(System.IAsyncResult)
                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// 接收函数的回调函数（个人认为，实际微软的实现应该是通过另一线程促发，发送同）
        /// </summary>
        /// <param name="ar">此参数就是我们传递的state，而这个参数中的属性存储这传过来的数据</param>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.
                //这里ar.AsyncState携带的就是我们传递过来的state拉
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // 递归调用，使得获取数据完全
                    // Get the rest of the data.
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// 发送函数
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        private static void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        /// <summary>
        /// 发送函数的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static int Main(String[] args)
        {
            StartClient();
            return 0;
        }
    }

    /// <summary>
    /// 预定义的用于委托之间的类，但是我还是不清楚这里里面的字段息是如何赋值的
    /// </summary>
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 256;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

}
