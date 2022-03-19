using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedSharp.FileSystem.Sys.Enums;
using RedSharp.Sys.Helpers;

namespace RedSharp.FileSystem.Sys.Utils
{
    /// <summary>
    /// Special object to hold a hierarchical path to the object. 
    /// Instead of string - can contain restricted characters.
    /// <br/>Immutable.
    /// </summary>
    public class FileSystemPath
    {
        public const char DirectorySeparatorChar = '\\';
        public const char AltDirectorySeparatorChar = '/';
        public const char VolumeSeparatorChar = ':';
        public const char PathSeparatorChar = ';';

        public const char EscapeChar = '%';
        public const String RelativePathSegment = "..";
        public const String UnknowPathSegment = "??";

        public static readonly String DirectorySeparatorString = DirectorySeparatorChar.ToString();
        public static readonly String AltDirectorySeparatorString = AltDirectorySeparatorChar.ToString();

        private static readonly HashSet<char> _invalidFileNamesChars;
        private static readonly Dictionary<char, String> _escapeDictionary;
        private static readonly Dictionary<String, char> _unescapeDictionary;

        private String[] _segments;
        private int? _cachedHashCode;
        private bool _wasEscaped;
        private String _cachedExtension;
        private String _cachedFileNameWithoutExtension;

        static FileSystemPath()
        {
            var invalidPathChars = new char[]
            {
                    '\"', '<', '>', '|', '\0',
                (char)1,  (char)2,  (char)3,  (char)4,  (char)5,  (char)6,  (char)7,  (char)8,  (char)9,  (char)10,
                (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
                (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
                (char)31
            };

            _invalidFileNamesChars = new HashSet<char>();

            foreach (var item in invalidPathChars)
                _invalidFileNamesChars.Add(item);

            _invalidFileNamesChars.Add(AltDirectorySeparatorChar);
            _invalidFileNamesChars.Add(DirectorySeparatorChar);
            _invalidFileNamesChars.Add(PathSeparatorChar);
            _invalidFileNamesChars.Add(VolumeSeparatorChar);

            _escapeDictionary = new Dictionary<char, string>()
            {
                { DirectorySeparatorChar, "%5c"}, { AltDirectorySeparatorChar, "%2f"},

                { '\"',     "%93"}, { '<',        "%3c"}, { '>',      "%3e"}, { '|',      "%7c"}, { '\0',     "%00"},
                { (char)1,  "%01"}, { (char)2,    "%02"}, { (char)3,  "%03"}, { (char)4,  "%04"}, { (char)5,  "%05"},
                { (char)6,  "%06"}, { (char)7,    "%07"}, { (char)8,  "%08"}, { (char)9,  "%09"}, { (char)10, "%0a"},
                { (char)11, "%0b"}, { (char)12,   "%0c"}, { (char)13, "%0d"}, { (char)14, "%0e"}, { (char)15, "%0f"},
                { (char)16, "%10"}, { (char)17,   "%11"}, { (char)18, "%12"}, { (char)19, "%13"}, { (char)20, "%14"},
                { (char)21, "%15"}, { (char)22,   "%16"}, { (char)23, "%17"}, { (char)24, "%18"}, { (char)25, "%19"},
                { (char)26, "%1a"}, { (char)27,   "%1b"}, { (char)28, "%1c"}, { (char)29, "%1d"}, { (char)30, "%1e"},
                { (char)31, "%1f"}, { EscapeChar, "%25"}
            };

            _unescapeDictionary = _escapeDictionary.ToDictionary(item => item.Value, item => item.Key);
        }

        public FileSystemPath(String path, bool isEscaped = true)
        {
            ArgumentsGuard.ThrowIfNullOrEmpty(path);

            path = path.Replace(AltDirectorySeparatorChar, DirectorySeparatorChar);

            //UNC roots starts with double slash
            if ((path[0] == DirectorySeparatorChar && path[1] != DirectorySeparatorChar) || path.StartsWith(RelativePathSegment + DirectorySeparatorChar))
            {
                Type = PathType.Relative;
            }
            else if (path.StartsWith(UnknowPathSegment + DirectorySeparatorChar))
            {
                Type = PathType.Unknown;
            }
            else
            {
                Type = PathType.Absolute;
            }

            InitializeFromString(path, isEscaped);
        }

        public FileSystemPath(PathType type, String path, bool isEscaped = true)
        {
            ArgumentsGuard.ThrowIfNullOrEmpty(path);

            path = path.Replace(AltDirectorySeparatorChar, DirectorySeparatorChar);

            Type = type;

            InitializeFromString(path, isEscaped);
        }

        public FileSystemPath(PathType type, params String[] segments)
        {
            ArgumentsGuard.ThrowIfNull(segments);

            Type = type;

            for (int i = 0; i < segments.Length; i++)
                ArgumentsGuard.ThrowIfNullOrEmpty(segments[i], "segment");

            _segments = segments.ToArray();

            for (int i = 0; i < _segments.Length; i++)
            {
                if (IsNeedToEscape(_segments[i]))
                {
                    _wasEscaped = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Returns path, type in the most case is absolute.
        /// </summary>
        public PathType Type { get; }

        /// <summary>
        /// Returns number or segments.
        /// </summary>
        public int Length => _segments.Length;

        public String this[int index] => _segments[index];
        
        /// <summary>
        /// Returns last segment without any changes.
        /// </summary>
        public String Name => _segments[Length - 1];

        /// <summary>
        /// Returns last segment without extension.
        /// </summary>
        /// <remarks>
        /// Extension counts to the first dot from the end.
        /// Double (or more) extensions are not supported.
        /// Uses cache.
        /// </remarks>
        public String NameWithoutExtension 
        {
            get
            {

                if(_cachedFileNameWithoutExtension == null)
                {
                    var extension = Extension;
                    var filename = Name;

                    if (extension.Length == 0)
                        _cachedFileNameWithoutExtension = filename;
                    else
                        _cachedFileNameWithoutExtension = filename.Substring(0, filename.Length - extension.Length);
                }

                return _cachedFileNameWithoutExtension;
            }
        }

        /// <summary>
        /// Returns extension without dot character in lover case.
        /// </summary>
        /// <remarks>
        /// Extension counts to the first dot from the end.
        /// Double (or more) extensions are not supported.
        /// Uses cache.
        /// </remarks>
        public String Extension
        {
            get
            {
                if (_cachedExtension == null) 
                {
                    var filename = Name;
                    var index = -1;

                    for (int i = filename.Length - 1; i >= 0; i--)
                        if (filename[i] == '.')
                            index = i;

                    if (index == -1)
                        _cachedExtension = String.Empty;
                    else
                        _cachedExtension = filename.Substring(index).ToLower();
                }

                return _cachedExtension;
            }
        }

        /// <summary>
        /// Returns new <see cref="FileSystemPath"/> without last segment.
        /// </summary>
        /// <exception cref="Exception">If the length of the current path is too short to make a new <see cref="FileSystemPath"/></exception>
        /// <exception cref="Exception">If the relative segment was reached.</exception>
        public FileSystemPath GetParent()
        {
            if (Length == 1) 
                throw new Exception("Cannot get parent path: current path is too short.");

            if (_segments[Length - 1] == RelativePathSegment)
                throw new Exception("Cannot get parent path: relative path segment was reached.");

            var newSegments = new String[Length - 1];

            Array.Copy(_segments, newSegments, newSegments.Length);

            return new FileSystemPath(Type, newSegments);
        }

        /// <summary>
        /// Creates new <see cref="FileSystemPath"/> with first segment.
        /// </summary>
        /// <remarks>
        /// Only for <see cref="PathType.Absolute"/> pathes can invoke this without problems.
        /// </remarks>
        /// <exception cref="Exception">If the path is not <see cref="PathType.Absolute"/></exception>
        public FileSystemPath GetRoot()
        {
            if (Type != PathType.Absolute) 
                throw new Exception("Root can be retrieved only from absolute paths.");

            if (Length == 1)
                return this;

            var newSegments = new String[1];

            newSegments[0] = _segments[0];

            return new FileSystemPath(Type, newSegments);
        }

        /// <summary>
        /// Returns new <see cref="FileSystemPath"/> that will be relative to the input path.
        /// </summary>
        /// <remarks>
        /// The current path and the input path have to be <see cref="PathType.Absolute"/>
        /// Roots have to be different.
        /// Also, this is stupid but, the current and the input pathes have to be different.
        /// </remarks>
        /// <exception cref="ArgumentNullException">If the input is null</exception>
        /// <exception cref="ArgumentException">If the input path is not <see cref="PathType.Absolute"/></exception>
        /// <exception cref="Exception">If the current path is not <see cref="PathType.Absolute"/></exception>
        /// <exception cref="ArgumentException">If roots are different</exception>
        /// <exception cref="Exception">If both pathes are equal</exception>
        public FileSystemPath MakeRelative(FileSystemPath input)
        {
            ArgumentsGuard.ThrowIfNull(input);

            if (input.Type != PathType.Absolute)
                throw new ArgumentException("Input path must be an absolute.");

            if (Type != PathType.Absolute)
                throw new Exception("Only absolute paths can be convert to relatives.");

            if (!_segments[0].Equals(input[0], StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException("Cannot create a relative path: a current path and an input path have different roots.");

            if (EqualsInternal(this, input))
                throw new Exception("Cannot create a relative path: paths are equal.");

            var resultSegments = new List<String>();

            var inputSegmetns = input._segments;

            var minLength = Math.Min(Length, input.Length);

            int differenceIndex = minLength;

            for (int i = 0; i < minLength; i++)
            {
                if (!_segments[i].Equals(inputSegmetns[i], StringComparison.InvariantCultureIgnoreCase))
                {
                    differenceIndex = i;
                    break;
                }
            }

            for (int i = differenceIndex; i < inputSegmetns.Length; i++)
                resultSegments.Add(RelativePathSegment);

            for (int i = differenceIndex; i < _segments.Length; i++)
                resultSegments.Add(_segments[i]);

            return new FileSystemPath(PathType.Relative, resultSegments.ToArray());
        }

        /// <summary>
        /// Returns new <see cref="FileSystemPath"/> that will be absolute based on the input path.
        /// </summary>
        /// <remarks>
        /// The input path have to be <see cref="PathType.Absolute"/> and current has to be <see cref="PathType.Relative"/>
        /// </remarks>
        /// <exception cref="ArgumentNullException">If the input is null</exception>
        /// <exception cref="ArgumentException">If the input path is not <see cref="PathType.Absolute"/></exception>
        /// <exception cref="Exception">If the current path is not <see cref="PathType.Relative"/></exception>
        public FileSystemPath MakeAbsolute(FileSystemPath input)
        {
            ArgumentsGuard.ThrowIfNull(input);

            if (input.Type != PathType.Absolute)
                throw new ArgumentException("Input path must be an absolute.");

            if (Type != PathType.Relative)
                throw new Exception("Only relative paths can be convert to absolutes.");

            var resultSegments = new List<String>();

            int returnsCount = 0;

            var inputSegmetns = input._segments;

            for (int i = 0; i < _segments.Length; i++)
            {
                if (_segments[i] == RelativePathSegment)
                    returnsCount++;
                else
                    break;
            }

            for (int i = 0; i < inputSegmetns.Length - returnsCount; i++)
                resultSegments.Add(inputSegmetns[i]);

            for (int i = returnsCount; i < _segments.Length; i++)
                resultSegments.Add(_segments[i]);

            return new FileSystemPath(PathType.Absolute, resultSegments.ToArray());
        }

        /// <summary>
        /// Adds new segments to this instance and creates a new instance.
        /// </summary>
        public FileSystemPath Combine(params String[] arguments)
        {
            var newPath = new String[Length + arguments.Length];

            Array.Copy(_segments, newPath, _segments.Length);
            Array.Copy(arguments, 0, newPath, _segments.Length, arguments.Length);

            return new FileSystemPath(Type, newPath);
        }

        public override bool Equals(object obj)
        {
            return EqualsInternal(this, obj as FileSystemPath);
        }

        public static bool operator==(FileSystemPath first, FileSystemPath second)
        {
            return EqualsInternal(first, second);
        }

        public static bool operator!=(FileSystemPath first, FileSystemPath second)
        {
            return !EqualsInternal(first, second);
        }

        public override int GetHashCode()
        {
            if (!_cachedHashCode.HasValue)
                _cachedHashCode = ToString().ToLower().GetHashCode();

            return _cachedHashCode.Value;
        }

        public override String ToString()
        {
            return ToString(false);
        }

        /// <summary>
        /// Returns initial string representation.
        /// </summary>
        public String ToString(bool useAltSeparator)
        {
            char separator = DirectorySeparatorChar;

            if (useAltSeparator)
                separator = AltDirectorySeparatorChar;

            var builder = new StringBuilder();

            if (Type == PathType.Relative)
            {
                if (_segments[0] != RelativePathSegment)
                    builder.Append(separator);
            }
            else if (Type == PathType.Unknown)
            {
                builder.Append(UnknowPathSegment);
            }

            for (int i = 0; i < _segments.Length; i++)
            {
                if(_wasEscaped)
                    builder.Append(EscapeString(_segments[i]));
                else
                    builder.Append(_segments[i]);

                if (i + 1 < _segments.Length)
                    builder.Append(separator);
            }

            //Add this because Uri from string throws an exception
            //Root directory has to be with a separator after if it is a Windows root
            //or with two separator before if it is UNC
            if (Type == PathType.Absolute && _segments.Length == 1)
            {
                var rootName = _segments[0];

                if (rootName[rootName.Length - 1] == VolumeSeparatorChar)
                {
                    builder.Append(separator);
                }
                else
                {
                    builder.Insert(0, separator);
                    builder.Insert(0, separator);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Replaces restricted symbols to unrestricted.
        /// </summary>
        /// <remarks>
        /// Will not throw an exception if string is null or empty.
        /// </remarks>
        public static String EscapeString(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return name;

            if (!IsNeedToEscape(name))
                return name;

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < name.Length; i++)
            {
                if (_escapeDictionary.TryGetValue(name[i], out String value))
                    stringBuilder.Append(value);
                else
                    stringBuilder.Append(name[i]);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Tries to replace back (to line with restricted characters)
        /// </summary>
        public static bool TryUnescapeString(String name, out String result)
        {
            result = name;

            if (String.IsNullOrWhiteSpace(name))
                return false;

            if (!IsNamePossiblyEscaped(name))
                return false;

            StringBuilder keyBuilder = new StringBuilder();
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] == EscapeChar && i + 2 < name.Length)
                {
                    keyBuilder.Append(name[i]);
                    keyBuilder.Append(name[i + 1]);
                    keyBuilder.Append(name[i + 2].ToString().ToLower());

                    var key = keyBuilder.ToString();

                    if (_unescapeDictionary.TryGetValue(key, out char value))
                        stringBuilder.Append(value);
                    else
                        stringBuilder.Append(key);

                    i += 2;

                    keyBuilder.Clear();
                }
                else
                {
                    stringBuilder.Append(name[i]);
                }
            }

            result = stringBuilder.ToString();

            return true;
        }

        /// <inheritdoc cref="TryUnescapeString(string, out string)"/>
        public static String UnescapeString(String name)
        {
            TryUnescapeString(name, out String result);

            return result;
        }

        /// <summary>
        /// Returns true if this line contains restricted characters.
        /// </summary>
        public static bool IsNeedToEscape(String name)
        {
            for (int i = 0; i < name.Length; i++)
                if (_escapeDictionary.ContainsKey(name[i]))
                    return true;

            return false;
        }

        /// <summary>
        /// Returns true if name contains segments which are used in escaping process.
        /// </summary>
        private static bool IsNamePossiblyEscaped(String name)
        {
            for (int i = 0; i < name.Length; i++)
                if (name[i] == EscapeChar)
                    return true;

            return false;
        }

        private void InitializeFromString(String path, bool isEscaped = true)
        {
            _segments = path.Trim(DirectorySeparatorChar)
                            .Split(DirectorySeparatorChar);

            if (isEscaped)
            {
                for (int i = 0; i < _segments.Length; i++)
                {
                    if (TryUnescapeString(_segments[i], out String result))
                    {
                        _wasEscaped = true;

                        _segments[i] = result;
                    }
                }
            }
            else
            {
                _wasEscaped = false;
            }
        }

        private static bool EqualsInternal(FileSystemPath first, FileSystemPath second)
        {
            if (Object.ReferenceEquals(first, second))
            {
                return true;
            }
            else if (!Object.ReferenceEquals(first, null) && !Object.ReferenceEquals(second, null))
            {
                return first.Type == second.Type &&
                       first.Length == second.Length &&
                       CompareSegments(first._segments, second._segments);
            }
            else
            {
                return false;
            }
        }

        private static bool CompareSegments(String[] first, String[] second)
        {
            for (int i = 0; i < first.Length; i++)
                if (!String.Equals(first[i], second[i], StringComparison.InvariantCultureIgnoreCase))
                    return false;

            return true;
        }
    }
}
