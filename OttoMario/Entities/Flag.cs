using Microsoft.Xna.Framework;

namespace OttoMario.Entities;

public class Flag : Entity
{
    public Flag(Vector2 position) : base(position, 16, 64)
    {
        IsActive = true;
    }
}
