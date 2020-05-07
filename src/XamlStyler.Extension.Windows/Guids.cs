using System;

namespace Xavalon.XamlStyler.Extension.Windows
{
    internal static class Guids
    {
        public const string GuidXamlStylerPackageString = "a224be3c-88d1-4a57-9804-181dbef68021"; 
        public const string GuidXamlStylerMenuSetString = "83fc41d5-eacb-4fa8-aaa3-9a9bdd5f6407";
        public const string UIContextGuidString = "2dc9b780-2911-46e9-bd66-508dfd5f68a3";
        public const string GuidVsStd97CmdIDString = "5EFC7975-14BC-11CF-9B2B-00AA00573819";

        public static readonly Guid GuidXamlStylerMenuSet = new Guid(Guids.GuidXamlStylerMenuSetString);
    };
}