// © Xavalon. All rights reserved.

using System;

namespace Xavalon.XamlStyler.Extension.Mac.Utils
{
    public static class PathUtils
    {
        private const string RelativePathStart = "~";

        public static string ToAbsolutePath(string path)
        {
            if (!path.StartsWith(RelativePathStart, StringComparison.InvariantCultureIgnoreCase))
            {
                return path;
            }

            var absolutePathStartPart = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var relativePathPart = path.Substring(RelativePathStart.Length);
            var absolutePath = absolutePathStartPart + relativePathPart;
            return absolutePath;
        }
    }
}