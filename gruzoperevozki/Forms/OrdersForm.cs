using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Gruzoperevozki.Data;
using Gruzoperevozki.Models;

namespace Gruzoperevozki.Forms
{
    public partial class OrdersForm : Form
    {
        private DataStorage _storage = DataStorage.Instance;
        private ListView _listView;
        private Button _addButton;
        private Button _editButton;
        private Button _deleteButton;
        private Button _refreshButton;

        public OrdersForm()
        {
            InitializeComponent();
            this.FormClosing += (s, e) => _storage.SaveData();
            LoadOrders();
        }

        private void InitializeComponent()
        {
            this.Text = "Управление заказами";
            this.Size = new Size(1200, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            _listView.Columns.Add("Дата заказа", 100);
            _listView.Columns.Add("Отправитель", 150);
            _listView.Columns.Add("Получатель", 150);
            _listView.Columns.Add("Длина маршрута", 100);
            _listView.Columns.Add("Стоимость", 100);
            _listView.Columns.Add("Статус", 120);
            _listView.Columns.Add("Кол-во позиций", 100);

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
            _refreshButton.Click += (s, e) => LoadOrders();

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

        private void LoadOrders()
        {
            _listView.Items.Clear();
            var clients = _storage.GetClients();
            foreach (var order in _storage.GetOrders())
            {
                var sender = clients.FirstOrDefault(c => c.Id == order.SenderClientId);
                var receiver = clients.FirstOrDefault(c => c.Id == order.ReceiverClientId);
                var senderName = sender?.Type == ClientType.Individual ? sender.FullName : sender?.CompanyName ?? "Неизвестно";
                var receiverName = receiver?.Type == ClientType.Individual ? receiver.FullName : receiver?.CompanyName ?? "Неизвестно";

                var item = new ListViewItem(order.OrderDate.ToString("dd.MM.yyyy"));
                item.SubItems.Add(senderName);
                item.SubItems.Add(receiverName);
                item.SubItems.Add(order.RouteLength.ToString());
                item.SubItems.Add(order.Cost.ToString("C"));
                item.SubItems.Add(order.Status.ToString());
                item.SubItems.Add(order.CargoItems.Count.ToString());
                item.Tag = order;
                _listView.Items.Add(item);
            }
        }

        private void AddButton_Click(object? sender, EventArgs e)
        {
            using var form = new OrderEditForm(_storage);
            if (form.ShowDialog() == DialogResult.OK && form.Order != null)
            {
                _storage.AddOrder(form.Order);
                _storage.SaveData();
                LoadOrders();
            }
        }

        private void EditButton_Click(object? sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите заказ для редактирования", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var order = (Order)_listView.SelectedItems[0].Tag;
            using var form = new OrderEditForm(_storage, order);
            if (form.ShowDialog() == DialogResult.OK && form.Order != null)
            {
                _storage.UpdateOrder(form.Order);
                _storage.SaveData();
                LoadOrders();
            }
        }

        private void DeleteButton_Click(object? sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите заказ для удаления", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Вы уверены, что хотите удалить этот заказ?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var order = (Order)_listView.SelectedItems[0].Tag;
                _storage.DeleteOrder(order.Id);
                _storage.SaveData();
                LoadOrders();
            }
        }
    }
}

