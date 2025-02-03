using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Misc;

/// <summary>
/// Manages texture return for animation; iteratively <br/>
/// </summary>
public class SpriteSheetManager
    : SpriteSheetContainer
{
    protected Tracker _counter; // Tracks the state for returning textures

    /// <summary>
    /// Initializes the spritesheet manager with the given range expression.
    /// </summary>
    /// <param name="currentVal">Starting index for texture selection.</param>
    /// <param name="minVal">Minimum texture index.</param>
    /// <param name="maxVal">Maximum texture index.</param>
    /// <param name="actionVal">Increment value for selecting the next texture.</param>
    public SpriteSheetManager(int currentVal, int minVal, int maxVal, UpdateAction actionUpdate)
    {
        _counter = new(currentVal, minVal, maxVal, actionUpdate);
    }

    public SpriteSheetManager() {
        _counter = new(8);
    }
    /// <summary>
    /// Gets the next texture based on the current state of the expression.<br/>
    /// Handles the state of private member <see cref="Tracker"/>
    /// </summary>
    /// <returns>The next texture to render.</returns>
    public Texture2D GetNextTexture()
    {
        return Spritesheet[_counter.CurrentVal];
    }
}

public class SpriteSheetContainer
{
    public List<Texture2D> Spritesheet { get; } = new(8);

    public int TextureWidth { get; init; } = 64;
    public int TextureHeight { get; init; } = 96;

    /// <summary>
    /// Loads a spritesheet and extracts individual textures.
    /// </summary>
    /// <param name="spritesheetPath">Path to the spritesheet image file.</param>
    /// <param name="graphicsDevice">Graphics device for creating textures.</param>
    public SpriteSheetContainer LoadSpritesheet(
        string spritesheetPath,
        GraphicsDevice graphicsDevice,
        Microsoft.Xna.Framework.Content.ContentManager content) 
    {
        Texture2D spritesheet;
        spritesheet = content.Load<Texture2D>(spritesheetPath);

        int columns = spritesheet.Width / TextureWidth;

        for (int x = 0; x < columns; x++)
        {
            Texture2D texture;
            texture = new Texture2D(graphicsDevice, TextureWidth, TextureHeight);

            var data = new Microsoft.Xna.Framework.Color[TextureWidth * TextureHeight];

            var rect = new Microsoft.Xna.Framework.Rectangle(
                x * TextureWidth, 0,
                TextureWidth, TextureHeight);

            spritesheet.GetData(0, rect, data, 0, data.Length);

            texture.SetData(data);

            Spritesheet.Add(texture);
        }
        //spritesheet.Dispose();
        return this;
    }
}