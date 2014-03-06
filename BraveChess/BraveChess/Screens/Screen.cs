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
    public abstract class Screen
    {
        public Color Color { get; set; }
        protected Gui _gui;

        public abstract void Init(GameEngine game);

        public virtual void OnResize()//allows UI to scale with window if user resizing == true
        {
            if (_gui != null)
                _gui.Resize();
        }

        public virtual void Update()//Updates Gui, handles input detection
        {
            if(_gui != null)
                _gui.Update();
        }

        void _gui_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Microsoft.Xna.Framework.Input.Keys.A)
            {
               
            }
        }

        public virtual void Draw()//draws interface
        {
            if(_gui != null)
                _gui.Draw();
        }
    }
}
