using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OttoMario.Core;

public static class TextureGenerator
{
    public static Texture2D CreateSolidColorTexture(GraphicsDevice graphicsDevice, int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        for (int i = 0; i < data.Length; i++)
            data[i] = color;
        texture.SetData(data);
        return texture;
    }
    
    public static Texture2D CreateBrickBlock(GraphicsDevice graphicsDevice, int size = 32)
    {
        Texture2D texture = new Texture2D(graphicsDevice, size, size);
        Color[] data = new Color[size * size];
        
        Color brickColor = new Color(180, 80, 40);
        Color mortarColor = new Color(140, 60, 30);
        Color highlightColor = new Color(200, 100, 50);
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = y * size + x;
                
                // Mortar lines
                if (y % (size / 2) == 0 || x % (size / 2) == 0)
                {
                    data[index] = mortarColor;
                }
                // Highlight
                else if (x < 2 || y < 2)
                {
                    data[index] = highlightColor;
                }
                else
                {
                    data[index] = brickColor;
                }
            }
        }
        
        texture.SetData(data);
        return texture;
    }
    
    public static Texture2D CreateQuestionBlock(GraphicsDevice graphicsDevice, int size = 32)
    {
        Texture2D texture = new Texture2D(graphicsDevice, size, size);
        Color[] data = new Color[size * size];
        
        Color blockColor = new Color(220, 180, 0);
        Color darkColor = new Color(180, 140, 0);
        Color questionColor = new Color(255, 255, 255);
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = y * size + x;
                
                // Question mark pattern
                bool isQuestion = false;
                int cx = x - size / 2;
                int cy = y - size / 2;
                
                // Simple question mark shape
                if (y > size / 3 && y < size / 2 && Math.Abs(cx) < size / 6)
                    isQuestion = true;
                if (y > size / 2 && y < size * 2 / 3 && cx > -size / 12 && cx < size / 6)
                    isQuestion = true;
                if (y > size * 3 / 4 && Math.Abs(cx) < size / 12)
                    isQuestion = true;
                
                if (isQuestion)
                    data[index] = questionColor;
                else if (x < 3 || y < 3)
                    data[index] = blockColor;
                else
                    data[index] = darkColor;
            }
        }
        
        texture.SetData(data);
        return texture;
    }
    
    public static Texture2D CreatePlayer(GraphicsDevice graphicsDevice, int width = 32, int height = 32)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        
        Color skinColor = new Color(255, 200, 150);
        Color shirtColor = new Color(220, 0, 0);
        Color overallsColor = new Color(0, 0, 200);
        Color hatColor = new Color(220, 0, 0);
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                
                // Hat (top third)
                if (y < height / 3)
                    data[index] = hatColor;
                // Face (middle third)
                else if (y < height * 2 / 3)
                {
                    if (x > width / 4 && x < width * 3 / 4)
                        data[index] = skinColor;
                    else
                        data[index] = Color.Transparent;
                }
                // Body (bottom third)
                else
                {
                    if (x > width / 4 && x < width * 3 / 4)
                    {
                        if (y > height * 3 / 4)
                            data[index] = overallsColor;
                        else
                            data[index] = shirtColor;
                    }
                    else
                        data[index] = Color.Transparent;
                }
            }
        }
        
        texture.SetData(data);
        return texture;
    }
    
    public static Texture2D CreateEnemy(GraphicsDevice graphicsDevice, int size = 32)
    {
        Texture2D texture = new Texture2D(graphicsDevice, size, size);
        Color[] data = new Color[size * size];
        
        Color bodyColor = new Color(139, 69, 19);
        Color faceColor = new Color(180, 90, 30);
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = y * size + x;
                int cx = x - size / 2;
                int cy = y - size / 2;
                
                // Circular body
                if (cx * cx + cy * cy < (size / 2) * (size / 2))
                {
                    // Eyes
                    if (y > size / 3 && y < size / 2)
                    {
                        if ((x > size / 4 && x < size / 3) || (x > size * 2 / 3 && x < size * 3 / 4))
                            data[index] = Color.White;
                        else
                            data[index] = bodyColor;
                    }
                    else if (y < size / 3)
                        data[index] = bodyColor;
                    else
                        data[index] = faceColor;
                }
                else
                    data[index] = Color.Transparent;
            }
        }
        
        texture.SetData(data);
        return texture;
    }
    
    public static Texture2D CreateCoin(GraphicsDevice graphicsDevice, int size = 24)
    {
        Texture2D texture = new Texture2D(graphicsDevice, size, size);
        Color[] data = new Color[size * size];
        
        Color goldColor = new Color(255, 215, 0);
        Color darkGoldColor = new Color(218, 165, 32);
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = y * size + x;
                int cx = x - size / 2;
                int cy = y - size / 2;
                
                if (cx * cx + cy * cy < (size / 2) * (size / 2))
                {
                    if (cx < 0)
                        data[index] = goldColor;
                    else
                        data[index] = darkGoldColor;
                }
                else
                    data[index] = Color.Transparent;
            }
        }
        
        texture.SetData(data);
        return texture;
    }
    
    public static Texture2D CreateMushroom(GraphicsDevice graphicsDevice, int size = 32)
    {
        Texture2D texture = new Texture2D(graphicsDevice, size, size);
        Color[] data = new Color[size * size];
        
        Color capColor = new Color(220, 20, 60);
        Color spotColor = new Color(255, 255, 255);
        Color stemColor = new Color(255, 250, 205);
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = y * size + x;
                int cx = x - size / 2;
                int cy = y - size / 2;
                
                // Cap (top half, circular)
                if (y < size * 2 / 3 && cx * cx + (cy + size / 6) * (cy + size / 6) < (size / 2) * (size / 2))
                {
                    // White spots
                    bool isSpot = false;
                    if ((x - size / 4) * (x - size / 4) + (y - size / 4) * (y - size / 4) < size / 16)
                        isSpot = true;
                    if ((x - size * 3 / 4) * (x - size * 3 / 4) + (y - size / 4) * (y - size / 4) < size / 16)
                        isSpot = true;
                    
                    data[index] = isSpot ? spotColor : capColor;
                }
                // Stem (bottom third)
                else if (y > size * 2 / 3 && Math.Abs(cx) < size / 4)
                {
                    data[index] = stemColor;
                }
                else
                    data[index] = Color.Transparent;
            }
        }
        
        texture.SetData(data);
        return texture;
    }
    
    public static Texture2D CreateFlag(GraphicsDevice graphicsDevice, int width = 16, int height = 64)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        
        Color poleColor = new Color(100, 100, 100);
        Color flagColor = new Color(255, 255, 255);
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                
                // Pole
                if (x < width / 4)
                    data[index] = poleColor;
                // Flag
                else if (y < height / 2)
                    data[index] = flagColor;
                else
                    data[index] = Color.Transparent;
            }
        }
        
        texture.SetData(data);
        return texture;
    }
    
    public static Texture2D CreateGroundTile(GraphicsDevice graphicsDevice, int size = 32)
    {
        Texture2D texture = new Texture2D(graphicsDevice, size, size);
        Color[] data = new Color[size * size];
        
        Color groundColor = new Color(139, 90, 43);
        Color darkColor = new Color(101, 67, 33);
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = y * size + x;
                
                // Checkered pattern
                if ((x / 4 + y / 4) % 2 == 0)
                    data[index] = groundColor;
                else
                    data[index] = darkColor;
            }
        }
        
        texture.SetData(data);
        return texture;
    }
}
