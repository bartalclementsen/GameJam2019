using System;

namespace Core.Mediators
{
    public interface IStorageService
    {
        void Delete(string key);

        void DeleteAll();

        string Get(string key);

        bool HasKey(string key);

        void Set(string key, string value);
    }
}
