using System.Diagnostics;
using System.Dynamic;
using System.Numerics;
using System.Reflection;
using Gorge.World;
using Gorge.Core;
using Raylib_cs;
using Gorge.Services;

namespace Gorge.World;

public class Part : WorldObject
{
    public bool Anchored = false;
    public Material Material;
    public Model Model;
    public enum PartType
    {
        Brick,
    }
    public PartType type;
    public Part(PartType type = PartType.Brick, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null)
    {
        Name = "Part";
        this.type = type;

        position ??= new Vector3(1, 1, 1);
        rotation ??= Quaternion.Zero;
        scale ??= new Vector3(4, 1, 2);

        Transform.Position = position.Value;
        Transform.Rotation = rotation.Value;
        Transform.Scale = scale.Value;

        RegenerateModel();
    }

    protected override void OnTransformChanged()
    {
        base.OnTransformChanged();
        RegenerateModel();
    }

    public void RegenerateModel()
    {
        switch (type)
        {
            case PartType.Brick:
                Mesh mesh = Raylib.GenMeshCube(Transform.Scale.X, Transform.Scale.Y, Transform.Scale.Z);
                unsafe
                {
                    for (int i = 0; i < mesh.VertexCount * 2; i += 2)
                    {
                        mesh.TexCoords[i + 0] *= Transform.Scale.X; // U
                        mesh.TexCoords[i + 1] *= Transform.Scale.Z; // V
                    }
                }
                Raylib.UploadMesh(ref mesh, true);
                Model = Raylib.LoadModelFromMesh(mesh);
                unsafe
                {
                    int texCount = mesh.VertexCount * 2;
                    for (int i = 0; i < texCount; i += 2) Log.LogDebug($"UV[{i / 2}]={mesh.TexCoords[i]},{mesh.TexCoords[i + 1]}");
                }
                break;
        }
        byte[] studImg = ServiceManager.GetService<ResourceService>().GetResource("assets.textures.stud.png");
        Texture2D texture = Raylib.LoadTextureFromImage(Raylib.LoadImageFromMemory(".png", studImg));
        Raylib.SetTextureWrap(texture, TextureWrap.Repeat);


        Raylib.SetMaterialTexture(ref Model, 0, MaterialMapIndex.Albedo, ref texture);

    }
}