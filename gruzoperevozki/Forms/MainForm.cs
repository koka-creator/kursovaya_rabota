using System;
using System.Windows.Forms;
using Gruzoperevozki.Data;
using Gruzoperevozki.Forms;

namespace Gruzoperevozki.Forms
{
    public partial class MainForm : Form
    {
        private DataStorage _storage = DataStorage.Instance;

        public MainForm()
        {
            InitializeComponent();
            this.FormClosing += MainForm_FormClosing;
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _storage.SaveData();
        }

        private void InitializeComponent()
        {
            this.Text = "Система учета грузовых автоперевозок";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            var titleLabel = new Label
            {
                Text = "Система учета грузовых автоперевозок",
                Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold),
                AutoSize = true,
                Location = new System.Drawing.Point(20, 20)
            };

            var carsButton = new Button
            {
                Text = "Автомобили",
                Size = new System.Drawing.Size(200, 50),
                Location = new System.Drawing.Point(20, 80),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 12F)
            };
            carsButton.Click += CarsButton_Click;

            var driversButton = new Button
            {
                Text = "Водители",
                Size = new System.Drawing.Size(200, 50),
                Location = new System.Drawing.Point(20, 140),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 12F)
            };
            driversButton.Click += DriversButton_Click;

            var clientsButton = new Button
            {
                Text = "Клиенты",
                Size = new System.Drawing.Size(200, 50),
                Location = new System.Drawing.Point(20, 200),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 12F)
            };
            clientsButton.Click += ClientsButton_Click;

            var ordersButton = new Button
            {
                Text = "Заказы",
                Size = new System.Drawing.Size(200, 50),
                Location = new System.Drawing.Point(20, 260),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 12F)
            };
            ordersButton.Click += OrdersButton_Click;

            var tripsButton = new Button
            {
                Text = "Рейсы",
                Size = new System.Drawing.Size(200, 50),
                Location = new System.Drawing.Point(20, 320),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 12F)
            };
            tripsButton.Click += TripsButton_Click;

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(carsButton);
            panel.Controls.Add(driversButton);
            panel.Controls.Add(clientsButton);
            panel.Controls.Add(ordersButton);
            panel.Controls.Add(tripsButton);

            this.Controls.Add(panel);
        }

        private void CarsButton_Click(object? sender, EventArgs e)
        {
            using var form = new CarsForm();
            form.ShowDialog();
        }

        private void DriversButton_Click(object? sender, EventArgs e)
        {
            using var form = new DriversForm();
            form.ShowDialog();
        }

        private void ClientsButton_Click(object? sender, EventArgs e)
        {
            using var form = new ClientsForm();
            form.ShowDialog();
        }

        private void OrdersButton_Click(object? sender, EventArgs e)
        {
            using var form = new OrdersForm();
            form.ShowDialog();
        }

        private void TripsButton_Click(object? sender, EventArgs e)
        {
            using var form = new TripsForm();
            form.ShowDialog();
        }
    }
}

