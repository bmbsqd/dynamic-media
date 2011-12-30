using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bombsquad.DynamicMedia.Imaging.BitmapTransforms
{
    class ScaleBitmapTransformFactory : IBitmapTransformFactory
    {
        public string OptionKey
        {
            get { return "scale"; }
        }

        public bool TryGetBitmapTransform(string optionValue, IEnumerable<KeyValuePair<string, string>> settings, out Func<BitmapSource, BitmapSource> bitmapTransformerFunc)
        {
            double scale;
            if (TryParseDouble(optionValue, out scale))
            {
                bitmapTransformerFunc = sourceBitmap => new TransformedBitmap(sourceBitmap, new ScaleTransform(scale, scale));
                return true;
            }

            bitmapTransformerFunc = null;
            return false;
        }

        private bool TryParseDouble(string s, out double value)
        {
            if (string.IsNullOrEmpty(s))
            {
                value = 0;
                return false;
            }

            s = s.Replace(',', '.');
            return double.TryParse(s, NumberStyles.Any, new NumberFormatInfo { NumberDecimalSeparator = "." }, out value);
        }
    }
}
