using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Game.Misc;
using Game.Optimization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game.Renderables.Player;
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
    : IRenderable, ICamareFollowable, ICheckStateInParallel
{
    public Vector2 Position { get; private set; }

    private Vector2 _velocity;
    public Vector2 Velocity { get => _velocity; private set => _velocity = value; }
    public float SpeedModifier { get; set; } = 1.75f;
    public Texture2D CurrentTexture { get; private set; }

    private ConcurrentDictionary<Direction, SpriteSheetManager> _animTextures;
    private SpriteSheetManager _idleSpriteManager;

    public PlayerState state;
    public Direction direction;
    public Func<bool>[] Checkings { get; }

    ICheckStateInParallel ParallelCheckingInstance;
    private GraphicsDevice _device;
    byte frame = 0;

    public Player(Vector2 startPosition, GraphicsDevice device)
    {
        Position = startPosition;
        _device = device;
        state = PlayerState.Idle;
        direction = Direction.S;

        ParallelCheckingInstance = this;
        Checkings =
        [
            CheckAnimationRendering
        ];
    }

    /// <summary>
    /// Checks whether <see cref="CurrentTexture"/> needs to be updated with the next <see cref="AnimationTexture2D"/> object
    /// <br/> If state is idle - does nothing
    /// </summary>
    /// <returns><see cref="true"/> if texture was changed;<br/>
    /// <see cref="false"/> if no changes were made
    /// </returns>
    private bool CheckAnimationRendering()
    {
        if (state == PlayerState.Idle) return false;
        if (frame % 4 == 0) UpdateCurrentAnimTexture();
        else { return false; }
        return true;
    }

    private bool CheckAndUpdateFrameCounter()
    {
        if (frame >= 60) frame = 0;
        else
        {
            ++frame;
            return false;
        }
        return true;
    }

    public void LoadContent(ContentManager content)
    {
        _animTextures = new ConcurrentDictionary<Direction, SpriteSheetManager>();
        _idleSpriteManager = new SpriteSheetManager() { IsAnimation = false };
        _idleSpriteManager.LoadSpritesheet("Player/Default/#idle", _device, content);


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
                    new SpriteSheetManager() { IsAnimation = true }
                        .LoadSpritesheet(
                            $"Player/Default/{dir}#walking",
                            _device,
                            content)
                );
        }

        /*UpdateCurrentAnimTexture();*/
    }

    public void Update(GameTime gameTime)
    {
        CheckAndUpdateFrameCounter();
        //if (state == PlayerState.Idle) Debug.WriteLine("Idle"); 
        HandleInput(gameTime);
        ParallelCheckingInstance.RunAllCheckings();
    }

    /// <summary>
    /// Handles input, updates state, direction and position respectfully
    /// </summary>
    /// <param name="gameTime"></param>
    private void HandleInput(GameTime gameTime)
    {
        if (!UpdateDirection()) { state = PlayerState.Idle; return; }
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
    public bool UpdateDirection()
    {
        bool up = Keyboard.GetState().IsKeyDown(Keys.W);
        bool down = Keyboard.GetState().IsKeyDown(Keys.S);
        bool left = Keyboard.GetState().IsKeyDown(Keys.A);
        bool right = Keyboard.GetState().IsKeyDown(Keys.D);

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

    public void Draw(SpriteBatch spriteBatch)
    {
        if (CurrentTexture != null)
        {
            if (state == PlayerState.Idle)
            {
                spriteBatch.Draw(_idleSpriteManager.Spritesheet[DirectionHelper.GetClockwiseIndex(direction)],
                    Position,
                    Color.White);
                return;
            }

            spriteBatch.Draw(CurrentTexture, Position, Color.White);
        }
    }
}
