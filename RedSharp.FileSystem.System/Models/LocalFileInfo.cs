using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedSharp.FileSystem.Sys.Abstracts;
using RedSharp.FileSystem.Sys.Enums;
using RedSharp.FileSystem.Sys.Interfaces.Models;
using RedSharp.FileSystem.Sys.Utils;
using RedSharp.Sys.Helpers;

namespace RedSharp.FileSystem.Sys.Models
{
    /// <summary>
    /// Implementation for local drive files. 
    /// </summary>
    public class LocalFileInfo : LocalDriveItemBase, IFileInfo
    {
        public LocalFileInfo(FileInfo item) : base(item)
        { }

        /// <inheritdoc/>
        public string Extension => Path.Extension;

        /// <inheritdoc/>
        public ulong Size => (ulong)(SystemInfo as FileInfo).Length;
    }
}
