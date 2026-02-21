using BakeryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace SP1
{
    public partial class MainPage : ContentPage
    {
        public BakeryDbContext dbContext = new BakeryDbContext();


        public MainPage()
        {
            InitializeComponent();
        }

        private void ShowPassword(object? sender, EventArgs e)
        {
            PasswordInputField.IsPassword = !PasswordInputField.IsPassword;
            ShowPasswordButton.Text = PasswordInputField.IsPassword ? "S" : "H";
        }

        private async void SignIn(object? sender, EventArgs e)
        {
            var users = dbContext.Users.Where(u => u.Username == UsernameInputField.Text && u.Password == PasswordInputField.Text).ToList();
            if (users.Count > 0)
            {
                var user = users.First();
                if (user.IsAdmin)
                {
                    await Navigation.PushAsync(new AdminPanel());
                }
                else
                {
                    await Navigation.PushAsync(new Cassa());
                }
            }
            else
            {
                await DisplayAlertAsync("Login Failed", "Invalid username or password.", "OK");
            }
        }
    }
}