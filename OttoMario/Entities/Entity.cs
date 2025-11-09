using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OttoMario.Entities;

public abstract class Entity
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public Rectangle Bounds { get; set; }
    public bool IsActive { get; set; }
    public bool IsAlive { get; set; }
    
    protected Entity(Vector2 position, int width, int height)
    {
        Position = position;
        Bounds = new Rectangle((int)position.X, (int)position.Y, width, height);
        IsActive = true;
        IsAlive = true;
        Velocity = Vector2.Zero;
    }
    
    public virtual void Update(GameTime gameTime)
    {
        if (!IsActive) return;
        
        Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        UpdateBounds();
    }
    
    public virtual void Draw(SpriteBatch spriteBatch, Texture2D texture)
    {
        if (!IsActive || !IsAlive) return;
        spriteBatch.Draw(texture, Bounds, Color.White);
    }
    
    protected void UpdateBounds()
    {
        Bounds = new Rectangle((int)Position.X, (int)Position.Y, Bounds.Width, Bounds.Height);
    }
    
    public void UpdateBoundsPublic()
    {
        UpdateBounds();
    }
    
    public bool CollidesWith(Entity other)
    {
        return Bounds.Intersects(other.Bounds);
    }
    
    public bool CollidesWith(Rectangle rect)
    {
        return Bounds.Intersects(rect);
    }
}

public enum EntityType
{
    Player,
    Goomba,
    Koopa,
    Coin,
    Mushroom,
    FireFlower,
    BrickBlock,
    QuestionBlock,
    GroundBlock,
    Flag
}
