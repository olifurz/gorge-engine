using System.Diagnostics;
using System.Numerics;
using Gorge.Core;
using Gorge.Game.Objects;
using Raylib_cs;

namespace Gorge.Services;

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
            switch (item.GetType())
            {
                case var value when value == typeof(Part):
                    RenderPart((Part)item);
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
                Raylib.DrawModelEx(part.Model, part.Transform.Position, axis, angle, part.Transform.Scale, Color.White);
                break;
        }
    }

    private void RenderSkybox(Skybox skybox)
    {
        Rlgl.DisableBackfaceCulling();
        Rlgl.DisableDepthMask();
        Raylib.DrawModel(skybox.model, Vector3.Zero, 1.0f, Color.White);
        Rlgl.EnableBackfaceCulling();
        Rlgl.EnableDepthMask();
    }

    private void RenderSkybox()
    {

    }
}