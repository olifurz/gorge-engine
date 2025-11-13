using System.Numerics;
using Gorge.Core;
using Gorge.World;

namespace Gorge.Services;

/// <summary>
/// Contains & manages all objects inside the game
/// </summary>
public class WorkspaceService : BaseService
{
    public List<WorldObject> Objects = [];

    Player? localPlayer;

    private PhysicsService? physics;
    public override void Start()
    {
        base.Start();
        physics = ServiceManager.GetService<PhysicsService>();

        var baseplate = new Part(Part.PartType.Brick, Vector3.Zero, Quaternion.Zero, new Vector3(16, 1, 16));
        baseplate.Name = "Baseplate";
        baseplate.Anchored = true;
        AddObject(baseplate);

        localPlayer = new Player();
        localPlayer.Start();
        AddObject(localPlayer);
    }

    public override void Update()
    {
        base.Update();

        localPlayer?.Update();
    }

    public Player? GetLocalPlayer()
    {
        return localPlayer;
    }

    /// <summary>
    /// Appends an object instance to the object collection
    /// </summary>
    /// <param name="obj">The object to add to the list</param>
    /// <returns>Unique ID to the object</returns>
    public int AddObject(WorldObject obj)
    {

        Objects.Add(obj);
        obj.Id = Objects.Count + 1;

        Log.LogDebug("Appended object " + obj.Name);
        if (obj.GetType() == typeof(Part))
            physics.AddBody((Part)obj);

        return obj.Id;
    }

    public override void Stop()
    {
        base.Stop();
    }
}