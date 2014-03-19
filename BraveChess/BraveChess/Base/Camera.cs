using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using BraveChess.Engines;

namespace BraveChess.Base
{
    public class Camera : GameObject3D
    {
        protected Vector3 CameraTarget;
        protected Vector3 CameraUpDirection;
        protected Vector3 CameraDirection;
        protected Vector3 CameraPosition;
        Vector3 _cameraReference = new Vector3(0, 0, 1);

        protected Matrix view;
        protected Matrix projection;

        protected float FieldOfView = MathHelper.PiOver4;
        protected float NearPlane = 0.25f;
        protected float FarPlane = 10000;
        private const float Speed = 1f;

        protected Vector3 StartTarget;

        protected float AspectRatio = 1.7f;

        public Camera(string id, Vector3 position, Vector3 target, float aspectRatio)
            : base(id, position)
        {
            StartTarget = target;
            AspectRatio = aspectRatio;
        }

        public override void Initialise()
        {
            CameraDirection = Vector3.Zero - World.Translation;
            CameraDirection.Normalize();
            CameraUpDirection = Vector3.Up;
            CameraTarget = World.Translation + CameraDirection;

            CreateLookAt(StartTarget);

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, AspectRatio, NearPlane, FarPlane);

            base.Initialise();
        }

        public override void Update(GameTime gametime)
        {
            WhiteCamControls();

            base.Update(gametime);
        }

        public virtual void CreateLookAt()
        {
            CameraTarget = World.Translation + CameraDirection;
            view = Matrix.CreateLookAt(World.Translation, CameraTarget, CameraUpDirection);
        }

        public virtual void CreateLookAt(Vector3 position)
        {
            view = Matrix.CreateLookAt(World.Translation, position, CameraUpDirection);
        }


        protected void WhiteCamControls()
        {
            if (InputEngine.IsKeyHeld(Keys.D))
            {
                World *= Matrix.CreateTranslation(new Vector3(Speed, 0, 0));
            }
            else if (InputEngine.IsKeyHeld(Keys.A))
            {
                World *= Matrix.CreateTranslation(new Vector3(-Speed, 0, 0));
            }

            if (InputEngine.IsKeyHeld(Keys.S))
            {
                World *= Matrix.CreateTranslation(new Vector3(0, 0, Speed));
            }
            else if (InputEngine.IsKeyHeld(Keys.W))
            {
                World *= Matrix.CreateTranslation(new Vector3(0, 0, -Speed));
            }

            if (InputEngine.IsKeyHeld(Keys.Add))
            {
                World *= Matrix.CreateTranslation(new Vector3(0, Speed, 0));
            }
            else if (InputEngine.IsKeyHeld(Keys.Subtract))
            {
                World *= Matrix.CreateTranslation(new Vector3(0, -Speed, 0));
            }

            
        }



        public Matrix View
        {
            get { return view; }
            set { view = value; }
        }

        public Matrix Projection
        {
            get { return projection; }
            set { projection = value; }
        }
    }
}
