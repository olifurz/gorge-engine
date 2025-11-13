using Gorge.Core;
using Raylib_cs;
using System;

namespace GorgeEngine;

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
        Raylib.InitWindow(800, 600, "Gorge Engine");
        Raylib.SetTargetFPS(60);
        Raylib.DisableCursor();

        ServiceManager.Initialise();

        while (!Raylib.WindowShouldClose())
        {
            ServiceManager.Update();
        }

        ServiceManager.Deinitialise();
        Raylib.CloseWindow();
    }

}