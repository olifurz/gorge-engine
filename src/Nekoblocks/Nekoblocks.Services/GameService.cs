using Nekoblocks.Core;
using Nekoblocks.Game;

namespace Nekoblocks.Services;

/// <summary>
/// Manages the currently loaded game 
/// </summary>
public class GameService : BaseService
{
    // TODO: CS8618 occurs here & I really don't want to have to mark it as nullable.
    // See comment in BaseService.cs for more.
#pragma warning disable CS8618
    public Instance Root { get; private set; }
#pragma warning restore CS8618
    private static int idCount = 0;
    public override void Start()
    {
        base.Start();
        Root = new Instance();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Stop()
    {
        base.Stop();
    }

    /// <summary>
    /// Get a unique ID for use with a Instance
    /// </summary>
    /// <returns></returns>
    public static int GetUniqueId()
    {
        return idCount++;
    }
}