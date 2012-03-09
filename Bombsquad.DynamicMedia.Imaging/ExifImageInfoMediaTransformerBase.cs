using System;
using System.IO;
using System.Web;
using System.Windows.Media.Imaging;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.DynamicMedia.Contracts.Transformation;
using Bombsquad.DynamicMedia.Implementations;
using Bombsquad.DynamicMedia.Implementations.Transformation;
using Bombsquad.Exif;
using Bombsquad.Exif.Models;

namespace Bombsquad.DynamicMedia.Imaging
{
    public abstract class ExifImageInfoMediaTransformerBase : TransformerFactoryBase
    {
        private readonly ExifParser _exifParser;

        protected ExifImageInfoMediaTransformerBase()
        {
            _exifParser = new ExifParser();
        }

        protected override bool IsValidFilePath(string absolutePath)
        {
            return true;
        }

        protected override bool CanHandleFormat(IFormatInfo format)
        {
            return string.Equals(format.Extension, ".jpg", StringComparison.InvariantCultureIgnoreCase);
        }

        protected override MediaTransformResult TransformStream(HttpRequestBase request, Stream stream, out Stream transformedStream)
        {
            var bitmap = BitmapFrame.Create(stream);
            var exifData = _exifParser.Parse(bitmap);
            transformedStream = new MemoryStream();
            Serialize(transformedStream, exifData);
            transformedStream.Seek(0, SeekOrigin.Begin);
            return MediaTransformResult.Success;
        }

        protected abstract void Serialize(Stream output, ExifData exifData);
    }
}