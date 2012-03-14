using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Windows.Media.Imaging;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.DynamicMedia.Contracts.Transformation;
using Bombsquad.DynamicMedia.Imaging.BitmapTransforms;
using Bombsquad.DynamicMedia.Implementations.FormatInfo;

namespace Bombsquad.DynamicMedia.Imaging
{
    public class ImageMediaTransformerFactory : IMediaTransformerFactory
    {
        private readonly IDictionary<string, IBitmapTransformFactory> _bitmapTransformFactories;

        public ImageMediaTransformerFactory()
        {
            _bitmapTransformFactories = new Dictionary<string, IBitmapTransformFactory>(StringComparer.InvariantCultureIgnoreCase);
            AddBitmapTransformFactory(new RotateBitmapTransformFactory());
            AddBitmapTransformFactory(new ScaleBitmapTransformFactory());
            AddBitmapTransformFactory(new MirrorBitmapTransformerFactory());
            AddBitmapTransformFactory(new WidthBitmapTransformerFactory());
            AddBitmapTransformFactory(new HeightBitmapTransformerFactory());
            AddBitmapTransformFactory(new MaxWidthBitmapTransformerFactory());
            AddBitmapTransformFactory(new MaxHeightBitmapTransformerFactory());
            AddBitmapTransformFactory(new ColorBitmapTransformerFactory());
            AddBitmapTransformFactory(new CropBitmapTransformerFactory());
            AddBitmapTransformFactory(new AreaBitmapTransformerFactory());
        }

        private void AddBitmapTransformFactory(IBitmapTransformFactory factory)
        {
            _bitmapTransformFactories.Add(factory.OptionKey, factory);
        }

        public bool TryCreateTransformer(HttpRequestBase request, IFormatInfo originalFormat, out IMediaTransformer mediaTransformer)
        {
            var transforms = GetBitmapTransforms(request.QueryString).ToArray();
            if (!transforms.Any())
            {
                mediaTransformer = null;
                return false;
            }

            IFormatInfo outputFormat;
            BitmapEncoder encoder;

            if (!TryGetContentType(originalFormat, request, out outputFormat, out encoder))
            {
                mediaTransformer = null;
                return false;
            }

            mediaTransformer = new ImageMediaTransformer(transforms, encoder, outputFormat);
            return true;
        }

        private IEnumerable<Func<BitmapSource, BitmapSource>> GetBitmapTransforms(NameValueCollection requestData)
        {
            var settings = GetSettings(requestData.ToString()).ToArray();

            foreach (var setting in settings)
            {
                IBitmapTransformFactory factory;
                Func<BitmapSource, BitmapSource> transformer;
                if (_bitmapTransformFactories.TryGetValue(setting.Key, out factory) &&
                    factory.TryGetBitmapTransform(setting.Value, settings, out transformer))
                {
                    yield return transformer;
                }
            }
        }

        private IEnumerable<KeyValuePair<string,string>> GetSettings(string requestData)
        {
            var settings = requestData.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var setting in settings)
            {
                var parts = setting.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                {
                    continue;
                }

                yield return new KeyValuePair<string, string>(HttpUtility.UrlDecode(parts[0]), HttpUtility.UrlDecode(parts[1]));
            }
        } 

        public bool TryGetContentType(IFormatInfo originalFormat, HttpRequestBase request, out IFormatInfo transformedFormat, out BitmapEncoder encoder)
        {
            var format = request.QueryString["format"];
            if (string.IsNullOrEmpty(format))
            {
                format = originalFormat.Extension.Substring(1);
            }

            switch (format.ToLower())
            {
                case "jpg":
                    int quality;
                    if (!int.TryParse(request.QueryString["quality"], out quality) || quality <= 0 || quality > 100)
                    {
                        quality = 80;
                    }

                    transformedFormat = new FormatInfo { Extension = ".jpg", ContentType = "image/jpeg" };
                    encoder = new JpegBitmapEncoder { QualityLevel = quality };
                    return true;

                case "png":
                    transformedFormat = new FormatInfo { Extension = ".png", ContentType = "image/png" };
                    encoder = new PngBitmapEncoder();
                    return true;

                default:
                    transformedFormat = null;
                    encoder = null;
                    return false;
            }
        }
    }
}