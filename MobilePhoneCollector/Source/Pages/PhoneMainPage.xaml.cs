using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MobilePhoneCollector.Source.Core.Database;
using MobilePhoneCollector.Source.Util;
using System;
using System.Collections.Generic;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MobilePhoneCollector.Source.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PhoneMainPage : Page
{
    private DBContext _dbContext;

    public Phone Phone;

    public PhoneMainPage()
    {
        InitializeComponent();
        _dbContext = new DBContext();
        Phone = new Phone();
        SpecTab.Resources["ButtonBackgroundPointerOver"] = new SolidColorBrush(Colors.Transparent);
        ImagesTab.Resources["ButtonBackgroundPointerOver"] = new SolidColorBrush(Colors.Transparent);
        ReviewsTab.Resources["ButtonBackgroundPointerOver"] = new SolidColorBrush(Colors.Transparent);
    }

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is string stringId)
        {
            int id = int.Parse(stringId);
            Phone = _dbContext.Phone.First(p => p.Id == id);
            Phone.SpecList = [.. _dbContext.Specs.Where(i => i.PhoneId == id)];
            Phone.ImageList = [.. _dbContext.PhoneImage.Where(i => i.PhoneId == id)];
            Phone.ReviewList = [.. _dbContext.PhoneReview.Where(i => i.PhoneId == id)];
            PhoneModelText.Text = $"{Phone.Brand} {Phone.Model}";

            byte[]? imageData = null;
            if(Phone.ImageList.Count > 0)
            {
                PhoneImage? phoneImage = Phone.ImageList.FirstOrDefault(ph => ph.IsCover);
                phoneImage ??= Phone.ImageList[0];
                var fileDate = _dbContext.FileData.First(f => f.Id == phoneImage.DataId);
                imageData = fileDate.Data;
            }
            PhoneImage.Source = await ImageUtil.ByteArrayToImageSource(imageData);

            DetailPage.Visibility = Visibility.Visible;
            ImagesTab.Content = $"Images ({Phone.ImageList.Count})";
            ReviewsTab.Content = $"Reviews ({Phone.ReviewList.Count})";

            SpecTab.BorderThickness = new Thickness(0, 0, 0, 2);
            SpecTab.BorderBrush = ColorUtil.HexToColorBrush("0c5392");
        }
    }

    private void BackToCollection(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(PhoneListingPage));
    }

    private void ChangeTab(Object sender, RoutedEventArgs e)
    {
        var b = sender as Button;
        switch(b!.Name)
        {
            case "SpecTab":
                DetailPage.Visibility = Visibility.Visible;
                ImagePage.Visibility = Visibility.Collapsed;
                ReviewPage.Visibility = Visibility.Collapsed;
                SpecTab.BorderThickness = new Thickness(0, 0, 0, 2);
                SpecTab.BorderBrush = ColorUtil.HexToColorBrush("0c5392");
                ImagesTab.BorderThickness = new Thickness(0);
                ImagesTab.BorderBrush = new SolidColorBrush(Colors.Transparent);
                ReviewsTab.BorderThickness = new Thickness(0);
                ReviewsTab.BorderBrush = new SolidColorBrush(Colors.Transparent);
                break;
            case "ImagesTab":
                DetailPage.Visibility = Visibility.Collapsed;
                ImagePage.Visibility = Visibility.Visible;
                ReviewPage.Visibility = Visibility.Collapsed;
                SpecTab.BorderThickness = new Thickness(0);
                SpecTab.BorderBrush = new SolidColorBrush(Colors.Transparent);
                ImagesTab.BorderThickness = new Thickness(0, 0, 0, 2);
                ImagesTab.BorderBrush = ColorUtil.HexToColorBrush("0c5392");
                ReviewsTab.BorderThickness = new Thickness(0);
                ReviewsTab.BorderBrush = new SolidColorBrush(Colors.Transparent);
                break;
            case "ReviewsTab":
                DetailPage.Visibility = Visibility.Collapsed;
                ImagePage.Visibility = Visibility.Collapsed;
                ReviewPage.Visibility = Visibility.Visible;
                SpecTab.BorderThickness = new Thickness(0);
                SpecTab.BorderBrush = new SolidColorBrush(Colors.Transparent);
                ImagesTab.BorderThickness = new Thickness(0);
                ImagesTab.BorderBrush = new SolidColorBrush(Colors.Transparent);
                ReviewsTab.BorderThickness = new Thickness(0, 0, 0, 2);
                ReviewsTab.BorderBrush = ColorUtil.HexToColorBrush("0c5392");
                break;
        }
    }

    private void DeletePhone(object sender, RoutedEventArgs e)
    {
        List<FileData> fileToDelete = [];
        Phone.ImageList = [.. _dbContext.PhoneImage.Where(x => x.PhoneId == Phone.Id)];
        Phone.ReviewList = [.. _dbContext.PhoneReview.Where(x => x.PhoneId == Phone.Id)];
        Phone.SpecList = [.. _dbContext.Specs.Where(x => x.PhoneId == Phone.Id)];
        foreach (var image in Phone.ImageList)
        {
            var used = _dbContext.PhoneImage.Count(i => i.DataId == image.DataId);
            var data = _dbContext.FileData.First(i => i.Id == image.DataId);
            if(used < 2) fileToDelete.Add(data);
        }

        foreach (var review in Phone.ReviewList)
        {
            var used = _dbContext.PhoneReview.Count(i => i.DataId == review.DataId);
            var data = _dbContext.FileData.First(i => i.Id == review.DataId);
            if (used < 2) fileToDelete.Add(data);
        }
        
        _dbContext.Phone.Remove(Phone);
        _dbContext.FileData.RemoveRange(fileToDelete);
        _dbContext.SaveChangesAsync();
        BackToCollection(sender, e);
    }
}
