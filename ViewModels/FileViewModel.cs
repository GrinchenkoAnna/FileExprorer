﻿using System.IO;

namespace FileExplorer.ViewModels
{
    public sealed class FileViewModel : FileEntityViewModel
    {
        public FileViewModel(string name) : base(name) { }

        public FileViewModel(FileInfo fileInfo) : base(fileInfo.Name) { FullName = fileInfo.FullName; }
    }
}