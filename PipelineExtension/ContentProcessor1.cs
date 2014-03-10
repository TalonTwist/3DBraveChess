using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

// TODO: replace these with the processor input and output types.
using TInput = System.String;
using TOutput = System.String;

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
    [ContentProcessor(DisplayName = "Waypoint Processor")]
    public class WaypointProcessor : ModelProcessor
    {
        private bool _preserveHeight;

        [DisplayName("Preserve Point Y Value")]
        [DefaultValue(true)]
        [Description("Saves the Original Y Value of each Point")]
        public bool PreservePointHeight
        {
            get { return _preserveHeight; }
            set { _preserveHeight = value; }
        }

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            ModelContent model = base.Process(input, context);

            //results will be stored in this collection
            List<Vector3> points = new List<Vector3>();

            //loop throught each mesh at the center of each of its bounding spheres
            foreach (ModelMeshContent mesh in model.Meshes)
            {
                //we will need to transform the center by the meshes parent bone atrix
                //if we don't they will all be at the same position
                Matrix transform;

                if (mesh.ParentBone.Transform != null)
                    transform = mesh.ParentBone.Transform;
                else
                    transform = Matrix.Identity;

                var p = Vector3.Transform(mesh.BoundingSphere.Center, mesh.ParentBone.Transform);

                //using the property above we can make decisions
                if (PreservePointHeight)
                    points.Add(p);
                else
                    points.Add(new Vector3(p.X,0,p.Z));

            }

            //we always store the additional data in the Tag property of the object
            model.Tag = points;

            return model;
        }

    }
}