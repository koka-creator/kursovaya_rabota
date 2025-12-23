using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gruzoperevozki.Models;
using Newtonsoft.Json;

namespace Gruzoperevozki.Data
{
    public class DataStorage
    {
        private static DataStorage? _instance;
        private static readonly object _lock = new object();

        public static DataStorage Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DataStorage();
                        }
                    }
                }
                return _instance;
            }
        }

        private static string DataFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Gruzoperevozki",
            "data.json"
        );

        private static string TxtFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Gruzoperevozki",
            "data.txt"
        );

        private DataModel _data = new DataModel();

        private DataStorage()
        {
            LoadData();
            // Инициализируем данные только если их нет (проверяем все категории)
            bool hasNoData = _data.Cars.Count == 0 && _data.Drivers.Count == 0 && 
                _data.Clients.Count == 0 && _data.Orders.Count == 0 && _data.Trips.Count == 0;
            
            if (hasNoData)
            {
                InitializeDefaultData();
                SaveData();
                ExportToTxt();
            }
        }

        private void LoadData()
        {
            try
            {
                if (File.Exists(DataFilePath))
                {
                    var json = File.ReadAllText(DataFilePath);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        _data = JsonConvert.DeserializeObject<DataModel>(json) ?? new DataModel();
                    }
                    else
                    {
                        _data = new DataModel();
                    }
                }
                else
                {
                    _data = new DataModel();
                }
            }
            catch
            {
                _data = new DataModel();
            }
        }

        public void SaveData()
        {
            try
            {
                var directory = Path.GetDirectoryName(DataFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonConvert.SerializeObject(_data, Formatting.Indented);
                File.WriteAllText(DataFilePath, json);
                
                // Также сохраняем в txt файл
                ExportToTxt();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Ошибка сохранения данных: {ex.Message}", "Ошибка",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void InitializeDefaultData()
        {
            // Инициализация автомобилей
            var carBrands = new[] { "ГАЗ", "КАМАЗ", "МАЗ", "Урал", "ЗИЛ", "Мерседес", "Вольво", "Скания", "МАН", "Ивеко" };
            var carModels = new[] { "3302", "65117", "5336", "4320", "5301", "Actros", "FH16", "R420", "TGX", "Daily" };
            var purposes = new[] { "Стройматериалы", "Продукты питания", "Мебель", "Одежда", "Электроника", "Химия", "Металл", "Дерево", "Бытовая техника", "Сельхозпродукция" };

            for (int i = 0; i < 10; i++)
            {
                _data.Cars.Add(new Car
                {
                    StateNumber = $"А{123 + i}БВ{45 + i}",
                    Brand = carBrands[i],
                    Model = carModels[i],
                    LoadCapacity = 5 + i * 2,
                    Purpose = purposes[i],
                    ManufactureYear = 2015 + i,
                    OverhaulYear = i % 2 == 0 ? 2020 + i : null,
                    MileageAtYearStart = 50000 + i * 10000
                });
            }

            // Инициализация водителей
            var driverNames = new[]
            {
                "Иванов Иван Иванович", "Петров Петр Петрович", "Сидоров Сидор Сидорович",
                "Кузнецов Алексей Владимирович", "Смирнов Дмитрий Сергеевич", "Попов Андрей Николаевич",
                "Васильев Василий Васильевич", "Михайлов Михаил Михайлович", "Новиков Николай Николаевич",
                "Федоров Федор Федорович"
            };
            var categories = new[] { "B", "C", "CE", "D", "DE", "B", "C", "CE", "D", "CE" };
            var classes = new[] { "1", "2", "1", "2", "1", "2", "1", "2", "1", "2" };

            for (int i = 0; i < 10; i++)
            {
                _data.Drivers.Add(new Driver
                {
                    FullName = driverNames[i],
                    EmployeeNumber = $"ТБ-{1000 + i}",
                    BirthYear = 1970 + i * 2,
                    WorkExperience = 5 + i,
                    Category = categories[i],
                    Class = classes[i]
                });
            }

            // Инициализация клиентов (5 физических, 5 юридических)
            var individualNames = new[]
            {
                "Сергеев Сергей Сергеевич", "Александров Александр Александрович",
                "Дмитриев Дмитрий Дмитриевич", "Андреев Андрей Андреевич",
                "Максимов Максим Максимович"
            };
            var companyNames = new[]
            {
                "ООО Торговый Дом", "ЗАО Промышленная Группа", "ООО СтройКомплекс",
                "ИП Коммерческая База", "ООО Логистические Решения"
            };
            var directors = new[]
            {
                "Степанов Степан Степанович", "Николаев Николай Николаевич",
                "Владимиров Владимир Владимирович", "Алексеев Алексей Алексеевич",
                "Романов Роман Романович"
            };

            for (int i = 0; i < 5; i++)
            {
                _data.Clients.Add(new Client
                {
                    Type = ClientType.Individual,
                    FullName = individualNames[i],
                    Phone = $"+7 (495) {100 + i}{10 + i}{10 + i}-{10 + i}{10 + i}-{10 + i}{10 + i}",
                    PassportSeries = $"{1000 + i}",
                    PassportNumber = $"{100000 + i}",
                    PassportIssueDate = new DateTime(2010 + i, 1, 1),
                    PassportIssuedBy = $"УФМС г. Москвы {i + 1}"
                });
            }

            for (int i = 0; i < 5; i++)
            {
                _data.Clients.Add(new Client
                {
                    Type = ClientType.LegalEntity,
                    CompanyName = companyNames[i],
                    DirectorName = directors[i],
                    LegalAddress = $"г. Москва, ул. Примерная, д. {10 + i}, оф. {100 + i}",
                    Phone = $"+7 (495) {200 + i}{20 + i}{20 + i}-{20 + i}{20 + i}-{20 + i}{20 + i}",
                    BankName = $"Банк {i + 1}",
                    AccountNumber = $"{40702810 + i}{1000000000 + i}",
                    TaxId = $"{7700000000 + i}"
                });
            }

            // Инициализация заказов
            var loadingAddresses = new[]
            {
                "г. Москва, ул. Загрузочная, 1", "г. Санкт-Петербург, пр. Погрузочный, 5",
                "г. Казань, ул. Транспортная, 10", "г. Екатеринбург, ул. Грузовая, 15",
                "г. Новосибирск, ул. Складская, 20", "г. Краснодар, ул. Логистическая, 25",
                "г. Нижний Новгород, ул. Товарная, 30", "г. Челябинск, ул. Снабженческая, 35",
                "г. Самара, ул. Поставки, 40", "г. Омск, ул. Доставки, 45"
            };
            var unloadingAddresses = new[]
            {
                "г. Москва, ул. Разгрузочная, 2", "г. Санкт-Петербург, пр. Выгрузочный, 6",
                "г. Казань, ул. Приемная, 11", "г. Екатеринбург, ул. Получательская, 16",
                "г. Новосибирск, ул. Доставки, 21", "г. Краснодар, ул. Конечная, 26",
                "г. Нижний Новгород, ул. Финальная, 31", "г. Челябинск, ул. Целевая, 36",
                "г. Самара, ул. Пункт назначения, 41", "г. Омск, ул. Конечный пункт, 46"
            };

            var orderStatuses = new[] { OrderStatus.Новый, OrderStatus.В_обработке, OrderStatus.Подтвержден, OrderStatus.В_пути, OrderStatus.Доставлен, OrderStatus.Новый, OrderStatus.Подтвержден, OrderStatus.В_пути, OrderStatus.Доставлен, OrderStatus.Отменен };
            
            for (int i = 0; i < 10; i++)
            {
                var order = new Order
                {
                    OrderDate = DateTime.Now.AddDays(-i * 2),
                    SenderClientId = _data.Clients[i % 10].Id,
                    LoadingAddress = loadingAddresses[i],
                    ReceiverClientId = _data.Clients[(i + 1) % 10].Id,
                    UnloadingAddress = unloadingAddresses[i],
                    RouteLength = 100 + i * 50,
                    Cost = 10000 + i * 5000,
                    Status = orderStatuses[i]
                };

                // Добавляем грузы к заказу
                order.CargoItems.Add(new CargoItem
                {
                    Name = $"Груз {i + 1}",
                    Unit = "кг",
                    Quantity = 100 + i * 50,
                    TotalWeight = 500 + i * 100,
                    InsuranceValue = 50000 + i * 10000
                });

                if (i % 2 == 0)
                {
                    order.CargoItems.Add(new CargoItem
                    {
                        Name = $"Дополнительный груз {i + 1}",
                        Unit = "шт",
                        Quantity = 10 + i,
                        TotalWeight = 200 + i * 50,
                        InsuranceValue = 20000 + i * 5000
                    });
                }

                _data.Orders.Add(order);
            }

            // Инициализация рейсов
            var tripStatuses = new[] { TripStatus.Запланирован, TripStatus.В_пути, TripStatus.На_погрузке, TripStatus.В_дороге, TripStatus.На_разгрузке, TripStatus.Завершен, TripStatus.Запланирован, TripStatus.В_пути, TripStatus.Завершен, TripStatus.Отменен };
            
            for (int i = 0; i < 10; i++)
            {
                var trip = new Trip
                {
                    OrderId = _data.Orders[i].Id,
                    CarId = _data.Cars[i].Id,
                    ArrivalDateTime = DateTime.Now.AddDays(i),
                    Status = tripStatuses[i]
                };

                // Добавляем водителей (1-2 водителя на рейс)
                trip.DriverIds.Add(_data.Drivers[i].Id);
                if (i % 3 == 0 && i + 1 < _data.Drivers.Count)
                {
                    trip.DriverIds.Add(_data.Drivers[(i + 1) % 10].Id);
                }

                _data.Trips.Add(trip);
            }
        }

        public void ExportToTxt()
        {
            try
            {
                var directory = Path.GetDirectoryName(TxtFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using var writer = new StreamWriter(TxtFilePath, false, System.Text.Encoding.UTF8);
                writer.WriteLine("=".PadRight(80, '='));
                writer.WriteLine("СИСТЕМА УЧЕТА ГРУЗОВЫХ АВТОПЕРЕВОЗОК");
                writer.WriteLine("=".PadRight(80, '='));
                writer.WriteLine($"Дата экспорта: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
                writer.WriteLine();

                // Автомобили
                writer.WriteLine("АВТОМОБИЛИ");
                writer.WriteLine("-".PadRight(80, '-'));
                for (int i = 0; i < _data.Cars.Count; i++)
                {
                    var car = _data.Cars[i];
                    writer.WriteLine($"{i + 1}. {car.Brand} {car.Model} (Гос. номер: {car.StateNumber})");
                    writer.WriteLine($"   Грузоподъемность: {car.LoadCapacity} т");
                    writer.WriteLine($"   Назначение: {car.Purpose}");
                    writer.WriteLine($"   Год выпуска: {car.ManufactureYear}, Год кап. ремонта: {car.OverhaulYear?.ToString() ?? "Не было"}");
                    writer.WriteLine($"   Пробег на начало года: {car.MileageAtYearStart} км");
                    writer.WriteLine();
                }

                // Водители
                writer.WriteLine("ВОДИТЕЛИ");
                writer.WriteLine("-".PadRight(80, '-'));
                for (int i = 0; i < _data.Drivers.Count; i++)
                {
                    var driver = _data.Drivers[i];
                    writer.WriteLine($"{i + 1}. {driver.FullName} (Табельный номер: {driver.EmployeeNumber})");
                    writer.WriteLine($"   Год рождения: {driver.BirthYear}, Стаж: {driver.WorkExperience} лет");
                    writer.WriteLine($"   Категория: {driver.Category}, Классность: {driver.Class}");
                    writer.WriteLine();
                }

                // Клиенты
                writer.WriteLine("КЛИЕНТЫ");
                writer.WriteLine("-".PadRight(80, '-'));
                for (int i = 0; i < _data.Clients.Count; i++)
                {
                    var client = _data.Clients[i];
                    writer.WriteLine($"{i + 1}. {(client.Type == ClientType.Individual ? "ФИЗИЧЕСКОЕ ЛИЦО" : "ЮРИДИЧЕСКОЕ ЛИЦО")}");
                    if (client.Type == ClientType.Individual)
                    {
                        writer.WriteLine($"   ФИО: {client.FullName}");
                        writer.WriteLine($"   Телефон: {client.Phone}");
                        writer.WriteLine($"   Паспорт: {client.PassportSeries} {client.PassportNumber}, выдан {client.PassportIssueDate:dd.MM.yyyy} {client.PassportIssuedBy}");
                    }
                    else
                    {
                        writer.WriteLine($"   Название: {client.CompanyName}");
                        writer.WriteLine($"   Руководитель: {client.DirectorName}");
                        writer.WriteLine($"   Адрес: {client.LegalAddress}");
                        writer.WriteLine($"   Телефон: {client.Phone}");
                        writer.WriteLine($"   Банк: {client.BankName}, Счет: {client.AccountNumber}");
                        writer.WriteLine($"   ИНН: {client.TaxId}");
                    }
                    writer.WriteLine();
                }

                // Заказы
                writer.WriteLine("ЗАКАЗЫ");
                writer.WriteLine("-".PadRight(80, '-'));
                for (int i = 0; i < _data.Orders.Count; i++)
                {
                    var order = _data.Orders[i];
                    var sender = _data.Clients.FirstOrDefault(c => c.Id == order.SenderClientId);
                    var receiver = _data.Clients.FirstOrDefault(c => c.Id == order.ReceiverClientId);
                    var senderName = sender?.Type == ClientType.Individual ? sender.FullName : sender?.CompanyName ?? "Неизвестно";
                    var receiverName = receiver?.Type == ClientType.Individual ? receiver.FullName : receiver?.CompanyName ?? "Неизвестно";

                    writer.WriteLine($"{i + 1}. Заказ от {order.OrderDate:dd.MM.yyyy}");
                    writer.WriteLine($"   Статус: {order.Status}");
                    writer.WriteLine($"   Отправитель: {senderName}");
                    writer.WriteLine($"   Адрес погрузки: {order.LoadingAddress}");
                    writer.WriteLine($"   Получатель: {receiverName}");
                    writer.WriteLine($"   Адрес разгрузки: {order.UnloadingAddress}");
                    writer.WriteLine($"   Длина маршрута: {order.RouteLength} км");
                    writer.WriteLine($"   Стоимость: {order.Cost:C}");
                    writer.WriteLine($"   Грузы ({order.CargoItems.Count} позиций):");
                    foreach (var cargo in order.CargoItems)
                    {
                        writer.WriteLine($"     - {cargo.Name}: {cargo.Quantity} {cargo.Unit}, Вес: {cargo.TotalWeight} кг, Страх. стоимость: {cargo.InsuranceValue:C}");
                    }
                    writer.WriteLine();
                }

                // Рейсы
                writer.WriteLine("РЕЙСЫ");
                writer.WriteLine("-".PadRight(80, '-'));
                for (int i = 0; i < _data.Trips.Count; i++)
                {
                    var trip = _data.Trips[i];
                    var order = _data.Orders.FirstOrDefault(o => o.Id == trip.OrderId);
                    var car = _data.Cars.FirstOrDefault(c => c.Id == trip.CarId);
                    var drivers = trip.DriverIds
                        .Select(id => _data.Drivers.FirstOrDefault(d => d.Id == id))
                        .Where(d => d != null)
                        .ToList();

                    writer.WriteLine($"{i + 1}. Рейс на {trip.ArrivalDateTime:dd.MM.yyyy HH:mm}");
                    writer.WriteLine($"   Статус: {trip.Status}");
                    writer.WriteLine($"   Заказ: от {order?.OrderDate:dd.MM.yyyy}");
                    writer.WriteLine($"   Автомобиль: {car?.Brand} {car?.Model} ({car?.StateNumber})");
                    writer.WriteLine($"   Водители: {string.Join(", ", drivers.Select(d => d?.FullName))}");
                    writer.WriteLine();
                }

                writer.WriteLine("=".PadRight(80, '='));
                writer.WriteLine($"Всего записей: Автомобили - {_data.Cars.Count}, Водители - {_data.Drivers.Count}, Клиенты - {_data.Clients.Count}, Заказы - {_data.Orders.Count}, Рейсы - {_data.Trips.Count}");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Ошибка экспорта в txt: {ex.Message}", "Ошибка",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        // Cars
        public List<Car> GetCars() => _data.Cars;
        public void AddCar(Car car) => _data.Cars.Add(car);
        public void UpdateCar(Car car)
        {
            var index = _data.Cars.FindIndex(c => c.Id == car.Id);
            if (index >= 0) _data.Cars[index] = car;
        }
        public void DeleteCar(string id) => _data.Cars.RemoveAll(c => c.Id == id);

        // Drivers
        public List<Driver> GetDrivers() => _data.Drivers;
        public void AddDriver(Driver driver) => _data.Drivers.Add(driver);
        public void UpdateDriver(Driver driver)
        {
            var index = _data.Drivers.FindIndex(d => d.Id == driver.Id);
            if (index >= 0) _data.Drivers[index] = driver;
        }
        public void DeleteDriver(string id) => _data.Drivers.RemoveAll(d => d.Id == id);

        // Clients
        public List<Client> GetClients() => _data.Clients;
        public void AddClient(Client client) => _data.Clients.Add(client);
        public void UpdateClient(Client client)
        {
            var index = _data.Clients.FindIndex(c => c.Id == client.Id);
            if (index >= 0) _data.Clients[index] = client;
        }
        public void DeleteClient(string id) => _data.Clients.RemoveAll(c => c.Id == id);

        // Orders
        public List<Order> GetOrders() => _data.Orders;
        public void AddOrder(Order order) => _data.Orders.Add(order);
        public void UpdateOrder(Order order)
        {
            var index = _data.Orders.FindIndex(o => o.Id == order.Id);
            if (index >= 0) _data.Orders[index] = order;
        }
        public void DeleteOrder(string id) => _data.Orders.RemoveAll(o => o.Id == id);

        // Trips
        public List<Trip> GetTrips() => _data.Trips;
        public void AddTrip(Trip trip) => _data.Trips.Add(trip);
        public void UpdateTrip(Trip trip)
        {
            var index = _data.Trips.FindIndex(t => t.Id == trip.Id);
            if (index >= 0) _data.Trips[index] = trip;
        }
        public void DeleteTrip(string id) => _data.Trips.RemoveAll(t => t.Id == id);
    }

    public class DataModel
    {
        public List<Car> Cars { get; set; } = new List<Car>();
        public List<Driver> Drivers { get; set; } = new List<Driver>();
        public List<Client> Clients { get; set; } = new List<Client>();
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<Trip> Trips { get; set; } = new List<Trip>();
    }
}

