using System;
using System.Drawing;
using System.Windows.Forms;
using Gruzoperevozki.Models;

namespace Gruzoperevozki.Forms
{
    public partial class ClientEditForm : Form
    {
        public Client? Client { get; private set; }
        private RadioButton _individualRadio;
        private RadioButton _legalRadio;
        private Panel _individualPanel;
        private Panel _legalPanel;
        private Button _saveButton;
        private Button _cancelButton;

        // Individual fields
        private TextBox _fullNameTextBox;
        private TextBox _phoneTextBox;
        private TextBox _passportSeriesTextBox;
        private TextBox _passportNumberTextBox;
        private DateTimePicker _passportIssueDatePicker;
        private TextBox _passportIssuedByTextBox;

        // Legal fields
        private TextBox _companyNameTextBox;
        private TextBox _directorNameTextBox;
        private TextBox _legalAddressTextBox;
        private TextBox _legalPhoneTextBox;
        private TextBox _bankNameTextBox;
        private TextBox _accountNumberTextBox;
        private TextBox _taxIdTextBox;

        public ClientEditForm(Client? client = null)
        {
            Client = client ?? new Client { Type = ClientType.Individual };
            InitializeComponent();
            LoadClientData();
        }

        private void InitializeComponent()
        {
            this.Text = Client?.Id != null ? "Редактирование клиента" : "Добавление клиента";
            this.Size = new Size(600, 600);
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

            var typeLabel = new Label { Text = "Тип клиента:", Location = new Point(10, y), AutoSize = true };
            panel.Controls.Add(typeLabel);
            y += 25;

            _individualRadio = new RadioButton { Text = "Физическое лицо", Location = new Point(10, y), Checked = true };
            _individualRadio.CheckedChanged += TypeRadio_CheckedChanged;
            panel.Controls.Add(_individualRadio);
            y += 25;

            _legalRadio = new RadioButton { Text = "Юридическое лицо", Location = new Point(150, y - 25) };
            _legalRadio.CheckedChanged += TypeRadio_CheckedChanged;
            panel.Controls.Add(_legalRadio);
            y += 10;

            // Individual panel
            _individualPanel = new Panel { Location = new Point(10, y), Size = new Size(550, 300), Visible = true };
            CreateIndividualPanel(_individualPanel);
            panel.Controls.Add(_individualPanel);

            // Legal panel
            _legalPanel = new Panel { Location = new Point(10, y), Size = new Size(550, 400), Visible = false };
            CreateLegalPanel(_legalPanel);
            panel.Controls.Add(_legalPanel);

            y += 450;

            _saveButton = new Button { Text = "Сохранить", Location = new Point(150, y), Size = new Size(100, 30), DialogResult = DialogResult.OK };
            _saveButton.Click += SaveButton_Click;
            panel.Controls.Add(_saveButton);

            _cancelButton = new Button { Text = "Отмена", Location = new Point(260, y), Size = new Size(100, 30), DialogResult = DialogResult.Cancel };
            panel.Controls.Add(_cancelButton);

            this.Controls.Add(panel);
        }

        private void CreateIndividualPanel(Panel panel)
        {
            int y = 0;

            panel.Controls.Add(new Label { Text = "ФИО:", Location = new Point(10, y), AutoSize = true });
            _fullNameTextBox = new TextBox { Location = new Point(150, y), Size = new Size(380, 23) };
            panel.Controls.Add(_fullNameTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Телефон:", Location = new Point(10, y), AutoSize = true });
            _phoneTextBox = new TextBox { Location = new Point(150, y), Size = new Size(380, 23) };
            panel.Controls.Add(_phoneTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Серия паспорта:", Location = new Point(10, y), AutoSize = true });
            _passportSeriesTextBox = new TextBox { Location = new Point(150, y), Size = new Size(380, 23) };
            panel.Controls.Add(_passportSeriesTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Номер паспорта:", Location = new Point(10, y), AutoSize = true });
            _passportNumberTextBox = new TextBox { Location = new Point(150, y), Size = new Size(380, 23) };
            panel.Controls.Add(_passportNumberTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Дата выдачи:", Location = new Point(10, y), AutoSize = true });
            _passportIssueDatePicker = new DateTimePicker { Location = new Point(150, y), Size = new Size(380, 23) };
            panel.Controls.Add(_passportIssueDatePicker);
            y += 35;

            panel.Controls.Add(new Label { Text = "Кем выдан:", Location = new Point(10, y), AutoSize = true });
            _passportIssuedByTextBox = new TextBox { Location = new Point(150, y), Size = new Size(380, 23) };
            panel.Controls.Add(_passportIssuedByTextBox);
        }

        private void CreateLegalPanel(Panel panel)
        {
            int y = 0;

            panel.Controls.Add(new Label { Text = "Название:", Location = new Point(10, y), AutoSize = true });
            _companyNameTextBox = new TextBox { Location = new Point(150, y), Size = new Size(380, 23) };
            panel.Controls.Add(_companyNameTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "ФИО руководителя:", Location = new Point(10, y), AutoSize = true });
            _directorNameTextBox = new TextBox { Location = new Point(150, y), Size = new Size(380, 23) };
            panel.Controls.Add(_directorNameTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Юридический адрес:", Location = new Point(10, y), AutoSize = true });
            _legalAddressTextBox = new TextBox { Location = new Point(150, y), Size = new Size(380, 23) };
            panel.Controls.Add(_legalAddressTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Телефон:", Location = new Point(10, y), AutoSize = true });
            _legalPhoneTextBox = new TextBox { Location = new Point(150, y), Size = new Size(380, 23) };
            panel.Controls.Add(_legalPhoneTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Банк:", Location = new Point(10, y), AutoSize = true });
            _bankNameTextBox = new TextBox { Location = new Point(150, y), Size = new Size(380, 23) };
            panel.Controls.Add(_bankNameTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "Расчетный счет:", Location = new Point(10, y), AutoSize = true });
            _accountNumberTextBox = new TextBox { Location = new Point(150, y), Size = new Size(380, 23) };
            panel.Controls.Add(_accountNumberTextBox);
            y += 35;

            panel.Controls.Add(new Label { Text = "ИНН:", Location = new Point(10, y), AutoSize = true });
            _taxIdTextBox = new TextBox { Location = new Point(150, y), Size = new Size(380, 23) };
            panel.Controls.Add(_taxIdTextBox);
        }

        private void TypeRadio_CheckedChanged(object? sender, EventArgs e)
        {
            if (Client == null) return;

            if (_individualRadio.Checked)
            {
                Client.Type = ClientType.Individual;
                _individualPanel.Visible = true;
                _legalPanel.Visible = false;
            }
            else
            {
                Client.Type = ClientType.LegalEntity;
                _individualPanel.Visible = false;
                _legalPanel.Visible = true;
            }
        }

        private void LoadClientData()
        {
            if (Client == null) return;

            if (Client.Type == ClientType.Individual)
            {
                _individualRadio.Checked = true;
                _fullNameTextBox.Text = Client.FullName ?? "";
                _phoneTextBox.Text = Client.Phone;
                _passportSeriesTextBox.Text = Client.PassportSeries ?? "";
                _passportNumberTextBox.Text = Client.PassportNumber ?? "";
                if (Client.PassportIssueDate.HasValue)
                    _passportIssueDatePicker.Value = Client.PassportIssueDate.Value;
                _passportIssuedByTextBox.Text = Client.PassportIssuedBy ?? "";
            }
            else
            {
                _legalRadio.Checked = true;
                _companyNameTextBox.Text = Client.CompanyName ?? "";
                _directorNameTextBox.Text = Client.DirectorName ?? "";
                _legalAddressTextBox.Text = Client.LegalAddress ?? "";
                _legalPhoneTextBox.Text = Client.Phone;
                _bankNameTextBox.Text = Client.BankName ?? "";
                _accountNumberTextBox.Text = Client.AccountNumber ?? "";
                _taxIdTextBox.Text = Client.TaxId ?? "";
            }
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            if (Client == null) return;

            if (Client.Type == ClientType.Individual)
            {
                if (string.IsNullOrWhiteSpace(_fullNameTextBox.Text))
                {
                    MessageBox.Show("Введите ФИО клиента", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Client.FullName = _fullNameTextBox.Text;
                Client.Phone = _phoneTextBox.Text;
                Client.PassportSeries = _passportSeriesTextBox.Text;
                Client.PassportNumber = _passportNumberTextBox.Text;
                Client.PassportIssueDate = _passportIssueDatePicker.Value;
                Client.PassportIssuedBy = _passportIssuedByTextBox.Text;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(_companyNameTextBox.Text))
                {
                    MessageBox.Show("Введите название компании", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Client.CompanyName = _companyNameTextBox.Text;
                Client.DirectorName = _directorNameTextBox.Text;
                Client.LegalAddress = _legalAddressTextBox.Text;
                Client.Phone = _legalPhoneTextBox.Text;
                Client.BankName = _bankNameTextBox.Text;
                Client.AccountNumber = _accountNumberTextBox.Text;
                Client.TaxId = _taxIdTextBox.Text;
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}


