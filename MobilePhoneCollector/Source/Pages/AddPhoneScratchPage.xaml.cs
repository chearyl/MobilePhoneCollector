using Microsoft.EntityFrameworkCore;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MobilePhoneCollector.Source.Core.Database;
using MobilePhoneCollector.Source.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MobilePhoneCollector.Source.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddPhoneScratchPage : Page
    {
        private DBContext _dbContext;

        public Phone Phone;
        public AddPhoneScratchPage()
        {
            InitializeComponent();
            Phone = new();
            _dbContext = new();
        }

        private void BackToCollection(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PhoneListingPage));
        }

        private void ToGsmArena(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddNewPhonePage));
        }

        private async void ShowNewSpecs(object sender, RoutedEventArgs e)
        {
            AddSpecsModel.IsOpen = !AddSpecsModel.IsOpen;
        }

        private void ScreenSizeChange(object sender, SizeChangedEventArgs e)
        {
            double height = RootGrid.ActualHeight;
            double width = RootGrid.ActualWidth;
            AddSpecsOverly.Height = height;
            AddSpecsOverly.Width = width;

            AddSpecsBorder.Height = Math.Max(0, height - 700);
            AddSpecsBorder.Width = Math.Min(400, width - 32);
        }

        private void AddNewSpecs(object sender, RoutedEventArgs e)
        {
            Specs specs = new()
            {
                Category = NewSpecsCategory.Text,
                Key = NewSpecsKey.Text,
                Value = NewSpecsValue.Text,
            };
            Phone.SpecList.Add(specs);
            AddSpecsModel.IsOpen = !AddSpecsModel.IsOpen;
            PopulateSpecs();
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            var model = PhoneModelText.Text;
            var brand = BrandText.Text;
            if (string.IsNullOrEmpty(model)) return;
            Phone.Model = model;
            Phone.Brand = brand;
            Phone.CreatedDate = DateTime.Now.ToString();

            foreach (var image in Phone.ImageList)
            {
                if (image.Existing && image.Data is not null)
                {
                    _dbContext.Entry(image.Data).State = EntityState.Unchanged;
                }
            }

            foreach (var review in Phone.ReviewList)
            {
                if (review.Existing && review.Data is not null)
                {
                    _dbContext.Entry(review.Data).State = EntityState.Unchanged;
                }
            }

            var exist = _dbContext.Brands.Count(b => b.Name.ToUpper().Equals(Phone.Brand.ToUpper()));
            if (exist < 1)
            {
                Brands newBrand = new() { Name = Phone.Brand };
                _dbContext.Brands.Add(newBrand);
            }

            _dbContext.Phone.Add(Phone);
            await _dbContext.SaveChangesAsync();

            ContentDialog dialog = new()
            {
                XamlRoot = this.XamlRoot,
                PrimaryButtonText = "Back",
                Content = "Save successful"
            };

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary) BackToCollection(new object(), new RoutedEventArgs());
        }

        private FrameworkElement CreateSpecsSpace(string category, List<Specs> specs)
        {
            var border = new Border
            {
                Padding = new Thickness(10),
                Margin = new Thickness(0, 20, 0, 0),
                BorderThickness = new Thickness(1),
                BorderBrush = ColorUtil.HexToColorBrush("acbcc9"),
                CornerRadius = new CornerRadius(10)
            };

            var grid = new Grid
            {
                RowSpacing = 10,
            };
            for (int i = 0; i <= specs.Count; i++) grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            var categoryTitle = new TextBlock
            {
                Text = category.ToUpper(),
                Foreground = ColorUtil.HexToColorBrush("13508e"),
                FontWeight = new Windows.UI.Text.FontWeight(600)
            };
            Grid.SetColumn(categoryTitle, 0);
            grid.Children.Add(categoryTitle);

            for (int i = 0; i < specs.Count; i++)
            {
                var specsGrid = new Grid();
                specsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
                specsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                specsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                Grid.SetRow(specsGrid, (i + 1));

                var specsTitle = new TextBlock
                {
                    FontWeight = new Windows.UI.Text.FontWeight(600),
                    Text = specs[i].Key,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center
                };
                var specsValue = new TextBox
                {
                    TextWrapping = TextWrapping.Wrap,
                    AcceptsReturn = true,
                    IsReadOnly = true,
                    Text = specs[i].Value,
                };
                var remove = new Button
                {
                    Content = "❌",
                    Name = $"Remove-{specs[i].Category}-{specs[i].Key}",
                    BorderThickness = new Thickness(0),
                    Background = new SolidColorBrush(Colors.Transparent),
                    Foreground = new SolidColorBrush(Colors.Transparent)
                };
                remove.Resources["ButtonBackgroundPointerOver"] = new SolidColorBrush(Colors.Transparent);
                remove.Click += (s, e) => { RemoveSpecs(s, e); };
                Grid.SetColumn(specsTitle, 0);
                Grid.SetColumn(specsValue, 1);
                Grid.SetColumn(remove, 2);
                specsGrid.Children.Add(specsTitle);
                specsGrid.Children.Add(specsValue);
                specsGrid.Children.Add(remove);
                grid.Children.Add(specsGrid);
            }

            border.Child = grid;
            return border;
        }

        private async void CreateImageSpace()
        {
            for (int i = 0; i < Phone.ImageList.Count; i++) 
            {
                var border = new Border()
                {
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 20, 0, 0),
                    BorderThickness = new Thickness(1),
                    BorderBrush = ColorUtil.HexToColorBrush("acbcc9"),
                    CornerRadius = new CornerRadius(10),
                    Width = 100,
                    Height = 150,
                };
                var grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

                Image newImage = new()
                {
                    Stretch = Stretch.Uniform,
                    Width = 100,
                    Height = 100,
                    Source = await ImageUtil.ByteArrayToImageSource(Phone.ImageList[i].Data!.Data!)
                };
                Grid.SetRow(newImage, 0);
                grid.Children.Add(newImage);
                var remove = new Button
                {
                    Content = "❌",
                    Name = $"Image-{i}",
                    BorderThickness = new Thickness(0),
                    Background = new SolidColorBrush(Colors.Transparent),
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                remove.Resources["ButtonBackgroundPointerOver"] = new SolidColorBrush(Colors.Transparent);
                remove.Click += (s, e) => { DeleteFile(s, e); };
                Grid.SetRow(remove, 1);
                grid.Children.Add(remove);
                border.Child = grid;
                ImageSpace.Items.Add(border);
            }
        }

        private void CreateReviewSpace()
        {
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
                    Text = $"{StringFormatUtil.FormatStorageUnit(Phone.ReviewList[i].FileSize)}",
                    Foreground = ColorUtil.HexToColorBrush("6e7784"),
                    FontSize = 12
                };
                infoStack.Children.Add(infoName);
                infoStack.Children.Add(infoInfo);

                var deleteButton = new Button
                {
                    Content = "Delete",
                    BorderThickness = new Thickness(1),
                    BorderBrush = ColorUtil.HexToColorBrush("cdd1d5"),
                    Name = $"Review-{i}",
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                deleteButton.Click += (s, e) => { DeleteFile(s, e); };

                Grid.SetColumn(infoStack, 0);
                Grid.SetColumn(deleteButton, 1);
                grid.Children.Add(infoStack);
                grid.Children.Add(deleteButton);
                border.Child = grid;
                ReviewSpace.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                Grid.SetRow(border, i);
                ReviewSpace.Children.Add(border);
            }
        }

        private async void PopulateSpecs()
        {
            SpecsSpace.Children.Clear();
            ImageSpace.Items.Clear();
            ReviewSpace.Children.Clear();
            Dictionary<string, List<Specs>> specsBycategory = Phone.SpecList.GroupBy(x => x.Category).ToDictionary(s => s.Key, s => s.ToList());
            int seq = 0;
            foreach (var category in specsBycategory)
            {
                var uiElement = CreateSpecsSpace(category.Key, category.Value);
                SpecsSpace.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                Grid.SetRow(uiElement, seq++);
                SpecsSpace.Children.Add(uiElement);
            }

            if (Phone.ImageList.Count > 0) ImageBorder.Visibility = Visibility.Visible;
            else ImageBorder.Visibility = Visibility.Collapsed;
            if (Phone.ReviewList.Count > 0) ReviewBorder.Visibility = Visibility.Visible;
            else ReviewBorder.Visibility = Visibility.Collapsed;
            CreateImageSpace();
            CreateReviewSpace();
        }

        private void RemoveSpecs(object sender, RoutedEventArgs e)
        {
            var s = (Button)sender;
            var category = s.Name.Split("-")[1];
            var key = s.Name.Split("-")[2];
            foreach (var specs in Phone.SpecList)
            {
                if (specs.Category.Equals(category) && specs.Key.Equals(key))
                {
                    Phone.SpecList.Remove(specs);
                    break;
                }
            }
            PopulateSpecs();
        }

        private async void AddNewFile(object sender, RoutedEventArgs e)
        {
            var s = sender as Button;
            string[] fileType = [];
            FileOpenPicker picker = new();

            if(s.Name.Equals("AddImageBtn"))
            {
                picker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    FileTypeFilter = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" },
                };
            }
            else if(s.Name.Equals("AddReviewBtn"))
            {
                picker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    FileTypeFilter = { ".pdf", ".doc", ".docx", ".html", ".txt", ".md" },
                };
            }

            nint windowHandle = WindowNative.GetWindowHandle(App.Window);
            InitializeWithWindow.Initialize(picker, windowHandle);
            StorageFile file = await picker.PickSingleFileAsync();
            if (file is not null)
            {
                if (s.Name.Equals("AddImageBtn"))
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
                    if (existingFile is not null)
                    {
                        newImage.Data = existingFile;
                        newImage.Existing = true;
                    }
                    else
                    {
                        FileData data = new()
                        {
                            Data = bytes,
                            HashCode = hashCode
                        };
                        newImage.Data = data;
                        newImage.Existing = false;
                    }
                    Phone.ImageList.Add(newImage);
                }
                else if(s.Name.Equals("AddReviewBtn"))
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
                    if (existingFile is not null)
                    {
                        newReview.Data = existingFile;
                        newReview.Existing = true;
                    }
                    else
                    {
                        FileData data = new()
                        {
                            Data = bytes,
                            HashCode = hashCode
                        };
                        newReview.Data = data;
                        newReview.Existing = false;
                    }
                    Phone.ReviewList.Add(newReview);
                }
                    
            }
            PopulateSpecs();
        }

        private void DeleteFile(Object sender, RoutedEventArgs e)
        {
            var s = sender as Button;
            string type = s.Name.Split("-")[0];
            int index = int.Parse(s.Name.Split("-")[1]);

            if(type.Equals("Image")) Phone.ImageList.RemoveAt(index);
            else if(type.Equals("Review")) Phone.ReviewList.RemoveAt(index);
            PopulateSpecs();
        }
    }
}
