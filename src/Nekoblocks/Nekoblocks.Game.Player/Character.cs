using Nekoblocks.Game.Objects;

namespace Nekoblocks.Game.Player;

public class Character : Part
{
    public Character()
    {
        Name = "Character";
        Transform.SetScale(4, 5, 1);
        Transform.SetPosition(0, 30, 0);
        Transparency = 0;
        CanCollide = true;
        Transform.Anchored = false;
    }

    public void Update()
    {

    }
}