using Nekoblocks.Core;
using Nekoblocks.Services;
using Jitter2.Dynamics.Constraints;
using Raylib_cs;
using System.Numerics;

namespace Nekoblocks.Game.Player;

/// <summary>
/// Local player class
/// </summary>
public class Player : Instance
{
    public Camera3D Camera;
    private WorkspaceService workspaceService = ServiceManager.GetService<WorkspaceService>();
    public Character Character = new();
    public Player()
    {
        Name = "Player";
        SetParent(workspaceService.Workspace);
        Camera = new Camera3D()
        {
            Position = new Vector3(-20, 8, 10),
            Target = new Vector3(0, 4, 0),
            Up = new Vector3(0, 1, 0),
            FovY = 80.0f,
            Projection = CameraProjection.Perspective
        };
        Character.SetParent(this);
    }
    public void Update()
    {
        Raylib.UpdateCamera(ref Camera, CameraMode.Free);
        Character.Update();
    }
}