using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Sever
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Connect();
        }

        IPEndPoint ipe;
        Socket server;
        List<Socket> clientlist;
        void Connect()
        {
            clientlist = new List<Socket>();
            ipe = new IPEndPoint(IPAddress.Any, 9999);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            server.Bind(ipe);
            Thread Listen = new Thread(() => {
                try
                {
                    while (true)
                    {
                        server.Listen(100);
                        Socket client = server.Accept();
                        clientlist.Add(client);

                        Thread receive = new Thread(Receive);
                        receive.IsBackground = true;
                        receive.Start(client);
                    }
                }
                catch
                {
                    ipe = new IPEndPoint(IPAddress.Any, 9999);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                }

            });
            Listen.IsBackground = true;
            Listen.Start();

        }
        void Close()// dong ket noi
        {
            server.Close();
        }

        void Send( Socket client)
        {
            if (client != null && txtMessage.Text != string.Empty)
                client.Send(Serialize(txtMessage.Text));
        }
        void Receive(object obj)
        {
            Socket client = obj as Socket;
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5];
                    client.Receive(data);

                    string message = (string)Deserilalize(data); // ep kieu object thanh string vi gui nhan tin nhan kieu string

                    foreach(Socket item in clientlist)
                    {
                        if (item != null && item !=client)
                        item.Send(Serialize(message));
                    }
                    AddMessage(message);
                }
            }
            catch
            {
                clientlist.Remove(client);
                client.Close();
            }
        }
        void AddMessage(string s)
        {
            lsvMessage.Items.Add(new ListViewItem() { Text = s });
        } //add message vao khung chat
        byte[] Serialize(object obj) //Phan manh
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }
        object Deserilalize(byte[] data)// gom Manh
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }
        private void Sever_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
        }

        private void btnSend_Click(object sender, EventArgs e)// gui tin cho tat ca client
        {
            foreach (Socket item in clientlist)
            {
                Send(item);
            }
            AddMessage(txtMessage.Text);
            txtMessage.Clear();
        }

        private void Server_Load(object sender, EventArgs e)
        {

        }
    }
}
