using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;



namespace Service.Classes
{
    public class Logger
    {
        //заглушка
        static object locker = new object();

        //уровни логера
        public Dictionary<string, bool> LevelLogger = new Dictionary<string, bool>
        {
            { "Debug", false }, //только для отладки
            { "Info", false },  //штатные события
            { "Warn", false},  //нештатное событие, не сильно влияющие на работу програмы
            { "Error", false}, //нештатное событие, сильно влияющие на работу програмы
            { "Fatal", false}, //нештатное событие, которое крашит роботу програмы
        };

        //имя файла лога
        string FileName;

        public Logger(string FileName)
        {
            this.FileName = FileName;
        }

        //записать в лог
        public void Write(string Message, string NameLevel)
        {
            lock (locker)
            {
                StreamWriter LoggerStream = new StreamWriter(new FileStream(FileName, FileMode.Append));
                if (LevelLogger[NameLevel])
                    //для дебага можно заменить на LoggerStream на Console
                    LoggerStream.WriteLine("{0}({1}): {2}:{3}:{4} - {5}", NameLevel, Thread.CurrentThread.Name, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, Message);
                LoggerStream.Close();
            }
        }
    }
}
