using BakeryApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace SP1
{
    public partial class AdminPanel : ContentPage
    {
        public BakeryDbContext dbContext = new BakeryDbContext();

        public AdminPanel()
        {
            InitializeComponent();
        }

        private async void CassaOpenButton(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new Cassa());
        }

        private async void OrderOpenButton(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new Report());
        }

        private async void UserOpenButton(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserRegister());
        }
    }
}
