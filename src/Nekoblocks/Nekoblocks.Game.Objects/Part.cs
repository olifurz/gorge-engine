using System.Numerics;
using Nekoblocks.Core;
using Raylib_cs;
using Nekoblocks.Services;
using Jitter2.Dynamics;
using System.Reflection;
using System.Diagnostics;

namespace Nekoblocks.Game.Objects;

// TODO: This entire class feels like a mess in formatting.
public class Part : Instance
{
    public enum PartType
    {
        Brick,
    }
    public PartType Type { get; set; }
    public Model Model { get; private set; }
    public float Transparency { get; set; } = 0; // TODO: Only 1 and 0 is supported right now in render service

    // Physics //
    public readonly Transform Transform = new();
    public RigidBody RigidBody;

    private bool canCollide;
    public bool CanCollide
    {
        get => canCollide; set
        {
            canCollide = value;
            UpdateCollider();
        }
    }

    readonly PhysicsService physicsService = ServiceManager.GetService<PhysicsService>();
    public Part(PartType type = PartType.Brick)
    {
        Name = "Part";
        Type = type;
        CanCollide = true;
        SetParent(ServiceManager.GetService<WorkspaceService>().Workspace);

        Transform.SetPosition(new Vector3(0, 0, 0));
        Transform.SetRotation(Quaternion.Identity);
        Transform.SetScale(new Vector3(4, 1, 2));

        physicsService.AddBody(this);

        // TODO: I have no clue if this works correctly or not, physics service may override this.
        Transform.PositionChanged += t => RigidBody.Position = Transform.Position;
        Transform.RotationChanged += t => RigidBody.Orientation = Quaternion.CreateFromYawPitchRoll(Transform.Rotation.X, Transform.Rotation.Y, Transform.Rotation.Z);
        Transform.ScaleChanged += t => RegenerateModel();
        RegenerateModel();
    }

    /// <summary>
    /// Regenerate the Part's mesh, for example when the scale has been changed
    /// </summary>
    private void RegenerateModel()
    {
        var resourceService = ServiceManager.GetService<ResourceService>();
        switch (Type)
        {
            case PartType.Brick:
                Mesh mesh = Raylib.GenMeshCube(Transform.Scale.X, Transform.Scale.Y, Transform.Scale.Z);
                Raylib.UploadMesh(ref mesh, false);
                Model = Raylib.LoadModelFromMesh(mesh);
                break;
        }
        byte[] studImg = resourceService.GetResource("textures.stud.png");
        Texture2D texture = Raylib.LoadTextureFromImage(Raylib.LoadImageFromMemory(".png", studImg));
        Raylib.SetTextureFilter(texture, TextureFilter.Bilinear);
        Model model = Model;
        Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Albedo, ref texture);

        float[] tiling =
        [
            Transform.Scale.X,
            Transform.Scale.Z
        ];

        Shader surfaceShader = resourceService.GetShader(null, "shaders.surface.fs");
        Raylib.SetShaderValue(surfaceShader, Raylib.GetShaderLocation(surfaceShader, "tiling"), tiling, ShaderUniformDataType.Vec2);
        unsafe
        {
            Model.Materials[0].Shader = surfaceShader;
        }

        UpdateCollider();
    }

    /// <summary>
    /// Helper function for CanCollide, checks to see if a collider is necessary
    /// </summary>
    private void UpdateCollider()
    {
        if (canCollide == true)
        {
            physicsService.AddCollider(this);
        }
        else
        {
            physicsService.RemoveCollider(this);
        }
    }

}