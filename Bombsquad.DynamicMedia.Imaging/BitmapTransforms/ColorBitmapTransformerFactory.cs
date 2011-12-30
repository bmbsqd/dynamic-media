using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bombsquad.DynamicMedia.Imaging.BitmapTransforms
{
    class ColorBitmapTransformerFactory : IBitmapTransformFactory
    {
        public string OptionKey
        {
            get { return "color"; }
        }

        public bool TryGetBitmapTransform(string optionValue, IEnumerable<KeyValuePair<string, string>> settings, out Func<BitmapSource, BitmapSource> bitmapTransformerFunc)
        {
            switch (optionValue.ToLower())
            {
                case "monochrome":
                    bitmapTransformerFunc =
                       sourceBitmap => new FormatConvertedBitmap(sourceBitmap, PixelFormats.BlackWhite, BitmapPalettes.BlackAndWhite, 0);
                    return true;

                case "grayscale":
                    bitmapTransformerFunc =
                        sourceBitmap => new FormatConvertedBitmap(sourceBitmap, PixelFormats.Gray32Float, BitmapPalettes.Gray256, 0);
                    return true;

                case "web256":
                    bitmapTransformerFunc =
                        sourceBitmap => new FormatConvertedBitmap(sourceBitmap, PixelFormats.Indexed8, BitmapPalettes.WebPalette, 0);
                    return true;
            }

            bitmapTransformerFunc = null;
            return false;
        }
    }
}