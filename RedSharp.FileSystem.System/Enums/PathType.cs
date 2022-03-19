using System;
using System.Collections.Generic;
using System.Text;

namespace RedSharp.FileSystem.Sys.Enums
{
    /// <summary>
    /// Type of path, in most cases must be <see cref="Absolute"/>
    /// </summary>
    public enum PathType : byte
    {
        /// <summary>
        /// Absolute path, includes root.
        /// </summary>
        Absolute,

        /// <summary>
        /// Relative path, doesn't include root, can contains return combination: ".."
        /// </summary>
        Relative,

        /// <summary>
        /// Means that the part of the path is unknown and cannot be restored.
        /// </summary>
        Unknown
    }
}
