using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Gruzoperevozki.Data;
using Gruzoperevozki.Models;

namespace Gruzoperevozki.Forms
{
    public partial class ClientsForm : Form
    {
        private DataStorage _storage = DataStorage.Instance;
        private ListView _listView;
        private Button _addButton;
        private Button _editButton;
        private Button _deleteButton;
        private Button _refreshButton;

        public ClientsForm()
        {
            InitializeComponent();
            this.FormClosing += (s, e) => _storage.SaveData();
            LoadClients();
        }

        private void InitializeComponent()
        {
            this.Text = "Управление клиентами";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            _listView.Columns.Add("Тип", 100);
            _listView.Columns.Add("Название/ФИО", 200);
            _listView.Columns.Add("Телефон", 120);
            _listView.Columns.Add("Доп. информация", 300);

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
            _refreshButton.Click += (s, e) => LoadClients();

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

        private void LoadClients()
        {
            _listView.Items.Clear();
            foreach (var client in _storage.GetClients())
            {
                var item = new ListViewItem(client.Type == ClientType.Individual ? "Физ. лицо" : "Юр. лицо");
                item.SubItems.Add(client.Type == ClientType.Individual ? client.FullName ?? "" : client.CompanyName ?? "");
                item.SubItems.Add(client.Phone);
                string additionalInfo = client.Type == ClientType.Individual
                    ? $"Паспорт: {client.PassportSeries} {client.PassportNumber}"
                    : $"ИНН: {client.TaxId}, Адрес: {client.LegalAddress}";
                item.SubItems.Add(additionalInfo);
                item.Tag = client;
                _listView.Items.Add(item);
            }
        }

        private void AddButton_Click(object? sender, EventArgs e)
        {
            using var form = new ClientEditForm();
            if (form.ShowDialog() == DialogResult.OK && form.Client != null)
            {
                _storage.AddClient(form.Client);
                _storage.SaveData();
                LoadClients();
            }
        }

        private void EditButton_Click(object? sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите клиента для редактирования", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var client = (Client)_listView.SelectedItems[0].Tag;
            using var form = new ClientEditForm(client);
            if (form.ShowDialog() == DialogResult.OK && form.Client != null)
            {
                _storage.UpdateClient(form.Client);
                _storage.SaveData();
                LoadClients();
            }
        }

        private void DeleteButton_Click(object? sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите клиента для удаления", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Вы уверены, что хотите удалить этого клиента?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var client = (Client)_listView.SelectedItems[0].Tag;
                _storage.DeleteClient(client.Id);
                _storage.SaveData();
                LoadClients();
            }
        }
    }
}

