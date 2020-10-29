using RedSharp.FileSystem.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedSharp.FileSystem.Helpers
{
    public static class DriveItemPathHelper
    {
        public static String GetEscapedPath(this IDriveItem driveItem)
        {
            if (driveItem == null)
                throw new NullReferenceException();

            return GetEscapedPath(driveItem.Path);
        }
        public static String GetEscapedPath(String[] path)
        {
            if (path == null)
                throw new NullReferenceException();

            var escapedPath = path.Select(item => DriveItemEscapingHelper.EscapeString(item));
            var result = String.Join(DriveItemEscapingHelper.AltDirectorySeparatorString, escapedPath);

            return result;
        }

        public static bool IsPathRelative(this IDriveItem driveItem)
        {
            if (driveItem == null)
                throw new NullReferenceException();

            return IsPathRelative(driveItem.Path);
        }

        public static bool IsPathRelative(String[] path)
        {
            if (path == null)
                throw new NullReferenceException();

            return path.First() == DriveItemEscapingHelper.RelativePathHolder;
        }

        public static bool IsPathUnknown(this IDriveItem driveItem)
        {
            if (driveItem == null)
                throw new NullReferenceException();

            return IsPathUnknown(driveItem.Path);
        }

        public static bool IsPathUnknown(String[] path)
        {
            if (path == null)
                throw new NullReferenceException();

            return path.First() == DriveItemEscapingHelper.UnknowPathHolder;
        }

        public static String[] MakeAbsolutePath(this IDriveItem driveItem, IDriveItem relatedToItem)
        {
            if (driveItem == null || relatedToItem == null)
                throw new NullReferenceException();

            if (relatedToItem is IDirectoryInfo)
                return MakeAbsolutePath(driveItem.Path, relatedToItem.Path);
            else
                return MakeAbsolutePath(driveItem.Path, GetParentPath(relatedToItem.Path));
        }

        public static String[] MakeAbsolutePath(String[] relativePath, String[] relatedToPath)
        {
            if (relativePath == null || relatedToPath == null)
                throw new NullReferenceException();

            if (!IsPathRelative(relativePath))
                throw new ArgumentException("Path must be relative.");

            if(IsPathRelative(relatedToPath) || IsPathUnknown(relatedToPath))
                throw new ArgumentException("Related to path must be absolute.");

            int holderCount = 0;

            for (int i = 0; i < relativePath.Length; i++)
            {
                if (relatedToPath[i] == DriveItemEscapingHelper.RelativePathHolder)
                    holderCount++;
                else
                    break;
            }

            if (holderCount == relativePath.Length)
                throw new ArgumentException("Input relative path is invalid.");

            if (holderCount >= relatedToPath.Length)
                throw new ArgumentException("Related to path is too short.");

            var relateToPathCopyLength = relatedToPath.Length - (holderCount - 1);
            var realtivePathCopyLength = relativePath.Length - holderCount;
            var resultPathLength = relatedToPath.Length - (holderCount - 1) + relativePath.Length - holderCount;

            var result = new String[resultPathLength];

            Array.Copy(relatedToPath, result, relateToPathCopyLength);
            Array.Copy(relatedToPath, holderCount, result, relateToPathCopyLength, realtivePathCopyLength);

            return result;
        }

        public static String[] GetParentPath(String[] path)
        {
            if (path == null || path.Length <= 1)
                return Array.Empty<String>();

            var result = new String[path.Length - 1];

            Array.Copy(path, result, result.Length);

            return result;
        }

        public static String[] MakeRelativePath(this IDriveItem driveItem, IDriveItem relatedToItem)
        {
            if (driveItem == null || relatedToItem == null)
                throw new NullReferenceException();

            if (relatedToItem is IDirectoryInfo)
                return MakeRelativePath(driveItem.Path, relatedToItem.Path);
            else
                return MakeRelativePath(driveItem.Path, GetParentPath(relatedToItem.Path));
        }

        public static String[] MakeRelativePath(String[] inputPath, String[] relatedToPath)
        {
            if (inputPath == null || relatedToPath == null)
                throw new NullReferenceException();

            if (IsPathRelative(inputPath) || IsPathUnknown(inputPath))
                throw new ArgumentException("Input path must be relative.");

            if (IsPathRelative(relatedToPath) || IsPathUnknown(relatedToPath))
                throw new ArgumentException("Related to path must be absolute.");

            int sameFragmentsCount = 0;
            int neededLength = Math.Min(inputPath.Length, relatedToPath.Length);

            for (int i = 0; i < inputPath.Length; i++)
            {
                //TODO
            }

            return null;
        }
    }
}
