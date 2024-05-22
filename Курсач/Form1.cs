using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Windows.Forms;


namespace Курсач
{
    public partial class Form1 : Form
    {
        private List<Order> orders;

        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();
            orders = new List<Order>();
        }

        private void AddOrder(object sender, EventArgs e)
        {
            Order newOrder = new Order
            {
                OrderNumber = int.Parse(txtOrderNumber.Text),
                OrderDate = dtpOrderDate.Value,
                CustomerName = txtCustomerName.Text,
                CustomerAddress = txtCustomerAddress.Text,
                CompletionTimeInDays = (int)nudCompletionTime.Value,
                OrderCost = decimal.Parse(txtOrderCost.Text)
            };
            orders.Add(newOrder);
            UpdateOrderList();
        }

        private void UpdateOrderList()
        {
            dgvOrders.DataSource = null;
            dgvOrders.DataSource = orders;

            // Set custom column names
            dgvOrders.Columns["OrderNumber"].HeaderText = "Номер замовлення";
            dgvOrders.Columns["OrderDate"].HeaderText = "Дата замовлення";
            dgvOrders.Columns["CustomerName"].HeaderText = "ПІБ замовника";
            dgvOrders.Columns["CustomerAddress"].HeaderText = "Адреса замовника";
            dgvOrders.Columns["CompletionTimeInDays"].HeaderText = "Термін виконання (днів)";
            dgvOrders.Columns["OrderCost"].HeaderText = "Вартість замовлення";
        }

        private void FilterOrdersByMonth(object sender, EventArgs e)
        {
            int month = int.Parse(txtMonth.Text);
            var filteredOrders = orders.Where(o => o.OrderDate.Month == month && o.OrderDate.Year == DateTime.Now.Year).ToList();
            dgvOrders.DataSource = null;
            dgvOrders.DataSource = filteredOrders;
        }

        private void CalculateOrdersForLastThreeYears(object sender, EventArgs e)
        {
            var threeYearsAgo = DateTime.Now.AddYears(-3);
            var recentOrders = orders.Where(o => o.OrderDate >= threeYearsAgo).ToList();
            int count = recentOrders.Count;
            decimal totalCost = recentOrders.Sum(o => o.OrderCost);

            CultureInfo ukCulture = new CultureInfo("uk-UA");
            string formattedTotalCost = totalCost.ToString("C", ukCulture);

            MessageBox.Show($"Всього замовлень: {count}, Загальна сумма: {formattedTotalCost}");
        }

        private void FindMostExpensiveOrderCurrentMonth(object sender, EventArgs e)
        {
            var currentMonthOrders = orders.Where(o => o.OrderDate.Month == DateTime.Now.Month && o.OrderDate.Year == DateTime.Now.Year).ToList();
            var mostExpensiveOrder = currentMonthOrders.OrderByDescending(o => o.OrderCost).FirstOrDefault();

            if (mostExpensiveOrder != null)
            {
                CultureInfo uaCulture = new CultureInfo("uk-UA");
                string formattedCost = mostExpensiveOrder.OrderCost.ToString("C", uaCulture);

                MessageBox.Show($"Найдорожче замовлення: {mostExpensiveOrder.OrderNumber}, Сумма: {formattedCost}");
            }
            else
            {
                MessageBox.Show("У системі відсутні замовлення");
            }
        }

        private void SaveOrdersToFile(object sender, EventArgs e)
        {
            var groupedOrders = orders.GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month });

            foreach (var group in groupedOrders)
            {
                string fileName = @"D:\Замовлення\" + $"{group.Key.Year}_{group.Key.Month}.txt";
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    foreach (var order in group)
                    {
                        writer.WriteLine($"{order.OrderNumber}, {order.OrderDate}, {order.CustomerName}, {order.CustomerAddress}, {order.CompletionTimeInDays}, {order.OrderCost}");
                    }
                }
            }
        }

        private void InitializeCustomComponents()
        {
            Button btnAddOrder = new Button();
            btnAddOrder.Text = "Додати замовлення";
            btnAddOrder.Location = new System.Drawing.Point(150, 260);
            btnAddOrder.Click += new EventHandler(AddOrder);
            this.Controls.Add(btnAddOrder);

            Label lblMonth = new Label();
            lblMonth.Text = "Місяць:";
            lblMonth.Location = new System.Drawing.Point(20, 520);
            this.Controls.Add(lblMonth);

            Button btnFilterByMonth = new Button();
            btnFilterByMonth.Text = "Фільтрувати";
            btnFilterByMonth.Location = new System.Drawing.Point(150, 520);
            btnFilterByMonth.Click += new EventHandler(FilterOrdersByMonth);
            this.Controls.Add(btnFilterByMonth);

            Button btnCalculateOrders = new Button();
            btnCalculateOrders.Text = "Підрахувати замовлення";
            btnCalculateOrders.Location = new System.Drawing.Point(250, 520);
            btnCalculateOrders.Click += new EventHandler(CalculateOrdersForLastThreeYears);
            this.Controls.Add(btnCalculateOrders);

            Button btnFindMostExpensive = new Button();
            btnFindMostExpensive.Text = "Найдорожче замовлення";
            btnFindMostExpensive.Location = new System.Drawing.Point(400, 520);
            btnFindMostExpensive.Click += new EventHandler(FindMostExpensiveOrderCurrentMonth);
            this.Controls.Add(btnFindMostExpensive);

            Button btnSaveToFile = new Button();
            btnSaveToFile.Text = "Зберегти у файли";
            btnSaveToFile.Location = new System.Drawing.Point(500, 520);
            btnSaveToFile.Click += new EventHandler(SaveOrdersToFile);
            this.Controls.Add(btnSaveToFile);
        }

        private void DgvOrders_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}