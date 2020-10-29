using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedSharp.FileSystem.Helpers
{
    public static class DriveItemEscapingHelper
    {
        public static readonly char DirectorySeparatorChar = '\\';
        public static readonly char AltDirectorySeparatorChar = '/';
        public static readonly char VolumeSeparatorChar = ':';
        public static readonly char PathSeparatorChar = ';';

        public static readonly String DirectorySeparatorString = DirectorySeparatorChar.ToString();
        public static readonly String AltDirectorySeparatorString = AltDirectorySeparatorChar.ToString();

        public static readonly char EscapeChar = '%';
        public static readonly String RelativePathHolder = "..";
        public static readonly String UnknowPathHolder = "??";

        private static readonly char[] InvalidPathChars =
        {
            '\"', '<', '>', '|', '\0',
            (char)1,  (char)2,  (char)3,  (char)4,  (char)5,  (char)6,  (char)7,  (char)8,  (char)9,  (char)10,
            (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
            (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
            (char)31
        };

        private static HashSet<char> _invalidFileNamesChars;
        private static Dictionary<char, String> _escapeDictionary;
        private static Dictionary<String, char> _unescapeDictionary;

        static DriveItemEscapingHelper()
        {
            _invalidFileNamesChars = new HashSet<char>();

            foreach (var item in InvalidPathChars)
                _invalidFileNamesChars.Add(item);

            _invalidFileNamesChars.Add(AltDirectorySeparatorChar);
            _invalidFileNamesChars.Add(DirectorySeparatorChar);
            _invalidFileNamesChars.Add(PathSeparatorChar);
            _invalidFileNamesChars.Add(VolumeSeparatorChar);

            _escapeDictionary = new Dictionary<char, string>()
            {
                { DirectorySeparatorChar, "%5c"}, { AltDirectorySeparatorChar, "%2f"},

                { '\"',     "%93"}, { '<',         "%3c"}, { '>',      "%3e"}, { '|',      "%7c"}, { '\0',     "%00"},
                { (char)1,  "%01"}, { (char)2,     "%02"}, { (char)3,  "%03"}, { (char)4,  "%04"}, { (char)5,  "%05"},
                { (char)6,  "%06"}, { (char)7,     "%07"}, { (char)8,  "%08"}, { (char)9,  "%09"}, { (char)10, "%0a"},
                { (char)11, "%0b"}, { (char)12,    "%0c"}, { (char)13, "%0d"}, { (char)14, "%0e"}, { (char)15, "%0f"},
                { (char)16, "%10"}, { (char)17,    "%11"}, { (char)18, "%12"}, { (char)19, "%13"}, { (char)20, "%14"},
                { (char)21, "%15"}, { (char)22,    "%16"}, { (char)23, "%17"}, { (char)24, "%18"}, { (char)25, "%19"},
                { (char)26, "%1a"}, { (char)27,    "%1b"}, { (char)28, "%1c"}, { (char)29, "%1d"}, { (char)30, "%1e"},
                { (char)31, "%1f"}, { EscapeChar,  "%25"}
            };

            _unescapeDictionary = _escapeDictionary.ToDictionary(item => item.Value, item => item.Key);
        }

        public static String[] Combine(String[] existed, params String[] parameters)
        {
            if (existed == null)
                return Array.Empty<String>();

            if (existed.Length == 0 || parameters.Length == 0)
                return existed;

            var result = new String[existed.Length + parameters.Length];

            Array.Copy(existed, result, existed.Length);
            Array.Copy(parameters, 0, result, existed.Length, parameters.Length);

            return result;
        }

        /// <summary>
        /// Returns false if an input name is null, whitespace or contains forbidden characters.
        /// </summary>
        public static bool IsNameValid(String name, params char[] except)
        {
            if (String.IsNullOrWhiteSpace(name))
                return false;

            except = except ?? Array.Empty<char>();

            foreach (var letter in name)
                if (!except.Contains(letter) && _invalidFileNamesChars.Contains(letter))
                    return false;

            return true;
        }

        public static bool IsNameEscaped(String name)
        {
            if (!IsNameValid(name))
                return false;

            HashSet<String> fragments = new HashSet<String>();

            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] == '%')
                {
                    if (i + 2 >= name.Length)
                        return false;

                    var escapedPart = ($"{name[i]}{name[i + 1]}{name[i + 2]}").ToLower();

                    fragments.Add(escapedPart);

                    i += 2;
                }
            }

            return fragments.All(item => _unescapeDictionary.ContainsKey(item));
        }

        public static String EscapeString(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return name;

            if (!IsNeedToEscape(name))
                return name;

            StringBuilder stringBuilder = new StringBuilder();

            foreach (var letter in name)
            {
                if (_escapeDictionary.TryGetValue(letter, out String value))
                    stringBuilder.Append(value);
                else
                    stringBuilder.Append(letter);
            }

            return stringBuilder.ToString();
        }

        public static String UnescapeString(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return name;

            if (!IsNameEscaped(name))
                return name;

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] == '%')
                {
                    var escapedPart = ($"{name[i]}{name[i + 1]}{name[i + 2]}").ToLower();

                    stringBuilder.Append(_unescapeDictionary[escapedPart]);

                    i += 2;
                }
                else
                {
                    stringBuilder.Append(name[i]);
                }
            }

            return stringBuilder.ToString();
        }

        private static bool IsNeedToEscape(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return false;

            foreach (var letter in name)
                if (_escapeDictionary.ContainsKey(letter))
                    return true;

            return false;
        }

    }
}
