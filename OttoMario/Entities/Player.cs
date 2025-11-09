using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace OttoMario.Entities;

public class Player : Entity
{
    public const float MaxSpeed = 200f;
    public const float Acceleration = 800f;
    public const float JumpVelocity = -400f;
    public const float Gravity = 1200f;
    public const float Friction = 600f;
    
    public bool IsOnGround { get; set; }
    public bool IsJumping { get; set; }
    public int Lives { get; set; }
    public int Score { get; set; }
    public int Coins { get; set; }
    public bool IsPoweredUp { get; set; }
    public bool CanFireball { get; set; }
    
    private KeyboardState previousKeyboardState;
    private float coyoteTime;
    private const float CoyoteTimeMax = 0.1f;
    
    public Player(Vector2 position) : base(position, 32, 32)
    {
        Lives = 3;
        Score = 0;
        Coins = 0;
        IsPoweredUp = false;
        CanFireball = false;
    }
    
    public override void Update(GameTime gameTime)
    {
        if (!IsActive || !IsAlive) return;
        
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        KeyboardState keyboardState = Keyboard.GetState();
        
        // Horizontal movement
        if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
        {
            Velocity = new Vector2(Math.Max(Velocity.X - Acceleration * deltaTime, -MaxSpeed), Velocity.Y);
        }
        else if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
        {
            Velocity = new Vector2(Math.Min(Velocity.X + Acceleration * deltaTime, MaxSpeed), Velocity.Y);
        }
        else
        {
            // Apply friction
            if (Velocity.X > 0)
                Velocity = new Vector2(Math.Max(Velocity.X - Friction * deltaTime, 0), Velocity.Y);
            else if (Velocity.X < 0)
                Velocity = new Vector2(Math.Min(Velocity.X + Friction * deltaTime, 0), Velocity.Y);
        }
        
        // Coyote time for jump
        if (IsOnGround)
        {
            coyoteTime = CoyoteTimeMax;
            IsJumping = false;
        }
        else
        {
            coyoteTime -= deltaTime;
        }
        
        // Jumping
        if ((keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W)) 
            && !previousKeyboardState.IsKeyDown(Keys.Space) 
            && !previousKeyboardState.IsKeyDown(Keys.Up)
            && !previousKeyboardState.IsKeyDown(Keys.W)
            && coyoteTime > 0 && !IsJumping)
        {
            Velocity = new Vector2(Velocity.X, JumpVelocity);
            IsJumping = true;
            IsOnGround = false;
        }
        
        // Apply gravity
        if (!IsOnGround)
        {
            Velocity = new Vector2(Velocity.X, Velocity.Y + Gravity * deltaTime);
        }
        
        previousKeyboardState = keyboardState;
        base.Update(gameTime);
    }
    
    public void CollectCoin()
    {
        Coins++;
        Score += 100;
        if (Coins >= 100)
        {
            Coins = 0;
            Lives++;
        }
    }
    
    public void CollectMushroom()
    {
        if (!IsPoweredUp)
        {
            IsPoweredUp = true;
            Bounds = new Rectangle(Bounds.X, Bounds.Y - 16, 32, 48);
        }
        Score += 1000;
    }
    
    public void TakeDamage()
    {
        if (IsPoweredUp)
        {
            IsPoweredUp = false;
            CanFireball = false;
            Bounds = new Rectangle(Bounds.X, Bounds.Y + 16, 32, 32);
        }
        else
        {
            Lives--;
            if (Lives <= 0)
            {
                IsAlive = false;
            }
        }
    }
    
    public void Reset(Vector2 position)
    {
        Position = position;
        Velocity = Vector2.Zero;
        IsOnGround = false;
        IsJumping = false;
        IsPoweredUp = false;
        CanFireball = false;
        UpdateBounds();
    }
}
