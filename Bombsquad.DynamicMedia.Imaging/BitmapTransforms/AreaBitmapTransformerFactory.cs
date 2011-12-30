using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Bombsquad.DynamicMedia.Imaging.BitmapTransforms
{
    class AreaBitmapTransformerFactory : IBitmapTransformFactory
    {
        public string OptionKey
        {
            get { return "area"; }
        }

        public bool TryGetBitmapTransform(string optionValue, IEnumerable<KeyValuePair<string, string>> settings, out Func<BitmapSource, BitmapSource> bitmapTransformerFunc)
        {
            var parts = optionValue.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 4)
            {
                bitmapTransformerFunc = null;
                return false;
            }

            var x = int.Parse(parts[0]);
            var y = int.Parse(parts[1]);
            var width = int.Parse(parts[2]);
            var height = int.Parse(parts[3]);

            bitmapTransformerFunc = (bitmapSource =>
            {
                var bitmap = new CroppedBitmap(bitmapSource, new Int32Rect(x, y, width, height));
                return bitmap;
            });
            return true;
        }
    }
}