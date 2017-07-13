using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace Service.Classes
{
    //класс для сохранение параметров приложения
    [Serializable]
    public class Properties
    {

        public int TimeOut;
        public int NumberOfRetries;
        public string NameToDataBase;
        public string SourseToDataBase;
        public int IntervalInspections;
        public List<string> DisplayLevels = new List<string>();

    }
}
