using System.Collections.Generic;
using BraveChess.Base;
using BraveChess.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BraveChess.Objects
{
    public class Piece : GameObject3D
    {
       public enum Colour
        {
           Black = 0,
           White = 1,
           None
        }

       public enum PieceType
        {
            Rook,
            Knight,
            Bishop,
            Queen,
            King,
            Pawn,
           None
        }

        public Model Model3D { get; set; }
        public Matrix[] BoneTransforms { get; set; }
        public Colour ColorType { get; set; }
        public PieceType Piece_Type { get; set; }
        

        readonly string _asset;

        public Piece(string id, string asset, Vector3 position, int colorT, PieceType pieceT)
            : base(id, position)
        {
            _asset = asset;
            ColorType = (Colour)colorT;
            Piece_Type = pieceT;
        }

        public Piece(string id, string asset, Vector3 position)
            : base(id, position)
        {
            _asset = asset;
        }

        public override void LoadContent(ContentManager content)
        {
            if (!string.IsNullOrEmpty(_asset))
            {
                Model3D = content.Load<Model>("Models\\New Pieces\\" + _asset);

                BoneTransforms = new Matrix[Model3D.Bones.Count];
                Model3D.CopyAbsoluteBoneTransformsTo(BoneTransforms);

                List<Vector3> vertices = new List<Vector3>();

                foreach (ModelMesh mesh in Model3D.Meshes)
                {
                    vertices.AddRange(Helper.ExtractVector3FromMesh(mesh, BoneTransforms));
                }

                AABB = BoundingBox.CreateFromPoints(vertices);

                UpdateBoundingBox(World);
            }

            base.LoadContent(content);
        }//End Method

        public override void Draw(Camera camera)
        {
            foreach (ModelMesh mesh in Model3D.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.World = BoneTransforms[mesh.ParentBone.Index] * World;

                    effect.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }

            base.Draw(camera);
        }//End Method

        public override void Update(GameTime gametime)
        {
           // DebugEngine.AddBoundingBox(AABB, Color.Yellow);

            base.Update(gametime);
        }//end of method

        public void UpdateBoundingBox(Matrix transform)
        {
            AABB = Helper.TransformBoundingBox(AABB, transform);
        }
    }//end of class
}//end of namespace
