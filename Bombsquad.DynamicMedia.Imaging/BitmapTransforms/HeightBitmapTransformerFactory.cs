using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bombsquad.DynamicMedia.Imaging.BitmapTransforms
{
    class HeightBitmapTransformerFactory : IBitmapTransformFactory
    {
        public string OptionKey
        {
            get { return "height"; }
        }

        public bool TryGetBitmapTransform(string optionValue, IEnumerable<KeyValuePair<string, string>> settings, out Func<BitmapSource, BitmapSource> bitmapTransformerFunc)
        {
            int height;
            if (!int.TryParse(optionValue, out height))
            {
                bitmapTransformerFunc = null;
                return false;
            }

            bitmapTransformerFunc = (sourceBitmap =>
            {
                var scale = height / (double)sourceBitmap.PixelHeight;
                return new TransformedBitmap(sourceBitmap, new ScaleTransform(scale, scale));
            });

            return true;
        }
    }
}