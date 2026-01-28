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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserApp.Model;
using UserApp.Services;

namespace UserApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        private TokenManager _tokenManager;
        private SessionManager _sessionManager;
        public UserPage()
        {
            InitializeComponent();
        }
        public UserPage(User currUser)
        {
            InitializeComponent();
            _tokenManager = new TokenManager();
            _sessionManager = new SessionManager();
            DataContext = currUser;
        }

        private async void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Disable button to prevent multiple clicks
            Button BtnLogout = (sender as Button);
            BtnLogout.IsEnabled = false;

            try
            {
                // Show progress to user
                BtnLogout.Content = "Logging out...";

                // Get current user login before clearing session
                string currentUserLogin = _tokenManager.GetCurrentUserLogin();

                // Step 1: Invalidate session on server and clear local token
                bool logoutSuccess = await _sessionManager.LogoutAsync();

                if (logoutSuccess)
                {
                    // Server logout was successful
                    MessageBox.Show(
                        $"You have been successfully logged out.\n\nUser: {currentUserLogin}",
                        "Logout Successful",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    // Server logout failed but local session was cleared
                    MessageBox.Show(
                        $"You have been logged out locally.\n\nNote: Server connection may be unavailable.\n\nUser: {currentUserLogin}",
                        "Logout Completed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }

                // Step 3: Navigate back to login page
                NavigationService.Navigate(new StartPage());
            }
            catch (Exception ex)
            {
                // Emergency logout - clear everything and navigate away
                System.Diagnostics.Debug.WriteLine($"Logout error: {ex.Message}");

                MessageBox.Show(
                    $"An error occurred during logout:\n\n{ex.Message}\n\nYou will be logged out for security reasons.",
                    "Logout Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                // Perform emergency logout to ensure clean state
                _sessionManager.EmergencyLogout();
                NavigationService.Navigate(new StartPage());
            }
            finally
            {
                BtnLogout.IsEnabled = true;
                BtnLogout.Content = "Logout";
            }
        }
    }
}
