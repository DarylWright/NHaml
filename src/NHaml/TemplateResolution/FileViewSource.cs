using System;
using System.IO;

namespace NHaml.TemplateResolution
{
    public class FileViewSource : ViewSource
    {
        private readonly FileInfo _fileInfo;

        public FileViewSource(FileInfo fileInfo)
        {
            if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));

            fileInfo.Refresh();

            if (!fileInfo.Exists) throw new FileNotFoundException("FileNotFound", fileInfo.FullName);
            
            _fileInfo = fileInfo;
        }

        public override TextReader GetTextReader()
        {
            return new StreamReader(_fileInfo.FullName);
        }

        public override string FilePath => _fileInfo.FullName;

        public override string FileName => _fileInfo.Name;

        public override DateTime TimeStamp => File.GetLastWriteTime(FilePath);
    }
}