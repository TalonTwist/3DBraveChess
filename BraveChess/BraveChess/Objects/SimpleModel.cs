using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BraveChess.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using BraveChess.Engines;
using BraveChess.Helpers;

namespace BraveChess.Objects
{
    public class SimpleModel : GameObject3D
    {
        public Model Model3D { get; set; }
        public Matrix[] BoneTransforms { get; set; }
        string _asset;

        public BoundingBox AABB { get; set; }

        public SimpleModel(string id, string asset, Vector3 position)
            : base(id, position)
        {
            _asset = asset;
        }

        public override void LoadContent(ContentManager _content)
        {
            if (!string.IsNullOrEmpty(_asset))
            {
                Model3D = _content.Load<Model>("Models\\" + _asset);

                BoneTransforms = new Matrix[Model3D.Bones.Count];
                Model3D.CopyAbsoluteBoneTransformsTo(BoneTransforms);

                List<Vector3> _vertices = new List<Vector3>();

                foreach (ModelMesh mesh in Model3D.Meshes)
                {
                    //_vertices.AddRange(Helpers.ExtractVector3FromMesh(mesh, BoneTransforms));
                    _vertices.AddRange(Helpers.Helper.ExtractVector3FromMesh(mesh, BoneTransforms));
                }

                AABB = BoundingBox.CreateFromPoints(_vertices);

                UpdateBoundingBox(World);
            }

            base.LoadContent(_content);
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
            DebugEngine.AddBoundingBox(AABB, Color.Yellow);

            base.Update(gametime);
        }//end of method

        public void UpdateBoundingBox(Matrix _transform)
        {
            //AABB = Helpers.TransformBoundingBox(AABB, _transform);
        }
    }//end of class
}//end of namespace
