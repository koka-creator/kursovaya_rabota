using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Gruzoperevozki.Data;
using Gruzoperevozki.Models;

namespace Gruzoperevozki.Forms
{
    public partial class OrderEditForm : Form
    {
        public Order? Order { get; private set; }
        private DataStorage _storage;
        private DateTimePicker _orderDatePicker;
        private ComboBox _senderComboBox;
        private TextBox _loadingAddressTextBox;
        private ComboBox _receiverComboBox;
        private TextBox _unloadingAddressTextBox;
        private NumericUpDown _routeLengthNumeric;
        private NumericUpDown _costNumeric;
        private ComboBox _statusComboBox;
        private ListView _cargoListView;
        private Button _addCargoButton;
        private Button _editCargoButton;
        private Button _deleteCargoButton;
        private Button _saveButton;
        private Button _cancelButton;

        public OrderEditForm(DataStorage storage, Order? order = null)
        {
            _storage = storage;
            Order = order ?? new Order { OrderDate = DateTime.Now };
            InitializeComponent();
            LoadOrderData();
        }

        private void InitializeComponent()
        {
            this.Text = Order?.Id != null ? "Редактирование заказа" : "Добавление заказа";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                AutoScroll = true
            };

            int y = 10;

            panel.Controls.Add(new Label { Text = "Дата заказа:", Location = new Point(10, y), AutoSize = true });
            _orderDatePicker = new DateTimePicker { Location = new Point(150, y), Size = new Size(300, 23) };
            panel.Controls.Add(_orderDatePicker);
            y += 35;

            panel.Controls.Add(new Label { Text = "Клиент-отправитель:", Location = new Point(10, y), AutoSize = true });
            _senderComboBox = new ComboBox { Location = new Point(150, y), Size = new Size(300, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            LoadClientsToComboBox(_senderComboBox);
            panel.Controls.Add(_senderComboBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Адрес погрузки:", Location = new Point(10, y), AutoSize = true });
            _loadingAddressTextBox = new TextBox { Location = new Point(150, y), Size = new Size(600, 23) };
            panel.Controls.Add(_loadingAddressTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Клиент-получатель:", Location = new Point(10, y), AutoSize = true });
            _receiverComboBox = new ComboBox { Location = new Point(150, y), Size = new Size(300, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            LoadClientsToComboBox(_receiverComboBox);
            panel.Controls.Add(_receiverComboBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Адрес разгрузки:", Location = new Point(10, y), AutoSize = true });
            _unloadingAddressTextBox = new TextBox { Location = new Point(150, y), Size = new Size(600, 23) };
            panel.Controls.Add(_unloadingAddressTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Длина маршрута (км):", Location = new Point(10, y), AutoSize = true });
            _routeLengthNumeric = new NumericUpDown { Location = new Point(150, y), Size = new Size(200, 23), Minimum = 0, Maximum = 100000, DecimalPlaces = 2 };
            panel.Controls.Add(_routeLengthNumeric);
            y += 35;

            panel.Controls.Add(new Label { Text = "Стоимость заказа:", Location = new Point(10, y), AutoSize = true });
            _costNumeric = new NumericUpDown { Location = new Point(150, y), Size = new Size(200, 23), Minimum = 0, Maximum = 10000000, DecimalPlaces = 2 };
            panel.Controls.Add(_costNumeric);
            y += 35;

            panel.Controls.Add(new Label { Text = "Статус заказа:", Location = new Point(10, y), AutoSize = true });
            _statusComboBox = new ComboBox { Location = new Point(150, y), Size = new Size(300, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            _statusComboBox.Items.AddRange(Enum.GetNames(typeof(OrderStatus)));
            panel.Controls.Add(_statusComboBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Перечень грузов:", Location = new Point(10, y), AutoSize = true });
            y += 25;

            _cargoListView = new ListView
            {
                Location = new Point(10, y),
                Size = new Size(800, 200),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            _cargoListView.Columns.Add("Название", 150);
            _cargoListView.Columns.Add("Ед. изм.", 80);
            _cargoListView.Columns.Add("Количество", 100);
            _cargoListView.Columns.Add("Вес (кг)", 100);
            _cargoListView.Columns.Add("Страх. стоимость", 120);
            panel.Controls.Add(_cargoListView);
            y += 210;

            _addCargoButton = new Button { Text = "Добавить груз", Location = new Point(10, y), Size = new Size(100, 30) };
            _addCargoButton.Click += AddCargoButton_Click;
            panel.Controls.Add(_addCargoButton);

            _editCargoButton = new Button { Text = "Редактировать", Location = new Point(120, y), Size = new Size(100, 30) };
            _editCargoButton.Click += EditCargoButton_Click;
            panel.Controls.Add(_editCargoButton);

            _deleteCargoButton = new Button { Text = "Удалить", Location = new Point(230, y), Size = new Size(100, 30) };
            _deleteCargoButton.Click += DeleteCargoButton_Click;
            panel.Controls.Add(_deleteCargoButton);
            y += 40;

            _saveButton = new Button { Text = "Сохранить", Location = new Point(150, y), Size = new Size(100, 30), DialogResult = DialogResult.OK };
            _saveButton.Click += SaveButton_Click;
            panel.Controls.Add(_saveButton);

            _cancelButton = new Button { Text = "Отмена", Location = new Point(260, y), Size = new Size(100, 30), DialogResult = DialogResult.Cancel };
            panel.Controls.Add(_cancelButton);

            this.Controls.Add(panel);
        }

        private void LoadClientsToComboBox(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            foreach (var client in _storage.GetClients())
            {
                var displayName = client.Type == ClientType.Individual
                    ? $"{client.FullName} (Физ. лицо)"
                    : $"{client.CompanyName} (Юр. лицо)";
                comboBox.Items.Add(new ClientComboBoxItem { Client = client, DisplayName = displayName });
            }
            comboBox.DisplayMember = "DisplayName";
        }

        private void LoadOrderData()
        {
            if (Order == null) return;

            _orderDatePicker.Value = Order.OrderDate;
            _loadingAddressTextBox.Text = Order.LoadingAddress;
            _unloadingAddressTextBox.Text = Order.UnloadingAddress;
            _routeLengthNumeric.Value = Order.RouteLength;
            _costNumeric.Value = Order.Cost;
            _statusComboBox.SelectedItem = Order.Status.ToString();

            var senderItem = _senderComboBox.Items.Cast<ClientComboBoxItem>()
                .FirstOrDefault(i => i.Client.Id == Order.SenderClientId);
            if (senderItem != null) _senderComboBox.SelectedItem = senderItem;

            var receiverItem = _receiverComboBox.Items.Cast<ClientComboBoxItem>()
                .FirstOrDefault(i => i.Client.Id == Order.ReceiverClientId);
            if (receiverItem != null) _receiverComboBox.SelectedItem = receiverItem;

            LoadCargoItems();
        }

        private void LoadCargoItems()
        {
            _cargoListView.Items.Clear();
            if (Order == null) return;

            foreach (var cargo in Order.CargoItems)
            {
                var item = new ListViewItem(cargo.Name);
                item.SubItems.Add(cargo.Unit);
                item.SubItems.Add(cargo.Quantity.ToString());
                item.SubItems.Add(cargo.TotalWeight.ToString());
                item.SubItems.Add(cargo.InsuranceValue.ToString("C"));
                item.Tag = cargo;
                _cargoListView.Items.Add(item);
            }
        }

        private void AddCargoButton_Click(object? sender, EventArgs e)
        {
            using var form = new CargoItemEditForm();
            if (form.ShowDialog() == DialogResult.OK && form.CargoItem != null)
            {
                if (Order != null)
                {
                    Order.CargoItems.Add(form.CargoItem);
                    LoadCargoItems();
                }
            }
        }

        private void EditCargoButton_Click(object? sender, EventArgs e)
        {
            if (_cargoListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите груз для редактирования", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var cargo = (CargoItem)_cargoListView.SelectedItems[0].Tag;
            using var form = new CargoItemEditForm(cargo);
            if (form.ShowDialog() == DialogResult.OK && form.CargoItem != null)
            {
                var index = Order?.CargoItems.IndexOf(cargo) ?? -1;
                if (index >= 0 && Order != null)
                {
                    Order.CargoItems[index] = form.CargoItem;
                    LoadCargoItems();
                }
            }
        }

        private void DeleteCargoButton_Click(object? sender, EventArgs e)
        {
            if (_cargoListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите груз для удаления", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var cargo = (CargoItem)_cargoListView.SelectedItems[0].Tag;
            Order?.CargoItems.Remove(cargo);
            LoadCargoItems();
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            if (Order == null) return;

            if (_senderComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента-отправителя", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_receiverComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента-получателя", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Order.OrderDate = _orderDatePicker.Value;
            Order.SenderClientId = ((ClientComboBoxItem)_senderComboBox.SelectedItem).Client.Id;
            Order.LoadingAddress = _loadingAddressTextBox.Text;
            Order.ReceiverClientId = ((ClientComboBoxItem)_receiverComboBox.SelectedItem).Client.Id;
            Order.UnloadingAddress = _unloadingAddressTextBox.Text;
            Order.RouteLength = _routeLengthNumeric.Value;
            Order.Cost = _costNumeric.Value;
            
            if (_statusComboBox.SelectedItem != null && Enum.TryParse<OrderStatus>(_statusComboBox.SelectedItem.ToString(), out var status))
            {
                Order.Status = status;
            }

            this.DialogResult = DialogResult.OK;
        }

        private class ClientComboBoxItem
        {
            public Client Client { get; set; } = null!;
            public string DisplayName { get; set; } = string.Empty;
        }
    }
}


