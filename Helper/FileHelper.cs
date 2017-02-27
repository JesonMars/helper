using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Helper
{
    public class FileHelper
    {
        /// <summary>
        /// based on the file path to get the file extension
        /// </summary>
        /// <param name="path">file full path</param>
        /// <returns>file extension</returns>
        public static string GetFileExtension(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                var file = new FileInfo(path);
                return file.Extension;
            }
            return "";
        }
    }
}
