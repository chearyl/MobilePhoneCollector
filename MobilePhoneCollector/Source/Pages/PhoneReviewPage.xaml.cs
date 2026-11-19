using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MobilePhoneCollector.Source.Core.Constants;
using MobilePhoneCollector.Source.Core.Database;
using MobilePhoneCollector.Source.Util;
using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MobilePhoneCollector.Source.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PhoneReviewPage : Page
{
    private DBContext _dbContext;
    public Phone Phone
    {
        get => (Phone)GetValue(PlayerProperty);
        set => SetValue(PlayerProperty, value);
    }

    public static readonly DependencyProperty PlayerProperty = DependencyProperty.Register(
        nameof(Phone), typeof(Phone), typeof(PhoneReviewPage), new PropertyMetadata(null, OnPhoneChanged));

    private static void OnPhoneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((PhoneReviewPage)d).LoadData();


    public PhoneReviewPage()
    {
        InitializeComponent();
        _dbContext = new();
    }

    private void LoadData()
    {
        FileListing.Children.Clear();
        for (int i = 0; i < Phone.ReviewList.Count; i++) 
        {
            var border = new Border
            {
                BorderThickness = (i + 1) == Phone.ReviewList.Count ? new Thickness(0) : new Thickness(0, 0, 0, 1),
                BorderBrush = ColorUtil.HexToColorBrush("edeef1"),
                Padding = new Thickness(20)
            };
            var grid = new Grid
            {
                Background = new SolidColorBrush(Colors.White)
            };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(300) });

            var infoStack = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Spacing = 10
            };
            var infoName = new TextBlock
            {
                Text = $"{Phone.ReviewList[i].FileName}",
                FontWeight = new Windows.UI.Text.FontWeight(600),
                FontSize = 15
            };
            var infoInfo = new TextBlock
            {
                Text = $"{StringFormatUtil.FormatStorageUnit(Phone.ReviewList[i].FileSize)}ㆍAdded {Phone.ReviewList[i].CreatedDate}",
                Foreground = ColorUtil.HexToColorBrush("6e7784"),
                FontSize = 12
            };
            infoStack.Children.Add(infoName);
            infoStack.Children.Add(infoInfo);

            var buttonStack = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 10,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            var openButton = new Button
            {
                Content = "Open",
                BorderThickness = new Thickness(1),
                BorderBrush = ColorUtil.HexToColorBrush("cdd1d5"),
                Name = $"Open-{i}"
            };
            openButton.Click += (s, e) => { OpenDoc(s, e); };
            var deleteButton = new Button
            {
                Content = "Delete",
                BorderThickness = new Thickness(1),
                BorderBrush = ColorUtil.HexToColorBrush("cdd1d5"),
                Name = $"Delete-{i}"
            };
            deleteButton.Click += (s, e) => { DeleteDoc(s, e); };
            buttonStack.Children.Add(openButton);
            buttonStack.Children.Add(deleteButton);

            Grid.SetColumn(infoStack, 0);
            Grid.SetColumn(buttonStack, 1);
            grid.Children.Add(infoStack);
            grid.Children.Add(buttonStack);
            border.Child = grid;
            FileListing.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            Grid.SetRow(border, i);
            FileListing.Children.Add(border);
        }
    }

    private async void AddNewFile(Object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.Thumbnail,
            FileTypeFilter = { ".pdf", ".doc", ".docx", ".html", ".txt", ".md" },
        };

        nint windowHandle = WindowNative.GetWindowHandle(App.Window);
        InitializeWithWindow.Initialize(picker, windowHandle);
        StorageFile file = await picker.PickSingleFileAsync();
        if (file is not null)
        {
            PhoneReview newReview = new()
            {
                FileName = file.Name,
                FileType = file.FileType,
                PhoneId = Phone.Id,
                CreatedDate = DateTime.Now.ToString()
            };
            var bytes = await File.ReadAllBytesAsync(file.Path);
            newReview.FileSize = bytes.Length;
            string hashCode = HashUtil.ByteToMd5(bytes);
            var existingFile = _dbContext.FileData.FirstOrDefault(f => f.HashCode.Equals(hashCode));
            if (existingFile is not null) newReview.DataId = existingFile.Id;
            else
            {
                FileData data = new()
                {
                    Data = bytes,
                    HashCode = hashCode
                };
                newReview.Data = data;
            }
            _dbContext.PhoneReview.Add(newReview);
            await _dbContext.SaveChangesAsync();
            Phone.ReviewList.Add(newReview);
            LoadData();
        }
    }

    private async void OpenDoc(Object sender, RoutedEventArgs e)
    {
        var s = sender as Button;
        var index = int.Parse(s.Name.Split("-")[1]);
        var openDoc = Phone.ReviewList[index];
        openDoc.Data = _dbContext.FileData.First(f => f.Id == openDoc.DataId);
        try
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), Constants.AppProgramName, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempFolder);

            string tempFilePath = Path.Combine(tempFolder, openDoc.FileName);
            await File.WriteAllBytesAsync(tempFilePath, openDoc.Data.Data!);
            File.SetAttributes(tempFilePath, System.IO.FileAttributes.ReadOnly);
            StorageFile file = await StorageFile.GetFileFromPathAsync(tempFilePath);
            bool success = await Launcher.LaunchFileAsync(file);

            if (!success)
            {
                // Handle case where MS Word or a .docx viewer is not installed
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to open binary docx: {ex.Message}");
        }
    }

    private void DeleteDoc(Object sender, RoutedEventArgs e)
    {
        var s = sender as Button;
        var index = int.Parse(s.Name.Split("-")[1]);
        var deleteDoc = Phone.ReviewList[index];
        var totalUsing = _dbContext.PhoneReview.Count(p => p.DataId == deleteDoc.DataId);
        Phone.ImageList.RemoveAt(index);
        _dbContext.PhoneReview.Remove(deleteDoc);
        if (totalUsing < 2)
        {
            deleteDoc.Data = _dbContext.FileData.First(f => f.Id == deleteDoc.DataId);
            _dbContext.FileData.Remove(deleteDoc.Data!);
        }
        _dbContext.SaveChangesAsync();
        LoadData();
    }
}
