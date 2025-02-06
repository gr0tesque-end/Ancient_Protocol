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

public class Player
    : IRenderable, IPositionTrackable
{
    public Vector2 Position { get; private set; }
    public Rectangle Bounds { get; private set; }

    private Vector2 _velocity;
    public Vector2 Velocity { get => _velocity; private set => _velocity = value; }
    public float SpeedModifier { get; set; } = 1.75f;
    public Texture2D Texture { get; private set; }

    private ConcurrentDictionary<Direction, SpriteSheetManager> _animTextures;
    private SpriteSheetContainer _idleSpriteContainer;

    private ParallelStateChecker _stateChecker;

    private PlayerState _state;
    public PlayerState State
    {
        get => _state;
        private set
        {
            _state = value;
            StateChanged.Invoke();
        }
    }

    private Direction _dir;
    public Direction Direction
    {
        get => _dir;
        private set
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
        State = PlayerState.Idle;
        Direction = Direction.S;

        _stateChecker = new()
        {
            StateCheckers = [
                CheckAnimationRendering
            ]
        };
    }

    /// <summary>
    /// Checks whether <see cref="Texture"/> needs to be updated with the next animation sprite
    /// <br/> If state is idle - does nothing
    /// </summary>
    /// <returns><see cref="true"/> if texture was changed;<br/>
    /// <see cref="false"/> if no changes were made
    /// </returns>
    private void CheckAnimationRendering()
    {
        if (State == PlayerState.Idle) return;
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
        Texture = _idleSpriteContainer.Spritesheet[1];

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

        if (!UpdateDirection(kState)) { State = PlayerState.Idle; return; }
        Velocity = Vector2.Zero;

        _velocity.X = 0;
        _velocity.Y = 0;

        if ((Direction & Direction.S) != 0) _velocity.Y += 1;
        if ((Direction & Direction.N) != 0) _velocity.Y -= 1;
        if ((Direction & Direction.E) != 0) _velocity.X += 1;
        if ((Direction & Direction.W) != 0) _velocity.X -= 1;

        if (_velocity.X != 0 && _velocity.Y != 0)
        {
            var length = Math.Sqrt(_velocity.X * _velocity.X + _velocity.Y * _velocity.Y);
            _velocity.X /= (float)length;
            _velocity.Y /= (float)length;
        }

        if (_velocity != Vector2.Zero)
        {
            _velocity.Normalize();
            State = PlayerState.Walking;
        }
        else
        {
            State = PlayerState.Idle;
        }

        Position += SpeedModifier * _velocity * 100f * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    private void UpdateCurrentAnimTexture()
    {
        Texture = _animTextures[Direction].GetNextTexture();
    }
    public bool UpdateDirection(KeyboardState kState)
    {
        bool up = kState.IsKeyDown(Keys.W);
        bool down = kState.IsKeyDown(Keys.S);
        bool left = kState.IsKeyDown(Keys.A);
        bool right = kState.IsKeyDown(Keys.D);

        if (!up && !down && !left && !right) return false;

        if (down && right) Direction = Direction.SE;
        else if (down && left) Direction = Direction.SW;
        else if (up && left) Direction = Direction.NW;
        else if (up && right) Direction = Direction.NE;
        else if (down) Direction = Direction.S;
        else if (up) Direction = Direction.N;
        else if (left) Direction = Direction.W;
        else if (right) Direction = Direction.E;

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

        if (isStateChanged & State == PlayerState.Idle)
        {
            spriteBatch.Draw(
                _idleSpriteContainer
                    .Spritesheet[
                        DirectionHelper.dirIndex[Direction]
                    ],
                Position,
                Color.White);
            return;
        }

        if (Texture is null) return;

        spriteBatch.Draw(Texture, Position, Color.White);

    }
}
