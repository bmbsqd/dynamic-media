using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bombsquad.DynamicMedia.Imaging.BitmapTransforms
{
    class MaxWidthBitmapTransformerFactory : IBitmapTransformFactory
    {
        public string OptionKey
        {
            get { return "max-width"; }
        }

        public bool TryGetBitmapTransform(string optionValue, IEnumerable<KeyValuePair<string, string>> settings, out Func<BitmapSource, BitmapSource> bitmapTransformerFunc)
        {
            int maxWidth;
            if(!int.TryParse(optionValue, out maxWidth))
            {
                bitmapTransformerFunc = null;
                return false;
            }

            bitmapTransformerFunc = (sourceBitmap =>
            {
                if (sourceBitmap.PixelWidth <= maxWidth)
                {
                    return sourceBitmap;
                }
                
                var scale = maxWidth / (double)sourceBitmap.PixelWidth;
                return new TransformedBitmap(sourceBitmap, new ScaleTransform(scale, scale));
            });
            return true;
        }
    }
}