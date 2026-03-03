using BakeryApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SP1
{
    public partial class UserRegister : ContentPage
    {
        bool isOpenPanel = false;
        public BakeryDbContext dbContext = new BakeryDbContext();

        public UserRegister()
        {
            InitializeComponent();
            CreateCustomers();
        }

        private void CreateCustomers()
        {
            MainLayout.Children.Clear();

            foreach (var customers in dbContext.Customers.ToList())
            {
                Label Label = new();
                var HorizontalStackLayout = new HorizontalStackLayout
                {
                    Spacing = 10
                };
                var frame = new Frame
                {
                    Margin = new Thickness(2)
                };
                var Getdiscount = new Button
                {
                    Text = "➕",
                    FontSize = 16
                };
                var Editdiscount = new Button
                {
                    Text = "✏️",
                    FontSize = 16
                };
                var discount = dbContext.Discounts.FirstOrDefault(x => x.Id == customers.Id);
                Editdiscount.Clicked += async (sender, e) => await EditDiscount(customers, label: Label);
                Getdiscount.Clicked += async (sender, e) => await GetDiscount(customers, label: Label);
                if (discount != null)
                {
                    Label.Text = $"{customers.Id}. {customers.FirstName} {customers.LastName} номер: {customers.Phone} почта: {customers.Email}. Скидка: {discount.Discount1} %";
                    Label.Margin = new Thickness(2);
                }
                else
                {
                    Label.Text = $"{customers.Id}. {customers.FirstName} {customers.LastName} номер: {customers.Phone} почта: {customers.Email}. Скидка: 0 %";
                }
                HorizontalStackLayout.Children.Add(Label);
                HorizontalStackLayout.Children.Add(Getdiscount);
                HorizontalStackLayout.Children.Add(Editdiscount);
                frame.Content = HorizontalStackLayout;
                MainLayout.Children.Add(frame);
            }
        }

        private async Task GetDiscount(Customer customers, Label label)
        {
            var discountcheck = dbContext.Discounts.FirstOrDefault(d => d.CustomerId == customers.Id);

            if (discountcheck != null)
            {
                await DisplayAlertAsync("Ошибка", "У этого клиента уже есть скидка", "Ок");
                return;

            }

            if (!isOpenPanel)
            {
                isOpenPanel = true;
                string discountInput = await DisplayPromptAsync(
                    title: "Выдача скидки",
                    message: null,
                    placeholder: "Введите во сколько процентов вы хотите дать скидку...",
                    accept: "Окей",
                    cancel: "Отмена",
                    keyboard: Keyboard.Numeric
                );
                isOpenPanel = false;

                if (string.IsNullOrWhiteSpace(discountInput)) return;
                if (!int.TryParse(discountInput, out int discountValue) || discountValue < 0 || discountValue > 100)
                {
                    await DisplayAlertAsync("Ошибка", "Введите корректное число от 0 до 100", "OK");
                    return;
                }

                var discount = new Discount
                {
                    CustomerId = customers.Id,
                    Discount1 = discountValue,
                    Activation = 1
                };

                dbContext.Discounts.Add(discount);

                try
                {
                    await dbContext.SaveChangesAsync();
                    await DisplayAlertAsync("Успех", $"Скидка в {discountValue}% успешно выдана для {customers.FirstName} {customers.LastName}", "OK");
                    label.Text = $"{customers.Id}. {customers.FirstName} {customers.LastName} номер: {customers.Phone} почта: {customers.Email}. Скидка: {discountValue} %";

                }
                catch (DbUpdateException ex)
                {
                    await DisplayAlertAsync("Ошибка", $"Не удалось сохранить скидку: {ex.InnerException?.Message ?? ex.Message}", "OK");
                }
            }
        }

        private async Task EditDiscount(Customer customer ,Label label)
        {
            if (!isOpenPanel)
            {
                isOpenPanel = true;
                string discountInput = await DisplayPromptAsync(
                    title: "Выдача скидки",
                    message: null,
                    placeholder: "Введите во сколько процентов вы хотите дать скидку...",
                    accept: "Окей",
                    cancel: "Отмена",
                    keyboard: Keyboard.Numeric
                );
                isOpenPanel = false;

                if(string.IsNullOrWhiteSpace(discountInput)) return;
                if (!int.TryParse(discountInput, out int discountValue) || discountValue < 0 || discountValue > 100)
                {
                    await DisplayAlertAsync("Ошибка", "Введите корректное число от 0 до 100", "OK");
                    return;
                }

                var existing = dbContext.Discounts.FirstOrDefault(d => d.CustomerId == customer.Id);
                if (existing != null)
                {
                    existing.Discount1 = discountValue;
                }
                else
                {
                    dbContext.Discounts.Add(new Discount
                    {
                        CustomerId = customer.Id,
                        Discount1 = discountValue,
                        Activation = 1
                    });
                }

                try
                {
                    await dbContext.SaveChangesAsync();
                    await DisplayAlertAsync("Успех", $"Скидка успешно измена для {customer.FirstName} {customer.LastName}", "Ok");
                    label.Text = $"{customer.Id}. {customer.FirstName} {customer.LastName} номер: {customer.Phone} почта: {customer.Email}. Скидка: {discountValue} %";
                }
                catch (Exception ex) {
                    await DisplayAlertAsync("Ошибка", $"Ошибка: {ex.InnerException?.Message ?? ex.Message}", "Ok");
                }
            }
        }
    }
}