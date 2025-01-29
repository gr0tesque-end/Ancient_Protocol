using System.Collections.Generic;
using System.Diagnostics;
using Game.Misc;
using Game.Renderables;
using Game.Renderables.Player;
using Game.Renderables.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Net.Mime.MediaTypeNames;

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
    private Camera _camera;

    private IRenderable[] _renderables;
    private Tile testTile;
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
        _player = new Player(new Vector2(940f, 440f), _graphics.GraphicsDevice);
        _camera = new Camera();
        testTile = new Tile() { Position = new Vector2(500f, 500f) };
        _renderables = [
            testTile,
            _player
            ];
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

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
        _camera.Follow(_player, GraphicsDevice.Viewport, new(64, 96));
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(transformMatrix: _camera.Transform);

        //DrawRectangle(_spriteBatch, _camera.ViewRectangle, Color.Red);

        foreach (var item in _renderables)
        {
            if (item is null || !_camera.ViewRectangle.Contains(item.Position.X, item.Position.Y)) {
                //Debug.WriteLine(item?.Bounds.ToString());
                continue; 
            }
            
            item.Draw(_spriteBatch);
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private void DrawRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color)
    {
        int thickness = 2; // Line thickness

        // Top line
        spriteBatch.Draw(_debugTexture, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
        // Bottom line
        spriteBatch.Draw(_debugTexture, new Rectangle(rect.X, rect.Y + rect.Height, rect.Width, thickness), color);
        // Left line
        spriteBatch.Draw(_debugTexture, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        // Right line
        spriteBatch.Draw(_debugTexture, new Rectangle(rect.X + rect.Width, rect.Y, thickness, rect.Height), color);
    }
}
