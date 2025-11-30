using Nekoblocks.Core;
using Nekoblocks.Services;
using Jitter2.Dynamics.Constraints;
using Raylib_cs;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;

namespace Nekoblocks.Game.Player;

/// <summary>
/// Local player class
/// </summary>
public class Player : Instance
{
    private WorkspaceService workspaceService = ServiceManager.GetService<WorkspaceService>();
    public Character Character = new();

    // Camera
    // See https://en.wikipedia.org/wiki/Spherical_coordinate_system for more information on how position is calculated
    public Camera3D Camera;
    float camDistance = 3f;
    float theta = 4.7f; // Temporary, just makes it start facing behind the player (probably)
    float phi = 1f;
    float camSensitivity = 0.2f;

    bool isGrounded = false;
    public Player()
    {
        Name = "Player";
        SetParent(workspaceService.Workspace);
        Camera = new Camera3D()
        {
            Position = Vector3.One,
            Target = Vector3.Zero,
            Up = new Vector3(0, 1, 0),
            FovY = 80.0f,
            Projection = CameraProjection.Perspective
        };
        Character.SetParent(this);
    }
    public void Update()
    {
        Raylib.UpdateCamera(ref Camera, CameraMode.Custom);

        Character.Update();
        UpdateCamera();
        UpdateMovement();
    }

    private void UpdateCamera()
    {
        var headPos = Character.BodyParts["Head"].Transform.Position;
        var target = new Vector3(headPos.X, headPos.Y - 0.5f, headPos.Z);

        if (Raylib.IsMouseButtonDown(MouseButton.Right))
        {
            var oldMousePos = Raylib.GetMousePosition();
            Vector2 mouseDelta = Raylib.GetMouseDelta();

            theta += Deg2Rad(mouseDelta.X * camSensitivity);
            phi -= Deg2Rad(mouseDelta.Y * camSensitivity);
            phi = Math.Clamp(phi, 0.01f, (float)Math.PI);
            theta = (theta + 2 * (float)Math.PI) % (2 * (float)Math.PI);

            Raylib.SetMousePosition((int)oldMousePos.X, (int)oldMousePos.Y); // TODO: this doesn't seem to lock the mouse? Unsure if this is linux-specific or not
        }
        if (Raylib.IsKeyDown(KeyboardKey.Left))
        {
            theta -= 0.025f;
        }
        if (Raylib.IsKeyDown(KeyboardKey.Right))
        {
            theta += 0.025f;
        }


        camDistance -= Raylib.GetMouseWheelMove() * 3;
        camDistance = Math.Clamp(camDistance, 2, 50);

        float x = target.X + (float)(camDistance * Math.Sin(phi) * Math.Cos(theta));
        float y = target.Y + (float)(camDistance * Math.Cos(phi));
        float z = target.Z + (float)(camDistance * Math.Sin(phi) * Math.Sin(theta)); // Remember Y is up

        Camera.Position = new Vector3(x, y, z);
        Camera.Target = target;
    }

    private void UpdateMovement()
    {
        var body = Character.RigidBody;
        isGrounded = body.Island != null;

        var transform = Character.Transform;
        var rotation = Character.Transform.Rotation;

        if (Raylib.IsKeyDown(KeyboardKey.Space))
        {
            body.AddForce(new Vector3(0, 20000, 0), true);
        }
        if (Raylib.IsKeyDown(KeyboardKey.W))
        {
            float targetRot = Rad2Deg(theta) / 2;
            transform.SetRotation(targetRot, 0, 0);
            body.AddForce(new Vector3(0, 0, 50), true);
            Character.StepWalkCycle();
        }
    }

    private static float Deg2Rad(float degrees)
    {
        return degrees * (float)(Math.PI / 180);
    }

    private static float Rad2Deg(float radians)
    {
        return radians * 180 / (float)Math.PI;
    }
}