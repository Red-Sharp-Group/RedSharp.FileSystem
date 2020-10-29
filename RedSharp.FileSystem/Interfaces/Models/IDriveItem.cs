using System;
using System.Collections.Generic;
using System.Text;

namespace RedSharp.FileSystem.Interfaces.Models
{
    public interface IDriveItem
    {
        String Name { get; }

        DateTime LastModifiedTime { get; }

        bool IsReadOnly { get; }

        bool IsShared { get; }

        Uri Identifier { get; }

        String[] Path { get; }
    }
}
