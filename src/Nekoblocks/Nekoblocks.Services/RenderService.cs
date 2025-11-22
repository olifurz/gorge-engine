using System.Diagnostics;
using System.Numerics;
using Nekoblocks.Core;
using Nekoblocks.Game.Objects;
using Nekoblocks.Game.Player;
using Raylib_cs;

namespace Nekoblocks.Services;

/// <summary>
/// Service to manage rendering of all objects in the workspace
/// </summary>
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
        Raylib.ClearBackground(Color.Black);

        Raylib.DrawGrid(128, 4);
        Raylib.BeginMode3D(player.Camera);

        if (workspace.Skybox != null)
            RenderSkybox(workspace.Skybox);

        foreach (var item in workspace.Workspace.GetChildren(true))
        {
            switch (item)
            {
                case Part part when part.Transparency < 1:
                    RenderPart(part);
                    break;
                default:
                    break;
            }

        }

        Raylib.EndMode3D();

        Raylib.DrawText($"FPS: {Raylib.GetFPS()} DT: {Math.Round(Raylib.GetFrameTime(), 4)}", 10, 10, 16, Color.Black);
        Raylib.EndDrawing();
    }

    public override void Stop()
    {
        base.Stop();
    }

    /// Rendering Functions ///
    private void RenderPart(Part part)
    {
        switch (part.Type)
        {
            case Part.PartType.Brick:
                Transform.QuaternionToAxisAngle(part.Transform.Rotation, out var axis, out var angle);
                Raylib.DrawModelEx(part.Model, part.Transform.Position, axis, angle, Vector3.One, Color.White);
                break;
        }
    }

    private void RenderSkybox(Skybox skybox)
    {
        Rlgl.DisableBackfaceCulling();
        Rlgl.DisableDepthMask();
        Raylib.DrawModel(skybox.Model, Vector3.Zero, 1.0f, Color.White);
        Rlgl.EnableBackfaceCulling();
        Rlgl.EnableDepthMask();
    }
}