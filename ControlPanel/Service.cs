using System;
using System.ServiceProcess;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace ControlPanel
{
    class Service : ServiceController
    {
        private Label LabelStatus;

        public Service(Label LabelStatus, string name) : base(name)
        {
            this.LabelStatus = LabelStatus;
        }
        private void AddOrDelete(string nameFile, bool add)
        {
            string pathToUtilit = "";
            string Arh32 = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe";
            string Arh64 = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe";
            if (File.Exists(Arh32)) pathToUtilit = Arh32;
            else if (File.Exists(Arh64)) pathToUtilit = Arh64;
            string pathToThisProgram = Directory.GetCurrentDirectory() + @"\" + nameFile;

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd";
            if (add)
                psi.Arguments = string.Format(@"/c {0} {1}", pathToUtilit, pathToThisProgram);
            else
                psi.Arguments = string.Format(@"/c {0} {1} /u", pathToUtilit, pathToThisProgram);
            //Start();
            Process.Start(psi);
        }

        public void Add(string nameFile)
        {
            AddOrDelete(nameFile, true);
        }

        public void Delete(string nameFile)
        {
            AddOrDelete(nameFile, false);
        }
        public void CheckStatus()
        {
            try
            {
                LabelStatus.Text = Status.ToString();
            }
            catch
            {
                LabelStatus.Text = "Error";
            }
        }
    }
}
