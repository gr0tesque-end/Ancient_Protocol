using System.Collections.Generic;
using System.Linq;
using Game.Misc;
using Game.Renderables;
using Game.Renderables.Player;
using Game.Renderables.Tiles;
using Game.Renderables.Tiles.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Game;

public class AProtocol
    : Microsoft.Xna.Framework.Game
{
    // Not Thread-Safe;
    // Not recommended to run in any thread but the Main one
    private GraphicsDeviceManager _graphics;

    // Not Thread-Safe
    // Not recommended to run in any thread but the Main one
    private SpriteBatch _spriteBatch;

    private Player _player;

    private IRenderable[] _renderables;
    private Dictionary<string, Texture2D> Textures;

    /// <summary>
    /// Any Tile present in this dictionary is automaticaly encased with walls
    /// </summary>
    private Dictionary<Rectangle, IRenderable> _globalTileMap = [];

   private Texture2D _debugTexture;
    public AProtocol()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.IsFullScreen = false;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

    }

    protected override void Initialize()
    {
        _player = new Player(Vector2.Zero, _graphics.GraphicsDevice);

        base.Initialize();

    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Textures = new() {
            { "Grass1", Content.Load<Texture2D>("Tiles/Grass/Grass1") },
            { "Stone2", Content.Load<Texture2D>("Tiles/Stone/Stone2") }
            };

        for (int i = 1; i < 6; i++)
        {
            Textures.Add(
                $"WallStone{i}", 
                Content.Load<Texture2D>($"Tiles/Wall/WallStone{i}"));
        }

#nullable enable
        var fac = new TileMapFactory<Tile>(
                 Textures["Stone2"],
                 new CircleShapeProvider(
                     Textures["Stone2"],
                     new Rectangle(Point.Zero, new(64, 64)),
                     5, 5,
                     true)
                 { Precision = 1.2f }
                 );

        _renderables = [
            fac.Produce().AddToGlobalTileMap(ref _globalTileMap),
            new TileMapFactory<Tile>(
                 Textures["Stone2"],
                 new RectangleShapeProvider(
                     Point.Zero + new Point(64*6, -64*6),
                     1, 1,
                     false
                     )
                 ).Produce().AddToGlobalTileMap(ref _globalTileMap),
            _player,
            /* Automatic Tile encasement with walls */
            new TileMap(
                DictionaryCaster.CastVal(
                    Tile.EncaseTiles(_globalTileMap, Textures["WallStone1"], Textures),
                    kvp => KeyValuePair.Create(kvp.Key, (IRenderable)kvp.Value)
            )).AddToGlobalTileMap(ref _globalTileMap)
        ];
#nullable disable
        _player.LoadContent(Content);

        _debugTexture = new Texture2D(GraphicsDevice, 1, 1);
        _debugTexture.SetData(new[] { Color.White });

        foreach (var item in _renderables)
        {
            item.LoadContent(this.Content);
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _player.Update(gameTime);
        Camera.Follow(_player, GraphicsDevice.Viewport, new(64, 96));
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(transformMatrix: Camera.Transform);

        //DrawRectangle(_spriteBatch, _camera.ViewRectangle, Color.Red);

        foreach (var item in _renderables)
        {
            if (item is Wall)
            {
                DrawRectangle(_spriteBatch, _debugTexture, item.Bounds, Color.Red);
            }
            item.Draw(_spriteBatch);
        }

        /*_player.Draw(_spriteBatch);*/

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private void DrawRectangle(SpriteBatch spriteBatch, Texture2D debugTexture, Rectangle rect, Color color)
    {
        int thickness = 2; // Line thickness

        // Top line
        spriteBatch.Draw(debugTexture, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
        // Bottom line
        spriteBatch.Draw(debugTexture, new Rectangle(rect.X, rect.Y + rect.Height, rect.Width, thickness), color);
        // Left line
        spriteBatch.Draw(debugTexture, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        // Right line
        spriteBatch.Draw(debugTexture, new Rectangle(rect.X + rect.Width, rect.Y, thickness, rect.Height), color);
    }
}
