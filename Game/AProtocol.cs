using Game.Misc;
using Game.Renderables.Player;
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
    private Camera _camera;

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
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _player.LoadContent(this.Content);
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
        _player.Draw(_spriteBatch);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
