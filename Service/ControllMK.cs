using System;
using System.ServiceProcess;
using System.IO;
using System.Threading;
using System.Data.SqlClient;
using System.IO.Ports;
using System.Xml.Serialization;
using Service.Classes;
using System.Linq;

namespace Service
{
    public partial class ControllMK : ServiceBase
    {
        static Logger log = new Logger("C:\\microclimate\\Log.txt");
        static Properties properties;
        static SqlConnection connection;
        static Thread mainThread;
        static bool onStop = false;
        public ControllMK()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            //Создание логгера
            

            XmlSerializer formatter = new XmlSerializer(typeof(Properties));

            // десериализация
            
            try
            {
                FileStream fs = new FileStream("C:\\microclimate\\Properties.xml", FileMode.Open);
                properties = (Properties)formatter.Deserialize(fs);
                log.Write("Полученны настройки службы", "Info");
            }
            catch
            {
                log.Write("Не удалось получить настройки службы", "Fatal");
                return;
            }

            foreach (string str in properties.DisplayLevels)
                log.LevelLogger[str] = true;
            DeviceReader.SetGlobalParams(log, properties.TimeOut, properties.NumberOfRetries, connection);
            connection = new SqlConnection(
                string.Format(@"Data Source={0};Initial Catalog={1};Integrated Security=True",
                properties.SourseToDataBase, properties.NameToDataBase));
            mainThread = new Thread(new ThreadStart(Loop));
            mainThread.Name = "Main";
            mainThread.Start();
            
        }

        static private void Loop()
        {
            while (!onStop)
            {
                foreach (string portName in SerialPort.GetPortNames())
                {
                    if (!DeviceReader.ActivePorts.Contains(portName))
                    {
                        DeviceReader reader = new DeviceReader(portName);
                        Thread thread = new Thread(new ThreadStart(reader.DeviceReaderRun));
                        thread.Start();
                    }
                }
                Thread.Sleep(properties.IntervalInspections * 1000);
            }
        }

        protected override void OnStop()
        {
            onStop = true;
            Thread.Sleep(1000);
        }
    }
}
