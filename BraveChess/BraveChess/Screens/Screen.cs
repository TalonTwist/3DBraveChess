using Microsoft.Xna.Framework;
using Ruminate.GUI.Framework;
using BraveChess.Base;

namespace BraveChess.Screens
{
    public abstract class Screen
    {
        public Color Color { get; set; }
        protected Gui Gui;

        public abstract void Init(GameEngine game);

        public virtual void OnResize()//allows UI to scale with window if user resizing == true
        {
            if (Gui != null)
                Gui.Resize();
        }

        public virtual void Update()//Updates Gui, handles input detection
        {
            if(Gui != null)
                Gui.Update();
        }

        void _gui_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Microsoft.Xna.Framework.Input.Keys.A)
            {
               
            }
        }

        public virtual void Draw()//draws interface
        {
            if(Gui != null)
                Gui.Draw();
        }
    }
}
