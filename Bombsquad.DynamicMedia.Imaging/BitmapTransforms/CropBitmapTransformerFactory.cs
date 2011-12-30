using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;

namespace Bombsquad.DynamicMedia.Imaging.BitmapTransforms
{
    class CropBitmapTransformerFactory : IBitmapTransformFactory
    {
        public string OptionKey
        {
            get { return "crop"; }
        }

        public bool TryGetBitmapTransform(string optionValue, IEnumerable<KeyValuePair<string, string>> settings, out Func<BitmapSource, BitmapSource> bitmapTransformerFunc)
        {
            var cropOriginFunc = GetCropPointFunc(GetSetting(settings, "cropOrigin", "center,center"));
            var cropSize = GetCropSize(optionValue);

            bitmapTransformerFunc = bitmapSource =>
            {
                double widthScale = cropSize.Width / bitmapSource.PixelWidth;
                double heightScale = cropSize.Height / bitmapSource.PixelHeight;
                double scale = Math.Max(widthScale, heightScale);

                bitmapSource = new TransformedBitmap(bitmapSource, new ScaleTransform(scale, scale));

                var cropOrigin = cropOriginFunc(bitmapSource);
                var x = cropOrigin.X - (cropSize.Width / 2);
                x = Math.Max(0, x);
                x = Math.Min(x, bitmapSource.PixelWidth - cropSize.Width);

                var y = cropOrigin.Y - (cropSize.Height / 2);
                y = Math.Max(0, y);
                y = Math.Min(y, bitmapSource.PixelHeight - cropSize.Height);

                return new CroppedBitmap(bitmapSource, new Int32Rect((int)Math.Round(x), (int)Math.Round(y), (int)cropSize.Width, (int)cropSize.Height));
            };

            return true;
        }

        private Size GetCropSize(string optionValue)
        {
            var parts = optionValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var x = int.Parse(parts[0]);
            if (parts.Length == 1)
            {
                return new Size(x, x);
            }

            var y = int.Parse(parts[1]);
            return new Size(x, y);
        }

        private Func<BitmapSource, Point> GetCropPointFunc(string cropPoint)
        {
            var parts = cropPoint.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return bitmapSource => new Point(GetXValue(bitmapSource, parts[0]), GetYValue(bitmapSource, parts[1]));
        }

        private int GetXValue(BitmapSource bitmapSource, string value)
        {
            switch (value.Trim().ToLower())
            {
                case "left":
                    return 0;

                case "right":
                    return bitmapSource.PixelWidth;

                case "center":
                    return bitmapSource.PixelWidth / 2;

                default:
                    var val = int.Parse(value);
                    val = Math.Max(0, val);
                    val = Math.Min(bitmapSource.PixelWidth, val);
                    return val;
            }
        }

        private int GetYValue(BitmapSource bitmapSource, string value)
        {
            switch (value.ToLower())
            {
                case "top":
                    return 0;

                case "bottom":
                    return bitmapSource.PixelHeight;

                case "center":
                    return bitmapSource.PixelHeight / 2;

                default:
                    var val = int.Parse(value);
                    val = Math.Max(0, val);
                    val = Math.Min(bitmapSource.PixelHeight, val);
                    return val;
            }
        }


        private string GetSetting(IEnumerable<KeyValuePair<string, string>> settings, string key, string defaultValue)
        {
            var setting = settings.LastOrDefault(s => s.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            return setting.Value ?? defaultValue;
        }
    }
}