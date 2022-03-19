using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RedSharp.FileSystem.Sys.Enums;
using RedSharp.FileSystem.Sys.Exceptions;
using RedSharp.FileSystem.Sys.Interfaces.Shared;
using RedSharp.FileSystem.Sys.Utils;

namespace RedSharp.FileSystem.Sys.Helpers
{
    public static class DrivesGuard
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfPathIsNotAbsolute(FileSystemPath path, [CallerArgumentExpression("path")] String name = "path")
        {
            if (path.Type != PathType.Absolute)
                throw new ArgumentException("Input path has to be an Absolute.", name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfPathIsNotAbsolute(Uri identifier, [CallerArgumentExpression("identifier")] String name = "identifier")
        {
            if (!identifier.IsAbsoluteUri)
                throw new ArgumentException("Input path has to be an Absolute.", name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfItemTypeIsInvalid<TType>(IDriveItem item, [CallerArgumentExpression("item")] String name = "item") where TType : IDriveItem
        {
            if (!(item is TType))
                throw new DriveItemNotSupportedException(name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfContainsResrictedCharacters(String value, [CallerArgumentExpression("value")] String name = "name")
        {
            if (FileSystemPath.IsNeedToEscape(value))
                throw new ArgumentException("Name contains restricted characters.", name);
        }
    }
}
