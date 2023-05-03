﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;



namespace FileExplorer.ViewModels
{
    internal class FileEntityToImageConverter : IValueConverter

    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var drawingGroup = new DrawingImage();

            if (value is FileEntityViewModel entityViewModel)
            {
                switch (entityViewModel)
                {
                    case DirectoryViewModel directoryViewModel:
                        return Application.Current.FindResource("FolderIconImage");

                    case FileEntityViewModel fileEntityViewModel:
                        return Application.Current.FindResource("FileIconImage");
                }
            }

            return drawingGroup;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}