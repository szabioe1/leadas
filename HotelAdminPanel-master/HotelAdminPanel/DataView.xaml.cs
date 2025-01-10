using System;
using System.Collections.Generic;
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

namespace HotelAdminPanel
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class DataView : Window
    {
        public DataView()
        {
            InitializeComponent();
        }

        private void bookingsbtn_Click(object sender, RoutedEventArgs e)
        {
            searchsel.Items.Add("");
            dataGrid.ItemsSource = null;
            Database database = new Database();
            database.LoadBookingsToDataGrid(dataGrid);
        }

        private void amenitiesbtn_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = null;
            Database database = new Database();
            database.LoadAmenitiesToDataGrid(dataGrid);
        }

        private void backbtn_Click(object sender, RoutedEventArgs e)
        {
            AdminPage adminPage = new AdminPage();
            adminPage.Show();
            Window.GetWindow(this)?.Close();
        }
    }
}
