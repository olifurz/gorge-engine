using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

public static class Log
{
    private static void Construct(string level, string message, ConsoleColor color = ConsoleColor.White, string? callingClass = null)
    {
        string timestamp = DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff");

        if (callingClass == null)
        {
            var stackTrace = new StackTrace();
            var callingMethod = stackTrace.GetFrame(2)?.GetMethod();
            callingClass = callingMethod?.DeclaringType?.Name;
        }

        string entry = $"[{timestamp}] {level}: {callingClass}: {message}";
        Console.ForegroundColor = color;
        Console.WriteLine(entry);
    }

    public static void Debug(string message)
    {
        Construct("DEBUG", message, ConsoleColor.Gray);
    }
    public static void Info(string message)
    {
        Construct("INFO", message, ConsoleColor.White);
    }

    public static WarningException Warning(string message)
    {
        Construct("WARN", message, ConsoleColor.Yellow);
        return new WarningException(message);

    }

    public static Exception Error(string message)
    {
        Construct("ERROR", message, ConsoleColor.Red);
        return new Exception(message);
    }

    public static Exception Critical(string message)
    {
        Construct("CRITICAL", message, ConsoleColor.DarkRed);
        return new Exception(message);
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    public static unsafe void RaylibLogCallback(int logLevel, sbyte* text, sbyte* args)
    {
        string msg = Marshal.PtrToStringAnsi((IntPtr)text) ?? "";
        string more = Marshal.PtrToStringAnsi((IntPtr)args) ?? "";
        ConsoleColor color;
        switch (logLevel)
        {
            case 1: // TRACE
            case 2: // DEBUG
                color = ConsoleColor.Gray;
                break;
            case 3: // INFO
                color = ConsoleColor.White;
                break;
            case 4: // WARNING
                color = ConsoleColor.Yellow;
                break;
            case 5: // ERROR
                color = ConsoleColor.Red;
                break;
            case 6: // FATAL
                color = ConsoleColor.DarkRed;
                break;
            default:
                color = ConsoleColor.White;
                break;
        }
        Construct(RaylibLogLevel(logLevel), msg, color, "Raylib");
    }

    static string RaylibLogLevel(int level) => level switch
    {
        1 => "TRACE",
        2 => "DEBUG",
        3 => "INFO",
        4 => "WARNING",
        5 => "ERROR",
        6 => "FATAL",
        _ => $"LEVEL({level})"
    };

}