using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BraveChess.Base
{
    public class Scene
    {
        public enum TurnState
        {
            Black,
            White
        }

        public string ID { get; set; }
        public TurnState Turn { get; set; }

        protected List<GameObject3D> _sceneObjects = new List<GameObject3D>();
        public List<GameObject3D> Objects { get { return _sceneObjects; } }

        protected GameEngine Engine;

        public Scene(string id, GameEngine engine)
        {
            ID = id;
            Engine = engine;
            Turn = TurnState.White;
        }//End of Constructor

        public virtual void Initialize()
        {
            for (int i = 0; i < _sceneObjects.Count; i++)
                _sceneObjects[i].Initialise();
        }//End of Method

        public virtual void Update(GameTime gametime)
        {
            //HandleInput();

            for (int i = 0; i < _sceneObjects.Count; i++)
                _sceneObjects[i].Update(gametime);
        }//End of Method

        protected virtual void HandleInput() { }

        public void AddObject(GameObject3D _newObject)
        {
            _newObject.Destroying += new GameObjectEventHandler(_newObject_Destroying);
            _sceneObjects.Add(_newObject);
        }//End of method

        void _newObject_Destroying(string id)
        {
            RemoveObject(id);
        }//End of Method

        public GameObject3D GetObject(string id)
        {
            for (int i = 0; i < _sceneObjects.Count; i++)
                if (_sceneObjects[i].ID == id)
                    return _sceneObjects[i];

            return null;
        }

        public void RemoveObject(string id)
        {
            for (int i = 0; i < _sceneObjects.Count; i++)
                if (_sceneObjects[i].ID == id)
                    _sceneObjects.RemoveAt(i);
        }
    }
}
