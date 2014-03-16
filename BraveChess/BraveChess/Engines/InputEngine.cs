using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BraveChess.Engines
{
    public class InputEngine : GameComponent
    {
        private static GamePadState _previousPadState;
        private static GamePadState _currentPadState;

        private static KeyboardState _previousKeyState;
        private static KeyboardState _currentKeyState;

        private static Vector2 _currentMousePos;

        private static MouseState _previousMouseState;
        private static MouseState _currentMouseState;

        public InputEngine(Game game)
            : base(game)
        {
            _currentPadState = GamePad.GetState(PlayerIndex.One);
            _currentKeyState = Keyboard.GetState();

            game.Components.Add(this);
        }

        public static void ClearState()
        {
            _previousMouseState = Mouse.GetState();
            _currentMouseState = Mouse.GetState();
            _previousKeyState = Keyboard.GetState();
            _currentKeyState = Keyboard.GetState();
        }

        public override void Update(GameTime gametime)
        {
            _previousPadState = _currentPadState;
            _previousKeyState = _currentKeyState;

            _currentPadState = GamePad.GetState(PlayerIndex.One);
            _currentKeyState = Keyboard.GetState();

#if WINDOWS
            _previousMouseState = _currentMouseState;
            _currentMousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            _currentMouseState = Mouse.GetState();
#endif

            base.Update(gametime);
        }

        public static bool IsButtonPressed(Buttons buttonToCheck)
        {
            return _currentPadState.IsButtonUp(buttonToCheck) && _previousPadState.IsButtonDown(buttonToCheck);
        }

        public static bool IsButtonHeld(Buttons buttonToCheck)
        {
            return _currentPadState.IsButtonDown(buttonToCheck);
        }

        public static bool IsKeyHeld(Keys buttonToCheck)
        {
            return _currentKeyState.IsKeyDown(buttonToCheck);
        }

        public static bool IsKeyPressed(Keys keyToCheck)
        {
            return _currentKeyState.IsKeyUp(keyToCheck) && _previousKeyState.IsKeyDown(keyToCheck);
        }

        public static GamePadState CurrentPadState
        {
            get { return _currentPadState; }
            set { _currentPadState = value; }
        }
        public static KeyboardState CurrentKeyState
        {
            get { return _currentKeyState; }
        }

        public static MouseState CurrentMouseState
        {
            get { return _currentMouseState; }
        }

        public static MouseState PreviousMouseState
        {
            get { return _previousMouseState; }
        }

#if WINDOWS

        public static bool IsMouseLeftClick()
        {
            return _currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool IsMouseRightClick()
        {
            return _currentMouseState.RightButton == ButtonState.Released && _previousMouseState.RightButton == ButtonState.Pressed;
        }

        public static bool IsMouseRightHeld()
        {
            return _currentMouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Pressed;
        }

        public static bool IsMouseLeftHeld()
        {
            return _currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Pressed;
        }

        public static Vector2 MousePosition
        {
            get { return _currentMousePos; }
        }
#endif

    }
}
