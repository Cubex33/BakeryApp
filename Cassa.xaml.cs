using BakeryApp.Models;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

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
                int prise = 0;

                selectedItems[item.Id] = (quantity: 0, price: (int)item.Price, Id: item.Id, prise: 0);

                var vertical = new VerticalStackLayout
                {
                    Padding = new Thickness(10),
                    HorizontalOptions = LayoutOptions.Center
                };
                Label? chooseText = null;
                Button? editquantityButton = null;
                Button? deletequantityButton = null;

                var button = new Button
                {
                    Text = $"{item.Name} {(int)item.Price}₸",
                    FontSize = 16,
                    BackgroundColor = Colors.LightGray,
                };


                button.Clicked += async (sender, e) =>
                {
                    prise = (int)item.Price;

                    if (chooseText == null)
                    {
                        string qualityTextInput = await DisplayPromptAsync(
                             title: $"Действия для товара '{item.Name}'",
                             message: null,
                             accept: "Добавить",
                             cancel: "Отмена",
                             placeholder: "Количество",
                             keyboard: Keyboard.Numeric
                        );
                        if (!string.IsNullOrWhiteSpace(qualityTextInput))
                        {
                            if(int.TryParse(qualityTextInput, out var quality) && quality > 0)
                            {
                                var horizontalGroup = new HorizontalStackLayout
                                {
                                    Spacing = 20
                                };

                                prise *= quality;
                                chooseText = new Label
                                {
                                    Text = $"Вы выбрали {item.Name} за {prise} ₸ в количестве {quality} шт.",
                                    FontSize = 16,
                                    Margin = new Thickness(0, 2)
                                };
                                editquantityButton = new Button
                                {
                                    Text = "✏️",
                                    FontSize = 16
                                };
                                deletequantityButton = new Button
                                {
                                    Text = "❌",
                                    FontSize = 16
                                };

                                deletequantityButton.Clicked += async (sender, e) =>
                                {
                                    var result = await DisplayAlertAsync("Внимание", "Вы действительно хотите удалить?", "Да", "Нет");
                                    if (result)
                                    {
                                        selectedItems.Remove(item.Id);
                                        MainLayout.Children.Remove(horizontalGroup);
                                        chooseText = null;
                                        editquantityButton = null;
                                        deletequantityButton = null;
                                    }
                                };

                                editquantityButton.Clicked += async (sender, e) =>
                                {
                                    string EditqualityTextInput = await DisplayPromptAsync(
                                        title: $"Действия для товара '{item.Name}'",
                                        message: null,
                                        accept: "Добавить",
                                        cancel: "Отмена",
                                        placeholder: "Введите новое колисчество",
                                        keyboard: Keyboard.Numeric
                                    );

                                    if (!string.IsNullOrWhiteSpace(EditqualityTextInput))
                                    {
                                        if (int.TryParse(EditqualityTextInput, out var Editquality) && Editquality > 0)
                                        {
                                            var newprise = (int)item.Price * Editquality;
                                            chooseText.Text = $"Вы выбрали {item.Name} за {newprise} ₸ в количестве {Editquality} шт.";
                                            selectedItems[item.Id] = (Editquality, (int)item.Price, item.Id, newprise);
                                            await DisplayAlertAsync("Успешно", "Количество товара изменено", "Ok");
                                        }
                                    }
                                };

                                horizontalGroup.Children.Add(chooseText);
                                horizontalGroup.Children.Add(editquantityButton);
                                horizontalGroup.Children.Add(deletequantityButton);
                                MainLayout.Children.Add(horizontalGroup);

                                selectedItems[item.Id] = (quality, (int)item.Price, item.Id, prise);
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlertAsync("Error", $"Товар {item.Name} уже добавлен", "Ok");
                    }
                };

                vertical.Children.Add(button);
                FlexLayoutContainer.Children.Add(vertical);
            }

            Calculate.Clicked += async(sender, e) => await OnCalculate(sender, e);
        }

        private async Task OnCalculate(object? sender, EventArgs e)
        {
            try
            {
                var itemsToOrder = selectedItems.Values
                    .Where(x => x.quantity > 0)
                    .ToList();

                if (!itemsToOrder.Any())
                {
                    await DisplayAlertAsync("Внимание", "Выберите товары", "Ok");
                    return;
                }

                var customer = dbContext.Customers.ToList().FirstOrDefault();
                var employee = dbContext.Employees.ToList().FirstOrDefault();

                if (customer == null || employee == null)
                {
                    await DisplayAlertAsync("Ошибка", "Клиент или сотрудник не найден", "Ok");
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
                await DisplayAlertAsync("Успех", "Заказ оформлен!", "Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ошибка", ex.InnerException?.Message ?? ex.Message, "Ok");
            }
        }
    }
}