using System;
using Microsoft.Xna.Framework;

namespace BraveChess.Base
{
    public class Camera : GameObject3D
    {
        protected Vector3 cameraTarget;
        protected Vector3 cameraUpDirection;
        protected Vector3 cameraDirection;
        protected Vector3 cameraPosition;

        protected Matrix view;
        protected Matrix projection;

        protected float fieldOfView = MathHelper.PiOver4;
        protected float nearPlane = 0.25f;
        protected float farPlane = 10000;

        protected Vector3 startTarget;

        protected float _aspectRatio = 1.7f;

        public Camera(string id, Vector3 position, Vector3 target, float aspectRatio)
            : base(id, position)
        {
            startTarget = target;
            _aspectRatio = aspectRatio;
        }

        public override void Initialise()
        {
            cameraDirection = Vector3.Zero - World.Translation;
            cameraDirection.Normalize();
            cameraUpDirection = Vector3.Up;
            cameraTarget = World.Translation + cameraDirection;

            CreateLookAt(startTarget);

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _aspectRatio, nearPlane, farPlane);

            base.Initialise();
        }

        public override void Update(GameTime gametime)
        {
            //CreateLookAt();

            base.Update(gametime);
        }

        public virtual void CreateLookAt()
        {
            cameraTarget = World.Translation + cameraDirection;
            view = Matrix.CreateLookAt(World.Translation, cameraTarget, cameraUpDirection);
        }

        public virtual void CreateLookAt(Vector3 position)
        {
            view = Matrix.CreateLookAt(World.Translation, position, cameraUpDirection);
        }

        public virtual void MoveCamera()//needs fixing
        {
            cameraPosition = Vector3.Transform(cameraPosition - cameraTarget,
                Matrix.CreateFromAxisAngle(new Vector3(1,0,1),MathHelper.ToRadians(90)))+cameraTarget;
            view = Matrix.CreateLookAt(cameraPosition,cameraTarget,Vector3.Up);
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
