using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bombsquad.DynamicMedia.Imaging.BitmapTransforms
{
    class MaxHeightBitmapTransformerFactory : IBitmapTransformFactory
    {
        public string OptionKey
        {
            get { return "max-height"; }
        }

        public bool TryGetBitmapTransform(string optionValue, IEnumerable<KeyValuePair<string, string>> settings, out Func<BitmapSource, BitmapSource> bitmapTransformerFunc)
        {
            int maxHeight;
            if (!int.TryParse(optionValue, out maxHeight))
            {
                bitmapTransformerFunc = null;
                return false;
            }

            bitmapTransformerFunc = (sourceBitmap =>
            {
                if (sourceBitmap.PixelHeight <= maxHeight)
                {
                    return sourceBitmap;
                }

                var scale = maxHeight / (double)sourceBitmap.PixelHeight;
                return new TransformedBitmap(sourceBitmap, new ScaleTransform(scale, scale));
            });
            return true;
        }
    }
}