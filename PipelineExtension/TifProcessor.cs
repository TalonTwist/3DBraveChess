using System;
using System.Collections.Generic;

using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using System.Drawing;

// TODO: replace these with the processor input and output types.
using TInput = System.String;
using TOutput = System.String;

namespace PipelineExtension
{
    class TifProcessor
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
        [ContentImporter(".tif", ".tiff", DisplayName = "TIFF Importer", DefaultProcessor = "TextureProcessor")]
        public class TiffImporter : ContentImporter<Texture2DContent>
        {
            public override Texture2DContent Import(string filename, ContentImporterContext context)
            {
                Bitmap bitmap = Image.FromFile(filename) as Bitmap;
                var bitmapContent = new PixelBitmapContent<Microsoft.Xna.Framework.Color>(bitmap.Width, bitmap.Height);

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        System.Drawing.Color from = bitmap.GetPixel(i, j);
                        Microsoft.Xna.Framework.Color to = new Microsoft.Xna.Framework.Color(from.R, from.G, from.B, from.A);
                        bitmapContent.SetPixel(i, j, to);
                    }
                }

                return new Texture2DContent()
                {
                    Mipmaps = new MipmapChain(bitmapContent)
                };
            }
        }
    }
}
