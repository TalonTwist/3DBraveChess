using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Common
{
    public class HeightMap
    {
        public List<float> Heights { get; set; }

        public float Scale { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public Vector3 Position { get; set; }

        public HeightMap() { }

        public HeightMap(float[,] heights, float terrainScale)
        {
            Scale = terrainScale;
            Heights = heights.Cast<float>().ToList();

            Width = (heights.GetLength(0) - 1) * terrainScale;
            Height = (heights.GetLength(1) - 1) * terrainScale;

            float x = -(heights.GetLength(0) - 1) /2 * terrainScale;
            float y = -(heights.GetLength(1) -1 )/2 * terrainScale;

            Position = new Vector3(x,0,y);
        }

        public bool IsOnHeightmap(Vector3 position)
        {
            //first we'll figure out where on the heightmap "position" is
            Vector3 positionOnHeightmap = position - Position;

            // and then check to see if that value goes outside the bounds of the
            //heightmap.
            return (positionOnHeightmap.X > 0 &&
                positionOnHeightmap.X < Width &&
                positionOnHeightmap.Z > 0 &&
                positionOnHeightmap.Z < Height);
        }

        public float GetHeight(Vector3 position)
        {
            //the first thing we need to do is figure out where on the heightmap position is
            Vector3 positionOnHeightmap = position - Position;

            //we'll use interger division to figure out where in the "heights" array
            //positionOnHeightmap is. Remember that interger division always rounds
            // down, so that the result of these divisions is the indices of the "upper left" of the 4 corners of that cell
            int left, top;
            left = (int)positionOnHeightmap.X / (int)Scale;
            top = (int)positionOnHeightmap.Z / (int)Scale;

            //next, we'll use modulous to find out how far away we are from the upper
            //left corner of the cell. Mod will gice us a value from 0 to terrainScale,
            //which we then divide by terrainScale to normalize 0 to 1
            float xNormalized = (positionOnHeightmap.X % Scale) / Scale;
            float zNormalized = (positionOnHeightmap.Z % Scale) / Scale;

            //Now that we've calculated the indices of the corners of our cell, and
            //where we are in that cell, we'll use bilinear interpolation to caluculate our height
            //First, caluculate the heights on the bottom and top edge of our cell by interpolationg from the left and right sides
            float topHeight = MathHelper.Lerp(
                Heights[left + top],
                Heights[left + 1 + top],
                xNormalized);

            float bottomHeight = MathHelper.Lerp(
                Heights[left + top + 1],
                Heights[left + 1 + top + 1],
                xNormalized);

            //next, interpolation between those two values to calculate the height at our
            //position
            return MathHelper.Lerp(topHeight, bottomHeight, zNormalized);



        }//End of Method

    }
}
