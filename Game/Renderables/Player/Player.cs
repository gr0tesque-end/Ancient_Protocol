using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Misc;
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
public enum Direction: byte
{
    N = 10,
    S = 11,
    W = 1,
    E = 0
}
public class Player
    : IRenderable
{
    public Vector2 Position { get; private set; }

    private Vector2 _velocity;
    public Vector2 Velocity { get => _velocity; private set => _velocity = value; }
    public Texture2D CurrentTexture { get; private set; }

    private Dictionary<(PlayerState, Direction), Texture2D[]> _animations;
    private PlayerState _state;
    private Direction _direction;
    private int _currentFrame;
    private float _animationTimer;
    private float _animationSpeed = 0.1f; // Time per frame in seconds

    private StateBasedPath _path;

    public Player(Vector2 startPosition)
    {
        Position = startPosition;
        _state = PlayerState.Idle;
        _direction = Direction.S;
    }

    public void LoadContent(ContentManager content)
    {
        // Load animations for each state and direction
        _animations = new Dictionary<(PlayerState, Direction), Texture2D[]>
        {
            {
                (PlayerState.Idle, Direction.S), new[] {
                content.Load<Texture2D>("Player/Default_Separate/Idle/S#Idle1")
                }
            },

            {
                (PlayerState.Idle, Direction.Up), new[] {
                content.Load<Texture2D>("idle_up_1")
                }
            },
            { 
                (PlayerState.Idle, Direction.Left), new[] {
                    content.Load<Texture2D>("idle_left_1") 
                } 
            },
            { 
                (PlayerState.Idle, Direction.Right), new[] { content.Load<Texture2D>("idle_right_1") 
                } 
            },

            { 
                (PlayerState.Walking, Direction.Down), new[]
                {
                    content.Load<Texture2D>("walk_down_1"),
                    content.Load<Texture2D>("walk_down_2"),
                    content.Load<Texture2D>("walk_down_3")
                }
            },
            { 
                (PlayerState.Walking, Direction.Up), new[]
                {
                    content.Load<Texture2D>("walk_up_1"),
                    content.Load<Texture2D>("walk_up_2"),
                    content.Load<Texture2D>("walk_up_3")
                }
            },
            { 
                (PlayerState.Walking, Direction.Left), new[]
                {
                    content.Load<Texture2D>("walk_left_1"),
                    content.Load<Texture2D>("walk_left_2"),
                    content.Load<Texture2D>("walk_left_3")
                }
            },
            { 
                (PlayerState.Walking, Direction.Right), new[]
                {
                    content.Load<Texture2D>("walk_right_1"),
                    content.Load<Texture2D>("walk_right_2"),
                    content.Load<Texture2D>("walk_right_3")
                }
            },
        };

        UpdateCurrentTexture();
    }

    public void Update(GameTime gameTime)
    {
        HandleInput(gameTime);
        UpdateCurrentTexture();
        Animate(gameTime);
    }

    private void HandleInput(GameTime gameTime)
    {
        // Update velocity and direction based on input
        Velocity = Vector2.Zero;

        if (Keyboard.GetState().IsKeyDown(Keys.W))
        {
            _velocity.Y = -1;
            _direction = Direction.Up;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.S))
        {
            _velocity.Y = 1;
            _direction = Direction.Down;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            _velocity.X = -1;
            _direction = Direction.Left;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            _velocity.X = 1;
            _direction = Direction.Right;
        }
        if (_velocity != Vector2.Zero)
        {
            _velocity.Normalize();
            _state = PlayerState.Walking;
        }
        else
        {
            _state = PlayerState.Idle;
        }

        Position += _velocity * 100f * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    private void Animate(GameTime gameTime)
    {
        var key = (_state, _direction);

        if (_animations.ContainsKey(key))
        {
            var frames = _animations[key];

            if (frames.Length > 1)
            {
                _animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_animationTimer >= _animationSpeed)
                {
                    _animationTimer = 0f;
                    _currentFrame = (_currentFrame + 1) % frames.Length;
                }
            }
            else
            {
                _currentFrame = 0;
            }

            CurrentTexture = frames[_currentFrame];
        }
    }

    private void UpdateCurrentTexture()
    {
        CurrentTexture = _animations[(_state, _direction)][_currentFrame];
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (CurrentTexture != null)
        {
            spriteBatch.Draw(CurrentTexture, Position, Color.White);
        }
    }
}
