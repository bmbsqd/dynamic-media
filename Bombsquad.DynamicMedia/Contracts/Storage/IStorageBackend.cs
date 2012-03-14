namespace Bombsquad.DynamicMedia.Contracts.Storage
{
    public interface IStorageBackend
    {
        bool TryGetStorageFile(string path, out IStorageFile storageFile);
    }
}