using System.Diagnostics;
using System.Numerics;
using Gorge.Core;
using Gorge.Game;
using Gorge.Game.Objects;
using Jitter2;
using Jitter2.Collision.Shapes;
using Jitter2.Dynamics;
using Jitter2.Dynamics.Constraints;
using Raylib_cs;


namespace Gorge.Services;

/// <summary>
/// Service to manage all physics-related events in the workspace
/// </summary>
public class PhysicsService : BaseService
{
    public Jitter2.World world = new();
    WorkspaceService workspace = ServiceManager.GetService<WorkspaceService>();
    readonly Dictionary<RigidBody, int> bodyIdBindings = []; // Binds the rigidbodies with the object IDs

    public override void Start()
    {
        base.Start();
        world.SubstepCount = 4;
        world.Gravity = new Vector3(0, -9.81f, 0);
    }

    public override void Update()
    {
        base.Update();
        var workspaceObjects = workspace.Workspace.GetChildren(true);
        if (workspaceObjects.Length == 0) return;

        // TODO: Delta time doesn't seem to be calculated correctly here, when the FPS is uncapped physics is incredibly slow.
        world.Step(Raylib.GetFrameTime(), true);

        foreach (var body in world.RigidBodies)
        {
            // Find object from bindings
            if (!bodyIdBindings.TryGetValue(body, out var id)) continue;
            GameObject? obj = Array.Find(workspaceObjects, x => x.Id == id);

            if (obj != null && obj is Part part)
            {
                part.Transform.SetPosition(body.Position);
                part.Transform.SetRotation(body.Orientation);
                body.MotionType = part.Transform.Anchored ? MotionType.Static : MotionType.Dynamic;

                if (part.RigidBody != null)
                {
                    // Mass = Density x Volume
                    part.RigidBody.SetMassInertia(15 * (part.Transform.Scale.X * part.Transform.Scale.Y * part.Transform.Scale.Z));
                }
            }
        }
    }

    public void AddBody(Part part)
    {
        RigidBody body = world.CreateRigidBody();
        body.Position = part.Transform.Position;
        body.Orientation = part.Transform.Rotation;
        bodyIdBindings.Add(body, part.Id);
        part.RigidBody = body;

        switch (part.Type)
        {
            case Part.PartType.Brick:
                body.AddShape(new BoxShape(part.Transform.Scale.X, part.Transform.Scale.Y, part.Transform.Scale.Z));
                break;
        }
    }
    public override void Stop()
    {
        base.Stop();
    }
}