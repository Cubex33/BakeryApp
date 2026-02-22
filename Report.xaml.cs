using BakeryApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;

namespace SP1
{
    public partial class Report : ContentPage
    {
        public BakeryDbContext dbContext = new BakeryDbContext();

        public Report()
        {
            InitializeComponent();
            CreateReport();
        }

        void CreateReport()
        {
            var ordersWithDetails = dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ToList();

            foreach (var order in ordersWithDetails)
            {
                var customer = dbContext.Customers.FirstOrDefault(c => c.Id == order.CustomerId);
                if (customer == null) continue;

                var frame = new Frame
                {
                    BackgroundColor = Colors.Black,
                    Padding = 10,
                    Margin = new Thickness(5)
                };

                var verticalStackLayout = new VerticalStackLayout();

                foreach (var item in order.OrderItems)
                {
                    var product = dbContext.Products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product == null) continue;

                    var reportLabel = new Label
                    {
                        Text = $"{order.Id}. Продавец: {customer.FirstName} {customer.LastName} продал {product.Name} за {item.Price}₸. Дата: {order.OrderDate}",
                        TextColor = Colors.White
                    };

                    verticalStackLayout.Children.Add(reportLabel);
                }

                frame.Content = verticalStackLayout;
                MainLayout.Children.Add(frame);
            }
        }
    }
}
