using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Bombsquad.DynamicMedia.Imaging
{
    public interface IBitmapTransformFactory
    {
        string OptionKey { get; }
        bool TryGetBitmapTransform(string optionValue, IEnumerable<KeyValuePair<string, string>> settings, out Func<BitmapSource, BitmapSource> bitmapTransformerFunc);
    }
}
