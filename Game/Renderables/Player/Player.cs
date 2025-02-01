using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Game.Misc;
using Game.Optimization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game.Renderables.Player;

//public delegate void DrawCall(Texture2D texture, Vector2 position, Color color);
public enum PlayerState
{
    Idle,
    Walking
}
[Flags]
public enum Direction : byte
{
    N = 2,    // North
    S = 4,    // South
    W = 8,    // West
    E = 16,   // East

    SE = S | E, // South-East
    SW = S | W, // South-West
    NW = N | W, // North-West
    NE = N | E  // North-East
}

public class Player
    : IRenderable
{
    public Vector2 Position { get; private set; }
    public Rectangle Bounds { get; private set; }

    private Vector2 _velocity;
    public Vector2 Velocity { get => _velocity; private set => _velocity = value; }
    public float SpeedModifier { get; set; } = 1.75f;
    public Texture2D CurrentTexture { get; private set; }

    private ConcurrentDictionary<Direction, SpriteSheetManager> _animTextures;
    private SpriteSheetContainer _idleSpriteContainer;

    private ParallelStateChecker _stateChecker;

    private PlayerState _state;
    public PlayerState state
    {
        get => _state;
        set
        {
            _state = value;
            StateChanged.Invoke();
        }
    }

    private Direction _dir;
    public Direction direction
    {
        get => _dir;
        set
        {
            _dir = value;
            DirectionChanged.Invoke();
        }
    }

    public event Action DirectionChanged;
    public event Action StateChanged;

    private bool isDirChanged;
    private bool isStateChanged;
    public Func<bool>[] Checkings { get; }

    private GraphicsDevice _device;
    byte frame = 0;

    public Player(Vector2 startPosition, GraphicsDevice device)
    {
        DirectionChanged += () => isDirChanged = true;
        StateChanged += () => isStateChanged = true;

        Position = startPosition;
        _device = device;
        state = PlayerState.Idle;
        direction = Direction.S;

        _stateChecker = new()
        {
            StateCheckers = [
                CheckAnimationRendering
            ]
        };
    }

    /// <summary>
    /// Checks whether <see cref="CurrentTexture"/> needs to be updated with the next animation sprite
    /// <br/> If state is idle - does nothing
    /// </summary>
    /// <returns><see cref="true"/> if texture was changed;<br/>
    /// <see cref="false"/> if no changes were made
    /// </returns>
    private void CheckAnimationRendering()
    {
        if (state == PlayerState.Idle) return;
        if (frame % 5 == 0) UpdateCurrentAnimTexture();
    }

    private void CheckAndUpdateFrameCounter()
    {
        if (frame >= 60) frame = 0;
        else
        {
            ++frame;
        }
    }

    public void LoadContent(ContentManager content)
    {
        _animTextures = new ConcurrentDictionary<Direction, SpriteSheetManager>();
        _idleSpriteContainer = new SpriteSheetContainer();
        _idleSpriteContainer.LoadSpritesheet
            ("Player/Default/#idle", _device, content);
        CurrentTexture = _idleSpriteContainer.Spritesheet[1];

        /*Parallel.ForEach(DirectionHelper.ClockwiseOrder, (dir) =>
        {
            _animTextures.TryAdd(
                    dir,
                    new SpriteSheetManager() { IsAnimation = true }
                        .LoadSpritesheet(
                            $"Player/Default/{dir}#walking",
                            _device,
                            content)
                );
        });*/

        foreach (var dir in DirectionHelper.ClockwiseOrder)
        {
            _animTextures.TryAdd(
                    dir,
                    (SpriteSheetManager) new SpriteSheetManager()
                        .LoadSpritesheet(
                            $"Player/Default/{dir}#walking",
                            _device,
                            content)
                );
        }

        Bounds = new(
            (int)Position.X,
            (int)Position.Y,
            _idleSpriteContainer.TextureWidth,
            _idleSpriteContainer.TextureHeight);

        /*UpdateCurrentAnimTexture();*/
    }

    /// <summary>
    /// Handles input, updates state, direction and position respectfully
    /// </summary>
    /// <param name="gameTime"></param>
    private void HandleInput(GameTime gameTime)
    {
        var kState = Keyboard.GetState();

        if (!UpdateDirection(kState)) { state = PlayerState.Idle; return; }
        Velocity = Vector2.Zero;

        _velocity.X = 0;
        _velocity.Y = 0;

        if ((direction & Direction.S) != 0) _velocity.Y += 1;
        if ((direction & Direction.N) != 0) _velocity.Y -= 1;
        if ((direction & Direction.E) != 0) _velocity.X += 1;
        if ((direction & Direction.W) != 0) _velocity.X -= 1;

        if (_velocity.X != 0 && _velocity.Y != 0)
        {
            var length = Math.Sqrt(_velocity.X * _velocity.X + _velocity.Y * _velocity.Y);
            _velocity.X /= (float)length;
            _velocity.Y /= (float)length;
        }

        if (_velocity != Vector2.Zero)
        {
            _velocity.Normalize();
            state = PlayerState.Walking;
        }
        else
        {
            state = PlayerState.Idle;
        }

        Position += SpeedModifier * _velocity * 100f * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    private void UpdateCurrentAnimTexture()
    {
        CurrentTexture = _animTextures[direction].GetNextTexture();
    }
    public bool UpdateDirection(KeyboardState kState)
    {
        bool up = kState.IsKeyDown(Keys.W);
        bool down = kState.IsKeyDown(Keys.S);
        bool left = kState.IsKeyDown(Keys.A);
        bool right = kState.IsKeyDown(Keys.D);

        if (!up && !down && !left && !right) return false;

        if (down && right) direction = Direction.SE;
        else if (down && left) direction = Direction.SW;
        else if (up && left) direction = Direction.NW;
        else if (up && right) direction = Direction.NE;
        else if (down) direction = Direction.S;
        else if (up) direction = Direction.N;
        else if (left) direction = Direction.W;
        else if (right) direction = Direction.E;

        return true;
    }

    public void Update(GameTime gameTime)
    {
        CheckAndUpdateFrameCounter();
        isDirChanged = false; isStateChanged = false;
        HandleInput(gameTime);
        _stateChecker.UpdateAllStates();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        //spriteBatch.Draw(_idleSpriteContainer.Spritesheet[0], Position, Color.White);

        //Debug.WriteLine($"Player: {Position}\n Camera: {Camera.ViewRectangle}");

        if (isStateChanged & state == PlayerState.Idle)
        {
            spriteBatch.Draw(
                _idleSpriteContainer
                    .Spritesheet[
                        DirectionHelper.dirIndex[direction]
                    ],
                Position,
                Color.White);
            return;
        }

        if (CurrentTexture is null) return;

        spriteBatch.Draw(CurrentTexture, Position, Color.White);

    }
}
