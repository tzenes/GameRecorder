using System;
using System.Windows.Forms;
using Gma.UserActivityMonitor;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;


namespace Gma.UserActivityMonitorDemo
{
    public partial class TestFormStatic : Form
    {
        //deligate
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeFileHandle CreateFile(
            String pipeName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplate);


        //globals
        private SafeFileHandle pipeHandle;
        private FileStream fStream;
        private double laststop = 0;
        private StreamWriter sw;
        private string gameName;
        private string userName;
        private bool changeFlag;


        public TestFormStatic()
        {
            InitializeComponent();

            /*
            uint GENERIC_READ = (0x80000000);
            uint GENERIC_WRITE = (0x40000000);
            uint OPEN_EXISTING = 3;
            uint FILE_FLAG_OVERLAPPED = (0x40000000);
            int BUFFER_SIZE = 1024;
            string PIPE_NAME = "\\\\.\\pipe\\mynamedpipe";

            
            if (pipeHandle == null)
                pipeHandle = CreateFile(
                  PIPE_NAME,
                  GENERIC_READ | GENERIC_WRITE,
                  0,
                  IntPtr.Zero,
                  OPEN_EXISTING,
                  FILE_FLAG_OVERLAPPED,
                  IntPtr.Zero);

            //could not get a handle to the named pipe
            if (pipeHandle.IsInvalid)
                return;

            if (fStream == null) fStream = new FileStream(pipeHandle, FileAccess.ReadWrite, BUFFER_SIZE, true);
             * 
             
            if (fStream == null)
            {
                int i = 0;
                while (File.Exists(string.Concat("UserActions",Convert.ToString(i),".txt")))
                {
                    i++;
                }
                fStream = new FileStream(string.Concat("UserActions", Convert.ToString(i), ".txt"), FileMode.Create, FileAccess.Write, FileShare.None);
                sw = new StreamWriter(fStream);
            }*/
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        //##################################################################
        #region Check boxes to set or remove particular event handlers.

        private void checkBoxOnMouseMove_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxOnMouseMove.Checked)
            {
                HookManager.MouseMove += HookManager_MouseMove;
            }
            else
            {
                HookManager.MouseMove -= HookManager_MouseMove;
            }
        }

        private void checkBoxOnMouseClick_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxOnMouseClick.Checked)
            {
                HookManager.MouseClick += HookManager_MouseClick;
            }
            else
            {
                HookManager.MouseClick -= HookManager_MouseClick;
            }
        }

        private void checkBoxOnMouseUp_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxOnMouseUp.Checked)
            {
                HookManager.MouseUp += HookManager_MouseUp;
            }
            else
            {
                HookManager.MouseUp -= HookManager_MouseUp;
            }
        }

        private void checkBoxOnMouseDown_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxOnMouseDown.Checked)
            {
                HookManager.MouseDown += HookManager_MouseDown;
            }
            else
            {
                HookManager.MouseDown -= HookManager_MouseDown;
            }
        }

        private void checkBoxMouseDoubleClick_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxMouseDoubleClick.Checked)
            {
                HookManager.MouseDoubleClick += HookManager_MouseDoubleClick;
            }
            else
            {
                HookManager.MouseDoubleClick -= HookManager_MouseDoubleClick;
            }
        }

        private void checkBoxMouseWheel_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxMouseWheel.Checked)
            {
                HookManager.MouseWheel += HookManager_MouseWheel;
            }
            else
            {
                HookManager.MouseWheel -= HookManager_MouseWheel;
            }
        }

        private void checkBoxKeyDown_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxKeyDown.Checked)
            {
                HookManager.KeyDown += HookManager_KeyDown;
            }
            else
            {
                HookManager.KeyDown -= HookManager_KeyDown;
            }
        }


        private void checkBoxKeyUp_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxKeyUp.Checked)
            {
                HookManager.KeyUp += HookManager_KeyUp;
            }
            else
            {
                HookManager.KeyUp -= HookManager_KeyUp;
            }
        }

        private void checkBoxKeyPress_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxKeyPress.Checked)
            {
                HookManager.KeyPress += HookManager_KeyPress;
            }
            else
            {
                HookManager.KeyPress -= HookManager_KeyPress;
            }
        }

        #endregion

        //##################################################################
        #region Event handlers of particular events. They will be activated when an appropriate checkbox is checked.

        private void HookManager_KeyDown(object sender, KeyEventArgs e)
        {
            byte[] message = new byte[16];
            char myKey = (char)e.KeyCode;
            byte[] time = BitConverter.GetBytes((double)sender);
            for (int i = 0; i < 8; i++)
                message[i] = time[i];
            byte[] key = BitConverter.GetBytes(myKey);
            message[8] = key[0];
            
            /* pipe code
            fStream.Write(message, 0, 16);
            fStream.Flush();            
            */

            //file code
            CreateFile();
            double stopwatch = (double)sender - laststop;
            laststop = (double)sender;
            string fileoutput = string.Format("{0}\t{1}\n",stopwatch,myKey);
            sw.Write(fileoutput); sw.Flush();


            textBoxLog.AppendText(string.Format("KeyDown - {0}\n", e.KeyCode));
            textBoxLog.ScrollToCaret();
        }

        private void HookManager_KeyUp(object sender, KeyEventArgs e)
        {

            byte[] message = new byte[16];
            char myKey = (char)e.KeyCode;
            byte[] time = BitConverter.GetBytes((double)sender);
            for (int i = 0; i < 8; i++)
                message[i] = time[i];
            byte[] key = BitConverter.GetBytes(myKey);
            message[8] = key[0];
            /* pipe code
            fStream.Write(message, 0, 16);
            fStream.Flush();            
            */

            //file code
            CreateFile();
            double stopwatch = (double)sender - laststop;
            laststop = (double)sender;
            string fileoutput = string.Format("{0}\t{1}\n", stopwatch, myKey);
            sw.Write(fileoutput); sw.Flush();           


            textBoxLog.AppendText(string.Format("KeyUp - {0}\n", e.KeyCode));
            textBoxLog.ScrollToCaret();
        }


        private void HookManager_KeyPress(object sender, KeyPressEventArgs e)
        {

            byte[] message = new byte[16];
            byte[] time = BitConverter.GetBytes((double)sender);
            for (int i = 0; i < 8; i++)
                message[i] = time[i];
            byte[] key = BitConverter.GetBytes(e.KeyChar);
            message[8] = key[0];
            /* pipe code
            fStream.Write(message, 0, 16);
            fStream.Flush();            
            */

            //file code
            CreateFile();
            double stopwatch = (double)sender - laststop;
            laststop = (double)sender;
            string fileoutput = string.Format("{0}\t{1}\n", stopwatch, e.KeyChar);
            sw.Write(fileoutput); sw.Flush();           
            


            textBoxLog.AppendText(string.Format("KeyPress - {1}:{0}\n", e.KeyChar, (double)sender));
            textBoxLog.ScrollToCaret();
        } 


        private void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            labelMousePosition.Text = string.Format("x={0:0000}; y={1:0000}", e.X, e.Y);
        }

        private void HookManager_MouseClick(object sender, MouseEventArgs e)
        {
            byte[] message = new byte[16];
            byte[] time = BitConverter.GetBytes((double)sender);
            for (int i = 0; i < 8; i++)
                message[i] = time[i];
            byte[] key = BitConverter.GetBytes('É');
            message[8] = key[0];
            /* pipe code
            fStream.Write(message, 0, 16);
            fStream.Flush();            
            */

            //file code
            CreateFile();
            double stopwatch = (double)sender - laststop;
            laststop = (double)sender;
            string fileoutput = string.Format("{0}\t{1}\n", stopwatch, 'É');
            sw.Write(fileoutput); sw.Flush();
        
            textBoxLog.AppendText(string.Format("MouseClick - {0}\n", e.Button));
            textBoxLog.ScrollToCaret();
        }


        private void HookManager_MouseUp(object sender, MouseEventArgs e)
        {
            textBoxLog.AppendText(string.Format("MouseUp - {0}\n", e.Button));
            textBoxLog.ScrollToCaret();
        }


        private void HookManager_MouseDown(object sender, MouseEventArgs e)
        {
            byte[] message = new byte[16];
            byte[] time = BitConverter.GetBytes((double)sender);
            for (int i = 0; i < 8; i++)
                message[i] = time[i];
            byte[] key = BitConverter.GetBytes('É');
            message[8] = key[0];
            /* pipe code
            fStream.Write(message, 0, 16);
            fStream.Flush();            
            */

            //file code
            CreateFile();
            double stopwatch = (double)sender - laststop;
            laststop = (double)sender;
            string fileoutput = string.Format("{0}\t{1}\n", stopwatch, 'É');
            sw.Write(fileoutput); sw.Flush();
       
            textBoxLog.AppendText(string.Format("MouseDown - {0}\n", e.Button));
            textBoxLog.ScrollToCaret();
        }


        private void HookManager_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBoxLog.AppendText(string.Format("MouseDoubleClick - {0}\n", e.Button));
            textBoxLog.ScrollToCaret();
        }


        private void HookManager_MouseWheel(object sender, MouseEventArgs e)
        {
            labelWheel.Text = string.Format("Wheel={0:000}", e.Delta);
        }

        #endregion

        //##################################################################
        #region Event handlers for game and user name
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            gameName = comboBox1.SelectedItem.ToString();
            changeFlag = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            userName = textBox1.Text;
            changeFlag = true;
            
        }

        #endregion 

        private void label1_Click(object sender, EventArgs e)
        {

        }

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
                while (File.Exists(string.Concat(userName, string.Concat(gameName, Convert.ToString(i), ".txt"))))
                {
                    i++;
                }
                fStream = new FileStream(string.Concat(userName, string.Concat(gameName, Convert.ToString(i), ".txt")), FileMode.Create, FileAccess.Write, FileShare.None);
                sw = new StreamWriter(fStream);
                
            }
        }


    }
}