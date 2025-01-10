using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace HotelAdminPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class Database
    {
        private readonly string connectionString;

        public Database()
        {
            connectionString = "Server=localhost;Database=hotels;Uid=root;Pwd=root;";
        }

        public bool ConnectToDatabase()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Successfully connected to the database.");
                    return true;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error connecting to the database: {ex.Message}");
                return false;
            }
        }
        public string GetPassw(string email)
        {
            string password = null;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT password FROM users WHERE email = @email";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                password = reader["password"].ToString();
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error retrieving password: {ex.Message}");
            }
            return password;


        }
        public void LoadUsersToDataGrid(DataGrid dataGrid)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM users";

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();

                        adapter.Fill(dataTable);

                        dataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void LoadHotelsToDataGrid(DataGrid dataGrid)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                    SELECT 
                        hotels.id AS HotelID,
                        hotels.name AS HotelName,
                        hotels.price AS Price,
                        hotels.city AS City,
                        hotels.payment AS PaymentMethod,
                        users.firstname AS OwnerName,
                        hotels.coords AS Coordinates,
                        hotels.class AS Class,
                        hotels.description AS Description
                        
                    FROM hotels
                    LEFT JOIN users ON hotels.owner_id = users.id";


                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();

                        adapter.Fill(dataTable);

                        dataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void LoadBookingsToDataGrid(DataGrid dataGrid)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                SELECT 
                    bookings.id AS BookingID,
                    users.firstname AS UserName,
                    hotels.name AS HotelName,
                    bookings.booked AS BookedDateTime,
                    bookings.checkin AS CheckInDateTime,
                    bookings.checkout AS CheckOutDateTime,
                    bookings.payment_id AS PaymentID
                FROM bookings
                LEFT JOIN users ON bookings.user_id = users.id
                LEFT JOIN hotels ON bookings.hotel_id = hotels.id";

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();

                        adapter.Fill(dataTable);

                        dataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading bookings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void LoadAmenitiesToDataGrid(DataGrid dataGrid)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT 
                        hotels.name AS HotelName,
                        amenities.amenity AS AmenityName
                    FROM hotelamenities
                    LEFT JOIN hotels ON hotelamenities.hotel_id = hotels.id
                    LEFT JOIN amenities ON hotelamenities.amenity_id = amenities.id;
                    ";

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();

                        adapter.Fill(dataTable);

                        dataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading amenities: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public string LoadImage(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = $"SELECT thumb FROM images WHERE id = @id";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        object result = command.ExecuteScalar();
                        return result?.ToString();
                    }
                     
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading picture: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }


        public void DeleteUser(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM users WHERE id = @id";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        public void UpdateUser(int id, TextBox firstn, TextBox lastn, ComboBox perm, TextBox phone, TextBox email, PasswordBox passw)
        {
            try
            {
                int permissionid = 0;
                if (perm.SelectedIndex == 0)
                {
                    permissionid = 1;
                }
                else if (perm.SelectedIndex == 1)
                {
                    permissionid = 2;
                }
                else if (perm.SelectedIndex == 2)
                {
                    permissionid = 3;
                }
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE users SET firstname = @first, lastname = @last, permissionId = @perm, phone = @phone, email = @email, registered = @date, password = @passw WHERE id = @id;";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        
                        command.Parameters.AddWithValue("@first", firstn.Text);
                        command.Parameters.AddWithValue("@last", lastn.Text);
                        command.Parameters.AddWithValue("@perm", permissionid);
                        command.Parameters.AddWithValue("@phone", phone.Text);
                        command.Parameters.AddWithValue("@email", email.Text);
                        command.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@passw", ComputeMd5Hash(passw.Password));
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        public void AddUser(TextBox firstn, TextBox lastn, ComboBox perm, TextBox phone, TextBox email, PasswordBox passw)
        {
            int permissionid = 0;
            if (perm.SelectedIndex == 0)
            {
                permissionid = 1;
            }
            else if (perm.SelectedIndex == 1)
            {
                permissionid = 2;
            }
            else if (perm.SelectedIndex == 2)
            {
                permissionid = 3;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO users (firstname, lastname, permissionId, phone, email, registered, password) VALUES (@first, @last, @perm, @phone, @email, @date, @passw);";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@first", firstn.Text);
                        command.Parameters.AddWithValue("@last", lastn.Text);
                        command.Parameters.AddWithValue("@perm", permissionid);
                        command.Parameters.AddWithValue("@phone", phone.Text);
                        command.Parameters.AddWithValue("@email", email.Text);
                        command.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@passw", ComputeMd5Hash(passw.Password));
                        command.ExecuteNonQuery();

                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error Adding User: {ex.Message}");
            }
        }

        public void AddHotel(TextBox name, TextBox price, TextBox city, ComboBox payment, TextBox coords, TextBox hotelclass, TextBox desc)
        {
            string coordsInput = coords.Text; // példa: "-74.006,40.7128"
            string[] parts = coordsInput.Split(',');

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO hotels (name, price, city, payment, owner_id, coords, class, description) " +
                                   "VALUES (@name, @price, @city, @pay, @ow, ST_GeomFromText(@coords), @class, @desc);";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", name.Text);
                        command.Parameters.AddWithValue("@price", price.Text);
                        command.Parameters.AddWithValue("@city", city.Text);
                        command.Parameters.AddWithValue("@pay", payment.Text);
                        command.Parameters.AddWithValue("@class", hotelclass.Text);
                        command.Parameters.AddWithValue("@desc", desc.Text);
                        command.Parameters.AddWithValue("@ow", 1);
                        if (parts.Length == 2 &&
                            double.TryParse(parts[0], out double longitude) &&
                            double.TryParse(parts[1], out double latitude) &&
                            longitude >= -180 && longitude <= 180 &&
                            latitude >= -90 && latitude <= 90)
                        {
                            command.Parameters.AddWithValue("@coords", $"POINT({longitude} {latitude})");
                        }
                        command.ExecuteNonQuery();

                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error Adding Hotel: {ex.Message}");
            }
        }
        public void DeleteHotel(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM hotels WHERE id = @id";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting hotel: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        public void SearchData(string table, string column, string searchText, DataGrid dataGrid)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = $"SELECT * FROM {table} WHERE {column} LIKE @searchText";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@searchText", $"%{searchText}%");

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            dataGrid.ItemsSource = dataTable.DefaultView;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during search: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        public string ComputeMd5Hash(string message)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] input = Encoding.UTF8.GetBytes(message);
                byte[] hash = md5.ComputeHash(input);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
        
    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void loginbtn_Click(object sender, RoutedEventArgs e)
        {
            Database login = new Database();
            login.ConnectToDatabase();
            string email = usertxt.Text;
            string pwd = login.GetPassw(email);
            if (login.ComputeMd5Hash(pwdtxt.Password) == pwd)
            {
                MessageBox.Show("Sikeres Bejelentkezés!");
                AdminPage adminPage = new AdminPage();
                adminPage.Show();
                Window.GetWindow(this)?.Close();
            }
            else
            {
                MessageBox.Show("Sikertelen Bejelentkezés!");

            }


        }
    }
}
