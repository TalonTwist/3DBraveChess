using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

// TODO: replace these with the processor input and output types.
using TInput = System.String;
using TOutput = System.String;
using System.ComponentModel;
using System.IO;
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
    [ContentProcessor(DisplayName = "HeightMap Processor")]
    public class HeightMapProcessor : ContentProcessor<Texture2DContent, ModelContent>
    {
        private float _terrainBumpiness = 640f;
        [DisplayName("Terrain Bumpiness")]
        [DefaultValue(640f)]
        [Description("Scale of the terrain geometry height.")]
        public float TerrainBumpiness
        {
            get { return _terrainBumpiness; }
            set { _terrainBumpiness = value; }
        }

        private float _terrainScale = 30f;
        [DisplayName("Terrain Scale")]
        [DefaultValue(30f)]
        [Description("Scale of the terrain geometry width and length.")]
        public float TerrainScale
        {
            get { return _terrainScale; }
            set { _terrainScale = value; }
        }

        private float _texCoordScale = 0.1f;
        [DisplayName("Texture Coordinate Scale")]
        [DefaultValue(0.1f)]
        [Description("Terrain texture tiling density.")]
        public float TexCoordScale
        {
            get { return _texCoordScale; }
            set { _texCoordScale = value; }
        }

        private string _terrainTextureFilename = "rocks.bmp";
        [DisplayName("Terrain Texture")]
        [DefaultValue("rocks.bmp")]
        [Description("the name of the terrain texture. Must be in Textures Folder")]
        public string TerrainTextureFilename
        {
            get { return _terrainTextureFilename; }
            set { _terrainTextureFilename = value; }
        }

        public override ModelContent Process(Texture2DContent input, ContentProcessorContext context)
        {
            MeshBuilder builder = MeshBuilder.StartMesh(input.Name);

            input.ConvertBitmapType(typeof(PixelBitmapContent<float>));

            PixelBitmapContent<float> heightfield;
            heightfield = (PixelBitmapContent<float>)input.Mipmaps[0];


            for (int y = 0; y < heightfield.Height; y++)
            {
                for (int x = 0; x < heightfield.Width; x++)
                {
                    Vector3 position;

                    //position the vertices so that the heightfield is centered
                    //around x=0, z=0
                    position.X = _terrainScale * (x - ((heightfield.Width - 1) / 2.0f));
                    position.Z = _terrainScale * (y - ((heightfield.Height - 1) / 2.0f));

                    position.Y = (heightfield.GetPixel(x, y) - 1) * _terrainBumpiness;
                    //inserts a vertex position into the vertex channel of the mesh
                    builder.CreatePosition(position);
                }
            }//End of For Loop

            BasicMaterialContent material = new BasicMaterialContent();
            material.SpecularColor = new Vector3(.4f, .4f, .4f);

            string directory = Path.GetDirectoryName(input.Identity.SourceFilename);
            string textureFolder = Directory.GetParent(directory).FullName + "\\Textures\\";
            string texture = Path.Combine(textureFolder, _terrainTextureFilename);

            material.Texture = new ExternalReference<TextureContent>(texture);

            builder.SetMaterial(material);

            //Create a vertex channel for holding texture coordinates
            int texCoordId = builder.CreateVertexChannel<Vector2>(VertexChannelNames.TextureCoordinate(0));

            //Create the individual triangles that make up our terrain
            for (int y = 0; y < heightfield.Height - 1; y++)
            {
                for (int x = 0; x < heightfield.Width - 1; x++)
                {
                    AddVertex(builder, texCoordId, heightfield.Width, x, y);
                    AddVertex(builder, texCoordId, heightfield.Width, x + 1, y);
                    AddVertex(builder, texCoordId, heightfield.Width, x + 1, y + 1);

                    AddVertex(builder, texCoordId, heightfield.Width, x, y);
                    AddVertex(builder, texCoordId, heightfield.Width, x + 1, y + 1);
                    AddVertex(builder, texCoordId, heightfield.Width, x, y + 1);
                }
            }//End of for loop

            MeshContent terrainMesh = builder.FinishMesh();
            ModelContent model = context.Convert<MeshContent, ModelContent>(terrainMesh, "ModelProcessor");

            model.Tag = new HeightMap(CreateHeightFieldFromFloats(heightfield, _terrainBumpiness), _terrainScale);

            return model;

        }//End of public override

        void AddVertex(MeshBuilder builder, int texCoordId, int w, int x, int y)
        {
            builder.SetVertexChannelData(texCoordId, new Vector2(x, y) * _texCoordScale);

            builder.AddTriangleVertex(x + y * w);
        }//End of Method

        float[,] CreateHeightFieldFromFloats(PixelBitmapContent<float> bitmap, float bmpiness)
        {
            float[,] heights = new float[bitmap.Width, bitmap.Height];

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    //the pixels will vary from 0(black) to 1 (White)
                    //by subtracting 1, out heights vary from -1 to 0, which we then
                    //multiply by the "bumpiness" to get our final height
                    heights[x, y] = (bitmap.GetPixel(x, y) - 1) * _terrainBumpiness;
                }
            }

            return heights;
        }

    }
}