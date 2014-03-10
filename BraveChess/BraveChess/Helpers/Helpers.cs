using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BraveChess.Helpers
{
    public static class Helper
    {
        public static GraphicsDevice GraphicsDevice { get; set; }

        public static List<Vector3> ExtractVector3FromMesh(ModelMesh mesh, Matrix[] boneTransforms)
        {
            //to store all the extracted vertice positions, will be returned
            List<Vector3> vertices = new List<Vector3>();

            //the possible transform of the current mesh bone
            Matrix _transform;
            //if it has a bone then gets its matric transform
            if (mesh.ParentBone != null)
            {
                _transform = boneTransforms[mesh.ParentBone.Index];
            }
            else
            {
                //otherwise set it an Identity matrix(Scale = 1)
                _transform = Matrix.Identity;
            }

            //loop through each mesh part
            foreach (ModelMeshPart part in mesh.MeshParts)
            {
                //create an array to match the numbers of vertices in the part
                var meshPartVertices = new Vector3[part.NumVertices];

                //use GetData to extract the positions
                part.VertexBuffer.GetData(meshPartVertices);

                //transform the vertices using the bone transform from above
                Vector3.Transform(meshPartVertices, ref _transform, meshPartVertices);
                
                //add to the list of vector 3, repeat
                vertices.AddRange(meshPartVertices);
            }

            return vertices;
        }//end of method

        public static BoundingBox TransformBoundingBox(BoundingBox origBox, Matrix matrix)
        {
            Vector3 origCorner1 = origBox.Min;
            Vector3 origCorner2 = origBox.Max;

            Vector3 transCorner1 = Vector3.Transform(origCorner1, matrix);
            Vector3 transCorner2 = Vector3.Transform(origCorner2, matrix);

            return new BoundingBox(transCorner1, transCorner2);
        }

    }//end of class
}//end of namespace
