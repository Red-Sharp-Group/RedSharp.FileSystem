using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RedSharp.FileSystem.Sys.Exceptions;
using RedSharp.FileSystem.Sys.Interfaces.Models;
using RedSharp.FileSystem.Sys.Utils;
using RedSharp.Sys.Interfaces.Shared;

namespace RedSharp.FileSystem.Sys.Interfaces.Services
{
    /// <summary>
    /// The central service allows you to manipulate files/directories on the chosen drive.
    /// </summary>
    /// <development>
    /// This is complex and huge service, so I decided to split it on section (in commenting meaning).
    /// </development>
    public interface IFileSystemManager : IDisposable, IDisposeIndication
    {
        //===========================================//
        //ITEMS SUPPORTING

        /// <summary>
        /// Returns true if this item was created from this service or if this service can accept it.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ObjectDisposedException"/>
        bool IsItemSupported(IFileInfo file);

        /// <inheritdoc cref="IsItemSupported(IFileInfo)"/>
        bool IsItemSupported(IDirectoryInfo directory);


        //===========================================//
        //ITEMS GETTING

        /// <summary>
        /// Returns a file by the given <see cref="FileSystemPath"/>
        /// </summary>
        /// <exception cref="FileNotFoundException">If file wasn't found</exception>
        /// <exception cref="ArgumentException">If path is not an absolute.</exception>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ObjectDisposedException"/>
        IFileInfo GetFile(FileSystemPath path);

        /// <summary>
        /// Returns a directory by the given <see cref="FileSystemPath"/>
        /// </summary>
        /// <exception cref="DirectoryNotFoundException">If directory wasn't found</exception>
        /// <exception cref="ArgumentException">If path is not an absolute.</exception>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ObjectDisposedException"/>
        IDirectoryInfo GetDirectory(FileSystemPath path);

        /// <inheritdoc cref="GetFile(FileSystemPath)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<IFileInfo> GetFileAsync(FileSystemPath path, CancellationToken token = default(CancellationToken));

        /// <inheritdoc cref="GetDirectory(FileSystemPath)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<IDirectoryInfo> GetDirectoryAsync(FileSystemPath path, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns a file by the given <see cref="Uri"/>
        /// </summary>
        /// <exception cref="FileNotFoundException">If file wasn't found</exception>
        /// <exception cref="ArgumentException">If path is not an absolute.</exception>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ObjectDisposedException"/>
        IFileInfo GetFile(Uri identifier);

        /// <summary>
        /// Returns a directory by the given <see cref="Uri"/>
        /// </summary>
        /// <exception cref="DirectoryNotFoundException">If directory wasn't found</exception>
        /// <exception cref="ArgumentException">If path is not an absolute.</exception>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ObjectDisposedException"/>
        IDirectoryInfo GetDirectory(Uri identifier);

        /// <inheritdoc cref="GetFile(Uri)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<IFileInfo> GetFileAsync(Uri identifier, CancellationToken token = default(CancellationToken));

        /// <inheritdoc cref="GetDirectory(Uri)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<IDirectoryInfo> GetDirectoryAsync(Uri identifier, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns a list of files that are in the given directory.
        /// </summary>
        /// <exception cref="ArgumentNullException">If the directory is null.</exception>
        /// <exception cref="DriveItemNotSupportedException"/>
        /// <exception cref="ObjectDisposedException"/>
        IEnumerable<IFileInfo> GetFiles(IDirectoryInfo directory);

        /// <summary>
        /// Returns a list of files that are in the given directory.
        /// </summary>
        /// <exception cref="ArgumentNullException">If the directory is null.</exception>
        /// <exception cref="DriveItemNotSupportedException"/>
        /// <exception cref="ObjectDisposedException"/>
        IEnumerable<IDirectoryInfo> GetDirectories(IDirectoryInfo directory);

        /// <inheritdoc cref="GetFiles(IDirectoryInfo)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<IEnumerable<IFileInfo>> GetFilesAsync(IDirectoryInfo directory, CancellationToken token = default(CancellationToken));

        /// <inheritdoc cref="GetDirectories(IDirectoryInfo)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<IEnumerable<IDirectoryInfo>> GetDirectoriesAsync(IDirectoryInfo directory, CancellationToken token = default(CancellationToken));


        //===========================================//
        //ITEMS SEARCHING

        /// <summary>
        /// Will search a file with the given name in the input directory.
        /// </summary>
        /// <returns>First file with the needed name or null.</returns>
        /// <exception cref="ArgumentException">If name is null or empty.</exception>
        /// <exception cref="ArgumentNullException">If the directory is null or empty</exception>
        /// <exception cref="DriveItemNotSupportedException"/>
        /// <exception cref="ObjectDisposedException"/>
        IFileInfo SearchFile(String name, IDirectoryInfo directory);

        /// <summary>
        /// Will search a directory with the given name in the input directory.
        /// </summary>
        /// <returns>First directory with the needed name or null.</returns>
        /// <exception cref="ArgumentException">If name is null or empty.</exception>
        /// <exception cref="ArgumentNullException">If the directory is null or empty</exception>
        /// <exception cref="DriveItemNotSupportedException"/>
        /// <exception cref="ObjectDisposedException"/>
        IDirectoryInfo SearchDirectory(String name, IDirectoryInfo directory);

        /// <inheritdoc cref="SearchFile(string, IDirectoryInfo)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<IFileInfo> SearchFileAsync(String name, IDirectoryInfo directory, CancellationToken token = default(CancellationToken));

        /// <inheritdoc cref="SearchDirectory(string, IDirectoryInfo)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<IDirectoryInfo> SearchDirectorAsyncy(String name, IDirectoryInfo directory, CancellationToken token = default(CancellationToken));


        //===========================================//
        //ITEMS EXISTING

        /// <summary>
        /// Returns true if the file by <see cref="FileSystemPath"/> exists.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException">If path is not an absolute.</exception>
        /// <exception cref="ObjectDisposedException"/>
        bool IsFileExist(FileSystemPath path);

        /// <summary>
        /// Returns true if the directory by <see cref="FileSystemPath"/> exists.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException">If path is not an absolute.</exception>
        /// <exception cref="ObjectDisposedException"/>
        bool IsDirectoryExist(FileSystemPath path);

        /// <inheritdoc cref="IsFileExist(FileSystemPath)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<bool> IsFileExistAsync(FileSystemPath path, CancellationToken token = default(CancellationToken));

        /// <inheritdoc cref="IsDirectoryExist(FileSystemPath)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<bool> IsDirectoryExistAsync(FileSystemPath path, CancellationToken token = default(CancellationToken));


        /// <summary>
        /// Returns true if the file by <see cref="Uri"/> exists.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ObjectDisposedException"/>
        bool IsFileExist(Uri identifier);

        /// <summary>
        /// Returns true if the directory by <see cref="Uri"/> exists.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ObjectDisposedException"/>
        bool IsDirectoryExist(Uri identifier);

        /// <inheritdoc cref="IsFileExist(Uri)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<bool> IsFileExistAsync(Uri identifier, CancellationToken token = default(CancellationToken));

        /// <inheritdoc cref="IsDirectoryExist(Uri)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<bool> IsDirectoryExistAsync(Uri identifier, CancellationToken token = default(CancellationToken));


        /// <summary>
        /// Returns true if the file wasn't removed from the moment when the <see cref="IFileInfo"/> was got.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DriveItemNotSupportedException"/>
        /// <exception cref="ObjectDisposedException"/>
        bool IsFileStillExist(IFileInfo file);

        /// <summary>
        /// Returns true if the file wasn't removed from the moment when the <see cref="IDirectoryInfo"/> was got.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DriveItemNotSupportedException"/>
        /// <exception cref="ObjectDisposedException"/>
        bool IsDirectoryExist(IDirectoryInfo directory);

        /// <inheritdoc cref="IsFileStillExist(IFileInfo)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<bool> IsFileStillExistAsync(IFileInfo file, CancellationToken token = default(CancellationToken));

        /// <inheritdoc cref="IsDirectoryExist(IDirectoryInfo)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<bool> IsDirectoryExistAsync(IDirectoryInfo directory, CancellationToken token = default(CancellationToken));


        //===========================================//
        //ITEMS CREATING

        /// <summary>
        /// Creates a new file in the chosen directory.
        /// </summary>
        /// <exception cref="ArgumentException">If string is null or empty.</exception>
        /// <exception cref="ArgumentNullException">If the directory is null.</exception>
        /// <exception cref="DriveItemNotSupportedException"/>
        /// <exception cref="ObjectDisposedException"/>
        IFileInfo CreateFile(String name, IDirectoryInfo directory);

        /// <summary>
        /// Creates a new directory in the chosen directory.
        /// </summary>
        /// <exception cref="ArgumentException">If string is null or empty.</exception>
        /// <exception cref="ArgumentNullException">If the directory is null.</exception>
        /// <exception cref="DriveItemNotSupportedException"/>
        /// <exception cref="ObjectDisposedException"/>
        IDirectoryInfo CreateDirectory(String name, IDirectoryInfo directory);

        /// <inheritdoc cref="CreateFile(string, IDirectoryInfo)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<IFileInfo> CreateFileAsync(String name, IDirectoryInfo directory, CancellationToken token = default(CancellationToken));

        /// <inheritdoc cref="CreateDirectory(string, IDirectoryInfo)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<IDirectoryInfo> CreateDirectoryAsync(String name, IDirectoryInfo directory, CancellationToken token = default(CancellationToken));


        //===========================================//
        //ITEMS SOURCE MANIPULATION

        /// <summary>
        /// Opens stream for reading.
        /// </summary>
        /// <exception cref="ArgumentNullException">If input file is null.</exception>
        /// <exception cref="DriveItemNotSupportedException"/>
        /// <exception cref="ObjectDisposedException"/>
        Stream Read(IFileInfo file);

        /// <summary>
        /// Opens stream for writing.
        /// Will add new information to existing.
        /// <br/><see cref="IFileInfo"/> will not be changed.
        /// </summary>
        /// <exception cref="ArgumentNullException">If input file is null.</exception>
        /// <exception cref="DriveItemNotSupportedException"/>
        /// <exception cref="ObjectDisposedException"/>
        Stream Write(IFileInfo file);

        /// <summary>
        /// Opens stream for writing.
        /// Will overwrite the file.
        /// <br/><see cref="IFileInfo"/> will not be changed.
        /// </summary>
        /// <exception cref="ArgumentNullException">If input file is null.</exception>
        /// <exception cref="DriveItemNotSupportedException"/>
        /// <exception cref="ObjectDisposedException"/>
        Stream Overwrite(IFileInfo file);

        /// <inheritdoc cref="Read(IFileInfo)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<Stream> ReadAsync(IFileInfo file, CancellationToken token = default(CancellationToken));

        /// <inheritdoc cref="Write(IFileInfo)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<Stream> WriteAsync(IFileInfo file, CancellationToken token = default(CancellationToken));

        /// <inheritdoc cref="Overwrite(IFileInfo)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<Stream> OverwriteAsync(IFileInfo file, CancellationToken token = default(CancellationToken));


        //===========================================//
        //ITEMS SPECIAL FUNCTIONS

        /// <summary>
        /// Returns a list with all root directories (drives, drive partitions, shared drives)
        /// </summary>
        /// <exception cref="ObjectDisposedException"/>
        IEnumerable<IDirectoryInfo> GetRootDirectories();

        /// <summary>
        /// Returns a list with all special directories (photos, videos, desktop etc.)
        /// </summary>
        /// <exception cref="ObjectDisposedException"/>
        IEnumerable<IDirectoryInfo> GetSpecialDirectories();

        /// <summary>
        /// Returns a list with all system directories (program data as an example)
        /// </summary>
        /// <exception cref="ObjectDisposedException"/>
        IEnumerable<IDirectoryInfo> GetSystemDirectories();

        /// <inheritdoc cref="GetRootDirectories"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<IEnumerable<IDirectoryInfo>> GetRootDirectoriesAsync(CancellationToken token = default(CancellationToken));

        /// <inheritdoc cref="GetSpecialDirectories"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<IEnumerable<IDirectoryInfo>> GetSpecialDirectoriesAsync(CancellationToken token = default(CancellationToken));

        /// <inheritdoc cref="GetSystemDirectories"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task<IEnumerable<IDirectoryInfo>> GetSystemDirectoriesAsync(CancellationToken token = default(CancellationToken));


        //===========================================//
        //ITEMS RENAMING

        /// <summary>
        /// Renaming the chosen file.
        /// <br/><see cref="IFileInfo"/> will not be changed.
        /// </summary>
        /// <exception cref="ArgumentException">If string is null or empty.</exception>
        /// <exception cref="ArgumentNullException">If the item info is null.</exception>
        /// <exception cref="DriveItemNotSupportedException"/>
        /// <exception cref="ObjectDisposedException"/>
        void Rename(String name, IFileInfo file);

        /// <summary>
        /// Renaming the chosen directory.
        /// <br/><see cref="IDirectoryInfo"/> will not be changed.
        /// </summary>
        /// <exception cref="ArgumentException">If string is null or empty.</exception>
        /// <exception cref="ArgumentNullException">If the item info is null.</exception>
        /// <exception cref="DriveItemNotSupportedException"/>
        /// <exception cref="ObjectDisposedException"/>
        void Rename(String name, IDirectoryInfo directory);

        /// <inheritdoc cref="Rename(string, IFileInfo)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task RenameAsync(String name, IFileInfo file, CancellationToken token = default(CancellationToken));

        /// <inheritdoc cref="Rename(string, IDirectoryInfo)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task RenameAsync(String name, IDirectoryInfo directory, CancellationToken token = default(CancellationToken));


        //===========================================//
        //ITEMS MOVING

        /// <summary>
        /// Moves the file to the another directory.
        /// <br/><see cref="IFileInfo"/> will not be changed.
        /// </summary>
        /// <exception cref="ArgumentNullException">If some of these items is/are null.</exception>
        /// <exception cref="DriveItemNotSupportedException"/>
        /// <exception cref="ObjectDisposedException"/>
        void Move(IFileInfo file, IDirectoryInfo toDirectory);

        /// <summary>
        /// Moves the directory to the another directory.
        /// <br/><see cref="IDirectoryInfo"/> will not be changed.
        /// </summary>
        /// <exception cref="ArgumentNullException">If some of these items is/are null.</exception>
        /// <exception cref="DriveItemNotSupportedException"/>
        /// <exception cref="ObjectDisposedException"/>
        void Move(IDirectoryInfo directory, IDirectoryInfo toDirectory);

        ///<inheritdoc cref="Move(IFileInfo, IDirectoryInfo)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task MoveAsync(IFileInfo file, IDirectoryInfo toDirectory, CancellationToken token = default(CancellationToken));

        ///<inheritdoc cref="Move(IDirectoryInfo, IDirectoryInfo)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task MoveAsync(IDirectoryInfo directory, IDirectoryInfo toDirectory, CancellationToken token = default(CancellationToken));


        //===========================================//
        //ITEMS REMOVING

        /// <summary>
        /// Deletes the file from the drive.
        /// </summary>
        /// <exception cref="ArgumentNullException">The input item is null.</exception>
        /// <exception cref="DriveItemNotSupportedException"/>
        /// <exception cref="ObjectDisposedException"/>
        void Remove(IFileInfo file);

        /// <summary>
        /// Deletes the directory from the drive.
        /// </summary>
        /// <exception cref="ArgumentNullException">The input item is null.</exception>
        /// <exception cref="DriveItemNotSupportedException"/>
        /// <exception cref="ObjectDisposedException"/>
        void Remove(IDirectoryInfo directory);

        ///<inheritdoc cref="Remove(IFileInfo)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task RemoveAsync(IFileInfo file, CancellationToken token = default(CancellationToken));

        ///<inheritdoc cref="Remove(IDirectoryInfo)"/>
        /// <remarks>
        /// Async version.
        /// </remarks>
        Task RemoveAsync(IDirectoryInfo directory, CancellationToken token = default(CancellationToken));
    }
}
