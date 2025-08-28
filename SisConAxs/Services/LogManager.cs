using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SisConAxs_DM.Models;

namespace SisConAxs.Services
{
    public static class LogManager
    {
        public enum LogType {
                Error,    
                Warn,
                Log,
                Debug
            }

        public static LogType LogMode = LogType.Log;
        public static object BlockFile = new object();

        public static void Log(string message)
        {
            if ((int)LogType.Log > (int)LogMode) return;
            SaveLog(LogType.Log, message);
        }

        public static void Warn(string message)
        {
            if ((int)LogType.Warn > (int)LogMode) return;
            SaveLog(LogType.Warn, message);
        }

        public static void Error(string message, Exception exception = null)
        {
            if ((int)LogType.Error > (int)LogMode) return;
            if (exception != null) {
                message += ": " + exception.Message;
                if (exception.InnerException != null) {
                    message += "\n\n[InnerException>\n" + exception.InnerException;
                }
                message += "\n\n[StackTrace>\n" + exception.StackTrace;
            }

            SaveLog(LogType.Error, message);
        }

        public static void Debug(string message)
        {
            if ((int)LogType.Debug > (int)LogMode) return;
            SaveLog(LogType.Debug, message);
        }

        private static void SaveLog(LogType type, string message)
        {
            SisConAxsContext db = new SisConAxsContext();
            string logTypeName = "LOG";
            try
            {
                SystemLog logger = db.SystemLog.Create();
                if (type == LogType.Warn) logTypeName = "WARN";
                else if (type == LogType.Error) logTypeName = "ERROR";
                else if (type == LogType.Debug) logTypeName = "DEBUG";
                logger.LogType = logTypeName;
                logger.LogMessage = message;
                logger.LogDate = DateTime.Now;
                db.SystemLog.Add(logger);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                string logPath = System.Configuration.ConfigurationManager.AppSettings["PathAppLog"].ToString();
                string ExceptionMsj = DateTime.Now.ToString() + " : " + ex.ToString();

                lock(BlockFile)
                {
                    // si ya existe
                    if (System.IO.File.Exists(logPath))
                        System.IO.File.AppendAllText(logPath, Environment.NewLine + ExceptionMsj);
                    else
                    {
                        // crear el fichero
                        using (var fileStream = System.IO.File.Create(logPath))
                        {
                            var texto = new System.Text.UTF8Encoding(true).GetBytes(ExceptionMsj);
                            fileStream.Write(texto, 0, texto.Length);
                            fileStream.Flush();
                        }
                    }

                    string logMessage = $"*************************>Error al escribir el log({logTypeName}): {message}";
                    System.IO.File.AppendAllText(logPath, Environment.NewLine + logMessage);
                }
            }
        }
    }
}