using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BraveChess.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using BraveChess.Engines;

namespace BraveChess.Objects
{
    public enum Rank
    {
        one = 0,
        two = 1,
        three = 2,
        four = 3,
        five = 4,
        six = 5,
        seven = 6,
        eight = 7
    }

    public enum File
    {
        a = 0,
        b = 1,
        c = 2,
        d = 3,
        e = 4,
        f = 5,
        g = 6,
        h = 7
    }

    public class Square : GameObject3D
    {
        public File file { get; set; }
        public Rank rank { get; set; }
        public Model Model3D { get; set; }
        public Matrix[] BoneTransforms { get; set; }
        public string Name { get {return string.Format("{0}{1}",file.ToString(), rank);}}
        public bool IsSelected { get; set; }
        public bool IsHover { get; set; }
        public bool IsMoveOption { get; set; }
        string _asset;


        
        //public Square(string id,Vector3 position)
        //    :base(id,position)
        //{ }

        public Square(string id,Vector3 position, File f, Rank r) 
            : base(id, position)
        {
            file = f;
            rank = r;
            
        }

        public Square(string id, Vector3 position, int f, int r, bool color)
            : base(id, position)
        {
            file = (File)f;
            rank = (Rank)r;
            if (color)
                _asset = "WhiteSquare";
            else
                _asset = "BlackSquare";
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
                    _vertices.AddRange(Helpers.ExtractVector3FromMesh(mesh, BoneTransforms));
                }
            }
            base.LoadContent(_content);
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

        public string ToAlgebraic()
        {
            string str = "";
            str += (char)(file + 97);
            str += (rank + 1).ToString();

            return str;
        }
    }
}
