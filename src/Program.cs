using Nekoblocks.Core;
using Raylib_cs;
using System;

namespace Nekoblocks;

internal class Program
{
    [STAThread]
    public static void Main()
    {
        unsafe
        {
            delegate* unmanaged[Cdecl]<int, sbyte*, sbyte*, void> ptr = &Log.RaylibLogCallback;
            Raylib.SetTraceLogCallback(ptr);
        }
        Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint);
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.SetTraceLogLevel(TraceLogLevel.Warning);
        Raylib.InitWindow(800, 600, "Nekoblocks");
        Raylib.SetTargetFPS(60);

        ServiceManager.Initialise();

        while (!Raylib.WindowShouldClose())
        {
            ServiceManager.Update();
        }

        ServiceManager.Deinitialise();
        Raylib.CloseWindow();
    }

}