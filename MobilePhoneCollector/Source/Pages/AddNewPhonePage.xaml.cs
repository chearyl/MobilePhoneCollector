using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MobilePhoneCollector.Source.Core.Database;
using MobilePhoneCollector.Source.Service;
using MobilePhoneCollector.Source.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MobilePhoneCollector.Source.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddNewPhonePage : Page, INotifyPropertyChanged
    {
        private Phone _addPhone = new();
        public Phone AddPhone
        {
            get { return _addPhone; }
            set { _addPhone = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddPhone))); }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public AddNewPhonePage()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            AddPhone = new();
        }

        private void BackToCollection(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PhoneListingPage));
        }

        private async void SearchPhone(object sender, RoutedEventArgs e)
        {
            GsmArenaScraper scrapper = new();
            var (phone, imageList) = await scrapper.SearchPhone(SearchPhoneText.Text);

            if (phone is not null)
            {
                using var db = new DBContext();
                foreach (var image in imageList)
                {
                    var existingImage = db.FileData.Where(f => f.HashCode.Equals(image.Data!.HashCode)).SingleOrDefault();
                    if (existingImage is not null)
                    {
                        image.Data = existingImage;
                        image.DataId = existingImage.Id;
                    }
                    phone.ImageList.Add(image);
                }

                AddPhone = phone;
                ModelText.Text = $"{AddPhone.Brand} {AddPhone.Model}";
                PopulateSpecs();
                System.Diagnostics.Debug.WriteLine($"added : {AddPhone.Model}");
            }
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

        private async Task<FrameworkElement> CreateImageSpace(PhoneImage image, int seq)
        {

            var border = new Border()
            {
                Padding = new Thickness(10),
                Margin = new Thickness(0, 20, 0, 0),
                BorderThickness = new Thickness(1),
                BorderBrush = ColorUtil.HexToColorBrush("acbcc9"),
                CornerRadius = new CornerRadius(10)
            };
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            Image newImage = new()
            {
                Stretch = Stretch.Uniform,
                Width = 300,
                Source = await ImageUtil.ByteArrayToImageSource(image.Data!.Data!)
            };
            Grid.SetRow(newImage, 0);
            grid.Children.Add(newImage);

            var stack = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Spacing = 10,
                Orientation = Orientation.Horizontal
            };
            var text = new TextBlock() { Text = image.FileName, VerticalAlignment = VerticalAlignment.Center };
            var remove = new Button
            {
                Content = "❌",
                Name = $"Remove-{seq}",
                BorderThickness = new Thickness(0),
                Background = new SolidColorBrush(Colors.Transparent)
            };
            remove.Resources["ButtonBackgroundPointerOver"] = new SolidColorBrush(Colors.Transparent);
            remove.Click += (s, e) => { RemoveImage(s, e); };
            stack.Children.Add(text);
            stack.Children.Add(remove);
            Grid.SetRow(stack, 1);
            grid.Children.Add(stack);
            border.Child = grid;

            return border;
        }

        private async void PopulateSpecs()
        {
            SpecsSpace.Children.Clear();
            ImageSpace.Children.Clear();
            Dictionary<string, List<Specs>> specsBycategory = AddPhone.SpecList.GroupBy(x => x.Category).ToDictionary(s => s.Key, s => s.ToList());
            int seq = 0;
            foreach (var category in specsBycategory)
            {
                var uiElement = CreateSpecsSpace(category.Key, category.Value);
                SpecsSpace.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                Grid.SetRow(uiElement, seq++);
                SpecsSpace.Children.Add(uiElement);
            }

            for (int i = 0; i < AddPhone.ImageList.Count; i++)
            {
                var image = await CreateImageSpace(AddPhone.ImageList[i], i);
                ImageSpace.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                Grid.SetRow(image, i);
                ImageSpace.Children.Add(image);
            }
        }

        private async void ShowNewSpecs(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AddPhone.Model)) return;
            AddSpecsModel.IsOpen = !AddSpecsModel.IsOpen;
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AddPhone.Model)) return;
            using var db = new DBContext();
            

            foreach (var image in AddPhone.ImageList)
            {
                if(image.DataId != 0 && image.Data is not null)
                {
                    db.Entry(image.Data).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                }
            }

            var exist = db.Brands.Count(b => b.Name.ToUpper().Equals(AddPhone.Brand.ToUpper()));
            if(exist < 1)
            {
                Brands brand = new() { Name = AddPhone.Brand, LogoId = null };
                db.Brands.Add(brand);
            }

            db.Phone.Add(AddPhone);
            await db.SaveChangesAsync();

            ContentDialog dialog = new()
            {
                XamlRoot = this.XamlRoot,
                PrimaryButtonText = "Back",
                Content = "Save successful"
            };

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary) BackToCollection(new object(), new RoutedEventArgs());
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

        private void RemoveImage(object sender, RoutedEventArgs e)
        {
            var s = (Button)sender;
            var seq = int.Parse(s.Name.Split("-")[1]);
            AddPhone.ImageList.RemoveAt(seq);
            PopulateSpecs();
        }

        private void RemoveSpecs(object sender, RoutedEventArgs e)
        {
            var s = (Button)sender;
            var category = s.Name.Split("-")[1];
            var key = s.Name.Split("-")[2];
            foreach (var specs in AddPhone.SpecList)
            {
                if(specs.Category.Equals(category) && specs.Key.Equals(key))
                {
                    AddPhone.SpecList.Remove(specs);
                    break;
                }
            }
            PopulateSpecs();
        }

        private void AddNewSpecs(object sender, RoutedEventArgs e)
        {
            Specs specs = new()
            {
                Category = NewSpecsCategory.Text,
                Key = NewSpecsKey.Text,
                Value = NewSpecsValue.Text,
            };
            AddPhone.SpecList.Add(specs);
            AddSpecsModel.IsOpen = !AddSpecsModel.IsOpen;
            PopulateSpecs();
        }
    }
}
