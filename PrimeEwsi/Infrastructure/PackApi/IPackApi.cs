using System.IO;
using PrimeEwsi.Models;

namespace PrimeEwsi
{
    public interface IPackApi
    {
        void AddPackToHistory(PackModel packModel);
        FileInfo CreatePackFile(PackModel packModel);
        void UpdateVersion(string component);
    }
}