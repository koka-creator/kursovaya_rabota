using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Gruzoperevozki.Data;
using Gruzoperevozki.Models;

namespace Gruzoperevozki.Forms
{
    public partial class TripsForm : Form
    {
        private DataStorage _storage = DataStorage.Instance;
        private ListView _listView;
        private Button _addButton;
        private Button _editButton;
        private Button _deleteButton;
        private Button _refreshButton;

        public TripsForm()
        {
            InitializeComponent();
            this.FormClosing += (s, e) => _storage.SaveData();
            LoadTrips();
        }

        private void InitializeComponent()
        {
            this.Text = "Управление рейсами";
            this.Size = new Size(1200, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            _listView.Columns.Add("Дата/время прибытия", 150);
            _listView.Columns.Add("Заказ", 100);
            _listView.Columns.Add("Автомобиль", 150);
            _listView.Columns.Add("Водители", 250);
            _listView.Columns.Add("Статус", 120);

            _addButton = new Button
            {
                Text = "Добавить",
                Size = new Size(100, 30),
                Location = new Point(10, 10)
            };
            _addButton.Click += AddButton_Click;

            _editButton = new Button
            {
                Text = "Редактировать",
                Size = new Size(100, 30),
                Location = new Point(120, 10)
            };
            _editButton.Click += EditButton_Click;

            _deleteButton = new Button
            {
                Text = "Удалить",
                Size = new Size(100, 30),
                Location = new Point(230, 10)
            };
            _deleteButton.Click += DeleteButton_Click;

            _refreshButton = new Button
            {
                Text = "Обновить",
                Size = new Size(100, 30),
                Location = new Point(340, 10)
            };
            _refreshButton.Click += (s, e) => LoadTrips();

            var buttonPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top
            };
            buttonPanel.Controls.AddRange(new Control[] { _addButton, _editButton, _deleteButton, _refreshButton });

            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            mainPanel.Controls.Add(_listView);

            this.Controls.Add(mainPanel);
            this.Controls.Add(buttonPanel);
        }

        private void LoadTrips()
        {
            _listView.Items.Clear();
            var orders = _storage.GetOrders();
            var cars = _storage.GetCars();
            var drivers = _storage.GetDrivers();

            foreach (var trip in _storage.GetTrips())
            {
                var order = orders.FirstOrDefault(o => o.Id == trip.OrderId);
                var car = cars.FirstOrDefault(c => c.Id == trip.CarId);
                var driverNames = trip.DriverIds
                    .Select(id => drivers.FirstOrDefault(d => d.Id == id))
                    .Where(d => d != null)
                    .Select(d => d!.FullName)
                    .ToList();

                var item = new ListViewItem(trip.ArrivalDateTime.ToString("dd.MM.yyyy HH:mm"));
                item.SubItems.Add(order != null ? $"Заказ от {order.OrderDate:dd.MM.yyyy}" : "Неизвестно");
                item.SubItems.Add(car != null ? $"{car.Brand} {car.Model} ({car.StateNumber})" : "Неизвестно");
                item.SubItems.Add(string.Join(", ", driverNames));
                item.SubItems.Add(trip.Status.ToString());
                item.Tag = trip;
                _listView.Items.Add(item);
            }
        }

        private void AddButton_Click(object? sender, EventArgs e)
        {
            using var form = new TripEditForm(_storage);
            if (form.ShowDialog() == DialogResult.OK && form.Trip != null)
            {
                _storage.AddTrip(form.Trip);
                _storage.SaveData();
                LoadTrips();
            }
        }

        private void EditButton_Click(object? sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите рейс для редактирования", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var trip = (Trip)_listView.SelectedItems[0].Tag;
            using var form = new TripEditForm(_storage, trip);
            if (form.ShowDialog() == DialogResult.OK && form.Trip != null)
            {
                _storage.UpdateTrip(form.Trip);
                _storage.SaveData();
                LoadTrips();
            }
        }

        private void DeleteButton_Click(object? sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите рейс для удаления", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Вы уверены, что хотите удалить этот рейс?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var trip = (Trip)_listView.SelectedItems[0].Tag;
                _storage.DeleteTrip(trip.Id);
                _storage.SaveData();
                LoadTrips();
            }
        }
    }
}

