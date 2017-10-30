using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace UnityEngine
{
    public static class DebugerExtension
    {
        [Conditional("EnableLog")]
        public static void Log(this object obj, string message)
        {
            if (!Debuger.EnableLog)
                return;
            Debuger.Log(GetLogTag(obj), message);
        }
        public static void LogError(this object obj, string message)
        {
            Debuger.LogError(GetLogTag(obj), message);
        }
        public static void LogWarning(this object obj, string message)
        {
            Debuger.LogWarning(GetLogTag(obj), message);
        }



        ////--------------------------------------------------------------------
        [Conditional("EnableLog")]
        public static void Log(this object obj, string format, params object[] args)
        {
            if (!Debuger.EnableLog)
            {
                return;
            }
            string message = string.Format(format, args);
            Debuger.Log(GetLogTag(obj), message);
        }
        public static void LogError(this object obj, string format, params object[] args)
        {
            string message = string.Format(format, args);
            Debuger.LogError(GetLogTag(obj), message);
        }
        public static void LogWarning(this object obj, string format, params object[] args)
        {
            string message = string.Format(format, args);
            Debuger.LogWarning(GetLogTag(obj), message);
        }


        private static string GetLogTag(object obj)
        {
            FieldInfo field = obj.GetType().GetField("LOG_TAG");
            if (field != null)
            {
                return (string)field.GetValue(obj);
            }
            return obj.GetType().Name;
        }

    }


}


