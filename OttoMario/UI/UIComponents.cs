using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace OttoMario.UI;

public class UIButton
{
    public Rectangle Bounds { get; set; }
    public string Text { get; set; }
    public Action? OnClick { get; set; }
    public bool IsHovered { get; private set; }
    
    private MouseState previousMouseState;
    
    public UIButton(Rectangle bounds, string text, Action onClick)
    {
        Bounds = bounds;
        Text = text;
        OnClick = onClick;
    }
    
    public void Update()
    {
        MouseState mouseState = Mouse.GetState();
        Point mousePos = new Point(mouseState.X, mouseState.Y);
        
        IsHovered = Bounds.Contains(mousePos);
        
        if (IsHovered && mouseState.LeftButton == ButtonState.Pressed 
            && previousMouseState.LeftButton == ButtonState.Released)
        {
            OnClick?.Invoke();
        }
        
        previousMouseState = mouseState;
    }
    
    public void Draw(SpriteBatch spriteBatch, SpriteFont font, Texture2D buttonTexture)
    {
        Color buttonColor = IsHovered ? Color.LightGray : Color.White;
        spriteBatch.Draw(buttonTexture, Bounds, buttonColor);
        
        Vector2 textSize = font.MeasureString(Text);
        Vector2 textPos = new Vector2(
            Bounds.X + (Bounds.Width - textSize.X) / 2,
            Bounds.Y + (Bounds.Height - textSize.Y) / 2
        );
        
        spriteBatch.DrawString(font, Text, textPos, Color.Black);
    }
}

public class MainMenu
{
    private List<UIButton> buttons;
    private Texture2D buttonTexture;
    private SpriteFont font;
    
    public MainMenu(GraphicsDevice graphicsDevice, SpriteFont font, 
        Action onPlayGame, Action onLevelEditor, Action onExit)
    {
        this.font = font;
        buttonTexture = CreateButtonTexture(graphicsDevice, 300, 60);
        
        int centerX = 800 / 2 - 150;
        int startY = 250;
        int spacing = 80;
        
        buttons = new List<UIButton>
        {
            new UIButton(new Rectangle(centerX, startY, 300, 60), "Play Game", onPlayGame),
            new UIButton(new Rectangle(centerX, startY + spacing, 300, 60), "Level Editor", onLevelEditor),
            new UIButton(new Rectangle(centerX, startY + spacing * 2, 300, 60), "Exit", onExit)
        };
    }
    
    public void Update()
    {
        foreach (var button in buttons)
        {
            button.Update();
        }
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        string title = "OTTO MARIO";
        Vector2 titleSize = font.MeasureString(title);
        Vector2 titlePos = new Vector2(800 / 2 - titleSize.X / 2, 100);
        
        spriteBatch.DrawString(font, title, titlePos, Color.White);
        
        foreach (var button in buttons)
        {
            button.Draw(spriteBatch, font, buttonTexture);
        }
    }
    
    private Texture2D CreateButtonTexture(GraphicsDevice graphicsDevice, int width, int height)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                
                // Border
                if (x < 3 || x >= width - 3 || y < 3 || y >= height - 3)
                    data[index] = Color.DarkGray;
                else
                    data[index] = Color.Gray;
            }
        }
        
        texture.SetData(data);
        return texture;
    }
}

public class HUD
{
    private SpriteFont font;
    
    public HUD(SpriteFont font)
    {
        this.font = font;
    }
    
    public void Draw(SpriteBatch spriteBatch, int score, int coins, int lives, float timeRemaining)
    {
        string scoreText = $"SCORE: {score:D6}";
        string coinsText = $"COINS: {coins:D2}";
        string livesText = $"LIVES: {lives}";
        string timeText = $"TIME: {(int)timeRemaining}";
        
        spriteBatch.DrawString(font, scoreText, new Vector2(10, 10), Color.White);
        spriteBatch.DrawString(font, coinsText, new Vector2(200, 10), Color.White);
        spriteBatch.DrawString(font, livesText, new Vector2(380, 10), Color.White);
        spriteBatch.DrawString(font, timeText, new Vector2(540, 10), Color.White);
    }
}

public class PauseMenu
{
    private List<UIButton> buttons;
    private Texture2D buttonTexture;
    private Texture2D backgroundTexture;
    private SpriteFont font;
    
    public PauseMenu(GraphicsDevice graphicsDevice, SpriteFont font, 
        Action onResume, Action onMainMenu)
    {
        this.font = font;
        buttonTexture = CreateButtonTexture(graphicsDevice, 300, 60);
        backgroundTexture = CreateBackgroundTexture(graphicsDevice, 800, 600);
        
        int centerX = 800 / 2 - 150;
        int startY = 250;
        int spacing = 80;
        
        buttons = new List<UIButton>
        {
            new UIButton(new Rectangle(centerX, startY, 300, 60), "Resume", onResume),
            new UIButton(new Rectangle(centerX, startY + spacing, 300, 60), "Main Menu", onMainMenu)
        };
    }
    
    public void Update()
    {
        foreach (var button in buttons)
        {
            button.Update();
        }
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 800, 600), Color.White * 0.7f);
        
        string title = "PAUSED";
        Vector2 titleSize = font.MeasureString(title);
        Vector2 titlePos = new Vector2(800 / 2 - titleSize.X / 2, 150);
        
        spriteBatch.DrawString(font, title, titlePos, Color.White);
        
        foreach (var button in buttons)
        {
            button.Draw(spriteBatch, font, buttonTexture);
        }
    }
    
    private Texture2D CreateButtonTexture(GraphicsDevice graphicsDevice, int width, int height)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                if (x < 3 || x >= width - 3 || y < 3 || y >= height - 3)
                    data[index] = Color.DarkGray;
                else
                    data[index] = Color.Gray;
            }
        }
        
        texture.SetData(data);
        return texture;
    }
    
    private Texture2D CreateBackgroundTexture(GraphicsDevice graphicsDevice, int width, int height)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = Color.Black;
        }
        
        texture.SetData(data);
        return texture;
    }
}

public class GameOverScreen
{
    private List<UIButton> buttons;
    private Texture2D buttonTexture;
    private SpriteFont font;
    private bool isVictory;
    
    public GameOverScreen(GraphicsDevice graphicsDevice, SpriteFont font, 
        Action onRestart, Action onMainMenu, bool victory = false)
    {
        this.font = font;
        this.isVictory = victory;
        buttonTexture = CreateButtonTexture(graphicsDevice, 300, 60);
        
        int centerX = 800 / 2 - 150;
        int startY = 300;
        int spacing = 80;
        
        buttons = new List<UIButton>
        {
            new UIButton(new Rectangle(centerX, startY, 300, 60), "Restart", onRestart),
            new UIButton(new Rectangle(centerX, startY + spacing, 300, 60), "Main Menu", onMainMenu)
        };
    }
    
    public void Update()
    {
        foreach (var button in buttons)
        {
            button.Update();
        }
    }
    
    public void Draw(SpriteBatch spriteBatch, int finalScore)
    {
        string title = isVictory ? "VICTORY!" : "GAME OVER";
        Vector2 titleSize = font.MeasureString(title);
        Vector2 titlePos = new Vector2(800 / 2 - titleSize.X / 2, 150);
        
        string scoreText = $"Final Score: {finalScore}";
        Vector2 scoreSize = font.MeasureString(scoreText);
        Vector2 scorePos = new Vector2(800 / 2 - scoreSize.X / 2, 220);
        
        spriteBatch.DrawString(font, title, titlePos, isVictory ? Color.Gold : Color.Red);
        spriteBatch.DrawString(font, scoreText, scorePos, Color.White);
        
        foreach (var button in buttons)
        {
            button.Draw(spriteBatch, font, buttonTexture);
        }
    }
    
    private Texture2D CreateButtonTexture(GraphicsDevice graphicsDevice, int width, int height)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                if (x < 3 || x >= width - 3 || y < 3 || y >= height - 3)
                    data[index] = Color.DarkGray;
                else
                    data[index] = Color.Gray;
            }
        }
        
        texture.SetData(data);
        return texture;
    }
}
