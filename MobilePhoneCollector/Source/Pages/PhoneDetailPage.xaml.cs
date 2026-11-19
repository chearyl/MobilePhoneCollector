using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MobilePhoneCollector.Source.Core.Database;
using MobilePhoneCollector.Source.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MobilePhoneCollector.Source.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PhoneDetailPage : Page
{
    private bool _editMode = false;
    private DBContext _dbContext;
    private int _assignedId = 0;

    public Phone Phone
    {
        get => (Phone)GetValue(PlayerProperty);
        set => SetValue(PlayerProperty, value);
    }

    public static readonly DependencyProperty PlayerProperty = DependencyProperty.Register(
        nameof(Phone), typeof(Phone), typeof(PhoneDetailPage), new PropertyMetadata(null, OnPhoneChanged));

    private static void OnPhoneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((PhoneDetailPage)d).AssignId();

    public PhoneDetailPage()
    {
        InitializeComponent();
        EditBtn.Visibility = Visibility.Visible;
        CancelBtn.Visibility = Visibility.Collapsed;
        SaveBtn.Visibility = Visibility.Collapsed;
        AddSpecsBtn.Visibility = Visibility.Collapsed;
        _dbContext = new DBContext();
    }

    private void AssignId()
    {
        foreach(var spec in Phone.SpecList)
        {
            spec.TempId = _assignedId;
            _assignedId++;
        }
        LoadData();
    }

    private void LoadData()
    {
        Dictionary<string, List<Specs>> specsBycategory = Phone.SpecList.GroupBy(x => x.Category.ToUpper()).ToDictionary(s => s.Key, s => s.ToList());
        int seq = 0, left = 0, right = 0;
        DetailColumn1.Children.Clear();
        DetailColumn2.Children.Clear();

        foreach (var category in specsBycategory)
        {
            var uiElement = CreateSpecs(category.Key, category.Value);
            if(seq % 2 == 0)
            {
                DetailColumn1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                Grid.SetRow(uiElement, left++);
                DetailColumn1.Children.Add(uiElement);
            }
            else
            {
                DetailColumn2.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                Grid.SetRow(uiElement, right++);
                DetailColumn2.Children.Add(uiElement);
            }
            ++seq;
        }
    }

    private FrameworkElement CreateSpecs(string category, List<Specs> specs)
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
            FontWeight = new FontWeight(600)
        };
        Grid.SetColumn(categoryTitle, 0);
        grid.Children.Add(categoryTitle);

        for (int i = 0; i < specs.Count; i++)
        {
            if (specs[i].ToDelete) continue;
            var specsGrid = new Grid();
            specsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            specsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            specsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            Grid.SetRow(specsGrid, (i + 1));

            var specsTitle = new TextBlock
            {
                FontWeight = new FontWeight(600),
                Text = specs[i].Key,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                Opacity = 0.7
            };
            Grid.SetColumn(specsTitle, 0);
            specsGrid.Children.Add(specsTitle);

            if (_editMode)
            {
                var specsValue = new TextBox
                {
                    TextWrapping = TextWrapping.Wrap,
                    AcceptsReturn = true,
                    Text = specs[i].Value,
                    Name = $"Textbox-{specs[i].TempId}"
                };
                var remove = new Button
                {
                    Content = "❌",
                    Name = $"Remove-{specs[i].TempId}",
                    BorderThickness = new Thickness(0),
                    Background = new SolidColorBrush(Colors.Transparent),
                    Foreground = new SolidColorBrush(Colors.Transparent)
                };
                remove.Resources["ButtonBackgroundPointerOver"] = new SolidColorBrush(Colors.Transparent);
                remove.Click += (s, e) => { RemoveSpecs(s, e); };
                Grid.SetColumn(specsValue, 1);
                Grid.SetColumn(remove, 2);
                specsGrid.Children.Add(specsValue);
                specsGrid.Children.Add(remove);
            }
            else
            {
                var specsValue = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = specs[i].Value,
                    FontWeight = new FontWeight(700)
                };
                Grid.SetColumn(specsValue, 1);
                specsGrid.Children.Add(specsValue);
            }

            grid.Children.Add(specsGrid);
        }

        border.Child = grid;
        return border;
    }

    private void EditEvent(object sender, RoutedEventArgs e)
    {
        var s = sender as Button;
        switch(s!.Name)
        {
            case "EditBtn":
                _editMode = true;
                EditBtn.Visibility = Visibility.Collapsed;
                CancelBtn.Visibility = Visibility.Visible;
                SaveBtn.Visibility = Visibility.Visible;
                AddSpecsBtn.Visibility = Visibility.Visible;
                break;
            case "CancelBtn":
                _editMode = false;
                EditBtn.Visibility = Visibility.Visible;
                CancelBtn.Visibility = Visibility.Collapsed;
                SaveBtn.Visibility = Visibility.Collapsed;
                AddSpecsBtn.Visibility = Visibility.Collapsed;
                Phone.SpecList.RemoveAll(sp => sp.NewAdd);
                foreach (var sp in Phone.SpecList) sp.ToDelete = false;
                break;
            case "SaveBtn":
                List<Specs> toDelete = [];
                List<Specs> toAdd = [];
                foreach (var sp in Phone.SpecList)
                {
                    System.Diagnostics.Debug.WriteLine(sp.Key);
                    if(sp.ToDelete)
                    {
                        toDelete.Add(sp);
                        continue;
                    }
                    if(sp.NewAdd)
                    {
                        toAdd.Add(sp);
                        continue;
                    }
                    var tb = FindName($"Textbox-{sp.TempId}") as TextBox;
                    sp.Value = tb!.Text;
                }
                _dbContext.Update(Phone);
                _dbContext.RemoveRange(toDelete);
                //_dbContext.Specs.Add()
                _dbContext.SaveChangesAsync();
                _editMode = false;
                EditBtn.Visibility = Visibility.Visible;
                CancelBtn.Visibility = Visibility.Collapsed;
                SaveBtn.Visibility = Visibility.Collapsed;
                AddSpecsBtn.Visibility = Visibility.Collapsed;
                break;
            default: break;
        }
        
        LoadData();
    }

    private void RemoveSpecs(object sender, RoutedEventArgs e)
    {
        var s = (Button)sender;
        var id = int.Parse(s.Name.Split("-")[1]);
        Phone.SpecList.First(sp => sp.TempId == id).ToDelete = true;
        LoadData();
    }

    private void AddNewSpecs(object sender, RoutedEventArgs e)
    {
        Specs specs = new()
        {
            Category = NewSpecsCategory.Text,
            Key = NewSpecsKey.Text,
            Value = NewSpecsValue.Text,
            PhoneId = Phone.Id,
            TempId = _assignedId,
            NewAdd = true
        };
        Phone.SpecList.Add(specs);
        LoadData();
        ShowNewSpecs(new object(), new RoutedEventArgs());
        _assignedId++;
    }

    private void ScreenSizeChange(object sender, SizeChangedEventArgs e)
    {
        double height = DetailRootGrid.ActualHeight;
        double width = DetailRootGrid.ActualWidth;
        AddSpecsOverly.Height = height;
        AddSpecsOverly.Width = width;

        AddSpecsBorder.Height = Math.Max(0, height - 300);
        AddSpecsBorder.Width = Math.Min(400, width);
    }

    private async void ShowNewSpecs(object sender, RoutedEventArgs e)
    {
        AddSpecsModel.IsOpen = !AddSpecsModel.IsOpen;
    }
}
