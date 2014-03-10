using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

// TODO: replace these with the processor input and output types.
using TInput = System.String;
using TOutput = System.String;
using Common;

namespace PipelineExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "Basic Model Processor")]
    public class BasicProcessor : ModelProcessor
    {
        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _indices = new List<int>();

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            ModelContent model = base.Process(input, context);

            FindVertices(input);

            ModelTag tag = new ModelTag()
            {
                Vertices = _vertices,
                Indices = _indices
            };

            model.Tag = tag;

            return model;
        }

        void FindVertices(NodeContent node)
        {
            //Is this node a mesh?
            MeshContent mesh = node as MeshContent;

            if (mesh != null)
            {
                //Look up the absolute transform of the mesh
                Matrix absoluteTransform = mesh.AbsoluteTransform;

                //Loop over all the pieces of geometry in the mesh
                foreach (GeometryContent geometry in mesh.Geometry)
                {
                    //Loop over all the indices in this piece of geometry
                    //Every group of three indices represents one triangle
                    foreach(int index in geometry.Indices)
                    {
                        //Look up the position of this vertex
                        Vector3 vertex = geometry.Vertices.Positions[index];

                        //Transform from local into world space
                        vertex = Vector3.Transform(vertex, absoluteTransform);

                        //Store this vertex
                        _vertices.Add(vertex);
                        _indices.Add(index);
                    }
                }

            }

            //Recursively scan over the children of this node
            foreach (NodeContent child in node.Children)
            {
                FindVertices(child);
            }

        }//End of FV
    }
}