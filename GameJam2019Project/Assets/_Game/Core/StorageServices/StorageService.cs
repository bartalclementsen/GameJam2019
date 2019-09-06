using System;
using UnityEngine;

namespace Core.Mediators
{
    public class StorageService : IStorageService
    {
        public void Set(string key, string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }

        public string Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return PlayerPrefs.GetString(key);
        }

        public bool HasKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return PlayerPrefs.HasKey(key);
        }

        public void Delete(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            PlayerPrefs.DeleteKey(key);
        }

        public void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
