using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedSharp.FileSystem.Sys.Enums;
using RedSharp.FileSystem.Sys.Interfaces.Shared;
using RedSharp.FileSystem.Sys.Utils;
using RedSharp.Sys.Helpers;

namespace RedSharp.FileSystem.Sys.Abstracts
{
    /// <summary>
    /// Simple model implementation for the local files and directories.
    /// </summary>
    public abstract class LocalDriveItemBase : IDriveItem
    {
        public const String LocalIdentity = "LOCAL";

        private Uri _cachedUri;
        private FileSystemPath _cachedPath;

        public LocalDriveItemBase(FileSystemInfo fileSystemInfo)
        {
            ArgumentsGuard.ThrowIfNull(fileSystemInfo);

            SystemInfo = fileSystemInfo;
        }

        /// <inheritdoc/>
        public String Name => Path.NameWithoutExtension;

        /// <inheritdoc/>
        public String Identity => LocalIdentity;

        /// <inheritdoc/>
        public bool IsReadOnly => SystemInfo.Attributes.HasFlag(FileAttributes.ReadOnly);

        /// <inheritdoc/>
        public bool IsShared => false;

        /// <inheritdoc/>
        public bool IsHidden => SystemInfo.Attributes.HasFlag(FileAttributes.Hidden);

        /// <inheritdoc/>
        /// <remarks>
        /// Lazy loading, created from <see cref="FileSystemInfo.FullName"/>
        /// </remarks>
        public Uri Identifier
        {
            get
            {
                if (_cachedUri == null)
                    _cachedUri = new Uri(SystemInfo.FullName);

                return _cachedUri;
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Lazy loading, created from <see cref="FileSystemInfo.FullName"/>
        /// </remarks>
        public FileSystemPath Path
        {
            get
            {
                if (_cachedPath == null)
                    _cachedPath = new FileSystemPath(PathType.Absolute, SystemInfo.FullName, false);

                return _cachedPath;
            }
        }

        /// <inheritdoc/>
        public DateTime LastModifiedTime => SystemInfo.LastWriteTime;

        public override string ToString()
        {
            return $"{Identity}: {SystemInfo.FullName}";
        }

        protected FileSystemInfo SystemInfo { get; private set; }
    }
}
