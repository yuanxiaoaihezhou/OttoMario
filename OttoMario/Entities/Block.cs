using Microsoft.Xna.Framework;

namespace OttoMario.Entities;

public class Block : Entity
{
    public EntityType Type { get; set; }
    public bool IsBreakable { get; set; }
    public bool HasItem { get; set; }
    public bool IsHit { get; set; }
    public EntityType ItemType { get; set; }
    
    public Block(Vector2 position, EntityType type) : base(position, 32, 32)
    {
        Type = type;
        IsBreakable = type == EntityType.BrickBlock;
        HasItem = type == EntityType.QuestionBlock;
        ItemType = EntityType.Coin;
    }
    
    public Entity? Hit(Player player)
    {
        if (IsHit) return null;
        
        if (Type == EntityType.QuestionBlock && HasItem)
        {
            IsHit = true;
            HasItem = false;
            
            // Return the item
            if (ItemType == EntityType.Coin)
            {
                player.CollectCoin();
                return new Item(new Vector2(Position.X, Position.Y - 32), EntityType.Coin);
            }
            else if (ItemType == EntityType.Mushroom)
            {
                return new Item(new Vector2(Position.X, Position.Y - 32), EntityType.Mushroom);
            }
        }
        else if (Type == EntityType.BrickBlock && player.IsPoweredUp)
        {
            IsAlive = false;
            IsActive = false;
        }
        
        return null;
    }
}

public class Item : Entity
{
    public EntityType Type { get; set; }
    private float lifetime = 0f;
    private const float MaxLifetime = 10f;
    
    public Item(Vector2 position, EntityType type) : base(position, 24, 24)
    {
        Type = type;
        
        if (type == EntityType.Mushroom)
        {
            Velocity = new Vector2(50, 0);
            Bounds = new Rectangle((int)position.X, (int)position.Y, 32, 32);
        }
    }
    
    public override void Update(GameTime gameTime)
    {
        if (!IsActive || !IsAlive) return;
        
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        lifetime += deltaTime;
        
        if (lifetime >= MaxLifetime && Type == EntityType.Coin)
        {
            IsAlive = false;
            IsActive = false;
        }
        
        // Apply gravity for mushroom
        if (Type == EntityType.Mushroom)
        {
            Velocity = new Vector2(Velocity.X, Velocity.Y + 800 * deltaTime);
        }
        
        base.Update(gameTime);
    }
}
