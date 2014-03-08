using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


            _gui = new Gui(game.Game, skin, text);

            _gui.Widgets = new Widget[]
            {
                new Button(20,80,200,"Start standard Game",buttonEvent:delegate(Widget widget)
                    {
                        game.StateChange(GameEngine.State.NonNetworkGame);
                        //game._state = GameEngine.State.NonNetworkGame;
                    }),
                    new Button(20,120,200,"Start Networked Game",buttonEvent:delegate(Widget widget)
                        {
                            game.StateChange(GameEngine.State.NetworkGame);
                        })
                    
            };
        }
    }
}
