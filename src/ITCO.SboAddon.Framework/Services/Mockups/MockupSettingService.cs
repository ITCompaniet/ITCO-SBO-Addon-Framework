namespace ITCO.SboAddon.Framework.Services.Mockups
{
    using System.Collections.Generic;
    /// <summary>
    /// MockupSettingService
    /// </summary>
    public class MockupSettingService : ISettingService
    {
        private readonly IDictionary<string, object> _settingsRepository;
        private readonly string _currentUser;
        /// <summary>
        /// MockupSettingService
        /// </summary>
        /// <param name="settingsRepository"></param>
        /// <param name="currentUser"></param>
        public MockupSettingService(IDictionary<string, object> settingsRepository, string currentUser)
        {
            _settingsRepository = settingsRepository;
            _currentUser = currentUser;
        }
        /// <summary>
        /// Initialize Settigns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        public void InitSetting<T>(string key, string name, T defaultValue = default(T))
        {
            if (!_settingsRepository.ContainsKey(key))
                _settingsRepository.Add(key, defaultValue);
        }
        /// <summary>
        /// Get Current User Setting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <param name="askIfNotFound"></param>
        /// <returns></returns>
        public T GetCurrentUserSettingByKey<T>(string key, T defaultValue = default(T), bool askIfNotFound = false)
        {
            key = $"{key}[{_currentUser}]";

            if (_settingsRepository.ContainsKey(key))
                return (T) _settingsRepository[key];

            return defaultValue;
        }
        /// <summary>
        /// Save Current User Setting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SaveCurrentUserSetting<T>(string key, T value)
        {
            SaveSetting($"{key}[{_currentUser}]", value);
        }
        /// <summary>
        /// Get Setting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <param name="userCode"></param>
        /// <param name="askIfNotFound"></param>
        /// <returns></returns>
        public T GetSettingByKey<T>(string key, T defaultValue = default(T), string userCode = null, bool askIfNotFound = false)
        {
            if (_settingsRepository.ContainsKey(key))
                return (T) _settingsRepository[key];

            return defaultValue;
        }
        /// <summary>
        /// Save Setting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="userCode"></param>
        /// <param name="name"></param>
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
