using Raylib_cs;
using Nekoblocks.Core;
using Nekoblocks.Services;
using System.Numerics;

namespace Nekoblocks.Game.Objects;

public class Skybox
{
    public Model Model { get; private set; }
    ResourceService resourceService = ServiceManager.GetService<ResourceService>();

    public Skybox(string filename, bool useHdr)
    {
        Mesh cube = Raylib.GenMeshCube(1, 1, 1);
        Model = Raylib.LoadModelFromMesh(cube);

        Shader shader = resourceService.GetShader("shaders.skybox.vs", "shaders.skybox.fs");

        Raylib.SetShaderValue(
            shader,
            Raylib.GetShaderLocation(shader, "environmentMap"),
            (int)MaterialMapIndex.Cubemap,
            ShaderUniformDataType.Int
        );

        Raylib.SetShaderValue(
            shader,
            Raylib.GetShaderLocation(shader, "doGamma"),
            useHdr ? 1 : 0,
            ShaderUniformDataType.Int
        );

        Raylib.SetShaderValue(
            shader,
            Raylib.GetShaderLocation(shader, "vflipped"),
            useHdr ? 1 : 0,
            ShaderUniformDataType.Int
        );
        Raylib.SetShaderValue(
            shader,
            Raylib.GetShaderLocation(shader, "equirectangularMap"),
            0,
            ShaderUniformDataType.Int
        );

        Model model = Model;
        Raylib.SetMaterialShader(ref model, 0, ref shader);

        Texture2D panorama;

        if (useHdr)
        {
            panorama = resourceService.GetTexture(filename);
            Texture2D cubemap = GenTextureCubemap(shader, panorama, 720, PixelFormat.UncompressedR8G8B8A8);
            Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Cubemap, ref cubemap);
            Raylib.UnloadTexture(panorama);
        }
        else
        {
            Image img = resourceService.GetImage(filename);
            Texture2D cubemap = Raylib.LoadTextureCubemap(img, CubemapLayout.AutoDetect);
            Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Cubemap, ref cubemap);
            Raylib.UnloadImage(img);
        }

        Log.Info($"Initialised ({filename})");
    }

    // Taken from raylib-cs/Examples/Models/SkyboxDemo.cs
    private static unsafe Texture2D GenTextureCubemap(Shader shader, Texture2D panorama, int size, PixelFormat format)
    {
        Texture2D cubemap;

        // Disable backface culling to render inside the cube
        Rlgl.DisableBackfaceCulling();

        // STEP 1: Setup framebuffer
        //------------------------------------------------------------------------------------------
        uint rbo = Rlgl.LoadTextureDepth(size, size, true);
        cubemap.Id = Rlgl.LoadTextureCubemap(null, size, format, 1);

        uint fbo = Rlgl.LoadFramebuffer();
        Rlgl.FramebufferAttach(
            fbo,
            rbo,
            FramebufferAttachType.Depth,
            FramebufferAttachTextureType.Renderbuffer,
            0
        );
        Rlgl.FramebufferAttach(
            fbo,
            cubemap.Id,
            FramebufferAttachType.ColorChannel0,
            FramebufferAttachTextureType.CubemapPositiveX,
            0
        );

        // Check if framebuffer is complete with attachments (valid)
        if (Rlgl.FramebufferComplete(fbo))
        {
            Console.WriteLine($"FBO: [ID {fbo}] Framebuffer object created successfully");
        }
        //------------------------------------------------------------------------------------------

        // STEP 2: Draw to framebuffer
        //------------------------------------------------------------------------------------------
        // NOTE: Shader is used to convert HDR equirectangular environment map to cubemap equivalent (6 faces)
        Rlgl.EnableShader(shader.Id);

        // Define projection matrix and send it to shader
        Matrix4x4 matFboProjection = Raymath.MatrixPerspective(
            90.0f * (Math.PI / 180),
            1.0f,
            Rlgl.CULL_DISTANCE_NEAR,
            Rlgl.CULL_DISTANCE_FAR
        );
        Rlgl.SetUniformMatrix(shader.Locs[(int)ShaderLocationIndex.MatrixProjection], matFboProjection);

        // Define view matrix for every side of the cubemap
        Matrix4x4[] fboViews = new[]
        {
            Raymath.MatrixLookAt(Vector3.Zero, new Vector3(-1.0f,  0.0f,  0.0f), new Vector3( 0.0f, -1.0f,  0.0f)),
            Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 1.0f,  0.0f,  0.0f), new Vector3( 0.0f, -1.0f,  0.0f)),
            Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 0.0f,  1.0f,  0.0f), new Vector3( 0.0f,  0.0f,  1.0f)),
            Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 0.0f, -1.0f,  0.0f), new Vector3( 0.0f,  0.0f, -1.0f)),
            Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 0.0f,  0.0f, -1.0f), new Vector3( 0.0f, -1.0f,  0.0f)),
            Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 0.0f,  0.0f,  1.0f), new Vector3( 0.0f, -1.0f,  0.0f)),
        };

        // Set viewport to current fbo dimensions
        Rlgl.Viewport(0, 0, size, size);

        // Activate and enable texture for drawing to cubemap faces
        Rlgl.ActiveTextureSlot(0);
        Rlgl.EnableTexture(panorama.Id);

        for (int i = 0; i < 6; i++)
        {
            // Set the view matrix for the current cube face
            Rlgl.SetUniformMatrix(shader.Locs[(int)ShaderLocationIndex.MatrixView], fboViews[i]);

            // Select the current cubemap face attachment for the fbo
            // WARNING: This function by default enables->attach->disables fbo!!!
            Rlgl.FramebufferAttach(
                fbo,
                cubemap.Id,
                FramebufferAttachType.ColorChannel0,
                FramebufferAttachTextureType.CubemapPositiveX + i,
                0
            );
            Rlgl.EnableFramebuffer(fbo);

            // Load and draw a cube, it uses the current enabled texture
            Rlgl.ClearScreenBuffers();
            Rlgl.LoadDrawCube();
        }
        //------------------------------------------------------------------------------------------

        // STEP 3: Unload framebuffer and reset state
        //------------------------------------------------------------------------------------------
        Rlgl.DisableShader();
        Rlgl.DisableTexture();
        Rlgl.DisableFramebuffer();

        // Unload framebuffer (and automatically attached depth texture/renderbuffer)
        Rlgl.UnloadFramebuffer(fbo);

        // Reset viewport dimensions to default
        Rlgl.Viewport(0, 0, Rlgl.GetFramebufferWidth(), Rlgl.GetFramebufferHeight());
        Rlgl.EnableBackfaceCulling();
        //------------------------------------------------------------------------------------------

        cubemap.Width = size;
        cubemap.Height = size;
        cubemap.Mipmaps = 1;
        cubemap.Format = format;

        return cubemap;
    }
}