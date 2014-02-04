#region Header

// Guids.cs
// MUST match guids.h

#endregion Header

using System;

namespace XamlStyler.VSPackage
{
    internal static class GuidList
    {
        #region Fields

        public const string GUID_XAML_STYLER_VS_PACKAGE_CMD_SET_STRING = "b18c9145-bd54-4870-a9c1-191f7a4b2e86";
        public const string GUID_XAML_STYLER_VS_PACKAGE_PKG_STRING = "7a174615-4371-4c1e-b7c5-3a3d94a1ba6c";

        public static readonly Guid GuidXamlStylerVsPackageCmdSet = new Guid(GUID_XAML_STYLER_VS_PACKAGE_CMD_SET_STRING);

        #endregion Fields
    }
}