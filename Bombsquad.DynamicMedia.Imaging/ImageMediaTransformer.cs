using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows.Media.Imaging;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.DynamicMedia.Contracts.Transformation;

namespace Bombsquad.DynamicMedia.Imaging
{
    public class ImageMediaTransformer : IMediaTransformer
    {
        private readonly IEnumerable<Func<BitmapSource, BitmapSource>> _bitmapTransformChain;
        private readonly BitmapEncoder _encoder;

        public ImageMediaTransformer( IEnumerable<Func<BitmapSource, BitmapSource>> bitmapTransformChain, BitmapEncoder encoder, IFormatInfo formatInfo )
        {
            _bitmapTransformChain = bitmapTransformChain;
            _encoder = encoder;
            OutputFormat = formatInfo;
        }

        public MediaTransformResult TransformStream(HttpRequestBase request, Stream stream, out Stream transformedStream)
        {
            var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
            BitmapSource bitmapSource = decoder.Frames[0];
            bitmapSource = _bitmapTransformChain.Aggregate(bitmapSource, (current, transform) => transform(current));

            _encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            transformedStream = new MemoryStream();
            _encoder.Save(transformedStream);
            transformedStream.Seek(0, SeekOrigin.Begin);

            return MediaTransformResult.Success;
        }

        public IFormatInfo OutputFormat { get; private set; }

        public string ModifyAbsolutePath(string absolutePath)
        {
            return absolutePath;
        }
    }
}