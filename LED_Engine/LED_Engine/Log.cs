using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LED_Engine
{
    public static class Log
    {
        static StringBuilder LogStr = new StringBuilder();

        #region Log
        static void AppendLog(string value)
        {
            try
            {
                LogStr.Append(value);
            }
            catch
            {
                try
                {
                    LogStr.Clear();
                    LogStr.Append(value);
                }
                catch
                {
                    Console.WriteLine("Append Log Error!!!");
                }
            }
        }

        static void AppendLog(string format, params object[] arg)
        {
            try
            {
                LogStr.AppendFormat(format, arg);
            }
            catch
            {
                try
                {
                    LogStr.Clear();
                    LogStr.AppendFormat(format, arg);
                }
                catch
                {
                    Console.WriteLine("Append Log Error!!!");
                }
            }
        }

        static void AppendLogLine(string value)
        {
            try
            {
                LogStr.AppendLine(value);
            }
            catch
            {
                try
                {
                    LogStr.Clear();
                    LogStr.AppendLine(value);
                }
                catch
                {
                    Console.WriteLine("Append Log Error!!!");
                }
            }
        }

        static void AppendLogLine(string format, params object[] arg)
        {
            try
            {
                LogStr.AppendFormat(format, arg);
                LogStr.AppendLine();
            }
            catch
            {
                try
                {
                    LogStr.Clear();
                    LogStr.AppendFormat(format, arg);
                    LogStr.AppendLine();
                }
                catch
                {
                    Console.WriteLine("Log.AppendLog() Error!!!");
                }
            }
        }

        public static string GetLog()
        {
            return LogStr.ToString();
        }
        #endregion

        #region Write
        public static void Write(string value)
        {
            Console.Write(value);
            AppendLog(value);
        }
        public static void Write(string format, params object[] arg)
        {
            Console.Write(format, arg);
            AppendLog(format, arg);
        }

        public static void Write(ConsoleColor Color, string value)
        {
            Console.ForegroundColor = Color;
            Write(value);
            Console.ResetColor();
        }
        public static void Write(ConsoleColor Color, string format, params object[] arg)
        {
            Console.ForegroundColor = Color;
            Write(format, arg);
            Console.ResetColor();
        }

        public static void WriteRed(string value)
        {
            Write(ConsoleColor.Red, value);
        }
        public static void WriteRed(string format, params object[] arg)
        {
            Write(ConsoleColor.Red, format, arg);
        }

        public static void WriteYellow(string value)
        {
            Write(ConsoleColor.Yellow, value);
        }
        public static void WriteYellow(string format, params object[] arg)
        {
            Write(ConsoleColor.Yellow, format, arg);
        }

        public static void WriteGreen(string value)
        {
            Write(ConsoleColor.Green, value);
        }
        public static void WriteGreen(string format, params object[] arg)
        {
            Write(ConsoleColor.Green, format, arg);
        }
        #endregion

        #region WriteLine
        public static void WriteLine()
        {
            WriteLine(String.Empty);
        }

        public static void WriteLine(string value)
        {
            Console.WriteLine(value);
            AppendLogLine(value);
        }
        public static void WriteLine(string format, params object[] arg)
        {
            Console.WriteLine(format, arg);
            AppendLogLine(format, arg);
        }

        public static void WriteLine(ConsoleColor Color, string value)
        {
            Console.ForegroundColor = Color;
            WriteLine(value);
            Console.ResetColor();
        }
        public static void WriteLine(ConsoleColor Color, string format, params object[] arg)
        {
            Console.ForegroundColor = Color;
            WriteLine(format, arg);
            Console.ResetColor();
        }

        public static void WriteLineRed(string value)
        {
            WriteLine(ConsoleColor.Red, value);
        }
        public static void WriteLineRed(string format, params object[] arg)
        {
            WriteLine(ConsoleColor.Red, format, arg);
        }

        public static void WriteLineYellow(string value)
        {
            WriteLine(ConsoleColor.Yellow, value);
        }
        public static void WriteLineYellow(string format, params object[] arg)
        {
            WriteLine(ConsoleColor.Yellow, format, arg);
        }

        public static void WriteLineGreen(string value)
        {
            WriteLine(ConsoleColor.Green, value);
        }
        public static void WriteLineGreen(string format, params object[] arg)
        {
            WriteLine(ConsoleColor.Green, format, arg);
        }

        // Write String Like "0: Hello", "1: World"...
        public static void WriteWithLineNumbers(string value, bool StartFromOne = true)
        {
            string[] V = value.Split('\n');
            for (int i = 0; i < V.Length; i++)
                WriteLine("{0}: {1}", i + ((StartFromOne) ? 1 : 0), V[i]);
        }
        #endregion
    }
}