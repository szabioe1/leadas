using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace HotelAdminPanel
{
    /// <summary>
    /// Interaction logic for HotelManagement.xaml
    /// </summary>
    public partial class HotelManagement : Window
    {
        private void LoadSearchColumns()
        {
            searchsel.Items.Clear();
            searchsel.Items.Add("name");
            searchsel.Items.Add("city");
            searchsel.Items.Add("description");
            searchsel.SelectedIndex = 0;
        }
        private void LoadImageFromUrl(string imageUrl)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imageUrl, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Ensures the image is fully loaded before use
                bitmap.EndInit();

                imgsrc.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public HotelManagement()
        {
            InitializeComponent();
            LoadSearchColumns();
            Database database = new Database();
            database.LoadHotelsToDataGrid(hotelsData);
        }

        private void backbtn_Click(object sender, RoutedEventArgs e)
        {
            AdminPage adminPage = new AdminPage();
            adminPage.Show();
            Window.GetWindow(this)?.Close();
        }

        private void addHotelBtn_Click(object sender, RoutedEventArgs e)
        {
            Database database = new Database();
            database.ConnectToDatabase();
            if (hotelName.Text == "" || price.Text == "" || city.Text == "" || payment.Text == "" || coords.Text == "" || @class.Text == "" || description.Text == "")
            {
                MessageBox.Show("Please fill in the necessary information.", "Empty Fields", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                database.AddHotel(hotelName, price, city, payment, coords, @class, description);
                database.LoadHotelsToDataGrid(hotelsData);
            }
        }

        private void deleteHotelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (hotelsData.SelectedItem == null)
            {
                MessageBox.Show("Please select a user to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView selectedRow = hotelsData.SelectedItem as DataRowView;

            if (selectedRow != null)
            {
                int hotelid = Convert.ToInt32(selectedRow["id"]);

                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this hotel?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Database database = new Database();
                    database.DeleteHotel(hotelid);

                    database.LoadHotelsToDataGrid(hotelsData);
                }
            }
        }

        private void searchbtn_Click(object sender, RoutedEventArgs e)
        {
            resetbtn.Visibility = Visibility.Visible;
            string selectedColumn = searchsel.Text;
            string searchText = searchbox.Text;

            if (string.IsNullOrWhiteSpace(selectedColumn) || string.IsNullOrWhiteSpace(searchText))
            {
                MessageBox.Show("Please select a column and enter a search term.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Database database = new Database();
            hotelsData.ItemsSource = null;
            database.SearchData("hotels", selectedColumn, searchText, hotelsData);
        }

        private void resetbtn_Click(object sender, RoutedEventArgs e)
        {
            Database database = new Database();
            hotelsData.ItemsSource = null;
            searchbox.Text = "";
            database.LoadHotelsToDataGrid(hotelsData);
        }

        private void hotelsData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (hotelsData.SelectedItem is DataRowView selectedRow)
            {
                int hotelId = Convert.ToInt32(selectedRow["HotelID"]);

                Database database = new Database();
                string imageUrl = database.LoadImage(hotelId); 
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    LoadImageFromUrl(imageUrl);
                }
                else
                {
                    imgsrc.Source = null; 
                }
            }
        }
    }
}
