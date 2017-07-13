using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace Service.Classes
{
    [DataContract]
    class Device
    {
        public int IdDevice = 0;
        [DataMember(Name = "Location")]
        public string Location = "";

        public int IdLocation;

        [DataMember(Name = "Sensor")]
        public Sensor[] SensorList = null;


        public void SaveToDateBase(SqlConnection Connection, Logger Log, Device Old)
        {
            if (Location != Old.Location)
            {
                object result = null;
                //получить ID локации, блок кода часто повторяется, нужна оптимизация 
                lock (Connection)
                {
                    try
                    {
                        Connection.Open();
                        Log.Write("Соединение с БД открыто", "Debug");
                        string query = @"
                            SELECT ID_Location
                            FROM Location
                            WHERE Name_Location = @Name_Location";

                        SqlCommand command = new SqlCommand(query, Connection);

                        command.Parameters.Add(new SqlParameter("@Name_Location", Location));
                        result = command.ExecuteScalar();
                    }
                    catch (Exception e)
                    {
                        Connection.Close();
                        Log.Write(string.Format("База недоступна или в процесе работы произошла ошибка: {0}", e.Message), "Error");
                        throw new Exception();
                    }
                    finally
                    {
                        Connection.Close();
                        Log.Write("Соединение с БД закрыто", "Debug");
                    }
                }
                if (result != null)
                {
                    IdLocation = (int)result;
                    Log.Write(string.Format("Полученно ID({0}) локации {1}", IdLocation, Location), "Debug");
                }
                else
                {
                    Log.Write(string.Format("Не удалось получить ID для локации {0}, отменна записи", Location), "Error");
                    throw new Exception();
                }

                //получить ID девайса
                lock (Connection)
                {
                    try
                    {
                        Connection.Open();
                        Log.Write("Соединение с БД открыто", "Debug");

                        string query = @"
                    SELECT ID_Device
                    FROM Device
                    WHERE ID_Location = @ID_Location";

                        SqlCommand command = new SqlCommand(query, Connection);

                        command.Parameters.Add(new SqlParameter("@ID_Location", IdLocation));
                        result = command.ExecuteScalar();
                    }
                    catch (Exception e)
                    {
                        Connection.Close();
                        Log.Write(string.Format("База недоступна или в процесе работы произошла ошибка: {0}", e.Message), "Error");
                        throw new Exception();
                    }
                    finally
                    {
                        Connection.Close();
                        Log.Write("Соединение с БД закрыто", "Debug");
                    }
                }

                if (result != null)
                {
                    IdDevice = (int)result;
                    Log.Write(string.Format("Полученно ID({0}) устройства", IdDevice), "Debug");
                }
                else
                {
                    Log.Write("Не удалось получить ID устройства, отменна записи", "Error");
                    throw new Exception();
                }
                
                //сохранить показатели
                foreach (Sensor e in SensorList)
                    e.SaveToDataBase(IdDevice, Connection, Log);
            }
            else if(SensorList.Length != Old.SensorList.Length)
            {
                Log.Write("Количество значений не совпадает", "Error");
                throw new Exception();
            }
            else
            {
                for (int i = 0; i < SensorList.Length; i++)
                {
                    SensorList[i].SaveToDataBase(IdDevice, Connection, Log, Old.SensorList[i]);
                }
            }
        }
    }

    [DataContract]
    class Sensor
    {
        public int IdSensor = 0;

        [DataMember(Name = "Type")]
        public string Type = "";

        public int IdTypeSensor;

        [DataMember(Name = "Location")]
        public string Location = "";

        public int IdLocation;

        [DataMember(Name = "Value")]
        public SensorValue[] SensorValueList = null;

        public void SaveToDataBase(int IdDevice, SqlConnection Connection, Logger Log)
        {
            object result = null;

            //полученние ID типа сенсора
            lock (Connection)
            {
                try
                {
                    Connection.Open();
                    Log.Write("Соединение с БД открыто", "Debug");

                    string query = @"
                    SELECT ID_Type_Sensor
                    FROM Type_Sensor
                    WHERE Name_Sensor = @Name_Sensor";

                    SqlCommand command = new SqlCommand(query, Connection);

                    command.Parameters.Add(new SqlParameter("@Name_Sensor", Type));
                    result = command.ExecuteScalar();
                }
                catch (Exception e)
                {
                    Connection.Close();
                    Log.Write(string.Format("База недоступна или в процесе работы произошла ошибка: {0}", e.Message), "Error");
                    throw new Exception();
                }
                finally
                {
                    Connection.Close();
                    Log.Write("Соединение с БД закрыто", "Debug");
                }
            }
            if (result != null)
            {
                IdTypeSensor = (int)result;
                Log.Write(string.Format("Полученно ID({0}) сенсора {1}", IdTypeSensor, Type), "Debug");
            }
            else
            {
                Log.Write(string.Format("Не удалось получить ID сенсора {0}, отменна записи", Type), "Error");
                throw new Exception();
            }

            //Получение ID локации
            lock (Connection)
            {
                try
                {
                    Connection.Open();
                    Log.Write("Соединение с БД открыто", "Debug");
                    string query = @"
                    SELECT ID_Location
                    FROM Location
                    WHERE Name_Location = @Name_Location";

                    SqlCommand command = new SqlCommand(query, Connection);

                    command.Parameters.Add(new SqlParameter("@Name_Location", Location));
                    result = command.ExecuteScalar();
                }
                catch (Exception e)
                {
                    Connection.Close();
                    Log.Write(string.Format("База недоступна или в процесе работы произошла ошибка: {0}", e.Message), "Error");
                    throw new Exception();
                }
                finally
                {
                    Connection.Close();
                    Log.Write("Соединение с БД закрыто", "Debug");
                }
            }
            if (result != null)
            {
                IdLocation = (int)result;
                Log.Write(string.Format("Полученно ID({0}) локации {1}", IdLocation, Location), "Debug");
            }
            else
            {
                Log.Write(string.Format("Не удалось получить ID локации {0}, отменна записи", Location), "Error");
                throw new Exception();
            }

            //полученние ID сенсора
            lock (Connection)
            {
                try
                {
                    Connection.Open();
                    Log.Write("Соединение с БД открыто", "Debug");
                    string query = @"
                    SELECT ID_Sensor
                    FROM Sensor
                    WHERE ID_Device = @ID_Device AND ID_Type_Sensor = @ID_Type_Sensor AND ID_Location = @ID_Location";

                    SqlCommand command = new SqlCommand(query, Connection);

                    command.Parameters.Add(new SqlParameter("@ID_Device", IdDevice));
                    command.Parameters.Add(new SqlParameter("@ID_Type_Sensor", IdTypeSensor));
                    command.Parameters.Add(new SqlParameter("@ID_Location", IdLocation));
                    result = command.ExecuteScalar();
                }
                catch (Exception e)
                {
                    Log.Write(string.Format("База недоступна или в процесе работы произошла ошибка: {0}", e.Message), "Error");
                    throw new Exception();
                }
                finally
                {
                    Connection.Close();
                    Log.Write("Соединение с БД закрыто", "Debug");
                }
            }

            if (result != null)
            {
                IdSensor = (int)result;
                Log.Write(string.Format("Полученно ID({0}) сенсора", IdSensor), "Debug");
            }
            else
            {
                Log.Write("Не удалось получить ID сенсора, отменна записи", "Error");
                throw new Exception();
            }

            foreach (SensorValue e in SensorValueList)
                e.SaveToDataBase(IdSensor, Connection, Log);
        }

        public void SaveToDataBase(int IdDevice, SqlConnection Connection, Logger Log, Sensor Old)
        {
            if (Type != Old.Type)
            {
                Log.Write(string.Format("Типы сенсоров не совпадают ({0}, {1})", Old.Type, Type), "Error");
                throw new Exception();
            }
            else if (Location != Old.Location)
            {
                Log.Write(string.Format("Локации не совпадают ({0}, {1})", Old.Location, Location), "Error");
                throw new Exception();
            }
            else if (SensorValueList.Length != Old.SensorValueList.Length)
            {
                Log.Write("Количество значений не совпадает", "Error");
                throw new Exception();
            }
            else
            {
                for (int i = 0; i < SensorValueList.Length; i++)
                {
                    IdSensor = Old.IdSensor;
                    SensorValueList[i].SaveToDataBase(IdSensor, Connection, Log, Old.SensorValueList[i]);
                }
            }
        }
    }

    [DataContract]
    class SensorValue
    {
        [DataMember(Name = "Type")]
        public string Type = "";

        public int IdType;

        [DataMember(Name = "Value")]
        public float Value = int.MaxValue;

        private void SaveValue(int IdSensor, SqlConnection Connection, Logger Log)
        {
            int result;
            try
            {
                Connection.Open();
                Log.Write("Соединение с БД открыто", "Debug");

                string query = @"
                    INSERT INTO Value_Sensor (ID_Sensor, ID_Type_Value, Value)
                    VALUES(@ID_Sensor, @ID_Type_Value, @Value)";

                SqlCommand command = new SqlCommand(query, Connection);

                command.Parameters.Add(new SqlParameter("@ID_Sensor", IdSensor));
                command.Parameters.Add(new SqlParameter("@ID_Type_Value", IdType));
                command.Parameters.Add(new SqlParameter("@Value", Value));

                result = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Connection.Close();
                Log.Write(string.Format("База недоступна или в процесе работы произошла ошибка: {0}", e.Message), "Error");
                throw new Exception();
            }
            finally
            {
                Connection.Close();
                Log.Write("Соединение с БД закрыто", "Debug");
            }

            if (result != 0)
            {
                Log.Write(string.Format("Добавлена запись в БД: {0} = {1}", Type, Value), "Info");
            }
            else
            {
                Log.Write("Не удалось добавить запись в БД", "Error");
                throw new Exception();
            }
        }

        public void SaveToDataBase(int IdSensor, SqlConnection Connection, Logger Log, SensorValue Old)
        {
            if (Old.Type == Type)
            {
                IdType = Old.IdType;
                if (Old.Value != Value)
                    SaveValue(IdSensor, Connection, Log);
            }
            else
            {
                Log.Write(string.Format("Типы данных не совпадают ({0}, {1})", Old.Type, Type), "Error");
                throw new Exception();
            }
        }

        public void SaveToDataBase(int IdSensor, SqlConnection Connection, Logger Log)
        {
            object result = null;
            lock (Connection)
            {
                try
                {
                    Connection.Open();
                    Log.Write("Соединение с БД открыто", "Debug");
                    string query = @"
                    SELECT ID_Type_Value
                    FROM Type_Value
                    WHERE Name_Value = @Name_Type";

                    SqlCommand command = new SqlCommand(query, Connection);

                    command.Parameters.Add(new SqlParameter("@Name_Type", Type));
                    result = command.ExecuteScalar();
                }
                catch (Exception e)
                {
                    Connection.Close();
                    Log.Write(string.Format("База недоступна или в процесе работы произошла ошибка: {0}", e.Message), "Error");
                    throw new Exception();
                }
                finally
                {
                    Connection.Close();
                    Log.Write("Соединение с БД закрыто", "Debug");
                }
            }

            if (result != null)
            {
                IdType = (int)result;
                Log.Write(string.Format("Полученно ID({0}) типа данных {1}", IdType, Type), "Debug");
                SaveValue(IdSensor, Connection, Log);
            }
            else
            {
                Log.Write("Не удалось получить ID типа данных, отменна записи", "Error");
                throw new Exception();

            }
        }
    }
}
