using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Misc;
public struct Expression(int currentVal, int minVal, int maxVal, int actionVal)
{
    public int CurrentVal = currentVal;
    public int MinVal = minVal;
    public int MaxVal = maxVal;
    public int ActionVal = actionVal;
};

/// <summary>
/// Manages texture return for animation; iteratively <br/>
/// </summary>
public class SpriteSheetManager
{
    public bool IsAnimation { get; init; } = false;
    public List<Texture2D> Spritesheet { get; } = new(8); // Each spritesheet contains a grid of textures
    protected Expression _expression; // Tracks the state for returning textures
    protected int TextureWidth { get; init; } = 64; // Width of a single texture
    protected int TextureHeight { get; init; } = 96; // Height of a single texture

    /// <summary>
    /// Initializes the spritesheet manager with the given range expression.
    /// </summary>
    /// <param name="currentVal">Starting index for texture selection.</param>
    /// <param name="minVal">Minimum texture index.</param>
    /// <param name="maxVal">Maximum texture index.</param>
    /// <param name="actionVal">Increment value for selecting the next texture.</param>
    public SpriteSheetManager(int currentVal, int minVal, int maxVal, int actionVal)
    {
        _expression = new Expression(currentVal, minVal, maxVal, actionVal);
    }

    public SpriteSheetManager() : this(-1, 0, 8, 1) { }

    /// <summary>
    /// Loads a spritesheet and extracts individual textures.
    /// </summary>
    /// <param name="spritesheetPath">Path to the spritesheet image file.</param>
    /// <param name="graphicsDevice">Graphics device for creating textures.</param>
    public SpriteSheetManager LoadSpritesheet(
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

            if (!IsAnimation) texture.Name = "Idle"; 

            Spritesheet.Add(texture);
        }
        spritesheet.Dispose();
        return this;
    }

    /// <summary>
    /// Gets the next texture based on the current state of the expression.<br/>
    /// Handles the state of private member <see cref="Expression"/>
    /// </summary>
    /// <returns>The next texture to render.</returns>
    public Texture2D GetNextTexture()
    {
        // Update the expression state

        _expression.CurrentVal += _expression.ActionVal;

        if (_expression.CurrentVal == _expression.MaxVal)
        {
            _expression.CurrentVal = _expression.MinVal;
        }

        return Spritesheet[_expression.CurrentVal];
    }
}
