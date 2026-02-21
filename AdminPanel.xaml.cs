using BakeryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace SP1
{
    public partial class AdminPanel : ContentPage
    {
        public BakeryDbContext dbContext = new BakeryDbContext();

        public AdminPanel()
        {
            InitializeComponent();
            CreateCategoryData();
        }
        public void CreateCategoryData()
        {
            var products = dbContext.Products.ToList();
            var productText = new Label
            {
                Text = "Продукты на складе: ",
                FontSize = 25,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center
            };
            MainLayout.Children.Add(productText);

            foreach (var product in products)
            {
                var frame = new Frame
                {
                    BorderColor = Colors.Gray,
                    CornerRadius = 10,
                    Padding = 10,
                    Margin = new Thickness(0, 5)
                };

                var productName = new Label
                {
                    Text = $"{product.Name}. Цена: {(int)product.Price} ₸",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.Center
                };

                frame.Content = productName;
                MainLayout.Children.Add(frame);
            }
        }

    }
}
