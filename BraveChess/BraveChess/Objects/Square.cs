using System.Collections.Generic;
using BraveChess.Base;
using BraveChess.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using BraveChess.Engines;

namespace BraveChess.Objects
{
    public enum Ranks
    {
        One = 0,
        Two = 1,
        Three = 2,
        Four = 3,
        Five = 4,
        Six = 5,
        Seven = 6,
        Eight = 7
    }

    public enum Files
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        E = 4,
        F = 5,
        G = 6,
        H = 7
    }

    public class Square : GameObject3D
    {
        public Files File { get; set; }
        public Ranks Rank { get; set; }
        public Model Model3D { get; set; }
        public Matrix[] BoneTransforms { get; set; }
        public string Name { get {return string.Format("{0}{1}",File.ToString(), Rank);}}
        public bool IsSelected { get; set; }
        public bool IsHover { get; set; }
        public bool IsMoveOption { get; set; }
        readonly string _asset;


        public Square(string id,Vector3 position, Files f, Ranks r) 
            : base(id, position)
        {
            File = f;
            Rank = r;
            
        }

        public Square(string id, Vector3 position, int f, int r, bool color)
            : base(id, position)
        {
            File = (Files)f;
            Rank = (Ranks)r;
            _asset = color ? "WhiteSquare" : "BlackSquare";
        }

        public override void LoadContent(ContentManager content)
        {
            if (!string.IsNullOrEmpty(_asset))
            {
                Model3D = content.Load<Model>("Models\\" + _asset);

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
        }

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

                    if (IsHover)  // Add the yellow colour.
                    {
                        effect.FogEnabled = true;
                        effect.FogColor = new Vector3(50.0f, 50.0f, 0.0f);
                    }
                    else if (IsSelected)
                    {
                        effect.FogEnabled = true;
                        effect.FogColor = new Vector3(50.0f, 55.0f, 0.0f);
                    }
                    else if (IsMoveOption)
                    {
                        effect.FogEnabled = true;
                        effect.FogColor = Color.CornflowerBlue.ToVector3();
                    }
                    else
                        effect.FogEnabled = false;

                }
                mesh.Draw();
            }


            base.Draw(camera);
        }//End Method

        public override void Update(GameTime gametime)
        {
            //DebugEngine.AddBoundingBox(AABB, Color.Yellow);

            base.Update(gametime);
        }//end of method

        public string ToAlgebraic()
        {
            string str = "";
            str += (char)(File + 97);
            str += ((int)Rank + 1).ToString();

            return str;
        }

        public void UpdateBoundingBox(Matrix transform)
        {
            AABB = Helper.TransformBoundingBox(AABB, transform);
        }
    }
}
