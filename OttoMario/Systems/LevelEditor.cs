using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OttoMario.Levels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OttoMario.Systems;

public class LevelEditor
{
    private LevelData currentLevel;
    private SpriteFont font;
    private Texture2D pixelTexture;
    private Vector2 cameraPosition;
    private int selectedTileType = 0;
    private string[] tileTypes = { "GroundBlock", "BrickBlock", "QuestionBlock", "Goomba", "Koopa", "Flag" };
    private string selectedItemType = "Coin";
    private string[] itemTypes = { "Coin", "Mushroom" };
    
    private const int TileSize = 32;
    private const int GridWidth = 200;
    private const int GridHeight = 15;
    
    private MouseState previousMouseState;
    private KeyboardState previousKeyboardState;
    
    private bool isTestMode = false;
    private Level? testLevel;
    
    private Dictionary<string, Texture2D> tileTextures;
    
    public LevelEditor(GraphicsDevice graphicsDevice, SpriteFont font)
    {
        this.font = font;
        pixelTexture = CreatePixelTexture(graphicsDevice);
        currentLevel = new LevelData
        {
            Name = "New Level",
            Width = GridWidth,
            Height = GridHeight,
            PlayerStartPosition = new Vector2(100, 300)
        };
        
        // Add default ground
        for (int x = 0; x < GridWidth; x++)
        {
            currentLevel.Entities.Add(new EntityData("GroundBlock", x * TileSize, (GridHeight - 1) * TileSize));
            currentLevel.Entities.Add(new EntityData("GroundBlock", x * TileSize, (GridHeight - 2) * TileSize));
        }
        
        tileTextures = new Dictionary<string, Texture2D>();
    }
    
    public void SetTileTextures(Dictionary<string, Texture2D> textures)
    {
        tileTextures = textures;
    }
    
    public void Update(GameTime gameTime)
    {
        if (isTestMode && testLevel != null)
        {
            testLevel.Update(gameTime);
            
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape) && !previousKeyboardState.IsKeyDown(Keys.Escape))
            {
                isTestMode = false;
                testLevel = null;
            }
            
            previousKeyboardState = keyboardState;
            return;
        }
        
        MouseState mouseState = Mouse.GetState();
        KeyboardState keyboardState2 = Keyboard.GetState();
        
        // Camera movement
        if (keyboardState2.IsKeyDown(Keys.Left))
            cameraPosition.X -= 300 * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (keyboardState2.IsKeyDown(Keys.Right))
            cameraPosition.X += 300 * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (keyboardState2.IsKeyDown(Keys.Up))
            cameraPosition.Y -= 300 * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (keyboardState2.IsKeyDown(Keys.Down))
            cameraPosition.Y += 300 * (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Clamp camera
        cameraPosition.X = Math.Clamp(cameraPosition.X, 0, GridWidth * TileSize - 800);
        cameraPosition.Y = Math.Clamp(cameraPosition.Y, 0, GridHeight * TileSize - 600);
        
        // Get mouse position in world space
        int worldX = (int)(mouseState.X + cameraPosition.X);
        int worldY = (int)(mouseState.Y + cameraPosition.Y);
        int gridX = worldX / TileSize;
        int gridY = worldY / TileSize;
        
        // Place tile
        if (mouseState.LeftButton == ButtonState.Pressed && gridX >= 0 && gridX < GridWidth && gridY >= 0 && gridY < GridHeight)
        {
            float posX = gridX * TileSize;
            float posY = gridY * TileSize;
            
            // Check if tile already exists at this position
            bool exists = false;
            foreach (var entity in currentLevel.Entities)
            {
                if (Math.Abs(entity.X - posX) < 1 && Math.Abs(entity.Y - posY) < 1)
                {
                    exists = true;
                    break;
                }
            }
            
            if (!exists)
            {
                string itemType = tileTypes[selectedTileType] == "QuestionBlock" ? selectedItemType : "";
                currentLevel.Entities.Add(new EntityData(tileTypes[selectedTileType], posX, posY, itemType));
            }
        }
        
        // Remove tile
        if (mouseState.RightButton == ButtonState.Pressed && gridX >= 0 && gridX < GridWidth && gridY >= 0 && gridY < GridHeight)
        {
            float posX = gridX * TileSize;
            float posY = gridY * TileSize;
            
            currentLevel.Entities.RemoveAll(e => Math.Abs(e.X - posX) < 1 && Math.Abs(e.Y - posY) < 1);
        }
        
        // Change tile type with number keys
        if (keyboardState2.IsKeyDown(Keys.D1) && !previousKeyboardState.IsKeyDown(Keys.D1))
            selectedTileType = 0;
        if (keyboardState2.IsKeyDown(Keys.D2) && !previousKeyboardState.IsKeyDown(Keys.D2))
            selectedTileType = 1;
        if (keyboardState2.IsKeyDown(Keys.D3) && !previousKeyboardState.IsKeyDown(Keys.D3))
            selectedTileType = 2;
        if (keyboardState2.IsKeyDown(Keys.D4) && !previousKeyboardState.IsKeyDown(Keys.D4))
            selectedTileType = 3;
        if (keyboardState2.IsKeyDown(Keys.D5) && !previousKeyboardState.IsKeyDown(Keys.D5))
            selectedTileType = 4;
        if (keyboardState2.IsKeyDown(Keys.D6) && !previousKeyboardState.IsKeyDown(Keys.D6))
            selectedTileType = 5;
        
        // Toggle item type for question blocks
        if (keyboardState2.IsKeyDown(Keys.I) && !previousKeyboardState.IsKeyDown(Keys.I))
        {
            int index = Array.IndexOf(itemTypes, selectedItemType);
            selectedItemType = itemTypes[(index + 1) % itemTypes.Length];
        }
        
        // Save level
        if (keyboardState2.IsKeyDown(Keys.S) && keyboardState2.IsKeyDown(Keys.LeftControl))
        {
            SaveLevel("level.json");
        }
        
        // Load level
        if (keyboardState2.IsKeyDown(Keys.O) && keyboardState2.IsKeyDown(Keys.LeftControl))
        {
            LoadLevel("level.json");
        }
        
        // Test level
        if (keyboardState2.IsKeyDown(Keys.T) && !previousKeyboardState.IsKeyDown(Keys.T))
        {
            isTestMode = true;
            testLevel = new Level(currentLevel);
        }
        
        previousMouseState = mouseState;
        previousKeyboardState = keyboardState2;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (isTestMode && testLevel != null)
        {
            DrawTestMode(spriteBatch);
            return;
        }
        
        // Draw grid
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                int screenX = (int)(x * TileSize - cameraPosition.X);
                int screenY = (int)(y * TileSize - cameraPosition.Y);
                
                if (screenX >= -TileSize && screenX < 800 && screenY >= -TileSize && screenY < 600)
                {
                    Rectangle rect = new Rectangle(screenX, screenY, TileSize, TileSize);
                    spriteBatch.Draw(pixelTexture, rect, Color.Gray * 0.2f);
                    spriteBatch.Draw(pixelTexture, new Rectangle(screenX, screenY, TileSize, 1), Color.Gray * 0.5f);
                    spriteBatch.Draw(pixelTexture, new Rectangle(screenX, screenY, 1, TileSize), Color.Gray * 0.5f);
                }
            }
        }
        
        // Draw entities
        foreach (var entity in currentLevel.Entities)
        {
            int screenX = (int)(entity.X - cameraPosition.X);
            int screenY = (int)(entity.Y - cameraPosition.Y);
            
            if (screenX >= -TileSize && screenX < 800 && screenY >= -TileSize && screenY < 600)
            {
                Rectangle rect = new Rectangle(screenX, screenY, TileSize, TileSize);
                Color color = GetColorForType(entity.Type);
                
                if (tileTextures.ContainsKey(entity.Type))
                {
                    spriteBatch.Draw(tileTextures[entity.Type], rect, Color.White);
                }
                else
                {
                    spriteBatch.Draw(pixelTexture, rect, color);
                }
            }
        }
        
        // Draw UI
        DrawUI(spriteBatch);
    }
    
    private void DrawTestMode(SpriteBatch spriteBatch)
    {
        if (testLevel == null) return;
        
        // Draw background
        spriteBatch.Draw(pixelTexture, new Rectangle(0, 0, 800, 600), testLevel.Data.BackgroundColor);
        
        // Camera follows player
        Vector2 cameraPos = testLevel.Player.Position - new Vector2(400, 300);
        cameraPos.X = Math.Clamp(cameraPos.X, 0, GridWidth * TileSize - 800);
        cameraPos.Y = Math.Clamp(cameraPos.Y, 0, GridHeight * TileSize - 600);
        
        // Draw entities
        foreach (var entity in testLevel.Entities)
        {
            if (!entity.IsActive) continue;
            
            int screenX = (int)(entity.Bounds.X - cameraPos.X);
            int screenY = (int)(entity.Bounds.Y - cameraPos.Y);
            Rectangle screenRect = new Rectangle(screenX, screenY, entity.Bounds.Width, entity.Bounds.Height);
            
            string type = GetEntityType(entity);
            if (tileTextures.ContainsKey(type))
            {
                spriteBatch.Draw(tileTextures[type], screenRect, Color.White);
            }
            else
            {
                spriteBatch.Draw(pixelTexture, screenRect, GetColorForType(type));
            }
        }
        
        // Draw player
        int playerScreenX = (int)(testLevel.Player.Bounds.X - cameraPos.X);
        int playerScreenY = (int)(testLevel.Player.Bounds.Y - cameraPos.Y);
        Rectangle playerRect = new Rectangle(playerScreenX, playerScreenY, testLevel.Player.Bounds.Width, testLevel.Player.Bounds.Height);
        
        if (tileTextures.ContainsKey("Player"))
        {
            spriteBatch.Draw(tileTextures["Player"], playerRect, Color.White);
        }
        else
        {
            spriteBatch.Draw(pixelTexture, playerRect, Color.Red);
        }
        
        // Draw HUD
        spriteBatch.DrawString(font, $"Score: {testLevel.Player.Score}", new Vector2(10, 10), Color.White);
        spriteBatch.DrawString(font, $"Lives: {testLevel.Player.Lives}", new Vector2(10, 30), Color.White);
        spriteBatch.DrawString(font, $"Coins: {testLevel.Player.Coins}", new Vector2(10, 50), Color.White);
        spriteBatch.DrawString(font, "Press ESC to exit test mode", new Vector2(10, 570), Color.White);
    }
    
    private string GetEntityType(Entities.Entity entity)
    {
        if (entity is Entities.Block block)
        {
            return block.Type.ToString();
        }
        else if (entity is Entities.Enemy enemy)
        {
            return enemy.Type.ToString();
        }
        else if (entity is Entities.Item item)
        {
            return item.Type.ToString();
        }
        else if (entity.Bounds.Width == 16 && entity.Bounds.Height == 64)
        {
            return "Flag";
        }
        return "Unknown";
    }
    
    private void DrawUI(SpriteBatch spriteBatch)
    {
        // Draw toolbar background
        spriteBatch.Draw(pixelTexture, new Rectangle(0, 0, 800, 60), Color.Black * 0.8f);
        
        // Draw selected tile type
        string tileTypeText = $"Tile: {tileTypes[selectedTileType]} (1-6)";
        spriteBatch.DrawString(font, tileTypeText, new Vector2(10, 10), Color.White);
        
        if (tileTypes[selectedTileType] == "QuestionBlock")
        {
            string itemText = $"Item: {selectedItemType} (I)";
            spriteBatch.DrawString(font, itemText, new Vector2(10, 30), Color.White);
        }
        
        // Draw controls
        string controls = "LMB: Place | RMB: Remove | Arrows: Move | Ctrl+S: Save | Ctrl+O: Load | T: Test";
        spriteBatch.DrawString(font, controls, new Vector2(300, 20), Color.Yellow);
    }
    
    private Color GetColorForType(string type)
    {
        return type switch
        {
            "GroundBlock" => new Color(139, 90, 43),
            "BrickBlock" => new Color(180, 80, 40),
            "QuestionBlock" => new Color(220, 180, 0),
            "Goomba" => new Color(139, 69, 19),
            "Koopa" => Color.Green,
            "Flag" => Color.White,
            _ => Color.Gray
        };
    }
    
    private void SaveLevel(string filename)
    {
        try
        {
            string json = JsonSerializer.Serialize(currentLevel, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filename, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving level: {ex.Message}");
        }
    }
    
    private void LoadLevel(string filename)
    {
        try
        {
            if (File.Exists(filename))
            {
                string json = File.ReadAllText(filename);
                var loaded = JsonSerializer.Deserialize<LevelData>(json);
                if (loaded != null)
                {
                    currentLevel = loaded;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading level: {ex.Message}");
        }
    }
    
    private Texture2D CreatePixelTexture(GraphicsDevice graphicsDevice)
    {
        Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
    }
    
    public LevelData GetCurrentLevel()
    {
        return currentLevel;
    }
    
    public void LoadLevelData(LevelData data)
    {
        currentLevel = data;
    }
}
