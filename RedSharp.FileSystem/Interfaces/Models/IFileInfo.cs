using System;
using System.Collections.Generic;
using System.Text;

namespace RedSharp.FileSystem.Interfaces.Models
{
    public interface IFileInfo : IDriveItem
    {
        String Extension { get; }

        ulong Size { get; }
    }
}
