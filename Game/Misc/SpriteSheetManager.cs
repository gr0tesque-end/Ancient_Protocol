using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Misc;

internal class SpriteSheetManager
{
    private readonly List<Texture2D[,]> _spritesheets = new(); // Each spritesheet contains a grid of textures
    private Expression _expression; // Tracks the state for returning textures
    private const int TextureWidth = 64; // Width of a single texture
    private const int TextureHeight = 96; // Height of a single texture

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

    /// <summary>
    /// Loads a spritesheet and extracts individual textures.
    /// </summary>
    /// <param name="spritesheetPath">Path to the spritesheet image file.</param>
    /// <param name="graphicsDevice">Graphics device for creating textures.</param>
    public void LoadSpritesheet(string spritesheetPath, GraphicsDevice graphicsDevice)
    {
        var spritesheet = Texture2D.FromFile(graphicsDevice, spritesheetPath);
        int columns = spritesheet.Width / TextureWidth;
        int rows = spritesheet.Height / TextureHeight;

        var textures = new Texture2D[rows, columns];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                var texture = new Texture2D(graphicsDevice, TextureWidth, TextureHeight);
                var data = new Color[TextureWidth * TextureHeight];
                spritesheet.GetData(0, new Microsoft.Xna.Framework.Rectangle(x * TextureWidth, y * TextureHeight, TextureWidth, TextureHeight), data, 0, data.Length);
                texture.SetData(data);
                textures[y, x] = texture;
            }
        }

        _spritesheets.Add(textures);
    }

    /// <summary>
    /// Gets the next texture based on the current state of the expression.
    /// </summary>
    /// <returns>The next texture to render.</returns>
    public Texture2D GetNextTexture()
    {
        // Calculate the spritesheet and texture indices
        int totalTexturesPerSheet = _spritesheets[0].GetLength(0) * _spritesheets[0].GetLength(1);
        int totalTextures = totalTexturesPerSheet * _spritesheets.Count;

        int flatIndex = _expression.CurrentVal % totalTextures;
        int spritesheetIndex = flatIndex / totalTexturesPerSheet;
        int textureIndex = flatIndex % totalTexturesPerSheet;

        int rows = _spritesheets[spritesheetIndex].GetLength(0);
        int columnIndex = textureIndex % rows;
        int rowIndex = textureIndex / rows;

        // Update the expression state
        if (_expression.CurrentVal == _expression.MaxVal)
        {
            _expression.CurrentVal = _expression.MinVal;
        }
        else
        {
            _expression.CurrentVal += _expression.ActionVal;
        }

        return _spritesheets[spritesheetIndex][rowIndex, columnIndex];
    }
}
