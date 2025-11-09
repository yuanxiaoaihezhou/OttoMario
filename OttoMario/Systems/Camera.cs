using Microsoft.Xna.Framework;
using System;

namespace OttoMario.Systems;

public class Camera
{
    public Vector2 Position { get; set; }
    public float Zoom { get; set; }
    
    private int screenWidth;
    private int screenHeight;
    private int levelWidth;
    private int levelHeight;
    
    public Camera(int screenWidth, int screenHeight, int levelWidth, int levelHeight)
    {
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;
        this.levelWidth = levelWidth;
        this.levelHeight = levelHeight;
        Position = Vector2.Zero;
        Zoom = 1.0f;
    }
    
    public void Follow(Vector2 targetPosition)
    {
        Position = targetPosition - new Vector2(screenWidth / 2, screenHeight / 2);
        
        // Clamp camera to level bounds
        Position = new Vector2(
            Math.Clamp(Position.X, 0, levelWidth - screenWidth),
            Math.Clamp(Position.Y, 0, levelHeight - screenHeight)
        );
    }
    
    public Matrix GetTransformMatrix()
    {
        return Matrix.CreateTranslation(-Position.X, -Position.Y, 0) * 
               Matrix.CreateScale(Zoom);
    }
}
