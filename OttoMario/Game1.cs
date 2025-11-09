using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OttoMario.Core;
using OttoMario.Entities;
using OttoMario.Levels;
using OttoMario.Systems;
using OttoMario.UI;
using System.Collections.Generic;

namespace OttoMario;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private GameStateManager _stateManager;
    private SpriteFont _font;
    
    // UI Components
    private MainMenu? _mainMenu;
    private HUD? _hud;
    private PauseMenu? _pauseMenu;
    private GameOverScreen? _gameOverScreen;
    
    // Game systems
    private Level? _currentLevel;
    private Camera? _camera;
    private LevelEditor? _levelEditor;
    
    // Textures
    private Dictionary<string, Texture2D> _textures;
    
    private KeyboardState _previousKeyboardState;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _stateManager = new GameStateManager();
        _textures = new Dictionary<string, Texture2D>();
        
        _graphics.PreferredBackBufferWidth = 800;
        _graphics.PreferredBackBufferHeight = 600;
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        // Create a simple font texture
        _font = CreateSimpleFont(GraphicsDevice);
        
        // Generate textures
        _textures["Player"] = TextureGenerator.CreatePlayer(GraphicsDevice);
        _textures["GroundBlock"] = TextureGenerator.CreateGroundTile(GraphicsDevice);
        _textures["BrickBlock"] = TextureGenerator.CreateBrickBlock(GraphicsDevice);
        _textures["QuestionBlock"] = TextureGenerator.CreateQuestionBlock(GraphicsDevice);
        _textures["Goomba"] = TextureGenerator.CreateEnemy(GraphicsDevice);
        _textures["Koopa"] = TextureGenerator.CreateEnemy(GraphicsDevice);
        _textures["Coin"] = TextureGenerator.CreateCoin(GraphicsDevice);
        _textures["Mushroom"] = TextureGenerator.CreateMushroom(GraphicsDevice);
        _textures["Flag"] = TextureGenerator.CreateFlag(GraphicsDevice);
        
        // Initialize UI
        _mainMenu = new MainMenu(GraphicsDevice, _font, OnPlayGame, OnLevelEditor, OnExit);
        _hud = new HUD(_font);
        _pauseMenu = new PauseMenu(GraphicsDevice, _font, OnResume, OnMainMenu);
        
        // Initialize level editor
        _levelEditor = new LevelEditor(GraphicsDevice, _font);
        _levelEditor.SetTileTextures(_textures);
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        
        switch (_stateManager.CurrentState)
        {
            case GameState.MainMenu:
                _mainMenu?.Update();
                break;
                
            case GameState.Playing:
                if (keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
                {
                    _stateManager.ChangeState(GameState.Paused);
                }
                
                _currentLevel?.Update(gameTime);
                
                if (_currentLevel != null && _camera != null)
                {
                    _camera.Follow(_currentLevel.Player.Position);
                    
                    // Check for game over
                    if (!_currentLevel.Player.IsAlive)
                    {
                        _gameOverScreen = new GameOverScreen(GraphicsDevice, _font, OnRestartGame, OnMainMenu, false);
                        _stateManager.ChangeState(GameState.GameOver);
                    }
                    
                    // Check for level complete
                    if (_currentLevel.IsCompleted)
                    {
                        _gameOverScreen = new GameOverScreen(GraphicsDevice, _font, OnRestartGame, OnMainMenu, true);
                        _stateManager.ChangeState(GameState.GameOver);
                    }
                }
                break;
                
            case GameState.Paused:
                if (keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
                {
                    _stateManager.ChangeState(GameState.Playing);
                }
                _pauseMenu?.Update();
                break;
                
            case GameState.GameOver:
                _gameOverScreen?.Update();
                break;
                
            case GameState.LevelEditor:
                if (keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
                {
                    _stateManager.ChangeState(GameState.MainMenu);
                }
                _levelEditor?.Update(gameTime);
                break;
        }
        
        _previousKeyboardState = keyboardState;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(92, 148, 252));

        _spriteBatch.Begin();
        
        switch (_stateManager.CurrentState)
        {
            case GameState.MainMenu:
                _mainMenu?.Draw(_spriteBatch);
                break;
                
            case GameState.Playing:
                DrawGame(_spriteBatch);
                break;
                
            case GameState.Paused:
                DrawGame(_spriteBatch);
                _pauseMenu?.Draw(_spriteBatch);
                break;
                
            case GameState.GameOver:
                DrawGame(_spriteBatch);
                if (_currentLevel != null)
                    _gameOverScreen?.Draw(_spriteBatch, _currentLevel.Player.Score);
                break;
                
            case GameState.LevelEditor:
                _levelEditor?.Draw(_spriteBatch);
                break;
        }
        
        _spriteBatch.End();
        base.Draw(gameTime);
    }
    
    private void DrawGame(SpriteBatch spriteBatch)
    {
        if (_currentLevel == null || _camera == null) return;
        
        spriteBatch.End();
        spriteBatch.Begin(transformMatrix: _camera.GetTransformMatrix());
        
        // Draw entities
        foreach (var entity in _currentLevel.Entities)
        {
            if (!entity.IsActive) continue;
            
            string type = GetEntityType(entity);
            if (_textures.ContainsKey(type))
            {
                entity.Draw(spriteBatch, _textures[type]);
            }
        }
        
        // Draw player
        if (_currentLevel.Player.IsAlive)
        {
            _currentLevel.Player.Draw(spriteBatch, _textures["Player"]);
        }
        
        spriteBatch.End();
        spriteBatch.Begin();
        
        // Draw HUD
        _hud?.Draw(spriteBatch, _currentLevel.Player.Score, _currentLevel.Player.Coins, 
                   _currentLevel.Player.Lives, _currentLevel.TimeRemaining);
    }
    
    private string GetEntityType(Entity entity)
    {
        if (entity is Block block)
        {
            return block.Type.ToString();
        }
        else if (entity is Enemy enemy)
        {
            return enemy.Type.ToString();
        }
        else if (entity is Item item)
        {
            return item.Type.ToString();
        }
        else if (entity.Bounds.Width == 16 && entity.Bounds.Height == 64)
        {
            return "Flag";
        }
        return "GroundBlock";
    }
    
    private void OnPlayGame()
    {
        var levelData = Level.CreateDefaultLevel();
        _currentLevel = new Level(levelData);
        _camera = new Camera(800, 600, levelData.Width * 32, levelData.Height * 32);
        _stateManager.ChangeState(GameState.Playing);
    }
    
    private void OnLevelEditor()
    {
        _stateManager.ChangeState(GameState.LevelEditor);
    }
    
    private void OnExit()
    {
        Exit();
    }
    
    private void OnResume()
    {
        _stateManager.ChangeState(GameState.Playing);
    }
    
    private void OnRestartGame()
    {
        _currentLevel?.Reset();
        _stateManager.ChangeState(GameState.Playing);
    }
    
    private void OnMainMenu()
    {
        _currentLevel = null;
        _camera = null;
        _stateManager.ChangeState(GameState.MainMenu);
    }
    
    private SpriteFont CreateSimpleFont(GraphicsDevice graphicsDevice)
    {
        // Create a simple bitmap font
        Texture2D fontTexture = new Texture2D(graphicsDevice, 256, 256);
        Color[] data = new Color[256 * 256];
        
        for (int i = 0; i < data.Length; i++)
            data[i] = Color.White;
        
        fontTexture.SetData(data);
        
        List<Rectangle> glyphBounds = new List<Rectangle>();
        List<Rectangle> cropping = new List<Rectangle>();
        List<char> characters = new List<char>();
        
        // Add printable ASCII characters
        for (char c = ' '; c <= '~'; c++)
        {
            characters.Add(c);
            glyphBounds.Add(new Rectangle(0, 0, 8, 16));
            cropping.Add(new Rectangle(0, 0, 8, 16));
        }
        
        return new SpriteFont(fontTexture, glyphBounds, cropping, characters, 16, 0, null, null);
    }
}
