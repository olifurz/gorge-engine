using System.Numerics;
using Jitter2;
using Jitter2.Dynamics.Constraints;
using Jitter2.LinearMath;
using Nekoblocks.Core;
using Nekoblocks.Game.Objects;
using Nekoblocks.Services;
using Raylib_cs;

namespace Nekoblocks.Game.Player;

public class Character : Part
{
    PhysicsService physicsService = ServiceManager.GetService<PhysicsService>();

    float walkCycle = 0;
    public Dictionary<string, Part> BodyParts = new Dictionary<string, Part>
    {
        { "Head", new Part(PartType.Brick)},
        { "Torso", new Part(PartType.Brick)},
        { "LeftArm", new Part(PartType.Brick)},
        { "RightArm", new Part(PartType.Brick)},
        { "LeftLeg", new Part(PartType.Brick)},
        { "RightLeg", new Part(PartType.Brick)},
    };
    public Character()
    {
        Name = "Character";
        Transform.SetScale(4, 5, 1);
        Transform.SetPosition(0, 30, 0);
        Transparency = 1;
        CanCollide = true;
        Transform.Anchored = false; // This doesn't seem to work properly?

        // Keep character upright
        var upright = physicsService.world.CreateConstraint<HingeAngle>(RigidBody, physicsService.world.NullBody);
        upright.Initialize(JVector.UnitY, AngularLimit.Full);

        foreach (var part in BodyParts)
        {
            part.Value.SetParent(this);
            part.Value.Name = part.Key;
            part.Value.Transform.Anchored = true;
            part.Value.CanCollide = false;

            switch (part.Value.Name)
            {
                case "Head":
                    part.Value.Transform.SetScale(1);
                    break;
                case "Torso":
                    part.Value.Transform.SetScale(2, 2, 1);
                    break;
                case "LeftArm":
                case "RightArm":
                case "LeftLeg":
                case "RightLeg":
                    part.Value.Transform.SetScale(1, 2, 1);
                    part.Value.Transform.SetOrigin(0, 1, 0);
                    break;
            }
        }
    }

    public void StepWalkCycle()
    {
        walkCycle++;
        if (walkCycle > 30) walkCycle = 0;

        float angle;

        if (walkCycle <= 15)
        {
            angle = float.Lerp(-20, 20, walkCycle / 15f);
        }
        else
        {
            angle = float.Lerp(20, -20, (walkCycle - 15) / 15f);
        }

        var leftArm = BodyParts["LeftArm"].Transform;
        leftArm.SetRotation(leftArm.Rotation.X, leftArm.Rotation.Y, angle);
        var rightArm = BodyParts["RightArm"].Transform;
        rightArm.SetRotation(rightArm.Rotation.X, rightArm.Rotation.Y, -angle);
        var leftLeg = BodyParts["LeftLeg"].Transform;
        leftLeg.SetRotation(leftLeg.Rotation.X, leftLeg.Rotation.Y, -angle);
        var rightLeg = BodyParts["RightLeg"].Transform;
        rightLeg.SetRotation(rightLeg.Rotation.X, rightLeg.Rotation.Y, angle);
    }

    public void Update()
    {
        UpdateCharacter();
    }

    private void UpdateCharacter()
    {
        BodyParts["Torso"].Transform.SetPosition(Transform.Position.X, Transform.Position.Y + 0.5f, Transform.Position.Z);
        Vector3 origin = BodyParts["Torso"].Transform.Position;
        BodyParts["Head"].Transform.SetPosition(origin.X, origin.Y + 1.5f, origin.Z);
        BodyParts["LeftArm"].Transform.SetPosition(origin.X - 1.5f, origin.Y, origin.Z);
        BodyParts["RightArm"].Transform.SetPosition(origin.X + 1.5f, origin.Y, origin.Z);
        BodyParts["LeftLeg"].Transform.SetPosition(origin.X - 0.5f, origin.Y - 2f, origin.Z);
        BodyParts["RightLeg"].Transform.SetPosition(origin.X + 0.5f, origin.Y - 2f, origin.Z);
    }
}