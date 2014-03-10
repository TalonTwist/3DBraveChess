using Microsoft.Xna.Framework;

using Ruminate.GUI.Content;
using Ruminate.GUI.Framework;
using BraveChess.Base;

namespace BraveChess.Screens
{
    public class MainMenu : Screen
    {
        public override void Init(GameEngine game)
        {
            Color = Color.White;

            var skin = new Skin(game.GreyImageMap, game.GreyMap);
            var text = new Text(game.GreySpriteFont, Color.Green);

<<<<<<< HEAD
            Gui = new Gui(game.Game, skin, text)
=======

            _gui = new Gui(game.Game, skin, text);

            _gui.Widgets = new Widget[]
>>>>>>> 3600e1b73be9658d8d3f62287410e2211eb5fe08
            {
                Widgets = new Widget[]
                {
                    new Button(20, 80, 200, "Start standard Game", delegate
                    {
                        game.StateChange(GameEngine.State.NonNetworkGame);
<<<<<<< HEAD
                        game.GameState = GameEngine.State.NonNetworkGame;
=======
                        //game._state = GameEngine.State.NonNetworkGame;
>>>>>>> 3600e1b73be9658d8d3f62287410e2211eb5fe08
                    }),
                    new Button(20, 120, 200, "Start Networked Game", delegate
                    {
                        game.StateChange(
                            GameEngine.State
                                .NetworkGame);
                    })

                }
            };
        }
    }
}
