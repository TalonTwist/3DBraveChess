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

            Gui = new Gui(game.Game, skin, text)
            {
                Widgets = new Widget[]
                {
                    new Button(20, 80, 200, "Start standard Game", delegate
                    {
                        game.StateChange(GameEngine.State.NonNetworkGame);
                        game.GameState = GameEngine.State.NonNetworkGame;
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
