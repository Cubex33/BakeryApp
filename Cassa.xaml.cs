using BakeryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace SP1
{
    public partial class Cassa : ContentPage
    {
        public BakeryDbContext dbContext = new BakeryDbContext();

        private Dictionary<int, (int quantity, int price, int Id, int prise)> selectedItems = new();

        public Cassa()
        {
            InitializeComponent();
            CreateProductCatalog();
        }

        public void CreateProductCatalog()
        {
            FlexLayoutContainer.Children.Clear();

            foreach (var item in dbContext.Products)
            {
                int quantity = 0;
                int prise = 0;

                selectedItems[item.Id] = (quantity: 0, price: (int)item.Price, Id: item.Id, prise: 0);

                var vertical = new VerticalStackLayout
                {
                    Padding = new Thickness(10),
                    HorizontalOptions = LayoutOptions.Center
                };

                var button = new Button
                {
                    Text = $"{item.Name} {(int)item.Price}₸",
                    FontSize = 16,
                    BackgroundColor = Colors.LightGray,
                };

                Label chooseText = null;

                button.Clicked += async (sender, e) =>
                {
                    quantity++;
                    prise += (int)item.Price;

                    selectedItems[item.Id] = (quantity, (int)item.Price, item.Id, prise);

                    if (chooseText == null)
                    {
                        chooseText = new Label
                        {
                            Text = $"Вы выбрали {item.Name} за {prise}₸ в количестве {quantity} шт.",
                            FontSize = 16,
                            Margin = new Thickness(0, 2)
                        };
                        MainLayout.Children.Add(chooseText);
                    }
                    else
                    {
                        chooseText.Text = $"Вы выбрали {item.Name} за {prise}₸ в количестве {quantity} шт.";
                    }
                };

                vertical.Children.Add(button);
                FlexLayoutContainer.Children.Add(vertical);
            }

            Calculate.Clicked += async (sender, e) =>
            {
                try
                {
                    var itemsToOrder = selectedItems.Values
                        .Where(x => x.quantity > 0)
                        .ToList();

                    if (!itemsToOrder.Any())
                    {
                        await DisplayAlert("Внимание", "Выберите товары", "Ok");
                        return;
                    }

                    var customer = dbContext.Customers.ToList().FirstOrDefault();
                    var employee = dbContext.Employees.ToList().FirstOrDefault();

                    if (customer == null || employee == null)
                    {
                        await DisplayAlert("Ошибка", "Клиент или сотрудник не найден", "Ok");
                        return;
                    }

                    var newOrder = new Order
                    {
                        CustomerId = customer.Id,
                        EmployeeId = employee.Id
                    };
                    dbContext.Orders.Add(newOrder);
                    await dbContext.SaveChangesAsync();

                    foreach (var item in itemsToOrder)
                    {
                        dbContext.OrderItems.Add(new OrderItem
                        {
                            OrderId = newOrder.Id,
                            ProductId = item.Id,
                            Quantity = item.quantity,
                            Price = item.prise
                        });
                    }

                    await dbContext.SaveChangesAsync();
                    await DisplayAlert("Успех", "Заказ оформлен!", "Ok");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.InnerException?.Message ?? ex.Message, "Ok");
                }
            };
        }
    }
}