namespace Phedg1Studios.StartingItemsGUI
{
    public static class Log
    {
        internal static BepInEx.Logging.ManualLogSource _logSource;

        internal static void Init(BepInEx.Logging.ManualLogSource logSource)
        {
#if DEBUG
            _logSource = logSource;
#endif
        }

        internal static void LogDebug(object data)
        {
#if DEBUG
            _logSource.LogDebug(data);
#endif
        }
       
        internal static void LogError(object data)
        {
#if DEBUG
            _logSource.LogError(data);
#endif
        }

        internal static void LogFatal(object data)
        {
#if DEBUG
            _logSource.LogFatal(data);
#endif
        }

        internal static void LogInfo(object data)
        {
#if DEBUG
            _logSource.LogInfo(data);
#endif
        }
        internal static void LogMessage(object data)
        {
#if DEBUG
            _logSource.LogMessage(data);
#endif
        }

        internal static void LogWarning(object data)
        {
#if DEBUG
            _logSource.LogWarning(data);
#endif
        }
    }
}