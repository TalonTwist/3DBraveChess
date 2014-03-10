using Microsoft.Xna.Framework;

namespace BraveChess.Base
{
    public class Camera : GameObject3D
    {
        protected Vector3 CameraTarget;
        protected Vector3 CameraUpDirection;
        protected Vector3 CameraDirection;
        protected Vector3 CameraPosition;

        protected Matrix view;
        protected Matrix projection;

        protected float FieldOfView = MathHelper.PiOver4;
        protected float NearPlane = 0.25f;
        protected float FarPlane = 10000;

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

        public virtual void CreateLookAt()
        {
            CameraTarget = World.Translation + CameraDirection;
            view = Matrix.CreateLookAt(World.Translation, CameraTarget, CameraUpDirection);
        }

        public virtual void CreateLookAt(Vector3 position)
        {
            view = Matrix.CreateLookAt(World.Translation, position, CameraUpDirection);
        }

        public virtual void MoveCamera()//needs fixing
        {
            CameraPosition = Vector3.Transform(CameraPosition - CameraTarget,
                Matrix.CreateFromAxisAngle(new Vector3(1,0,1),MathHelper.ToRadians(90)))+CameraTarget;
            view = Matrix.CreateLookAt(CameraPosition,CameraTarget,Vector3.Up);
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
