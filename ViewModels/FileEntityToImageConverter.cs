using Avalonia;
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
            //var drawingGroup = new DrawingImage();
            var image = new Image();

            if (value is FileEntityViewModel entityViewModel)
            {
                switch (entityViewModel)
                {
                    case DirectoryViewModel directoryViewModel:
                        return Application.Current.FindResource("FolderIconImage");                    

                    case FileViewModel fileEntityViewModel:
                        switch (Path.GetExtension(fileEntityViewModel.Name))
                        {
                            case ".txt":
                            case ".log":
                            case ".gitattributes":
                            case ".gitignore":
                                return Application.Current.FindResource("TextIconImage");

                            case ".doc":
                            case ".docx":
                            case ".rtf":
                                return Application.Current.FindResource("MWordIconImage");
                                                            
                            case ".xlsx":
                            case ".csv":
                                return Application.Current.FindResource("MExcelIconImage");

                            case ".vsix":
                            case ".sln":
                            case ".cpp":
                            case ".c":
                            case ".cs":
                            case ".csproj":
                            case ".html":
                            case ".xml":
                            case ".xaml":
                            case ".axaml":
                                return Application.Current.FindResource("CodeIconImage");
                            
                            case ".bmp":
                            case ".png":
                            case ".jpeg":
                            case ".jpg":
                            case ".svg":
                            case ".tiff":
                            case ".gif":
                            case ".ico":
                            case ".webp":
                                return Application.Current.FindResource("PictureIconImage");

                            default:
                                return Application.Current.FindResource("FileIconImage");
                        }
                                            
                }
            }

            //return drawingGroup;
            return image;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
