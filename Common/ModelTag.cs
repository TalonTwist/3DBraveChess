using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Common
{
    public class ModelTag
    {
        public List<Vector3> Vertices { get; set; }
        public List<int> Indices { get; set; }

        public ModelTag()
        {
            Vertices = new List<Vector3>();
            Indices = new List<int>();
        }

        public ModelTag(List<Vector3> vertices,List<int> indices)
        {
            Vertices = vertices;
            Indices = indices;
        }
    }
}
