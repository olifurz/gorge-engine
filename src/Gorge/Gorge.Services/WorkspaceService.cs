using System.Numerics;
using System.Threading.Tasks;
using Gorge.Core;
using Gorge.Game;
using Gorge.Game.Objects;
using Microsoft.VisualBasic;

namespace Gorge.Services;

/// <summary>
/// Service that contains & manages all objects inside the loaded game
/// </summary>
public class WorkspaceService : BaseService
{
    private GameService game = ServiceManager.GetService<GameService>();
    private PhysicsService? physics;
    private Player? localPlayer;
    public GameObject Workspace = new();
    public Skybox? Skybox;
    public override void Start()
    {
        base.Start();
        physics = ServiceManager.GetService<PhysicsService>();

        Workspace.Name = "Workspace";
        Workspace.SetParent(game.Root);
        AddObject(Workspace);

        var baseplate = new Part(Part.PartType.Brick, Vector3.Zero, Quaternion.Identity, new Vector3(16, 1, 16));
        baseplate.Name = "Baseplate";
        baseplate.Transform.Anchored = true;
        baseplate.SetParent(Workspace);
        AddObject(baseplate);

        var testPart = new Part(Part.PartType.Brick, new Vector3(0, 15, 0));
        testPart.Name = "TestPart";
        testPart.Transform.Anchored = false;
        testPart.SetParent(Workspace);
        AddObject(testPart);

        localPlayer = new Player();
        localPlayer.SetParent(Workspace);
        AddObject(localPlayer);

        Skybox = new Skybox("textures.skybox.png", false);
    }

    public override void Update()
    {
        base.Update();

        localPlayer?.Update();
    }

    /// <summary>
    /// Return the currently loaded local player
    /// </summary>
    /// <returns>Local Player object</returns>
    public Player? GetLocalPlayer()
    {
        return localPlayer;
    }

    /// <summary>
    /// Appends a GameObject to the workspace
    /// </summary>
    /// <param name="obj">The GameObject to add to the workspace</param>
    /// 
    /// <returns>Unique ID to the object</returns>
    public int AddObject(GameObject obj)
    {

        obj.Id = Workspace.GetChildren().Length + 1;

        if (obj.GetType() == typeof(Part))
            physics?.AddBody((Part)obj);

        return obj.Id;
    }

    public GameObject? GetObject(GameObject obj)
    {
        return Array.Find(Workspace.GetChildren(), x => x == obj);
    }

    public GameObject? GetObject(int id)
    {
        return Array.Find(Workspace.GetChildren(), x => x.Id == id);
    }


    public override void Stop()
    {
        base.Stop();
    }
}