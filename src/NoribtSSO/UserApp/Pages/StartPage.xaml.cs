using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using UserApp.Model;
using UserApp.Services;
using UserApp.Tools;

namespace UserApp.Pages
{
    public partial class StartPage : Page
    {
        private TokenManager _tokenManager;
        private const string AUTH_WEB_URL = "http://localhost:4080"; // Adjust based on your setup
        
        public StartPage()
        {
            InitializeComponent();
            _tokenManager = new TokenManager();
        }

        /// <summary>
        /// Sign In button click - launches browser for OAuth-like flow
        /// </summary>
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Define callback URL (local loopback)
                string callbackUrl = "http://localhost:5081/token-callback";
                
                // Build the login URL with return URL parameter
                string loginUrl = $"{AUTH_WEB_URL}/Login?returnUrl={Uri.EscapeDataString(callbackUrl)}";
                
                // Launch default browser
                Process.Start(new ProcessStartInfo
                {
                    FileName = loginUrl,
                    UseShellExecute = true
                });
                
                // Start local listener to capture token
                StartTokenListener();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error launching login: {ex.Message}", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Register button click - launches browser for registration
        /// </summary>
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string registerUrl = $"{AUTH_WEB_URL}/Register";
                
                Process.Start(new ProcessStartInfo
                {
                    FileName = registerUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error launching registration: {ex.Message}", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Starts a local HTTP listener to capture token from browser redirect
        /// </summary>
        private void StartTokenListener()
        {
            var listener = new System.Net.HttpListener();
            listener.Prefixes.Add("http://localhost:5081/");
            
            try
            {
                listener.Start();
                listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting token listener: {ex.Message}", "Listener Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles token callback from AuthWEB
        /// </summary>
        private void ListenerCallback(IAsyncResult result)
        {
            var listener = (System.Net.HttpListener)result.AsyncState;
            
            try
            {
                var context = listener.EndGetContext(result);
                
                // Extract token from query parameters
                string token = context.Request.QueryString["token"];
                string status = context.Request.QueryString["status"];
                
                if (status == "success" && !string.IsNullOrEmpty(token))
                {
                    // Save token locally
                    _tokenManager.SaveToken(token);

                    // Save userInfo
                    string id = ClaimsHelper.ExtractUuidFromToken(token);
                    ApiClient client = new ApiClient();
                    User user = client.GetAsync<User>($"/api/Users/{id}").Result;
                    _tokenManager.SaveUserInfo(user);

                    // Send success response to browser
                    string responseString = "<html><body><h1>Authentication Successful!</h1><p>You can close this window and return to the application.</p></body></html>";
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    context.Response.ContentLength64 = buffer.Length;
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    context.Response.OutputStream.Close();
                    
                    // Navigate to user page or main view
                    this.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        NavigateToUserPage(user);
                    });
                }
                else
                {
                    string responseString = "<html><body><h1>Authentication Failed!</h1><p>An error occurred during login. Please try again.</p></body></html>";
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    context.Response.ContentLength64 = buffer.Length;
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    context.Response.OutputStream.Close();
                }
            }
            catch (Exception ex)
            {
                this.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Error processing token: {ex.Message}", "Token Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                listener.Stop();
            }
        }

        private void NavigateToUserPage(User user)
        {
            this.NavigationService?.Navigate(new UserPage(user));
        }
    }
}
