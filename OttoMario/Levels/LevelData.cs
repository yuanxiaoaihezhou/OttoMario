using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OttoMario.Levels;

public class LevelData
{
    public string Name { get; set; } = "Untitled";
    public int Width { get; set; } = 200; // In tiles
    public int Height { get; set; } = 15; // In tiles
    public Vector2 PlayerStartPosition { get; set; } = new Vector2(100, 400);
    public List<EntityData> Entities { get; set; } = new();
    public Color BackgroundColor { get; set; } = new Color(92, 148, 252);
    public int TimeLimit { get; set; } = 300; // seconds
}

public class EntityData
{
    public string Type { get; set; } = "";
    public float X { get; set; }
    public float Y { get; set; }
    public string ItemType { get; set; } = ""; // For question blocks
    
    [JsonConstructor]
    public EntityData() { }
    
    public EntityData(string type, float x, float y, string itemType = "")
    {
        Type = type;
        X = x;
        Y = y;
        ItemType = itemType;
    }
}
