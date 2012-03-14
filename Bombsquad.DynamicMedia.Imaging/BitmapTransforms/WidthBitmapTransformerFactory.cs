using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bombsquad.DynamicMedia.Imaging.BitmapTransforms
{
    class WidthBitmapTransformerFactory : IBitmapTransformFactory
    {
        public string OptionKey
        {
            get { return "width"; }
        }

        public bool TryGetBitmapTransform(string optionValue, IEnumerable<KeyValuePair<string, string>> settings, out Func<BitmapSource, BitmapSource> bitmapTransformerFunc)
        {
            int width;
            if (!int.TryParse(optionValue, out width))
            {
                bitmapTransformerFunc = null;
                return false;
            }

            bitmapTransformerFunc = (sourceBitmap =>
            {
                var scale = width / (double)sourceBitmap.PixelWidth;
                return new TransformedBitmap(sourceBitmap, new ScaleTransform(scale, scale));
            });

            return true;
        }
    }
}