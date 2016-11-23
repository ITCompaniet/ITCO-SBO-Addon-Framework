namespace ITCO.SboAddon.Framework.Services.Mockups
{
    using System.Collections.Generic;

    public class MockupSettingService : ISettingService
    {
        private readonly IDictionary<string, object> _settingsRepository;
        private readonly string _currencyUser;

        public MockupSettingService(IDictionary<string, object> settingsRepository, string currencyUser)
        {
            _settingsRepository = settingsRepository;
            _currencyUser = currencyUser;
        }

        public void InitSetting<T>(string key, string name, T defaultValue = default(T))
        {
            if (!_settingsRepository.ContainsKey(key))
                _settingsRepository.Add(key, defaultValue);
        }

        public T GetCurrentUserSettingByKey<T>(string key, T defaultValue = default(T), bool askIfNotFound = false)
        {
            key = $"{key}[{_currencyUser}]";

            if (_settingsRepository.ContainsKey(key))
                return (T) _settingsRepository[key];

            return defaultValue;
        }

        public void SaveCurrentUserSetting<T>(string key, T value)
        {
            SaveSetting($"{key}[{_currencyUser}]", value);
        }

        public T GetSettingByKey<T>(string key, T defaultValue = default(T), string userCode = null, bool askIfNotFound = false)
        {
            if (_settingsRepository.ContainsKey(key))
                return (T) _settingsRepository[key];

            return defaultValue;
        }

        public void SaveSetting<T>(string key, T value = default(T), string userCode = null, string name = null)
        {
            if (userCode != null)
                key = $"{key}[{userCode}]";

            if (_settingsRepository.ContainsKey(key))
                _settingsRepository[key] = value;
            else
                _settingsRepository.Add(key, value);
        }
    }
}
