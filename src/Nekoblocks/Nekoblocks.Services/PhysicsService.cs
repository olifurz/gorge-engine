using System.Diagnostics;
using System.Numerics;
using Nekoblocks.Core;
using Nekoblocks.Game;
using Nekoblocks.Game.Objects;
using Nekoblocks.Game.Player;
using Jitter2;
using Jitter2.Collision.Shapes;
using Jitter2.Dynamics;
using Jitter2.Dynamics.Constraints;
using Raylib_cs;


namespace Nekoblocks.Services;

/// <summary>
/// Service to manage all physics-related events in the workspace
/// </summary>
public class PhysicsService : BaseService
{
    public Jitter2.World world = new();
    WorkspaceService workspaceService = ServiceManager.GetService<WorkspaceService>();
    GameService gameService = ServiceManager.GetService<GameService>();
    public override void Start()
    {
        base.Start();
        world.SubstepCount = 4;
        world.Gravity = new Vector3(0, -9.81f, 0);
    }

    public override void Update()
    {
        // TODO: Add Anchor support

        base.Update();
        var objects = workspaceService.Workspace.GetChildren();
        if (objects.Length == 0) return;

        // TODO: Delta time acts strangely here, when the FPS is uncapped physics is incredibly slow.
        // I wanna assume this is an issue with Jitter, unless the calculation is wrong here?
        world.Step(Raylib.GetFrameTime(), true);

        foreach (var obj in objects)
        {
            switch (obj)
            {
                case Part part:
                    if (part.RigidBody == null) break;
                    part.RigidBody.MotionType = part.Transform.Anchored ? MotionType.Static : MotionType.Dynamic;
                    part.Transform.SetPosition(part.RigidBody.Position);
                    part.Transform.SetRotation(part.RigidBody.Orientation);
                    break;
            }
        }
    }

    /// <summary>
    /// Add a rigidbody to a part
    /// </summary>
    /// <param name="part"></param>
    public void AddBody(Part part)
    {
        RigidBody body = world.CreateRigidBody();
        body.Position = part.Transform.Position;
        body.Orientation = part.Transform.Rotation;
        part.RigidBody = body;

        RegenerateCollider(part);
    }

    /// <summary>
    /// Regenerate the collider (e.g if the scale changes)
    /// </summary>
    /// <param name="part"></param>
    public void RegenerateCollider(Part part)
    {
        if (part.RigidBody == null) return;
        for (int i = 0; i < part.RigidBody.Shapes.Count;)
        {
            part.RigidBody.RemoveShape(part.RigidBody.Shapes[i]);
        }

        switch (part.Type)
        {
            case Part.PartType.Brick:
                part.RigidBody.AddShape(new BoxShape(part.Transform.Scale.X, part.Transform.Scale.Y, part.Transform.Scale.Z));
                break;
        }
    }
    public override void Stop()
    {
        base.Stop();
    }
}