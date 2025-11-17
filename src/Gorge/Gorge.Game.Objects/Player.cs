using Gorge.Core;
using Gorge.Services;
using Jitter2.Dynamics.Constraints;
using Raylib_cs;
using System.Numerics;

namespace Gorge.Game.Objects;

public class Player : GameObject
{
    // TODO: Create player body parts, try to use constraints if possible.
    public Camera3D Camera;

    private WorkspaceService workspaceService = ServiceManager.GetService<WorkspaceService>();
    private PhysicsService physicsService = ServiceManager.GetService<PhysicsService>();
    public Dictionary<string, Part> BodyParts { get; private set; }

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
        BodyParts = [];
        CreateCharacter();
    }
    public void Update()
    {
        if (BodyParts.Values.Any(part => part == null)) return;

        Raylib.UpdateCamera(ref Camera, CameraMode.Free);

        UpdateCharacter();
    }

    private void UpdateCharacter()
    {

    }

    private void CreateCharacter()
    {
        // BodyParts["Torso"] = CreateBodyPart("Torso", Part.PartType.Brick, new Vector3(2, 2, 1));
        // BodyParts["Torso"].Transform.SetPosition(0, 10, 0);
    }

    private Part CreateBodyPart(string Name, Part.PartType type, Vector3 scale)
    {
        var part = new Part(type);
        part.Name = Name;
        part.Transform.SetScale(scale);
        part.SetParent(this);
        workspaceService.AddObject(part);
        return part;
    }
}