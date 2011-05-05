using System;
using System.Collections.Generic;

namespace Assman
{
    public static class Comparers
    {
        public static IEqualityComparer<string> VirtualPath
        {
            get { return StringComparer.OrdinalIgnoreCase; }
        }

        public static IEqualityComparer<string> FileSystemPath
        {
            get { return StringComparer.OrdinalIgnoreCase; }
        }

        public static IEqualityComparer<string> RegistryNames
        {
            get { return StringComparer.OrdinalIgnoreCase; }
        }
    }
}