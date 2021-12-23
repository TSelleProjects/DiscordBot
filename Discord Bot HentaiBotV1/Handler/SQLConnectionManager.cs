using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Discord_Bot_HentaiBotV1.Handler
{
    class SQLConnectionManager
    {
        public SqlConnection connection = null;

        public int searchRes;

        public string jsonString;

        public int currentPage;

        public bool CreateConnection(string connStr)
        {
            try
            {
                connection = new SqlConnection(connStr);
            } catch (Exception ex)
            {
                if (Program.debug) Console.WriteLine(ex.Message);
                else Console.WriteLine("There was an error with the SQLConnection!");
                return false;
            }
            return true;
        }

        public bool Open()
        {
            try
            {
                connection.Open();
            } catch (Exception ex)
            {
                if (Program.debug) Console.WriteLine(ex.Message);
                else Console.WriteLine("There was an error with opening the SQLConnection!");
                return false;
            }
            return true;
        }

        public bool Close()
        {
            try
            {
                if (connection != null)
                {
                    connection.Close();
                }
            } catch (Exception ex)
            {
                if (Program.debug) Console.WriteLine(ex.Message);
                else Console.WriteLine("There was an error with closing the SQLConnection!");
                return false;
            }
            return true;
        }

        public static string SQLEscape(string sValue)
        {
            // SQL Encoding: r, n, x00, x1a, Backslash, einfache und doppelte Hochkommas
            if (sValue == null) return null;
            else Regex.Replace(sValue, "'", "\'");
            return sValue;
        }

        public static string EEscape(string sValue)
        {
            // SQL Encoding: r, n, x00, x1a, Backslash, einfache und doppelte Hochkommas
            if (sValue == null) return null;
            else sValue = sValue.Replace("]]", "]");
            sValue = sValue.Remove(0, 1);
            sValue = sValue.Remove(sValue.Length - 1);
            return sValue;
        }

        public bool SetHanimeSqlCommandExecute(string commandString)
        {
            try
            {
                using var cmd = new SqlCommand(commandString, connection);
                cmd.ExecuteNonQuery();
            } catch (Exception ex)
            {
                if (Program.debug) Console.WriteLine(ex.Message);
                if (Program.debug) Console.WriteLine(ex.StackTrace);
                if (Program.debug) Console.WriteLine(ex.InnerException);
                else Console.WriteLine("There was an error with Command execution!");
                return false;
            }
            return true;
        }

        public bool GetHanimeSqlCommandExecute(string commandString)
        {
            try
            {
                using var cmd = new SqlCommand(commandString, connection);
                var retrn = cmd.ExecuteReader();
                retrn.Read();
                searchRes = retrn.GetInt16(0);
                jsonString = EEscape(retrn.GetString(1));
                currentPage = retrn.GetInt32(2);
                
            } catch (Exception ex)
            {
                if (Program.debug) Console.WriteLine(ex.Message);
                if (Program.debug) Console.WriteLine(ex.StackTrace);
                if (Program.debug) Console.WriteLine(ex.InnerException);
                else Console.WriteLine("There was an error with Command execution!");
                return false;
            }
            return true;
        }
    }
}
