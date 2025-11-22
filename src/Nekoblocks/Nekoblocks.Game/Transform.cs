using System.Diagnostics;
using System.Numerics;
using Jitter2.Dynamics;

namespace Nekoblocks.World;

public class Transform
{
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    public Vector3 Scale { get; private set; }
    public bool Anchored = false;

    public event Action<Transform>? PositionChanged;
    public event Action<Transform>? RotationChanged;
    public event Action<Transform>? ScaleChanged;

    /// <summary>
    /// Convert a Quaternion to Axis/Angle rotation
    /// </summary>
    /// <param name="q">Quaternion to convert</param>
    /// <param name="axis">Axis</param>
    /// <param name="angle">Angle in degrees</param>
    public static void QuaternionToAxisAngle(Quaternion q, out Vector3 axis, out float angle)
    {
        var x = q.X;
        var y = q.Y;
        var z = q.Z;
        var w = q.W;

        var len = (float)Math.Sqrt(x * x + y * y + z * z + w * w);
        if (len == 0)
        {
            axis = new Vector3(1, 0, 0);
            angle = 0;
            return;
        }

        x /= len; y /= len; z /= len; w /= len;

        // Keep w >= 0
        if (w < 0.0)
        {
            w = -w; x = -x; y = -y; z = -z;
        }

        w = Math.Clamp(w, -1, 1);
        angle = 2f * (float)Math.Acos(w) * (180f / MathF.PI);
        var s = (float)Math.Sqrt(1 - w * w);

        if (s < 1e-8)
        {
            axis = new Vector3(1, 0, 0);
        }
        else
        {
            axis = new Vector3(x / s, y / s, z / s);
        }
        return;
    }

    /// <summary>
    /// Set all 3 transform values
    /// </summary>
    /// <param name="position">Position</param>
    /// <param name="rotation">Rotation as a Quaternion</param>
    /// <param name="scale">Scale</param>
    public void Set(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        if (Position != position)
        {
            SetPosition(position);
        }
        if (Rotation != rotation)
        {
            SetRotation(rotation);
        }
        if (Scale != scale)
        {
            SetScale(scale);
        }
    }

    /// <summary>
    /// Set the position of the transform
    /// </summary>
    /// <param name="position"></param>
    /// <param name="invokeEvent">A special bool used by the physics engine to disable invoking the PositionChanged event</param>
    /// My hope is that I'm never allowed to write physics code again, cause this is one nasty hack.
    public void SetPosition(Vector3 position, bool invokeEvent = true)
    {
        Position = position;
        if (invokeEvent == true) PositionChanged?.Invoke(this);
    }
    public void SetPosition(float x, float y, float z)
    {
        SetPosition(new Vector3(x, y, z));
    }

    /// <summary>
    /// Set the rotation of the transform
    /// </summary>
    /// <param name="rotation"></param>
    /// <param name="invokeEvent">A special bool used by the physics engine to disable invoking the RotationChanged event</param>
    public void SetRotation(Quaternion rotation, bool invokeEvent = true)
    {
        Rotation = rotation;
        if (invokeEvent == true) RotationChanged?.Invoke(this);
    }
    public void SetScale(Vector3 scale)
    {
        Scale = scale;
        ScaleChanged?.Invoke(this);
    }
    public void SetScale(float x, float y, float z)
    {
        SetScale(new Vector3(x, y, z));
    }
    public void SetScale(float uniformScale)
    {
        SetScale(new Vector3(uniformScale, uniformScale, uniformScale));
    }
}