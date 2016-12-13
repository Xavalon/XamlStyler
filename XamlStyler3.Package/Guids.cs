// Guids.cs
// MUST match guids.h

using System;

namespace Xavalon.XamlStyler.Package
{
    internal static class GuidList
    {
        public const string guidXamlStyler_PackagePkgString = "a224be3c-88d1-4a57-9804-181dbef68021";
        public const string guidXamlStyler_PackageCmdSetString = "83fc41d5-eacb-4fa8-aaa3-9a9bdd5f6407";
        public const string guidXamlStyler_PackageCmdXMLSetString = "1c2a8c14-b4f7-41a3-bb14-22d50a133c3f";

        public static readonly Guid guidXamlStyler_PackageCmdSet = new Guid(guidXamlStyler_PackageCmdSetString);
        public static readonly Guid guidXamlStyler_PackageCmdXMLSet = new Guid(guidXamlStyler_PackageCmdXMLSetString);

        public const string UIContextGuid = "33d9c493-2951-4238-af68-054da4e55a87";
    };
}