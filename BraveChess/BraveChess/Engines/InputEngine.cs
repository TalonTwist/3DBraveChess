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

        private static Vector2 previousMousePos;
        private static Vector2 currentMousePos;

        private static MouseState previousMouseState;
        private static MouseState currentMouseState;

        public InputEngine(Game game)
            : base(game)
        {
            _currentPadState = GamePad.GetState(PlayerIndex.One);
            _currentKeyState = Keyboard.GetState();

            game.Components.Add(this);
        }

        public static void ClearState()
        {
            previousMouseState = Mouse.GetState();
            currentMouseState = Mouse.GetState();
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
            previousMouseState = currentMouseState;
            currentMousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            currentMouseState = Mouse.GetState();
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
            get { return currentMouseState; }
        }

        public static MouseState PreviousMouseState
        {
            get { return previousMouseState; }
        }

#if WINDOWS

        public static bool IsMouseLeftClick()
        {
            return currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool IsMouseRightClick()
        {
            return currentMouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed;
        }

        public static bool IsMouseRightHeld()
        {
            return currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed;
        }

        public static bool IsMouseLeftHeld()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed;
        }

        public static Vector2 MousePosition
        {
            get { return currentMousePos; }
        }
#endif

    }
}
