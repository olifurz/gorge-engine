using System.Numerics;
using Nekoblocks.Core;
using Nekoblocks.Game;
using Nekoblocks.Game.Objects;
using Nekoblocks.Game.Player;

namespace Nekoblocks.Services;

/// <summary>
/// Service that contains & manages all objects inside the loaded game
/// </summary>
public class WorkspaceService : BaseService
{
    private GameService gameService = ServiceManager.GetService<GameService>();
    private Player? localPlayer;
    public Instance Workspace = new();
    public Skybox? Skybox;
    public override void Start()
    {
        base.Start();

        Workspace.Name = "Workspace";
        Workspace.SetParent(gameService.Root);

        var baseplate = new Part(Part.PartType.Brick, Vector3.Zero, Quaternion.Identity, new Vector3(16, 1, 16));
        baseplate.Name = "Baseplate";
        baseplate.Transform.Anchored = true;
        baseplate.SetParent(Workspace);

        var testPart = new Part(Part.PartType.Brick, new Vector3(0, 15, 0));
        testPart.Name = "TestPart";
        testPart.Transform.Anchored = false;
        testPart.SetParent(Workspace);

        localPlayer = new Player();
        localPlayer.SetParent(Workspace);

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

    public override void Stop()
    {
        base.Stop();
    }
}