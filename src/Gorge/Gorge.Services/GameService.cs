using Gorge.Core;
using Gorge.Game;

/// <summary>
/// Manages the currently loaded game 
/// </summary>
public class GameService : BaseService
{
    public GameObject Root = new();
    public List<GameObject> Objects = [];
    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Stop()
    {
        base.Stop();
    }
}