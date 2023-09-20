using System;
using UniRx;

namespace Game.Levels
{
    public static class CustomDebug
    {
        private static string logData;
        private static int logCount;

        public static ISubject<EDebugLevelResultState> EventActionLevel = new Subject<EDebugLevelResultState>();

        public static void WriteDebug(string log)
        {
            logCount++;
            logData += $"{logCount}: {log}\n";
        }
        
        public static void WriteDebugWarning(string log)
        {
            logCount++;
            logData += $"{logCount}: <color=#E7D515>Warning! {log}</color>\n";
        }
        
        public static void WriteDebugError(string log)
        {
            logCount++;
            logData += $"{logCount}: <color=#E72B15>Error! {log}</color>\n";
        }

        public static string GetLog()
        {
            return logData;
        }

        public static void ActionLevel(EDebugLevelResultState levelResultState)
        {
            EventActionLevel.OnNext(levelResultState);
        }

        public static void WriteDebugWithTime(string log)
        {
            var timestamp = (int) (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
            var newLog = $"{log}, time: {TimeSpan.FromSeconds(timestamp)}";
            WriteDebug(newLog);
        }
    }
}