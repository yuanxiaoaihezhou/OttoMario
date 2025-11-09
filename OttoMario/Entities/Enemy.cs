using Microsoft.Xna.Framework;

namespace OttoMario.Entities;

public class Enemy : Entity
{
    public EntityType Type { get; set; }
    private float moveDirection = -1f;
    private const float MoveSpeed = 50f;
    
    public Enemy(Vector2 position, EntityType type) : base(position, 32, 32)
    {
        Type = type;
        Velocity = new Vector2(MoveSpeed * moveDirection, 0);
    }
    
    public override void Update(GameTime gameTime)
    {
        if (!IsActive || !IsAlive) return;
        
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Apply gravity
        Velocity = new Vector2(Velocity.X, Velocity.Y + 800 * deltaTime);
        
        base.Update(gameTime);
    }
    
    public void ReverseDirection()
    {
        moveDirection *= -1;
        Velocity = new Vector2(MoveSpeed * moveDirection, Velocity.Y);
    }
    
    public void Stomp()
    {
        IsAlive = false;
        IsActive = false;
    }
}
