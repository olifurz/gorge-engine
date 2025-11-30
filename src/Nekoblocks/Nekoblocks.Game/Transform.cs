using System.Diagnostics;
using System.Numerics;
using System.Text.RegularExpressions;
using Jitter2.Dynamics;

namespace Nekoblocks.World;

public class Transform
{
    public Vector3 Position { get; private set; }
    public Vector3 Rotation { get; private set; }
    public Vector3 Scale { get; private set; }
    public Vector3 Origin { get; private set; }
    public Vector3 WorldOrigin { get; private set; }
    public bool Anchored = false;

    public event Action<Transform>? PositionChanged;
    public event Action<Transform>? RotationChanged;
    public event Action<Transform>? ScaleChanged;

    public Transform()
    {
        PositionChanged += t => CalculateWorldOrigin();
    }

    /// <summary>
    /// Set all 3 transform values
    /// </summary>
    public void Set(Vector3 position, Vector3 rotation, Vector3 scale)
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

    /// <param name="invokeEvent">A special bool used by the physics engine to disable invoking the PositionChanged event</param>
    public void SetPosition(Vector3 position, bool invokeEvent = true)
    {
        Position = position;
        if (invokeEvent == true) PositionChanged?.Invoke(this);
    }
    public void SetPosition(float x, float y, float z)
    {
        SetPosition(new Vector3(x, y, z));
    }

    /// <param name="invokeEvent">A special bool used by the physics engine to disable invoking the RotationChanged event</param>
    public void SetRotation(Vector3 rotation, bool invokeEvent = true)
    {
        Rotation = rotation;

        if (invokeEvent == true) RotationChanged?.Invoke(this);
    }
    public void SetRotation(Quaternion rotation)
    {
        SetRotation(QuaternionToEuler(rotation));
    }
    public void SetRotation(float yaw, float pitch, float roll)
    {
        SetRotation(new Vector3(yaw, pitch, roll));
    }
    public void SetRotation(Vector3 axis, float angle)
    {
        SetRotation(AxisAngleToEuler(axis, angle));
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


    public void SetOrigin(Vector3 localPosition)
    {
        Origin = localPosition;
        CalculateWorldOrigin();
    }
    public void SetOrigin(float x, float y, float z)
    {
        SetOrigin(new Vector3(x, y, z));
    }
    private void CalculateWorldOrigin()
    {
        WorldOrigin = Position + Origin;
    }


    ////////////////////////
    /// Helper Functions ///
    ////////////////////////
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
    public static Vector3 QuaternionToEuler(Quaternion q)
    {
        float yaw = (float)Math.Atan2(2.0f * (q.W * q.Z + q.X * q.Y), 1.0f - 2.0f * (q.Y * q.Y + q.Z * q.Z));
        float pitch = (float)Math.Asin(2.0f * (q.W * q.Y - q.Z * q.X));
        float roll = (float)Math.Atan2(2.0f * (q.W * q.X + q.Y * q.Z), 1.0f - 2.0f * (q.X * q.X + q.Y * q.Y));

        const float radToDeg = 180f / (float)Math.PI;
        return new Vector3(yaw * radToDeg, pitch * radToDeg, roll * radToDeg);
    }

    public static Quaternion EulerToQuaternion(Vector3 euler)
    {
        var yaw = euler.X * MathF.PI / 180;
        var pitch = euler.Y * MathF.PI / 180;
        var roll = euler.Z * MathF.PI / 180;

        return Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
    }

    public static Vector3 AxisAngleToEuler(Vector3 axis, float angle)
    {
        float a = angle * (float)Math.PI / 180f;

        float yaw = (float)Math.Atan2(axis.Y * Math.Sin(a) - axis.X * axis.Z * (1 - Math.Cos(a)), 1 - (axis.Y * axis.Y + axis.Z * axis.Z) * (1 - Math.Cos(a)));
        float pitch = (float)Math.Asin(axis.X * axis.Y * (1 - Math.Cos(a)) + axis.Z * Math.Sin(a));
        float roll = (float)Math.Atan2(axis.X * Math.Sin(a) - axis.Y * axis.Z * (1 - Math.Cos(a)), 1 - (axis.X * axis.X + axis.Z * axis.Z) * (1 - Math.Cos(a)));

        // Return in degrees
        const float radToDeg = 180f / (float)Math.PI;
        return new Vector3(yaw * radToDeg, pitch * radToDeg, roll * radToDeg);
    }

    public static void EulerToAxisAngle(Vector3 euler, out Vector3 axis, out double angle)
    {
        var radX = euler.X * Math.PI / 180;
        var radY = euler.Y * Math.PI / 180;
        var radZ = euler.Z * Math.PI / 180;

        var c1 = Math.Cos(radX / 2);
        var s1 = Math.Sin(radX / 2);
        var c2 = Math.Cos(radY / 2);
        var s2 = Math.Sin(radY / 2);
        var c3 = Math.Cos(radZ / 2);
        var s3 = Math.Sin(radZ / 2);

        var x = (float)(s1 * s2 * c3 + c1 * c2 * s3);
        var y = (float)(s1 * c2 * c3 + c1 * s2 * s3);
        var z = (float)(c1 * s2 * c3 - s1 * c2 * s3);
        var w = (float)(c1 * c2 * c3 - s1 * s2 * s3);

        var norm = Math.Sqrt(x * x + y * y + z * z);
        if (norm < 0.001)
        {
            axis = new Vector3(1, 0, 0);
        }
        else
        {
            axis = new Vector3((float)(x / norm), (float)(y / norm), (float)(z / norm));
        }
        angle = 2 * Math.Acos(w) * (180 / Math.PI);
    }
}