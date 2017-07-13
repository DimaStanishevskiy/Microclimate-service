using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.IO;
using System.Data.SqlClient;
using System.Runtime.Serialization.Json;
using System.Collections.Concurrent;

namespace Service.Classes
{
    //класс для создания потоков чтения порта
    public class DeviceReader
    {
        private string NamePort;

        //общие параметры потоков
        static private Logger Log;
        static private int TimeOut;
        static private int NumberOfRetries;
        static private SqlConnection Connection;

        //список портов которые уже заняты потоками
        static public ConcurrentBag<string> ActivePorts = new ConcurrentBag<string>();

        //задает параметры потоков
        static public void SetGlobalParams(Logger Log, int TimeOut, int NumberOfRetries, SqlConnection Connection)
        {
            DeviceReader.Log = Log;
            DeviceReader.TimeOut = TimeOut;
            DeviceReader.NumberOfRetries = NumberOfRetries;
            DeviceReader.Connection = Connection;
        }

        public DeviceReader(string NamePort)
        {
            this.NamePort = NamePort;
        }

        //основной метод потока
        public void DeviceReaderRun()
        {
            //добавляет имя порта в список
            ActivePorts.Add(NamePort);
            Thread.CurrentThread.Name = NamePort;

            using (SerialPort port = new SerialPort(NamePort))
            {
                //dev - для текущего состояния девайса, old для предыдущего
                Device dev = new Device();
                Device old = new Device();

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Device));
                Log.Write("Чтение порта", "Info");

                int countError = 0;
                port.WriteTimeout = TimeOut;
                while (countError < NumberOfRetries)
                {
                    try
                    {
                        port.Open();
                        string data = port.ReadLine();
                        port.Close();
                        var byteArray = Encoding.ASCII.GetBytes(data);
                        using (MemoryStream memoryStream = new MemoryStream(byteArray))
                        {
                            dev = (Device)serializer.ReadObject(memoryStream);

                        }
                        dev.SaveToDateBase(Connection, Log, old);
                        old = dev;
                        Log.Write("Успешное чтение данных порта", "Info");
                        countError = 0;
                    }
                    catch
                    {
                        Log.Write("Ошибка чтения порта", "Warn");
                        countError++;
                    }
                }
                Log.Write("Закрытие порта", "Info");
                ActivePorts.TryTake(out NamePort);
                
            }
        }
    }
}
