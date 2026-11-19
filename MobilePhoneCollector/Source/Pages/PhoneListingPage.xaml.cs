using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using MobilePhoneCollector.Source.Core.Database;
using MobilePhoneCollector.Source.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MobilePhoneCollector.Source.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PhoneListingPage : Page
    {
        private string _selectedBrand = "All";
        private string _searchKey = string.Empty;
        private DBContext _dbContext;
        private readonly int _displaySize = 24;
        private int _currentPage = 0;
        private int _totalPage = 0;

        public List<Brands> Brands = [];
        public List<Phone> PhoneList = [];
        public int CurrentPage => _currentPage + 1;
        public int TotalPage => _totalPage;

        public PhoneListingPage()
        {
            InitializeComponent();
            _dbContext = new DBContext();
            LoadBrandOption();
            LoadPhoneList();
        }

        private void LoadPhoneList()
        {
            var pl = _dbContext.Phone.OrderByDescending(x => x.Id).ToList();
            if (!_selectedBrand.Equals("All")) pl = [.. pl.Where(p => p.Brand.Equals(_selectedBrand))];
            if (!string.IsNullOrEmpty(_searchKey)) pl = [.. pl.Where(p => LikeOperator.LikeString(p.Model, _searchKey, CompareMethod.Text))];
            PhoneList = [.. pl.Skip(_currentPage * _displaySize).Take(_displaySize)];
            CreatePhoneCard();

            int displaySize = _displaySize;
            if (PhoneList.Count < _displaySize) displaySize = pl.Count;
            TotalDisplay.Text = $"Showing {displaySize} of {pl.Count} phones";
            _totalPage = pl.Count / _displaySize;
            if (pl.Count % _displaySize != 0) _totalPage++;

            PageDisplay.Text = $"Page {_currentPage + 1} of {_totalPage}";

            PreviousPageBtn.IsEnabled = true;
            NextPageBtn.IsEnabled = true;

            if (_currentPage == 0)
            {
                PreviousPageBtn.IsEnabled = false;
            }
            
            if(_currentPage == (_totalPage - 1))
            {
                NextPageBtn.IsEnabled = false;
            }
        }

        private async void CreatePhoneCard()
        {
            PhoneListingGrid.Children.Clear();
            int columnSeq = 0, rowSeq = -1;
            foreach (var p in PhoneList)
            {
                if (columnSeq >= 6) columnSeq = 0;
                if (columnSeq == 0)
                {
                    PhoneListingGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                    rowSeq++;
                }
                var border = new Border()
                {
                    Padding = new Thickness(10),
                    CornerRadius = new CornerRadius(8),
                    BorderThickness = new Thickness(1),
                    Width = 260,
                    BorderBrush = ColorUtil.HexToColorBrush("acbcc9"),
                    Background = new SolidColorBrush(Colors.Transparent),
                    Name = p.Id.ToString()
                };
                border.Tapped += SelectPhone;

                var stackPanel = new StackPanel() { Spacing = 6 };

                byte[]? imageData = null;
                PhoneImage? phoneImage = _dbContext.PhoneImage.FirstOrDefault(ph => ph.PhoneId == p.Id && ph.IsCover);
                phoneImage ??= _dbContext.PhoneImage.FirstOrDefault(ph => ph.PhoneId == p.Id);
                if (phoneImage is not null)
                {
                    var fileDate = _dbContext.FileData.First(f => f.Id == phoneImage.DataId);
                    imageData = fileDate.Data;
                }

                var image = new Image()
                {
                    Stretch = Stretch.Uniform,
                    Width = 225,
                    Height = 300,
                    Source = await ImageUtil.ByteArrayToImageSource(imageData)
                };

                var textBlock = new TextBlock()
                {
                    Text = $"{p.Brand} {p.Model}",
                    TextAlignment = TextAlignment.Center,
                    FontWeight = new FontWeight(600)
                };
                stackPanel.Children.Add(image);
                stackPanel.Children.Add(textBlock);
                border.Child = stackPanel;
                Grid.SetColumn(border, columnSeq);
                Grid.SetRow(border, rowSeq);
                PhoneListingGrid.Children.Add(border);
                columnSeq++;
            }
        }

        private void AddNewPhone(Object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddPhoneScratchPage));
        }

        private void LoadBrandOption()
        {
            Brands = [.. _dbContext.Brands];
            BrandOptionItem.Items.Clear();

            var item = new MenuFlyoutItem { Text = "All" };
            item.Click += (s, e) => { SelectBrand(s, e); };
            item.Width = 150;
            BrandOptionItem.Items.Add(item);

            foreach (var brand in Brands)
            {
                item = new MenuFlyoutItem { Text = brand.Name.ToString() };
                item.Click += (s, e) => { SelectBrand(s, e); };
                item.Width = 150;
                BrandOptionItem.Items.Add(item);
            }

            BrandOptionButton.Content = _selectedBrand;
        }

        private void SelectBrand(Object sender, RoutedEventArgs e)
        {
            var s = (MenuFlyoutItem)sender;
            var brand = s.Text;
            BrandOptionButton.Content = brand;
        }

        private void SearchClick(object sender, RoutedEventArgs e)
        {
            var s = sender as Button;
            if(s.Name.Equals("RemoveSearch"))
            {
                _searchKey = string.Empty;
                _selectedBrand = "All";
                BrandOptionButton.Content = _selectedBrand;
                SearchKeyText.Text = string.Empty;
            }
            else
            {
                if (!string.IsNullOrEmpty(SearchKeyText.Text)) _searchKey = $"*{SearchKeyText.Text}*";
                else _searchKey = string.Empty;
                _selectedBrand = (string)BrandOptionButton.Content;
            }

            _currentPage = 0;
            _totalPage = 0;
            LoadPhoneList();
        }

        private void PageChange(Object sender, RoutedEventArgs e)
        {
            var s = (Button)sender;
            if(s.Name.Equals("PreviousPageBtn"))
            {
                if (_currentPage < 1) _currentPage = 0;
                else _currentPage--;
            }
            else if(s.Name.Equals("NextPageBtn"))
            {
                if (_currentPage == _totalPage) _currentPage = _totalPage;
                else _currentPage++;
            }
            LoadPhoneList();
        }

        private void SelectPhone(object sender, RoutedEventArgs e)
        {
            var s = (Border)sender;
            Frame.Navigate(typeof(PhoneMainPage), s.Name);
        }
    }
}
