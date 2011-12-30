using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bombsquad.DynamicMedia.Imaging.BitmapTransforms
{
    class MirrorBitmapTransformerFactory : IBitmapTransformFactory
    {
        public string OptionKey
        {
            get { return "mirror"; }
        }

        public bool TryGetBitmapTransform(string optionValue, IEnumerable<KeyValuePair<string, string>> settings, out Func<BitmapSource, BitmapSource> bitmapTransformerFunc)
        {
            if (optionValue.ToLower() == "x")
            {
                var matrixTransform = new MatrixTransform(-1, 0, 0, 1, 0, 0);
                bitmapTransformerFunc = sourceBitmap => new TransformedBitmap(sourceBitmap, matrixTransform);
                return true;
            }

            if(optionValue.ToLower() == "y")
            {
                var matrixTransform = new MatrixTransform(1, 0, 0, -1, 0, 0);
                bitmapTransformerFunc = sourceBitmap => new TransformedBitmap(sourceBitmap, matrixTransform);
                return true;
            }

            bitmapTransformerFunc = null;
            return false;
        }
    }
}