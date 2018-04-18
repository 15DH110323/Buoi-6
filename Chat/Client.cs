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

namespace Chat
{
    public partial class Client : Form
    {
        IPEndPoint ipe;
        Socket client ;
        public Client()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Connect();

        }
        void Connect()
        {
            ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            try
            {
                client.Connect(ipe);
            }
            catch
            {
                MessageBox.Show("Không thể kết nối server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();

        }
        void    Close()// dong ket noi
        {
            client.Close();
        }

        void Send()
        {
            if (txtMessage.Text != string.Empty)
                client.Send(Serialize(txtMessage.Text));
        }
        void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5];
                    client.Receive(data);

                    string message = (string)Deserilalize(data); // ep kieu object thanh string vi gui nhan tin nhan kieu string

                    AddMessage(message);
                }
            }
            catch {
                Close();
            }
        }
        void AddMessage(string s)
        {
            lsvMessage.Items.Add(new ListViewItem() { Text = s });
            txtMessage.Clear();
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
        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();// dong ket noi khi dong form;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            Send();
            AddMessage(txtMessage.Text);  
        }

        private void txtMessage_TextChanged(object sender, EventArgs e)
        {

        }

        private void lsvMessage_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
