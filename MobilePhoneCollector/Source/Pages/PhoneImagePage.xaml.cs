using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MobilePhoneCollector.Source.Core.Database;
using MobilePhoneCollector.Source.Util;
using System;
using System.IO;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MobilePhoneCollector.Source.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PhoneImagePage : Page
{
    private DBContext _dbContext;
    private int _selectedImage = 0;
    public Phone Phone
    {
        get => (Phone)GetValue(PlayerProperty);
        set => SetValue(PlayerProperty, value);
    }

    public static readonly DependencyProperty PlayerProperty = DependencyProperty.Register(
        nameof(Phone), typeof(Phone), typeof(PhoneImagePage), new PropertyMetadata(null, OnPhoneChanged));

    private static void OnPhoneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((PhoneImagePage)d).LoadData();


    public PhoneImagePage()
    {
        InitializeComponent();
        _dbContext = new();
    }

    private void LoadData()
    {
        CreateImageList();
        LoadSelectedImage();
    }

    private async void LoadSelectedImage()
    {
        byte[]? displayImage = null;
        if(Phone.ImageList.Count > 0)
        {
            var selectedImage = Phone.ImageList[_selectedImage];
            displayImage = _dbContext.FileData.First(f => f.Id == selectedImage.DataId).Data;
            ImageName.Text = selectedImage.FileName;
            var (width, height, format, size) = ImageUtil.GetImageInfo(displayImage!);
            ImageResolution.Text = $"{width} x {height}";
            ImageFormat.Text = format;
            ImageSize.Text = $"{StringFormatUtil.FormatStorageUnit(size)}";
            ImageCover.Text = selectedImage.IsCover ? "Cover Image" : "Not Cover Image";
            ImageAdded.Text = $"Added {selectedImage.CreatedDate}";
            SetCoverBtn.Visibility = selectedImage.IsCover ? Visibility.Collapsed : Visibility.Visible;
        }
        DisplayImagePanel.Source = await ImageUtil.ByteArrayToImageSource(displayImage);
    }

    private async void CreateImageList()
    {
        ImageList.Children.Clear();
        for (int i = 0; i < Phone.ImageList.Count; i++) 
        {
            var img = Phone.ImageList[i];
            var border = new Border
            {
                BorderBrush = _selectedImage == i ? ColorUtil.HexToColorBrush("0c5295") : new SolidColorBrush(Colors.Transparent),
                BorderThickness = new Thickness(3),
                CornerRadius = new CornerRadius(10),
                Background = ColorUtil.HexToColorBrush("eef1f6"),
                Padding = new Thickness(10),
                Name = $"Image-{i}"
            };
            border.Tapped += (s, e) => { ChangeImage(s, e); };
            var binaryImage = _dbContext.FileData.First(d => d.Id == img.DataId).Data;
            var image = new Image
            {
                Width = 100,
                Height = 100,
                Source = await ImageUtil.ByteArrayToImageSource(binaryImage)
            };
            border.Child = image;
            ImageList.Children.Add(border);
        }
    }

    private void ChangeImage(Object sender, RoutedEventArgs e)
    {
        var border = sender as Border;
        var previous = FindName($"Image-{_selectedImage}") as Border;
        previous!.BorderBrush = new SolidColorBrush(Colors.Transparent);
        _selectedImage = int.Parse(border!.Name.Split("-")[1]);
        LoadSelectedImage();
        border.BorderBrush = ColorUtil.HexToColorBrush("0c5295");
    }

    private async void AddNewFile(Object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.Thumbnail,
            SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            FileTypeFilter = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" },
        };

        nint windowHandle = WindowNative.GetWindowHandle(App.Window);
        InitializeWithWindow.Initialize(picker, windowHandle);
        StorageFile file = await picker.PickSingleFileAsync();
        if(file is not null)
        {
            PhoneImage newImage = new()
            {
                FileName = file.Name,
                FileType = file.FileType,
                PhoneId = Phone.Id,
                CreatedDate = DateTime.Now.ToString()
            };
            var bytes = await File.ReadAllBytesAsync(file.Path);
            string hashCode = HashUtil.ByteToMd5(bytes);
            var existingFile = _dbContext.FileData.FirstOrDefault(f => f.HashCode.Equals(hashCode));
            if(existingFile is not null) newImage.DataId = existingFile.Id;
            else
            {
                FileData data = new()
                {
                    Data = bytes,
                    HashCode = hashCode
                };
                newImage.Data = data;
            }
            _dbContext.PhoneImage.Add(newImage);
            await _dbContext.SaveChangesAsync();
            Phone.ImageList.Add(newImage);
        }
        LoadData();
    }

    private void DeleteImage(Object sender, RoutedEventArgs e)
    {
        var selectedImage = Phone.ImageList[_selectedImage];
        selectedImage = _dbContext.PhoneImage.First(i => i.Id == selectedImage.Id);
        var totalUsing = _dbContext.PhoneImage.Count(p => p.DataId == selectedImage.DataId);
        Phone.ImageList.RemoveAt(_selectedImage);
        _dbContext.PhoneImage.Remove(selectedImage);
        if(totalUsing < 2)
        {
            _dbContext.FileData.Remove(selectedImage.Data!);
        }
        _dbContext.SaveChangesAsync();
        _selectedImage = 0;
        LoadData();
    }

    private void SetCover(Object sender, RoutedEventArgs e)
    {
        foreach (var img in Phone.ImageList)
        {
            img.IsCover = false;
        }
        Phone.ImageList[_selectedImage].IsCover = true;
        var selectedImage = _dbContext.PhoneImage.First(i => i.Id == Phone.ImageList[_selectedImage].Id);
        selectedImage.IsCover = true;
        _dbContext.PhoneImage.Update(selectedImage);
        _dbContext.SaveChangesAsync();
        LoadSelectedImage();
    }
}
