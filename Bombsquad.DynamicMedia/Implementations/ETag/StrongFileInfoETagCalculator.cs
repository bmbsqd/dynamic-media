using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Bombsquad.DynamicMedia.Contracts.ETag;

namespace Bombsquad.DynamicMedia.Implementations.ETag
{
    public class StrongFileInfoETagCalculator : IFileInfoETagCalculator
    {
        private readonly HashAlgorithm m_hashAlgorithm;

        public StrongFileInfoETagCalculator(HashAlgorithm hashAlgorithm)
        {
            m_hashAlgorithm = hashAlgorithm;
        }

        public StrongFileInfoETagCalculator()
        {
            m_hashAlgorithm = new SHA1Managed();
        }

        public string CalculateETag(FileInfo file)
        {
            using( var stream = file.Open( FileMode.Open, FileAccess.Read, FileShare.Read ) )
            {
                var hash = m_hashAlgorithm.ComputeHash(stream);
                return ETagUtil.CreateStrongETag(new string(hash.SelectMany(b => b.ToString("x2").ToLower()).ToArray()));
            }
        }
    }
}