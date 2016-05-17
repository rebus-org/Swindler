using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Swindler
{
    /// <summary>
    /// Awesome hack, found here: http://stackoverflow.com/a/6151688/6560
    /// </summary>
    public static class AppConfig
    {
        /// <summary>
        /// Sets the configuration file found at <paramref name="path"/> as the current App.config and resets <see cref="ConfigurationManager"/> so that it will load the configuration from it.
        /// If <paramref name="path"/> is not rooted, it is assumed to reside in the base directory of the current AppDomain.
        /// </summary>
        public static IDisposable Use(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Could not find configuration file '{path}'");
            }

            return new ChangedAppConfigContext(path);
        }

        class ChangedAppConfigContext : IDisposable
        {
            readonly string _oldConfig = AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE").ToString();

            bool _disposedValue;

            public ChangedAppConfigContext(string path)
            {
                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", path);
                ResetConfigMechanism();
            }

            public void Dispose()
            {
                if (!_disposedValue)
                {
                    AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", _oldConfig);
                    ResetConfigMechanism();
                    _disposedValue = true;
                }
                GC.SuppressFinalize(this);
            }

            static void ResetConfigMechanism()
            {
                try
                {
                    SetPrivateFields();
                }
                catch (Exception exception)
                {
                    throw new SwindlerException("Could not change the configuration context of the current AppDomain", exception);
                }
            }

            static void SetPrivateFields()
            {
                const BindingFlags privateStatic = BindingFlags.NonPublic | BindingFlags.Static;

                var initStateField = typeof(ConfigurationManager)
                    .GetField("s_initState", privateStatic);

                if (initStateField == null)
                {
                    throw new SwindlerException("Could not find private static 's_initState' field on the ConfigurationManager");
                }

                var configSystemField = typeof(ConfigurationManager)
                    .GetField("s_configSystem", privateStatic);

                if (configSystemField == null)
                {
                    throw new SwindlerException("Could not find private static 's_configSystem' field on the ConfigurationManager");
                }

                var systemConfigurationAssembly = typeof(ConfigurationManager).Assembly;
                var clientConfigPathsType = systemConfigurationAssembly.GetTypes()
                    .FirstOrDefault(x => x.FullName == "System.Configuration.ClientConfigPaths");

                if (clientConfigPathsType == null)
                {
                    throw new SwindlerException($"Could not find the 'System.Configuration.ClientConfigPaths' type from assembly {systemConfigurationAssembly}");
                }

                var currentField = clientConfigPathsType
                    .GetField("s_current", privateStatic);

                if (currentField == null)
                {
                    throw new SwindlerException("Could not find private status 's_current' field on System.Configuration.ClientConfigPaths");
                }

                initStateField.SetValue(null, 0);
                configSystemField.SetValue(null, null);
                currentField.SetValue(null, null);
            }
        }
    }
}
