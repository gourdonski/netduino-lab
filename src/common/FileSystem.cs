using System;
using Microsoft.SPOT;
using System.IO;
using System.Collections;

namespace NetduinoLab.Common
{
    public static class FileSystem
    {
        #region Public Properties

        public static string RootDirectory
        { 
            get 
            {
                return @"\SD";
            } 
        }

        #endregion

        #region Public Methods

        public static bool ReadFile(string filePath, out string text)
        {
            if (!File.Exists(filePath))
            {
                text = null;
                
                return false;
            }

            try
            {
                using (var streamReader = new StreamReader(filePath))
                    text = streamReader.ReadToEnd();

                return true;
            }
            catch
            {
                text = null;

                return false;
            }
        }

        public static bool ReadFile(string filePath, out FileStream fileStream)
        {
            if (!File.Exists(filePath))
            {
                fileStream = null;

                return false;
            }

            try
            {
                fileStream = new FileStream(filePath, FileMode.Open);

                return true;
            }
            catch
            {
                fileStream = null;

                return false;
            }
        }

        public static bool WriteFile(
            string filePath, byte[] fileBytes, bool overwrite = false)
        {
            try
            {
                if (File.Exists(filePath) && !overwrite)
                    return false;

                File.WriteAllBytes(filePath, fileBytes);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string[] GetFilePath(string filename)
        {
            ArrayList paths = getFilePaths(
                filename, new DirectoryInfo(RootDirectory));

            string[] filePaths;

            if (paths.Count > 0)
            {
                filePaths = new string[paths.Count];

                for (int i = 0; i < paths.Count; i++)
                    filePaths[i] = (string)paths[i];
            }
            else
                filePaths = new string[0];

            return filePaths;
        }

        public static string GetFileExtension(string filePath)
        {
            string extension = Path.GetExtension(filePath);

            if (extension == string.Empty)
                extension = null;

            return extension;
        }

        public static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        #endregion

        #region Private Methods

        private static ArrayList getFilePaths(
            string filename, DirectoryInfo currentDirectory)
        {
            var filePaths = new ArrayList();

            foreach (FileInfo fileInfo in currentDirectory.GetFiles())
                if (fileInfo.Name == filename)
                    filePaths.Add(fileInfo.FullName);

            foreach (DirectoryInfo subdirectory in currentDirectory.GetDirectories())
                foreach (string filepath in getFilePaths(filename, subdirectory))
                    filePaths.Add(filePaths);

            return filePaths;
        }

        #endregion
    }
}
