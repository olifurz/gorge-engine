using System.Diagnostics;
using System.Numerics;
using Gorge.Core;
using Gorge.World;
using Raylib_cs;

namespace Gorge.Services;

public class RenderService : BaseService
{
    WorkspaceService workspace = ServiceManager.GetService<WorkspaceService>();

    Player? player;
    public override void Start()
    {
        base.Start();
        player = workspace.GetLocalPlayer();
    }

    public override void Update()
    {
        base.Update();
        if (player == null) return;

        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.White);

        Raylib.DrawGrid(128, 4);
        Raylib.BeginMode3D(player.Camera);

        foreach (var item in workspace.Objects)
        {
            if (item.GetType() == typeof(Part))
            {
                var part = (Part)item;
                var position = part.Transform.Position;
                var rotation = part.Transform.Rotation;
                var scale = part.Transform.Scale;

                switch (part.type)
                {
                    case Part.PartType.Brick:
                        Transform.QuaternionToAxisAngle(rotation, out var axis, out var angle);
                        Raylib.DrawModelEx(part.Model, part.Transform.Position, axis, angle, part.Transform.Scale / 5, Color.White);
                        break;
                }
            }
        }

        Raylib.EndMode3D();

        Raylib.DrawText(Raylib.GetFPS().ToString(), 10, 10, 20, Color.Black);
        Raylib.EndDrawing();
    }

    public override void Stop()
    {
        base.Stop();
    }
}