using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

using BraveChess.Base;

namespace BraveChess.Engines
{
    public class CameraEngine : GameComponent
    {
        private Dictionary<string, Camera> cameras;
        private Camera _activeCamera;
        private string _activeCameraId;

        public CameraEngine(Game game)
            : base(game)
        {
            cameras = new Dictionary<string, Camera>();

            game.Components.Add(this);
        }

        public List<string> GetCurrentCameras()
        {
            return cameras.Keys.ToList();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gametime)
        {
            if (_activeCamera != null)
            {
            }

            base.Update(gametime);
        }

        public void SetActiveCamera(string id)
        {
            if (cameras.ContainsKey(id))
            {
                if (_activeCameraId != id)
                {
                    _activeCamera = cameras[id];
                    _activeCamera.Initialise();

                    _activeCameraId = id;
                }
            }
        }

        public void AddCamera(Camera camera)
        {
            if (!cameras.ContainsKey(camera.Id))
            {
                cameras.Add(camera.Id, camera);

                if (cameras.Count == 1)
                    SetActiveCamera(camera.Id);
            }
        }

        public void RemoveCamera(string id)
        {
            if (cameras.ContainsKey(id))
            {
                if (cameras.Count > 1)
                {
                    cameras.Remove(id);
                }
            }
        }


        public Camera ActiveCamera
        {
            get { return _activeCamera; }
        }

    }
}
