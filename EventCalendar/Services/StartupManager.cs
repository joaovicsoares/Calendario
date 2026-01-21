using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace EventCalendar.Services
{
    /// <summary>
    /// Manages Windows startup configuration for the application.
    /// </summary>
    public static class StartupManager
    {
        private const string APP_NAME = "EventCalendar";
        private const string REGISTRY_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        /// <summary>
        /// Adds the application to Windows startup.
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public static bool EnableStartup()
        {
            try
            {
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY, true))
                {
                    if (key != null)
                    {
                        string executablePath = $"\"{Application.ExecutablePath}\" --minimized";
                        key.SetValue(APP_NAME, executablePath);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash
                Console.WriteLine($"Erro ao habilitar startup: {ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// Removes the application from Windows startup.
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public static bool DisableStartup()
        {
            try
            {
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue(APP_NAME, false);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash
                Console.WriteLine($"Erro ao desabilitar startup: {ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// Checks if the application is configured to start with Windows.
        /// </summary>
        /// <returns>True if startup is enabled, false otherwise</returns>
        public static bool IsStartupEnabled()
        {
            try
            {
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY, false))
                {
                    if (key != null)
                    {
                        object? value = key.GetValue(APP_NAME);
                        return value != null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash
                Console.WriteLine($"Erro ao verificar startup: {ex.Message}");
            }
            return false;
        }
    }
}