using Microsoft.Xna.Framework;
using Ruminate.GUI.Framework;
using BraveChess.Base;

namespace BraveChess.Screens
{
    public class Screen
    {
        public Color Color { get; set; }
<<<<<<< HEAD
        protected Gui Gui;
=======
        public Gui _gui;
>>>>>>> 3600e1b73be9658d8d3f62287410e2211eb5fe08

        public virtual void Init(GameEngine game) {}

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


        public virtual void Draw()//draws interface
        {
            if(Gui != null)
                Gui.Draw();
        }
    }
}
