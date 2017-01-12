using System;

namespace Xavalon.XamlStyler.Package
{
    internal static class Guids
    {
        public const string XamlStylerPackageGuidString = "a224be3c-88d1-4a57-9804-181dbef68021"; 
        public const string CommandSetGuidString = "83fc41d5-eacb-4fa8-aaa3-9a9bdd5f6407";
        public const string UIContextGuidString = "2dc9b780-2911-46e9-bd66-508dfd5f68a3";

        public static readonly Guid CommandSetGuid = new Guid(Guids.CommandSetGuidString);
    };
}