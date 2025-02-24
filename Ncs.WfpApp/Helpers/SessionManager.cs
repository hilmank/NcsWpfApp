using System;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace Ncs.WpfApp.Helpers
{
    public static class SessionManager
    {
        private const string RegistryPath = @"HKEY_CURRENT_USER\Software\NcsWpfApp";

        private const string AdminTokenKey = "AdminAccessToken";
        private const string CustomerTokenKey = "CustomerAccessToken";
        private const string AdminRefreshTokenKey = "AdminRefreshToken";
        private const string CustomerRefreshTokenKey = "CustomerRefreshToken";
        private const string LoginTypeKey = "LoginType"; // New key to track login type

        private static string _adminToken;
        private static string _customerToken;
        private static string _adminRefreshToken;
        private static string _customerRefreshToken;
        private static string _loginType;

        public static string AdminToken
        {
            get => _adminToken ?? LoadToken(AdminTokenKey);
            private set => _adminToken = value;
        }

        public static string CustomerToken
        {
            get => _customerToken ?? LoadToken(CustomerTokenKey);
            private set => _customerToken = value;
        }

        public static string AdminRefreshToken
        {
            get => _adminRefreshToken ?? LoadToken(AdminRefreshTokenKey);
            private set => _adminRefreshToken = value;
        }

        public static string CustomerRefreshToken
        {
            get => _customerRefreshToken ?? LoadToken(CustomerRefreshTokenKey);
            private set => _customerRefreshToken = value;
        }

        public static string LoginType
        {
            get => _loginType ?? LoadToken(LoginTypeKey);
            private set => _loginType = value;
        }

        public static bool IsAdmin => LoginType == "Admin";
        public static bool IsCustomer => LoginType == "Customer";

        public static void SetAdminToken(string accessToken, string refreshToken = null)
        {
            AdminToken = accessToken;
            LoginType = "Admin";
            SaveToken(AdminTokenKey, accessToken);
            SaveToken(LoginTypeKey, "Admin");

            if (!string.IsNullOrEmpty(refreshToken))
            {
                AdminRefreshToken = refreshToken;
                SaveToken(AdminRefreshTokenKey, refreshToken);
            }
        }

        public static void SetCustomerToken(string accessToken, string refreshToken = null)
        {
            CustomerToken = accessToken;
            LoginType = "Customer";
            SaveToken(CustomerTokenKey, accessToken);
            SaveToken(LoginTypeKey, "Customer");

            if (!string.IsNullOrEmpty(refreshToken))
            {
                CustomerRefreshToken = refreshToken;
                SaveToken(CustomerRefreshTokenKey, refreshToken);
            }
        }

        public static void ClearAdminSession()
        {
            AdminToken = null;
            AdminRefreshToken = null;

            if (LoginType == "Admin")
                LoginType = null; // Clear login type if Admin was logged in

            DeleteToken(AdminTokenKey);
            DeleteToken(AdminRefreshTokenKey);
            DeleteToken(LoginTypeKey);
        }

        public static void ClearCustomerSession()
        {
            CustomerToken = null;
            CustomerRefreshToken = null;

            if (LoginType == "Customer")
                LoginType = null; // Clear login type if Customer was logged in

            DeleteToken(CustomerTokenKey);
            DeleteToken(CustomerRefreshTokenKey);
            DeleteToken(LoginTypeKey);
        }

        private static void SaveToken(string key, string token)
        {
            if (string.IsNullOrEmpty(token)) return;

            Registry.SetValue(RegistryPath, key, token);
        }

        private static string LoadToken(string key)
        {
            object storedValue = Registry.GetValue(RegistryPath, key, null);
            if (storedValue == null) return string.Empty;

            return storedValue?.ToString() ?? string.Empty;
        }

        private static void DeleteToken(string key)
        {
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\NcsWpfApp", true))
            {
                registryKey?.DeleteValue(key, false);
            }
        }
    }
}
