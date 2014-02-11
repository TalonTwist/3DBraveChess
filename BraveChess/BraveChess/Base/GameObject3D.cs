using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace BraveChess.Base
{
    public delegate void GameObjectEventHandler(string id);

    public class GameObject3D
    {
        public string ID { get; set; }
        public Matrix World { get; set; }

        public event GameObjectEventHandler Destroying;


        public GameObject3D(string id)
        {
            ID = id;
            World = Matrix.Identity;
        }

        public GameObject3D(string id, Matrix position)
        {
            ID = id;
            World = position;
        }

        public GameObject3D(string id, Vector3 position)
        {
            ID = id;
            World = Matrix.Identity * Matrix.CreateTranslation(position);
        }

        public virtual void Initialise() { }
        public virtual void LoadContent(ContentManager content) { }
        public virtual void Update(GameTime gametime) { }
        public virtual void Draw(Camera camera) { }

        public void UpdateWorld(Vector3 pos)
        {
            World = Matrix.Identity * Matrix.CreateTranslation(pos);
        }

        public void Destroy()
        {
            if (Destroying != null)
                Destroying(ID);
        }
    }
}
