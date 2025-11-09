using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OttoMario.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OttoMario.Levels;

public class Level
{
    public LevelData Data { get; private set; }
    public Player Player { get; private set; }
    public List<Entity> Entities { get; private set; }
    public List<Block> Blocks { get; private set; }
    public List<Enemy> Enemies { get; private set; }
    public List<Item> Items { get; private set; }
    public float TimeRemaining { get; set; }
    public bool IsCompleted { get; set; }
    
    private const int TileSize = 32;
    
    public Level(LevelData data)
    {
        Data = data;
        Entities = new List<Entity>();
        Blocks = new List<Block>();
        Enemies = new List<Enemy>();
        Items = new List<Item>();
        Player = new Player(data.PlayerStartPosition);
        TimeRemaining = data.TimeLimit;
        IsCompleted = false;
        
        LoadEntities();
    }
    
    private void LoadEntities()
    {
        foreach (var entityData in Data.Entities)
        {
            Vector2 pos = new Vector2(entityData.X, entityData.Y);
            
            switch (entityData.Type)
            {
                case "GroundBlock":
                    var ground = new Block(pos, EntityType.GroundBlock);
                    Blocks.Add(ground);
                    Entities.Add(ground);
                    break;
                case "BrickBlock":
                    var brick = new Block(pos, EntityType.BrickBlock);
                    Blocks.Add(brick);
                    Entities.Add(brick);
                    break;
                case "QuestionBlock":
                    var question = new Block(pos, EntityType.QuestionBlock);
                    if (!string.IsNullOrEmpty(entityData.ItemType))
                    {
                        question.ItemType = Enum.Parse<EntityType>(entityData.ItemType);
                    }
                    Blocks.Add(question);
                    Entities.Add(question);
                    break;
                case "Goomba":
                    var goomba = new Enemy(pos, EntityType.Goomba);
                    Enemies.Add(goomba);
                    Entities.Add(goomba);
                    break;
                case "Koopa":
                    var koopa = new Enemy(pos, EntityType.Koopa);
                    Enemies.Add(koopa);
                    Entities.Add(koopa);
                    break;
                case "Flag":
                    var flag = new Flag(pos);
                    Entities.Add(flag);
                    break;
            }
        }
    }
    
    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        TimeRemaining -= deltaTime;
        
        if (TimeRemaining <= 0)
        {
            Player.IsAlive = false;
            return;
        }
        
        Player.Update(gameTime);
        
        // Update entities
        for (int i = Entities.Count - 1; i >= 0; i--)
        {
            if (!Entities[i].IsAlive)
            {
                Entities.RemoveAt(i);
            }
        }
        
        foreach (var entity in Entities)
        {
            entity.Update(gameTime);
        }
        
        // Physics and collision
        HandlePhysics();
        HandleCollisions();
    }
    
    private void HandlePhysics()
    {
        // Ground collision for player
        Player.IsOnGround = false;
        
        foreach (var block in Blocks)
        {
            if (!block.IsActive) continue;
            
            Rectangle playerBounds = Player.Bounds;
            Rectangle blockBounds = block.Bounds;
            
            if (playerBounds.Intersects(blockBounds))
            {
                // Determine collision direction
                Rectangle intersection = Rectangle.Intersect(playerBounds, blockBounds);
                
                if (intersection.Width < intersection.Height)
                {
                    // Horizontal collision
                    if (playerBounds.Center.X < blockBounds.Center.X)
                    {
                        // Player on left
                        Player.Position = new Vector2(blockBounds.Left - playerBounds.Width, Player.Position.Y);
                        Player.Velocity = new Vector2(0, Player.Velocity.Y);
                    }
                    else
                    {
                        // Player on right
                        Player.Position = new Vector2(blockBounds.Right, Player.Position.Y);
                        Player.Velocity = new Vector2(0, Player.Velocity.Y);
                    }
                }
                else
                {
                    // Vertical collision
                    if (playerBounds.Center.Y < blockBounds.Center.Y)
                    {
                        // Player on top
                        Player.Position = new Vector2(Player.Position.X, blockBounds.Top - playerBounds.Height);
                        Player.Velocity = new Vector2(Player.Velocity.X, 0);
                        Player.IsOnGround = true;
                    }
                    else
                    {
                        // Player on bottom (hit block from below)
                        Player.Position = new Vector2(Player.Position.X, blockBounds.Bottom);
                        Player.Velocity = new Vector2(Player.Velocity.X, 0);
                        
                        var item = block.Hit(Player);
                        if (item != null && item is Item itm)
                        {
                            Items.Add(itm);
                            Entities.Add(itm);
                        }
                    }
                }
                
                Player.UpdateBoundsPublic();
            }
        }
        
        // Enemy physics
        foreach (var enemy in Enemies)
        {
            if (!enemy.IsAlive) continue;
            
            foreach (var block in Blocks)
            {
                if (!block.IsActive) continue;
                
                Rectangle enemyBounds = enemy.Bounds;
                Rectangle blockBounds = block.Bounds;
                
                if (enemyBounds.Intersects(blockBounds))
                {
                    Rectangle intersection = Rectangle.Intersect(enemyBounds, blockBounds);
                    
                    if (intersection.Width < intersection.Height)
                    {
                        // Horizontal collision - reverse direction
                        enemy.ReverseDirection();
                        if (enemyBounds.Center.X < blockBounds.Center.X)
                            enemy.Position = new Vector2(blockBounds.Left - enemyBounds.Width, enemy.Position.Y);
                        else
                            enemy.Position = new Vector2(blockBounds.Right, enemy.Position.Y);
                    }
                    else if (enemyBounds.Center.Y < blockBounds.Center.Y)
                    {
                        // On ground
                        enemy.Position = new Vector2(enemy.Position.X, blockBounds.Top - enemyBounds.Height);
                        enemy.Velocity = new Vector2(enemy.Velocity.X, 0);
                    }
                    
                    enemy.UpdateBoundsPublic();
                }
            }
        }
        
        // Item physics
        foreach (var item in Items)
        {
            if (!item.IsAlive || item.Type != EntityType.Mushroom) continue;
            
            foreach (var block in Blocks)
            {
                if (!block.IsActive) continue;
                
                Rectangle itemBounds = item.Bounds;
                Rectangle blockBounds = block.Bounds;
                
                if (itemBounds.Intersects(blockBounds))
                {
                    Rectangle intersection = Rectangle.Intersect(itemBounds, blockBounds);
                    
                    if (intersection.Width < intersection.Height)
                    {
                        // Horizontal collision - reverse
                        item.Velocity = new Vector2(-item.Velocity.X, item.Velocity.Y);
                    }
                    else if (itemBounds.Center.Y < blockBounds.Center.Y)
                    {
                        item.Position = new Vector2(item.Position.X, blockBounds.Top - itemBounds.Height);
                        item.Velocity = new Vector2(item.Velocity.X, 0);
                    }
                    
                    item.UpdateBoundsPublic();
                }
            }
        }
    }
    
    private void HandleCollisions()
    {
        // Player-Enemy collision
        foreach (var enemy in Enemies)
        {
            if (!enemy.IsAlive) continue;
            
            if (Player.Bounds.Intersects(enemy.Bounds))
            {
                // Check if player is stomping
                if (Player.Velocity.Y > 0 && Player.Bounds.Bottom - 10 < enemy.Bounds.Center.Y)
                {
                    // Stomp enemy
                    enemy.Stomp();
                    Player.Velocity = new Vector2(Player.Velocity.X, Player.JumpVelocity / 2);
                    Player.Score += 200;
                }
                else
                {
                    // Player takes damage
                    Player.TakeDamage();
                }
            }
        }
        
        // Player-Item collision
        for (int i = Items.Count - 1; i >= 0; i--)
        {
            var item = Items[i];
            if (!item.IsAlive) continue;
            
            if (Player.Bounds.Intersects(item.Bounds))
            {
                if (item.Type == EntityType.Coin)
                {
                    Player.CollectCoin();
                }
                else if (item.Type == EntityType.Mushroom)
                {
                    Player.CollectMushroom();
                }
                
                item.IsAlive = false;
                item.IsActive = false;
            }
        }
        
        // Check for goal
        foreach (var entity in Entities)
        {
            if (entity.Bounds.Width == 16 && entity.Bounds.Height == 64) // Flag
            {
                if (Player.Bounds.Intersects(entity.Bounds))
                {
                    IsCompleted = true;
                }
            }
        }
    }
    
    public void Reset()
    {
        Player.Reset(Data.PlayerStartPosition);
        TimeRemaining = Data.TimeLimit;
        IsCompleted = false;
        
        Entities.Clear();
        Blocks.Clear();
        Enemies.Clear();
        Items.Clear();
        
        LoadEntities();
    }
    
    public static LevelData CreateDefaultLevel()
    {
        var level = new LevelData
        {
            Name = "Level 1",
            Width = 200,
            Height = 15,
            PlayerStartPosition = new Vector2(100, 300)
        };
        
        // Add ground
        for (int x = 0; x < 200; x++)
        {
            level.Entities.Add(new EntityData("GroundBlock", x * 32, 13 * 32));
            level.Entities.Add(new EntityData("GroundBlock", x * 32, 14 * 32));
        }
        
        // Add some platforms
        for (int x = 10; x < 14; x++)
        {
            level.Entities.Add(new EntityData("BrickBlock", x * 32, 9 * 32));
        }
        
        // Add question blocks
        level.Entities.Add(new EntityData("QuestionBlock", 15 * 32, 9 * 32, "Mushroom"));
        level.Entities.Add(new EntityData("QuestionBlock", 17 * 32, 9 * 32, "Coin"));
        level.Entities.Add(new EntityData("QuestionBlock", 19 * 32, 9 * 32, "Coin"));
        
        // Add enemies
        level.Entities.Add(new EntityData("Goomba", 20 * 32, 12 * 32));
        level.Entities.Add(new EntityData("Goomba", 30 * 32, 12 * 32));
        level.Entities.Add(new EntityData("Koopa", 40 * 32, 12 * 32));
        
        // Add flag at end
        level.Entities.Add(new EntityData("Flag", 190 * 32, 11 * 32));
        
        return level;
    }
}
