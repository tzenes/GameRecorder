using System;
using System.Windows.Forms;
using Gma.UserActivityMonitor;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Net;


namespace Gma.UserActivityMonitorDemo
{
    public partial class UserInterface : Form
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeFileHandle CreateFile(
            String pipeName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplate);

        private FileStream fStream;
        private double laststop = 0;
        private StreamWriter sw;
        private StreamReader sr;
        private string gameName;
        private string userName;
        private bool changeFlag;
        private bool toggle;
        private bool debug;
        private string filename;

        public UserInterface()
        {
            InitializeComponent();
            changeFlag = true;
            toggle = true;
            debug = false;
        }

        //##################################################################

        #region Hook Loaders
        private void button1_Click(object sender, EventArgs e)
        {
            if (toggle)
            {
                CreateFile();
                HookManager.KeyUp += HookManager_Key;
                HookManager.MouseDown += HookManager_Mouse;
                toggle = false;
                button1.Text = "Stop";
            }
            else
            {
                sw.Close();
                fStream.Close();
                fStream = null;
                changeFlag = true;
                HookManager.KeyUp -= HookManager_Key;
                HookManager.MouseDown -= HookManager_Mouse;
                toggle = true;
                button1.Text = "Start";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (debug == true) debug = false;
            else debug = true;
        }

        #endregion 

        //##################################################################

        #region Hook Managers
        private void HookManager_Key(object sender, KeyEventArgs e)
        {

            byte[] message = new byte[16];
            char myKey = (char)e.KeyCode;
            byte[] time = BitConverter.GetBytes((double)sender);
            for (int i = 0; i < 8; i++)
                message[i] = time[i];
            byte[] key = BitConverter.GetBytes(myKey);
            message[8] = key[0];

            //file code
            CreateFile();
            double stopwatch = (double)sender - laststop;
            laststop = (double)sender;
            string fileoutput = string.Format("{0}\t{1}\n", stopwatch, myKey);
            sw.Write(fileoutput); sw.Flush();

            if (debug)
            {
                textBoxLog.AppendText(string.Format("Key - {0}\n", e.KeyCode));
                textBoxLog.ScrollToCaret();
            }
        }

        private void HookManager_Mouse(object sender, MouseEventArgs e)
        {
            byte[] message = new byte[16];
            byte[] time = BitConverter.GetBytes((double)sender);
            for (int i = 0; i < 8; i++)
                message[i] = time[i];
            byte[] key = BitConverter.GetBytes('É');
            message[8] = key[0];

            //file code
            CreateFile();
            double stopwatch = (double)sender - laststop;
            laststop = (double)sender;
            string fileoutput = string.Format("{0}\t{1}\n", stopwatch, 'É');
            sw.Write(fileoutput); sw.Flush();

            if (debug)
            {
                textBoxLog.AppendText(string.Format("Mouse - {0}\n", e.Button));
                textBoxLog.ScrollToCaret();
            }

        }
        #endregion 

        //##################################################################

        #region Event handlers for game and user name
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            gameName = GameNameCB.SelectedItem.ToString();
            changeFlag = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            userName = UserNameTxt.Text;
            changeFlag = true;

        }

        #endregion 

        //##################################################################

        private void CreateFile() //full create
        {
            
            if (fStream == null)
            {
                int i = 0;
                while (File.Exists(string.Concat(userName, string.Concat(gameName, Convert.ToString(i), ".txt"))))
                {
                    i++;
                }
                fStream = new FileStream(string.Concat(userName, string.Concat(gameName, Convert.ToString(i), ".txt")), FileMode.Create, FileAccess.Write, FileShare.None);
                sw = new StreamWriter(fStream);
            }
            else if (changeFlag == false)
                return;
            else
            {

                changeFlag = false;
                sw.Close();
                fStream.Close();

                int i = 0;
                do
                {
                    filename = string.Concat(userName, string.Concat(gameName, Convert.ToString(i++), ".txt"));
                } while (File.Exists(filename));
                fStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
                sw = new StreamWriter(fStream);

            }
        }

        private void UploadSlow()
        {
            if (sw == null)//don't have a file yet
                return;
            //get the file to send
            button1_Click(null, null);
            fStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
            sr = new StreamReader(fStream);
            string str = sr.ReadToEnd();

            //Send the data
            string output, uri = String.Concat("http://odouls.cs.uvic.ca/Difficulty/upload.php?filename=",filename);//?name=uploaded&
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri); request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";

            byte[] postBytes = Encoding.ASCII.GetBytes(str);
            request.ContentType = "text";
            request.ContentLength = postBytes.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //Console.WriteLine(new StreamReader(response.GetResponseStream()).ReadToEnd());
            //Console.WriteLine("Headers:");
            //Console.WriteLine(response.Headers.ToString());
            Stream responseStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8);
            output = readStream.ReadToEnd();
            response.Close();
            readStream.Close();
        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            UploadSlow();
        }


    }
}
