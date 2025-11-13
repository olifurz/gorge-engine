using System.Diagnostics;
using Gorge.Core;
using Gorge.World;
using Jitter2;
using Jitter2.Collision.Shapes;
using Jitter2.Dynamics;
using Raylib_cs;


namespace Gorge.Services;

public class PhysicsService : BaseService
{
    readonly Jitter2.World world = new();
    readonly WorkspaceService workspace = ServiceManager.GetService<WorkspaceService>();
    readonly Dictionary<RigidBody, int> bodyIdBindings = []; // Binds the rigidbodies with the object IDs

    public override void Start()
    {
        base.Start();

        world.SubstepCount = 4;

    }

    public override void Update()
    {
        base.Update();
        if (workspace.Objects.Count == 0) return;

        world.Step(Raylib.GetFrameTime(), true);

        foreach (var body in world.RigidBodies)
        {
            // Find object from bindings
            bodyIdBindings.TryGetValue(body, out var id);
            WorldObject? obj = workspace.Objects.Find(x => x.Id == id);

            if (obj != null && obj is Part part)
            {
                part.Transform.Position = body.Position;
                part.Transform.Rotation = body.Orientation;
                body.MotionType = part.Anchored ? MotionType.Static : MotionType.Dynamic;
            }
        }
    }

    public void AddBody(Part part)
    {
        RigidBody body = world.CreateRigidBody();
        body.Position = part.Transform.Position;
        body.Orientation = part.Transform.Rotation;
        bodyIdBindings.Add(body, part.Id);

        switch (part.type)
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