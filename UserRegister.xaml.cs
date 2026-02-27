using BakeryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace SP1
{
    public partial class UserRegister : ContentPage
    {
        public BakeryDbContext dbContext = new BakeryDbContext();

        public UserRegister()
        {
            InitializeComponent();
            CreateCustomers();
        }

        private void CreateCustomers()
        {
            foreach (var customers in dbContext.Customers)
            {
                var HorizontalStackLayout = new HorizontalStackLayout();
                var frame = new Frame
                {
                    Margin = new Thickness(2)
                };
                var Getdiscount = new Button
                {
                    Text = "Дать скидку"
                };

                Getdiscount.Clicked += async (sender, e) => await GetDiscount(sender, e);
                var Label = new Label
                {
                    Text = $"{customers.Id} {customers.FirstName} {customers.LastName} номер: {customers.Phone} почта: {customers.Email}"
                };
                HorizontalStackLayout.Children.Add(Label);
                frame.Content = HorizontalStackLayout;
                FlexLayoutContainer.Children.Add(frame);
            }
        }

        private async Task GetDiscount(object? sender, EventArgs e)
        {
            string discount = await DisplayPromptAsync(
                title: "Выдача скидки",
                message: null,
                placeholder: "Введите во сколько процентов вы хотите дать скидку...",
                accept: "Окей",
                cancel: "Отмена",
                keyboard: Keyboard.Numeric
            );

            if (!string.IsNullOrWhiteSpace(discount))
            {
                if (int.TryParse(discount, out int value))
                {

                }
            }
        }
    }
}
