using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using System;

namespace Engine.Utility
{
    /// <summary>
    /// A read-only stream of all the rules files in a given directory
    /// </summary>
    public class DirectoryRulesStreams: IRulesStreamsProvider
    {
        private const string SearchPattern = "*.rules";
        private readonly List<FileInfo> _rulesFiles;

        private static ILog Log => LogManager.GetLogger(typeof(DirectoryRulesStreams));

        /// <summary>
        /// Initialize the stream with all the .rules files in a directory (non-recursive) 
        /// </summary>
        /// <param name="path">a directory, or null for the current directory</param>
        public DirectoryRulesStreams(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = Directory.GetCurrentDirectory();
            }
            else if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), path)))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), path);
            }
            else if (!Directory.Exists(path))
            {
                throw new FileNotFoundException($"The path '{path}' does not exist.");
            }

            _rulesFiles = new List<FileInfo>(
                Directory.GetFiles(path, SearchPattern, SearchOption.TopDirectoryOnly)
                .Select(x => new FileInfo(x))
                );
            if (_rulesFiles.Count == 0) Log.Warn($"No rules files found in {path} directory.");
        }

        // ReSharper disable once CoVariantArrayConversion
        public Stream[] Streams => _rulesFiles.Select(file => new FileStream(file.FullName, FileMode.Open, FileAccess.Read)).ToArray();

    }
}
