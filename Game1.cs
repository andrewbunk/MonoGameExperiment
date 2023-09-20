using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using System;
using System.Diagnostics;

namespace Project2
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        TiledMap _tiledMap;
        TiledMapRenderer _tiledMapRenderer;
        private OrthographicCamera _camera;
        private Vector2 _cameraPosition;
        private AnimatedSprite _charSprite;
        private Vector2 _charPosition;
        private AnimatedSprite _front;
        private AnimatedSprite _back;
        private bool flip;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            var viewportadapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 600);
            _camera = new OrthographicCamera(viewportadapter);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _tiledMap = Content.Load<TiledMap>("untitled");
            _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tiledMap);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _front = new AnimatedSprite(Content.Load<SpriteSheet>("CharacterSheet_CharacterFront.sf", new JsonContentLoader()));
            _back = new AnimatedSprite(Content.Load<SpriteSheet>("CharacterSheet_FroggoBack.sf", new JsonContentLoader()));

            _front.Play("idle");
            _charPosition = new Vector2(0, 0);
            _charSprite = _front;
        }
        protected override void Update(GameTime gameTime)
        {
            _tiledMapRenderer.Update(gameTime);

            MoveCamera(gameTime);
            _camera.LookAt(_charPosition);
            
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            _tiledMapRenderer.Draw(_camera.GetViewMatrix());
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_charSprite, _charPosition);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private Vector2 GetMovementDirection(GameTime gameTime)
        {
            var movementDirection = Vector2.Zero;
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var walkSpeed = deltaSeconds * 128;
            var state = Keyboard.GetState();
            var animation = "idle";

            if (state.IsKeyDown(Keys.Down))
            {
                animation = "walk";
                _charSprite = _front;
                movementDirection.Y += walkSpeed;
                Debug.WriteLine(movementDirection.Y);
            }
            if (state.IsKeyDown(Keys.Up))
            {
                animation = "walk";
                _charSprite = _back;
                movementDirection.Y -= walkSpeed;
                
            }
            if (state.IsKeyDown(Keys.Left))
            {
                animation = "walk";
                movementDirection.X -= walkSpeed;
                
            }
            if (state.IsKeyDown(Keys.Right))
            {
                animation = "walk";
                flip = true;
                movementDirection.X += walkSpeed;
            }
            _charSprite.Play(animation);
            _charSprite.Update(deltaSeconds);
            // Can't normalize the zero vector so test for it before normalizing
            if (movementDirection != Vector2.Zero)
            {
                movementDirection.Normalize();
            }

            return movementDirection;
        }

        private void MoveCamera(GameTime gameTime)
        {
            var speed = 200;
            var seconds = gameTime.GetElapsedSeconds();
            var movementDirection = GetMovementDirection(gameTime);
            //_cameraPosition += speed * movementDirection * seconds;
            _charPosition += speed * movementDirection * seconds;
        }
    }
}