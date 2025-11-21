using System.Numerics;
using Nekoblocks.Core;
using Raylib_cs;
using Nekoblocks.Services;
using Jitter2.Dynamics;

namespace Nekoblocks.Game.Objects;

public class Part : Instance
{
    public Model Model;
    public enum PartType
    {
        Brick,
    }
    public PartType Type;
    public RigidBody? RigidBody;
    public Transform Transform = new();
    public float Transparency = 0; // TODO: Only 1 and 0 is supported right now in render service
    public Part(PartType type = PartType.Brick, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null)
    {
        Name = "Part";
        Type = type;
        SetParent(ServiceManager.GetService<WorkspaceService>().Workspace);

        position ??= new Vector3(0, 0, 0);
        rotation ??= Quaternion.Identity;
        scale ??= new Vector3(4, 1, 2);

        Transform.SetPosition(position.Value);
        Transform.SetRotation(rotation.Value);
        Transform.SetScale(scale.Value);

        ServiceManager.GetService<PhysicsService>().AddBody(this);

        Transform.ScaleChanged += t => RegenerateModel();
        RegenerateModel();
    }

    /// <summary>
    /// Regenerate the Part's mesh, for example when the scale has been changed
    /// </summary>
    public void RegenerateModel()
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
        Raylib.SetMaterialTexture(ref Model, 0, MaterialMapIndex.Albedo, ref texture);

        float[] tiling =
        [
            Transform.Scale.X,
            Transform.Scale.Z
        ];

        Shader surfaceShader = resourceService.GetShader(null, "shaders.surfaces.fs");
        Raylib.SetShaderValue(surfaceShader, Raylib.GetShaderLocation(surfaceShader, "tiling"), tiling, ShaderUniformDataType.Vec2);
        unsafe
        {
            Model.Materials[0].Shader = surfaceShader;
        }

        ServiceManager.GetService<PhysicsService>().RegenerateCollider(this);
    }
}