using System;
using System.Collections.Generic;
using System.Text;
using RedSharp.FileSystem.Sys.Utils;

namespace RedSharp.FileSystem.Sys.Interfaces.Shared
{
    /// <summary>
    /// Shared interface with general options for all drive items.
    /// </summary>
    /// <remarks>
    /// Does not contain file handle or something like this, 
    /// this is like a file model - the actual information at the time of taking
    /// </remarks>
    public interface IDriveItem
    {
        /// <summary>
        /// Item name, if it is filename - will be without extension.
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Drive identity.
        /// </summary>
        String Identity { get; }

        /// <summary>
        /// Means that the item cannot be manipulated.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Means that the item is outside of the user drive.
        /// </summary>
        bool IsShared { get; }

        /// <summary>
        /// Has hidden attribute.
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        /// Universal resource identifier. Can be variable.
        /// </summary>
        Uri Identifier { get; }

        /// <summary>
        /// Hierarchical path, if drive doesn't support it - must be imitated.
        /// </summary>
        FileSystemPath Path { get; }

        /// <summary>
        /// Time and date of last manipulation on the item.
        /// </summary>
        DateTime LastModifiedTime { get; }
    }
}
