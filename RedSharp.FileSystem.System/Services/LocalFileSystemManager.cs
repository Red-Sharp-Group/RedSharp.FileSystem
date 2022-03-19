using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RedSharp.FileSystem.Sys.Abstracts;
using RedSharp.FileSystem.Sys.Helpers;
using RedSharp.FileSystem.Sys.Interfaces.Models;
using RedSharp.FileSystem.Sys.Interfaces.Services;
using RedSharp.FileSystem.Sys.Models;
using RedSharp.FileSystem.Sys.Utils;
using RedSharp.Sys.Abstracts;
using RedSharp.Sys.Helpers;

namespace RedSharp.FileSystem.Sys.Services
{
    public class LocalFileSystemManager : DisposableBase, IFileSystemManager
    {
        //===========================================//
        //ITEMS SUPPORTING

        /// <inheritdoc/>
        public bool IsItemSupported(IFileInfo file)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(file);

            return file is LocalDriveItemBase;
        }

        /// <inheritdoc/>
        public bool IsItemSupported(IDirectoryInfo directory)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(directory);

            return directory is LocalDriveItemBase;
        }

        //===========================================//
        //ITEMS GETTING

        /// <inheritdoc/>
        public IFileInfo GetFile(FileSystemPath path)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(path);
            DrivesGuard.ThrowIfPathIsNotAbsolute(path);

            var stringPath = path.ToString();

            if (!File.Exists(stringPath))
                throw new FileNotFoundException();

            return new LocalFileInfo(new FileInfo(stringPath));
        }

        /// <inheritdoc/>
        public IDirectoryInfo GetDirectory(FileSystemPath path)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(path);
            DrivesGuard.ThrowIfPathIsNotAbsolute(path);

            var stringPath = path.ToString();

            if (!Directory.Exists(stringPath))
                throw new DirectoryNotFoundException();

            return new LocalDirectoryInfo(new DirectoryInfo(stringPath));
        }

        /// <inheritdoc/>
        public Task<IFileInfo> GetFileAsync(FileSystemPath path, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => GetFile(path), token);
        }

        /// <inheritdoc/>
        public Task<IDirectoryInfo> GetDirectoryAsync(FileSystemPath path, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => GetDirectory(path), token);
        }

        /// <inheritdoc/>
        public IFileInfo GetFile(Uri identifier)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(identifier);
            DrivesGuard.ThrowIfPathIsNotAbsolute(identifier);

            var stringPath = identifier.ToString();

            if (!File.Exists(stringPath))
                throw new FileNotFoundException();

            return new LocalFileInfo(new FileInfo(stringPath));
        }

        /// <inheritdoc/>
        public IDirectoryInfo GetDirectory(Uri identifier)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(identifier);
            DrivesGuard.ThrowIfPathIsNotAbsolute(identifier);

            var stringPath = identifier.ToString();

            if (!Directory.Exists(stringPath))
                throw new DirectoryNotFoundException();

            return new LocalDirectoryInfo(new DirectoryInfo(stringPath));
        }

        /// <inheritdoc/>
        public Task<IFileInfo> GetFileAsync(Uri identifier, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => GetFile(identifier), token);
        }

        /// <inheritdoc/>
        public Task<IDirectoryInfo> GetDirectoryAsync(Uri identifier, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => GetDirectory(identifier), token);
        }


        /// <inheritdoc/>
        public IEnumerable<IFileInfo> GetFiles(IDirectoryInfo directory)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(directory);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(directory);

            var items = Directory.GetFiles(directory.Path.ToString());
            var result = new IFileInfo[items.Length];

            for (int i = 0; i < items.Length; i++)
                result[i] = new LocalFileInfo(new FileInfo(items[i]));

            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<IDirectoryInfo> GetDirectories(IDirectoryInfo directory)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(directory);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(directory);

            var items = Directory.GetDirectories(directory.Path.ToString());
            var result = new IDirectoryInfo[items.Length];

            for (int i = 0; i < items.Length; i++)
                result[i] = new LocalDirectoryInfo(new DirectoryInfo(items[i]));

            return result;
        }

        /// <inheritdoc/>
        public Task<IEnumerable<IFileInfo>> GetFilesAsync(IDirectoryInfo directory, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => GetFiles(directory), token);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<IDirectoryInfo>> GetDirectoriesAsync(IDirectoryInfo directory, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => GetDirectories(directory), token);
        }


        //===========================================//
        //ITEMS SEARCHING

        /// <inheritdoc/>
        public IFileInfo SearchFile(String name, IDirectoryInfo directory)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNullOrEmpty(name);
            ArgumentsGuard.ThrowIfNull(directory);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(directory);

            var newpath = directory.Path.Combine(name);
            var newPathString = newpath.ToString();

            if (!File.Exists(newPathString))
                return null;

            return new LocalFileInfo(new FileInfo(newPathString));
        }

        /// <inheritdoc/>
        public IDirectoryInfo SearchDirectory(String name, IDirectoryInfo directory)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNullOrEmpty(name);
            ArgumentsGuard.ThrowIfNull(directory);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(directory);

            var newpath = directory.Path.Combine(name);
            var newPathString = newpath.ToString();

            if (!Directory.Exists(newPathString))
                return null;

            return new LocalDirectoryInfo(new DirectoryInfo(newPathString));
        }

        /// <inheritdoc/>
        public Task<IFileInfo> SearchFileAsync(String name, IDirectoryInfo directory, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => SearchFile(name, directory), token);
        }

        /// <inheritdoc/>
        public Task<IDirectoryInfo> SearchDirectorAsyncy(String name, IDirectoryInfo directory, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => SearchDirectory(name, directory), token);
        }


        //===========================================//
        //ITEMS EXISTING

        /// <inheritdoc/>
        public bool IsFileExist(FileSystemPath path)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(path);
            DrivesGuard.ThrowIfPathIsNotAbsolute(path);

            return File.Exists(path.ToString());
        }

        /// <inheritdoc/>
        public bool IsDirectoryExist(FileSystemPath path)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(path);
            DrivesGuard.ThrowIfPathIsNotAbsolute(path);

            return Directory.Exists(path.ToString());
        }

        /// <inheritdoc/>
        public Task<bool> IsFileExistAsync(FileSystemPath path, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => IsFileExist(path), token);
        }

        /// <inheritdoc/>
        public Task<bool> IsDirectoryExistAsync(FileSystemPath path, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => IsDirectoryExist(path), token);
        }

        /// <inheritdoc/>
        public bool IsFileExist(Uri identifier)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(identifier);
            DrivesGuard.ThrowIfPathIsNotAbsolute(identifier);

            return File.Exists(identifier.ToString());
        }

        /// <inheritdoc/>
        public bool IsDirectoryExist(Uri identifier)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(identifier);
            DrivesGuard.ThrowIfPathIsNotAbsolute(identifier);

            return Directory.Exists(identifier.ToString());
        }

        /// <inheritdoc/>
        public Task<bool> IsFileExistAsync(Uri identifier, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => IsFileExist(identifier), token);
        }

        /// <inheritdoc/>
        public Task<bool> IsDirectoryExistAsync(Uri identifier, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => IsDirectoryExist(identifier), token);
        }

        /// <inheritdoc/>
        public bool IsFileStillExist(IFileInfo file)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(file);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(file);

            return File.Exists(file.Path.ToString());
        }

        /// <inheritdoc/>
        public bool IsDirectoryExist(IDirectoryInfo directory)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(directory);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(directory);

            return Directory.Exists(directory.Path.ToString());
        }

        /// <inheritdoc/>
        public Task<bool> IsFileStillExistAsync(IFileInfo file, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => IsFileStillExist(file), token);
        }

        /// <inheritdoc/>
        public Task<bool> IsDirectoryExistAsync(IDirectoryInfo directory, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => IsDirectoryExist(directory), token);
        }


        //===========================================//
        //ITEMS CREATING

        /// <inheritdoc/>
        public IFileInfo CreateFile(String name, IDirectoryInfo directory)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNullOrEmpty(name);
            DrivesGuard.ThrowIfContainsResrictedCharacters(name);
            ArgumentsGuard.ThrowIfNull(directory);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(directory);

            var newPath = directory.Path.Combine(name);
            var newPathString = newPath.ToString();

            ThrowIfFileExists(newPathString);

            File.Create(newPathString).Dispose();
            
            return new LocalFileInfo(new FileInfo(newPathString));
        }

        /// <inheritdoc/>
        public IDirectoryInfo CreateDirectory(String name, IDirectoryInfo directory)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNullOrEmpty(name);
            DrivesGuard.ThrowIfContainsResrictedCharacters(name);
            ArgumentsGuard.ThrowIfNull(directory);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(directory);

            var newPath = directory.Path.Combine(name);
            var newPathString = newPath.ToString();

            ThrowIfDirectoryExists(newPathString);

            return new LocalDirectoryInfo(Directory.CreateDirectory(newPathString));
        }

        /// <inheritdoc/>
        public Task<IFileInfo> CreateFileAsync(String name, IDirectoryInfo directory, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => CreateFile(name, directory), token);
        }

        /// <inheritdoc/>
        public Task<IDirectoryInfo> CreateDirectoryAsync(String name, IDirectoryInfo directory, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => CreateDirectory(name, directory), token);
        }

        //===========================================//
        //ITEMS SOURCE MANIPULATION

        /// <inheritdoc/>
        public Stream Read(IFileInfo file)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(file);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(file);

            return new FileStream(file.Path.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <inheritdoc/>
        public Stream Write(IFileInfo file)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(file);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(file);

            return new FileStream(file.Path.ToString(), FileMode.Append, FileAccess.Write, FileShare.None);
        }

        /// <inheritdoc/>
        public Stream Overwrite(IFileInfo file)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(file);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(file);

            return new FileStream(file.Path.ToString(), FileMode.Truncate, FileAccess.Write, FileShare.None);
        }

        /// <inheritdoc/>
        public Task<Stream> ReadAsync(IFileInfo file, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => Read(file), token);
        }

        /// <inheritdoc/>
        public Task<Stream> WriteAsync(IFileInfo file, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => Write(file), token);
        }

        /// <inheritdoc/>
        public Task<Stream> OverwriteAsync(IFileInfo file, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => Overwrite(file), token);
        }


        //===========================================//
        //ITEMS SPECIAL FUNCTIONS

        /// <inheritdoc/>
        public IEnumerable<IDirectoryInfo> GetRootDirectories()
        {
            ThrowIfDisposed();

            var drives = DriveInfo.GetDrives();

            var result = new IDirectoryInfo[drives.Length];

            for(int i = 0; i < drives.Length; i++)
                result[i] = new LocalDirectoryInfo(drives[i].RootDirectory);

            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<IDirectoryInfo> GetSpecialDirectories()
        {
            ThrowIfDisposed();

            var paths = new String[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                Environment.GetFolderPath(Environment.SpecialFolder.Favorites),
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            var result = new List<IDirectoryInfo>(paths.Length);

            for(int i = 0; i < paths.Length; i++)
                if(!String.IsNullOrEmpty(paths[i]))
                    result.Add(new LocalDirectoryInfo(new DirectoryInfo(paths[i])));

            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<IDirectoryInfo> GetSystemDirectories()
        {
            ThrowIfDisposed();

            var paths = new String[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.SystemX86)
            };

            var result = new List<IDirectoryInfo>(paths.Length);

            for (int i = 0; i < paths.Length; i++)
                if (!String.IsNullOrEmpty(paths[i]))
                    result.Add(new LocalDirectoryInfo(new DirectoryInfo(paths[i])));

            return result;
        }

        /// <inheritdoc/>
        public Task<IEnumerable<IDirectoryInfo>> GetRootDirectoriesAsync(CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(GetRootDirectories, token);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<IDirectoryInfo>> GetSpecialDirectoriesAsync(CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(GetSpecialDirectories, token);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<IDirectoryInfo>> GetSystemDirectoriesAsync(CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(GetSystemDirectories, token);
        }


        //===========================================//
        //ITEMS RENAMING

        /// <inheritdoc/>
        public void Rename(String name, IFileInfo file)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNullOrEmpty(name);
            ArgumentsGuard.ThrowIfNull(file);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(file);
            DrivesGuard.ThrowIfContainsResrictedCharacters(name);

            var newPath = file.Path.GetParent().Combine(name);
            var newPathString = newPath.ToString();

            if (file.Path == newPath)
                return;

            ThrowIfFileExists(newPathString);

            File.Move(file.Path.ToString(), newPathString);
        }

        /// <inheritdoc/>
        public void Rename(String name, IDirectoryInfo directory)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNullOrEmpty(name);
            ArgumentsGuard.ThrowIfNull(directory);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(directory);
            DrivesGuard.ThrowIfContainsResrictedCharacters(name);

            var newPath = directory.Path.GetParent().Combine(name);
            var newPathString = newPath.ToString();

            if (directory.Path == newPath)
                return;

            ThrowIfDirectoryExists(newPathString);

            Directory.Move(directory.Path.ToString(), newPathString);
        }

        /// <inheritdoc/>
        public Task RenameAsync(String name, IFileInfo file, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => Rename(name, file), token);
        }

        /// <inheritdoc/>
        public Task RenameAsync(String name, IDirectoryInfo directory, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => Rename(name, directory), token);
        }


        //===========================================//
        //ITEMS MOVING

        /// <inheritdoc/>
        public void Move(IFileInfo file, IDirectoryInfo toDirectory)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(file);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(file);
            ArgumentsGuard.ThrowIfNull(toDirectory);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(toDirectory);

            if (file.Path.GetParent() == toDirectory.Path)
                return;

            var newPath = toDirectory.Path.Combine(file.Name);
            var newPathString = newPath.ToString();

            ThrowIfFileExists(newPathString);

            File.Move(file.Path.ToString(), newPathString);
        }

        /// <inheritdoc/>
        public void Move(IDirectoryInfo directory, IDirectoryInfo toDirectory)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(directory);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(directory);
            ArgumentsGuard.ThrowIfNull(toDirectory);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(toDirectory);

            if (directory.Path.GetParent() == toDirectory.Path)
                return;

            var newPath = toDirectory.Path.Combine(directory.Name);
            var newPathString = newPath.ToString();

            ThrowIfDirectoryExists(newPathString);

            Directory.Move(directory.Path.ToString(), newPathString);
        }

        /// <inheritdoc/>
        public Task MoveAsync(IFileInfo file, IDirectoryInfo toDirectory, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => Move(file, toDirectory), token);
        }

        /// <inheritdoc/>
        public Task MoveAsync(IDirectoryInfo directory, IDirectoryInfo toDirectory, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => Move(directory, toDirectory), token);
        }

        //===========================================//
        //ITEMS REMOVING

        /// <inheritdoc/>
        public void Remove(IFileInfo file)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(file);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(file);

            var itemPath = file.Path.ToString();

            if (!File.Exists(itemPath))
                return;

            File.Delete(itemPath);
        }

        /// <inheritdoc/>
        public void Remove(IDirectoryInfo directory)
        {
            ThrowIfDisposed();
            ArgumentsGuard.ThrowIfNull(directory);
            DrivesGuard.ThrowIfItemTypeIsInvalid<LocalDriveItemBase>(directory);

            var itemPath = directory.Path.ToString();

            if (!Directory.Exists(itemPath))
                return;

            Directory.Delete(itemPath);
        }

        /// <inheritdoc/>
        public Task RemoveAsync(IFileInfo file, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => Remove(file), token);
        }

        /// <inheritdoc/>
        public Task RemoveAsync(IDirectoryInfo directory, CancellationToken token = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => Remove(directory), token);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfFileExists(String newPathString)
        {
            if (File.Exists(newPathString))
                throw new Exception("File with this name is already existed.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfDirectoryExists(String newPathString)
        {
            if (Directory.Exists(newPathString))
                throw new Exception("Directory with this name is already existed.");
        }
    }
}
