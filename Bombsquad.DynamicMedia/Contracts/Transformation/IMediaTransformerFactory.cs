using System;
using System.Web;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Contracts.Transformation
{
    public interface IMediaTransformerFactory
    {
        bool TryCreateTransformer(HttpRequestBase request, IFormatInfo originalFormat, IFormatInfoProvider formatInfoProvider, out IMediaTransformer mediaTransformer);
    }
}