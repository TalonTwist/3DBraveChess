#region File Description
//-----------------------------------------------------------------------------
// DebugShapeRenderer.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using BraveChess.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using BraveChess.Base;

namespace BraveChess.Engines
{
    public class DebugEngine
	{
		class DebugShape
		{
			public VertexPositionColor[] Vertices;
			public int LineCount;
			public float Lifetime;
		}
        
		private static readonly List<DebugShape> CachedShapes = new List<DebugShape>();
		private static readonly List<DebugShape> ActiveShapes = new List<DebugShape>();

        private static VertexPositionColor[] _verts = new VertexPositionColor[64];

		private static BasicEffect _effect;

		private static readonly Vector3[] Corners = new Vector3[8];

        private const int SphereResolution = 30;
        private const int SphereLineCount = (SphereResolution + 1) * 3;
        private static Vector3[] _unitSphere;

        public void Initialize()
        {
            InitializeSphere();
        }

        public void LoadContent(ContentManager content)
        {
            _effect = new BasicEffect(Helper.GraphicsDevice)
            {
                VertexColorEnabled = true,
                TextureEnabled = false,
                DiffuseColor = Vector3.One,
                World = Matrix.Identity
            };
        }

        public static void Clear()
        {
            ActiveShapes.Clear();
            CachedShapes.Clear();
        }

		public static void AddLine(Vector3 a, Vector3 b, Color color)
		{
			AddLine(a, b, color, 0f);
		}

		public static void AddLine(Vector3 a, Vector3 b, Color color, float life)
        {
			DebugShape shape = GetShapeForLines(1, life);

			shape.Vertices[0] = new VertexPositionColor(a, color);
			shape.Vertices[1] = new VertexPositionColor(b, color);
		}

		public static void AddTriangle(Vector3 a, Vector3 b, Vector3 c, Color color)
		{
			AddTriangle(a, b, c, color, 0f);
		}

		public static void AddTriangle(Vector3 a, Vector3 b, Vector3 c, Color color, float life)
        {
            DebugShape shape = GetShapeForLines(3, life);

			shape.Vertices[0] = new VertexPositionColor(a, color);
			shape.Vertices[1] = new VertexPositionColor(b, color);
			shape.Vertices[2] = new VertexPositionColor(b, color);
			shape.Vertices[3] = new VertexPositionColor(c, color);
			shape.Vertices[4] = new VertexPositionColor(c, color);
			shape.Vertices[5] = new VertexPositionColor(a, color);
		}

		public static void AddBoundingFrustum(BoundingFrustum frustum, Color color)
		{
			AddBoundingFrustum(frustum, color, 0f);
		}

		public static void AddBoundingFrustum(BoundingFrustum frustum, Color color, float life)
        {
            DebugShape shape = GetShapeForLines(12, life);

			frustum.GetCorners(Corners);

			shape.Vertices[0] = new VertexPositionColor(Corners[0], color);
			shape.Vertices[1] = new VertexPositionColor(Corners[1], color);
			shape.Vertices[2] = new VertexPositionColor(Corners[1], color);
			shape.Vertices[3] = new VertexPositionColor(Corners[2], color);
			shape.Vertices[4] = new VertexPositionColor(Corners[2], color);
			shape.Vertices[5] = new VertexPositionColor(Corners[3], color);
			shape.Vertices[6] = new VertexPositionColor(Corners[3], color);
			shape.Vertices[7] = new VertexPositionColor(Corners[0], color);

			shape.Vertices[8] = new VertexPositionColor(Corners[4], color);
			shape.Vertices[9] = new VertexPositionColor(Corners[5], color);
			shape.Vertices[10] = new VertexPositionColor(Corners[5], color);
			shape.Vertices[11] = new VertexPositionColor(Corners[6], color);
			shape.Vertices[12] = new VertexPositionColor(Corners[6], color);
			shape.Vertices[13] = new VertexPositionColor(Corners[7], color);
			shape.Vertices[14] = new VertexPositionColor(Corners[7], color);
			shape.Vertices[15] = new VertexPositionColor(Corners[4], color);

			shape.Vertices[16] = new VertexPositionColor(Corners[0], color);
			shape.Vertices[17] = new VertexPositionColor(Corners[4], color);
			shape.Vertices[18] = new VertexPositionColor(Corners[1], color);
			shape.Vertices[19] = new VertexPositionColor(Corners[5], color);
			shape.Vertices[20] = new VertexPositionColor(Corners[2], color);
			shape.Vertices[21] = new VertexPositionColor(Corners[6], color);
			shape.Vertices[22] = new VertexPositionColor(Corners[3], color);
			shape.Vertices[23] = new VertexPositionColor(Corners[7], color);
		}

		public static void AddBoundingBox(BoundingBox box, Color color)
		{
			AddBoundingBox(box, color, 0f);
		}

		public static void AddBoundingBox(BoundingBox box, Color color, float life)
		{
			DebugShape shape = GetShapeForLines(12, life);

			box.GetCorners(Corners);

			shape.Vertices[0] = new VertexPositionColor(Corners[0], color);
			shape.Vertices[1] = new VertexPositionColor(Corners[1], color);
			shape.Vertices[2] = new VertexPositionColor(Corners[1], color);
			shape.Vertices[3] = new VertexPositionColor(Corners[2], color);
			shape.Vertices[4] = new VertexPositionColor(Corners[2], color);
			shape.Vertices[5] = new VertexPositionColor(Corners[3], color);
			shape.Vertices[6] = new VertexPositionColor(Corners[3], color);
			shape.Vertices[7] = new VertexPositionColor(Corners[0], color);

			shape.Vertices[8] = new VertexPositionColor(Corners[4], color);
			shape.Vertices[9] = new VertexPositionColor(Corners[5], color);
			shape.Vertices[10] = new VertexPositionColor(Corners[5], color);
			shape.Vertices[11] = new VertexPositionColor(Corners[6], color);
			shape.Vertices[12] = new VertexPositionColor(Corners[6], color);
			shape.Vertices[13] = new VertexPositionColor(Corners[7], color);
			shape.Vertices[14] = new VertexPositionColor(Corners[7], color);
			shape.Vertices[15] = new VertexPositionColor(Corners[4], color);

			shape.Vertices[16] = new VertexPositionColor(Corners[0], color);
			shape.Vertices[17] = new VertexPositionColor(Corners[4], color);
			shape.Vertices[18] = new VertexPositionColor(Corners[1], color);
			shape.Vertices[19] = new VertexPositionColor(Corners[5], color);
			shape.Vertices[20] = new VertexPositionColor(Corners[2], color);
			shape.Vertices[21] = new VertexPositionColor(Corners[6], color);
			shape.Vertices[22] = new VertexPositionColor(Corners[3], color);
			shape.Vertices[23] = new VertexPositionColor(Corners[7], color);
		}

        public static void AddBoundingSphere(BoundingSphere sphere, Color color)
        {
            AddBoundingSphere(sphere, color, 0f);
        }

        public static void AddBoundingSphere(BoundingSphere sphere, Color color, float life)
        {
            DebugShape shape = GetShapeForLines(SphereLineCount, life);

            for (int i = 0; i < _unitSphere.Length; i++)
            {
                Vector3 vertPos = _unitSphere[i] * sphere.Radius + sphere.Center;

                shape.Vertices[i] = new VertexPositionColor(vertPos, color);
            }
        }

		public void Draw(Camera camera)
		{
		    if (camera == null) return;
		    _effect.View = camera.View;
		    _effect.Projection = camera.Projection;

		    int vertexCount = ActiveShapes.Sum(shape => shape.LineCount*2);

		    if (vertexCount > 0)
		    {
		        if (_verts.Length < vertexCount)
		        {
		            _verts = new VertexPositionColor[vertexCount * 2];
		        }

		        int lineCount = 0;
		        int vertIndex = 0;
		        foreach (DebugShape shape in ActiveShapes)
		        {
		            lineCount += shape.LineCount;
		            int shapeVerts = shape.LineCount * 2;
		            for (int i = 0; i < shapeVerts; i++)
		                _verts[vertIndex++] = shape.Vertices[i];
		        }

		        _effect.CurrentTechnique.Passes[0].Apply();

		        int vertexOffset = 0;
		        while (lineCount > 0)
		        {
		            int linesToDraw = Math.Min(lineCount, 65535);

		            Helper.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

		            Helper.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _verts, vertexOffset, linesToDraw);

		            vertexOffset += linesToDraw * 2;

		            lineCount -= linesToDraw;
		        }
		    }


		    bool resort = false;
		    for (int i = ActiveShapes.Count - 1; i >= 0; i--)
		    {
		        DebugShape s = ActiveShapes[i];

		        if (s.Lifetime <= 0)
		        {
		            CachedShapes.Add(s);
		            ActiveShapes.RemoveAt(i);
		            resort = true;
		        }
		    }

		    if (resort)
		        CachedShapes.Sort(CachedShapesSort);
		}
        
        private static void InitializeSphere()
        {
            // We need two vertices per line, so we can allocate our vertices
            _unitSphere = new Vector3[SphereLineCount * 2];

            // Compute our step around each circle
            const float step = MathHelper.TwoPi / SphereResolution;

            // Used to track the index into our vertex array
            int index = 0;

            // Create the loop on the XY plane first
            for (float a = 0f; a < MathHelper.TwoPi; a += step)
            {
                _unitSphere[index++] = new Vector3((float)Math.Cos(a), (float)Math.Sin(a), 0f);
                _unitSphere[index++] = new Vector3((float)Math.Cos(a + step), (float)Math.Sin(a + step), 0f);
            }

            // Next on the XZ plane
            for (float a = 0f; a < MathHelper.TwoPi; a += step)
            {
                _unitSphere[index++] = new Vector3((float)Math.Cos(a), 0f, (float)Math.Sin(a));
                _unitSphere[index++] = new Vector3((float)Math.Cos(a + step), 0f, (float)Math.Sin(a + step));
            }

            // Finally on the YZ plane
            for (float a = 0f; a < MathHelper.TwoPi; a += step)
            {
                _unitSphere[index++] = new Vector3(0f, (float)Math.Cos(a), (float)Math.Sin(a));
                _unitSphere[index++] = new Vector3(0f, (float)Math.Cos(a + step), (float)Math.Sin(a + step));
            }
        }

        private static int CachedShapesSort(DebugShape s1, DebugShape s2)
        {
            return s1.Vertices.Length.CompareTo(s2.Vertices.Length);
        }

        private static DebugShape GetShapeForLines(int lineCount, float life)
        {
            DebugShape shape = null;

            // We go through our cached list trying to find a shape that contains
            // a large enough array to hold our desired line count. If we find such
            // a shape, we move it from our cached list to our active list and break
            // out of the loop.
            int vertCount = lineCount * 2;
            for (int i = 0; i < CachedShapes.Count; i++)
            {
                if (CachedShapes[i].Vertices.Length >= vertCount)
                {
                    shape = CachedShapes[i];
                    CachedShapes.RemoveAt(i);
                    ActiveShapes.Add(shape);
                    break;
                }
            }

            // If we didn't find a shape in our cache, we create a new shape and add it
            // to the active list.
            if (shape == null)
            {
                shape = new DebugShape { Vertices = new VertexPositionColor[vertCount] };
                ActiveShapes.Add(shape);
            }

            // Set the line count and lifetime of the shape based on our parameters.
            shape.LineCount = lineCount;
            shape.Lifetime = life;

            return shape;
        }
	}
}
