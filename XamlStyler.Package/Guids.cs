// Guids.cs
// MUST match guids.h

using System;

namespace Xavalon.XamlStyler.Package
{
    internal static class GuidList
    {
        public const string guidXamlStyler_PackagePkgString = "a224be3c-88d1-4a57-9804-181dbef68021";
        public const string guidXamlStyler_PackageCmdSetString = "83fc41d5-eacb-4fa8-aaa3-9a9bdd5f6407";

        public static readonly Guid guidXamlStyler_PackageCmdSet = new Guid(guidXamlStyler_PackageCmdSetString);
    };
}