using System;
using Microsoft.SPOT;
using NetduinoLab.Common.Abstract;
using System.IO;

namespace NetduinoLab.Common
{
    //Set date/time on the device before starting logging.
    public class FileLogger : ILogger
    {
        private static FileLogger _instance;
        private static object _lockObject = new Object();

        #region Constructors

        private FileLogger(string filePath, long maxFileSize, bool isRolling)
        {
            this.FilePath = filePath;
            this.MaxFileSize = maxFileSize;
            this.IsRolling = isRolling;
        }

        #endregion

        #region Public Properties

        public static FileLogger Instance
        {
            get
            {
                if (_instance == null)
                    throw new Exception("File logger has not been instantiated.");

                return _instance;
            }
        }

        public static bool Exists { get; private set; }

        public string FilePath { get; private set; }

        public long MaxFileSize { get; private set; }

        public bool IsRolling { get; private set; }

        #endregion

        #region ILogger Members

        public void Log(Severity severity, string message)
        {
            lock (_lockObject)
            {
                if (File.Exists(this.FilePath))
                {
                    var fileInfo = new FileInfo(this.FilePath);

                    if (this.MaxFileSize != -1 && fileInfo.Length > this.MaxFileSize)
                    {
                        if (!this.IsRolling)
                            File.Delete(this.FilePath);
                        else
                            this.rollFile();
                    }
                }

                //Seems to be hit or miss with this working correctly.
                using (var fileStream = new FileStream(this.FilePath, FileMode.Append))
                using (var streamWriter = new StreamWriter(fileStream))
                    streamWriter.WriteLine(DateTime.Now.ToString() + " - " +
                        severity.ToString().ToUpper() + " - " + message);
            }
        }

        #endregion

        #region Public Methods

        public static FileLogger Create(
            string filePath, long maxFileSize = -1, bool isRolling = false)
        {
            if (Exists)
                throw new Exception("File logger has already been create.");

            if (filePath == null || filePath.Trim().Length == 0)
                throw new Exception("Must specify file path for logging.");

            lock (_lockObject)
            {
                _instance = new FileLogger(filePath, maxFileSize, isRolling);

                Exists = true;
            }

            return _instance;
        }

        #endregion

        #region Private Methods

        private void rollFile()
        {
            if (!File.Exists(this.FilePath))
                throw new Exception(
                    "Failed to roll log file.  '" + this.FilePath + "' does not exist.");

            int fileExtensionIndex = this.FilePath.LastIndexOf('.');
            //Use Guid to generate unique file name.
            string rolledFilePath = this.FilePath.Substring(0, fileExtensionIndex) +
                "_" + Guid.NewGuid().ToString() + 
                this.FilePath.Substring(fileExtensionIndex, this.FilePath.Length - fileExtensionIndex);
            
            File.Move(this.FilePath, rolledFilePath);
        }

        #endregion
    }
}
