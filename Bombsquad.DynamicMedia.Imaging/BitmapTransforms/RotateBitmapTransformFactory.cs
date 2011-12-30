using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bombsquad.DynamicMedia.Imaging.BitmapTransforms
{
    class RotateBitmapTransformFactory : IBitmapTransformFactory
    {
        public string OptionKey
        {
            get { return "rotate"; }
        }

        public bool TryGetBitmapTransform(string optionValue, IEnumerable<KeyValuePair<string, string>> settings, out Func<BitmapSource, BitmapSource> bitmapTransformerFunc)
        {
            int angle;
            if (int.TryParse(optionValue, out angle))
            {
                if (angle % 90 == 0)
                {
                    bitmapTransformerFunc = sourceBitmap => new TransformedBitmap(sourceBitmap, new RotateTransform(angle));
                    return true;
                }
            }

            bitmapTransformerFunc = null;
            return false;
        }
    }
}
