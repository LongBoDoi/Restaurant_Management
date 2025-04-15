using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Pkcs;

namespace API.Services
{
    public class FakeDataService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public FakeDataService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Tính thời gian còn lại tới nửa đêm
                    Console.WriteLine("===========================================================================");
                    Console.WriteLine("ĐANG FAKE DỮ LIỆU ORDER...");

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                        // Fake order
                        var randomizer = new Random();

                        // Fake dữ liệu món
                        int menuCount = dbContext.MenuItem.Count();

                        if (menuCount == 0)
                        {
                            Console.WriteLine("Không có dữ liệu món, không thể tạo order!");
                            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                            return;
                        }

                        var itemToTake = randomizer.Next(1, menuCount);
                        List<MenuItem> randomMenuItems = dbContext.MenuItem.OrderBy(x => EF.Functions.Random()).Take(itemToTake).ToList();
                        IEnumerable<OrderDetail> orderDetails = randomMenuItems.Select(mi => {
                            int quantity = randomizer.Next(1, 5);

                            return new OrderDetail
                            {
                                MenuItemID = mi.MenuItemID,
                                MenuItemName = mi.Name,
                                Quantity = quantity,
                                Price = mi.Price,
                                Amount = quantity * mi.Price
                            };
                        });

                        // Fake dữ liệu khách hàng
                        Customer? randomCustomer = null;
                        List<Customer> lstCustomers = dbContext.Customer.OrderBy(x => EF.Functions.Random()).ToList();
                        var randomCustomerIndex = randomizer.Next(0, lstCustomers.Count());
                        if (randomCustomerIndex < lstCustomers.Count())
                        {
                            randomCustomer = lstCustomers.ElementAt(randomCustomerIndex);
                        }

                        // Fake dữ liệu thời gian
                        DateTime orderDate = GetRandomDateTime(new DateTime(2024, 1, 1, 0, 0, 0), DateTime.UtcNow);

                        // Fake dữ liệu bàn
                        IEnumerable<OrderTable> orderTables = [];
                        string tableName = "";
                        int tableCount = dbContext.Table.Count();
                        if (tableCount > 0)
                        {
                            var tablesToTake = randomizer.Next(1, tableCount);
                            List<Table> randomTables = dbContext.Table.OrderBy(x => EF.Functions.Random()).Take(tablesToTake).ToList();
                            orderTables = randomTables.Select(t => new OrderTable
                            {
                                TableID = t.TableID
                            });
                            tableName = String.Join(", ", randomTables.Select(t => t.TableName));
                        }

                        // Fake dữ liệu order
                        decimal tipAmount = randomizer.Next(0, 4) * 5000;
                        decimal netAmount = orderDetails.Sum(od => od.Amount);
                        Order fakeOrder = new Order
                        {
                            CustomerID = randomCustomer?.CustomerID,
                            CustomerName = randomCustomer != null ? randomCustomer.CustomerName : $"Khách hàng {randomizer.Next(1, 100)}",
                            OrderDate = orderDate,
                            NetAmount = netAmount,
                            TipAmount = tipAmount,
                            TotalAmount = netAmount + tipAmount,
                            Status = EnumOrderStatus.Paid,
                            PaymentMethod = (EnumPaymentMethod)randomizer.Next(Enum.GetValues(typeof(EnumPaymentMethod)).Length),
                            OrderTables = orderTables,
                            TableName = tableName
                        };

                        dbContext.Order.Add(fakeOrder);
                        if (dbContext.SaveChanges() > 0)
                        {
                            Console.WriteLine("FAKE THÀNH CÔNG!");
                        }
                    }

                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Có lỗi xảy ra: {ex.Message}");
                }
            }
        }

        public static DateTime GetRandomDateTime(DateTime minDate, DateTime maxDate)
        {
            var random = new Random();
            var range = (maxDate - minDate).TotalSeconds;
            var randomSeconds = random.NextDouble() * range;
            return minDate.AddSeconds(randomSeconds);
        }
    }
}
