using System;
using System.Collections.Generic;
using System.Text;
using RedSharp.FileSystem.Sys.Interfaces.Shared;

namespace RedSharp.FileSystem.Sys.Interfaces.Models
{
    /// <summary>
    /// Represents an actual information about file.
    /// </summary>
    public interface IFileInfo : IDriveItem
    {
        /// <summary>
        /// File extension in lover case, doesn't include dot.
        /// </summary>
        String Extension { get; }

        /// <summary>
        /// Size of the file.
        /// </summary>
        ulong Size { get; }
    }
}
