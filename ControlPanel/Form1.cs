using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ControlPanel
{
    public partial class Form1 : Form
    {
        Service service;
        public Form1()
        {
            InitializeComponent();
            service = new Service(labelStatus, "ControllMK");
            service.CheckStatus();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            service.Add("Service.exe");
            Thread.Sleep(2000);
            service.CheckStatus();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            service.Delete("Service.exe");
            Thread.Sleep(2000);
            service.CheckStatus();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            service.Start();
            Thread.Sleep(2000);
            service.CheckStatus();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            service.Stop();
            Thread.Sleep(2000);
            service.CheckStatus();
        }
    }
}
