using System.Numerics;
using Gorge.Core;
using Raylib_cs;
using Gorge.Services;
using Jitter2.Dynamics;

namespace Gorge.Game.Objects;

public class Part : GameObject
{
    public Model Model;
    public enum PartType
    {
        Brick,
    }
    public PartType Type;
    public RigidBody? RigidBody;
    public Transform Transform = new();
    public Part(PartType type = PartType.Brick, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null)
    {
        Name = "Part";
        Type = type;

        position ??= new Vector3(0, 0, 0);
        rotation ??= Quaternion.Identity;
        scale ??= new Vector3(4, 1, 2);

        Transform.SetPosition(position.Value);
        Transform.SetRotation(rotation.Value);
        Transform.SetScale(scale.Value);

        Transform.ScaleChanged += t => RegenerateModel();
        RegenerateModel();
    }

    /// <summary>
    /// Regenerate the Part's mesh, for example when the scale has been changed
    /// </summary>
    public void RegenerateModel()
    {
        switch (Type)
        {
            case PartType.Brick:
                Mesh mesh = Raylib.GenMeshCube(1, 1, 1);
                unsafe
                {
                    for (int i = 0; i < mesh.VertexCount * 2; i += 2)
                    {
                        mesh.TexCoords[i + 0] *= Transform.Scale.X * 128; // U
                        mesh.TexCoords[i + 1] *= Transform.Scale.Z * 128; // V
                    }
                }
                // Raylib.UploadMesh(ref mesh, false);
                Model = Raylib.LoadModelFromMesh(mesh);
                unsafe
                {
                    int texCount = mesh.VertexCount * 2;
                    for (int i = 0; i < texCount; i += 2) Log.Debug($"UV[{i / 2}]={mesh.TexCoords[i]},{mesh.TexCoords[i + 1]}");
                }
                break;
        }
        byte[] studImg = ServiceManager.GetService<ResourceService>().GetResource("textures.stud.png");
        Texture2D texture = Raylib.LoadTextureFromImage(Raylib.LoadImageFromMemory(".png", studImg));
        Raylib.SetTextureWrap(texture, TextureWrap.Repeat);


        Raylib.SetMaterialTexture(ref Model, 0, MaterialMapIndex.Albedo, ref texture);
    }
}