using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.Extensions;
using Microsoft.EntityFrameworkCore;

public class SimulateOrderService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public SimulateOrderService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var randomizer = new Random();

                int nextOrderWait = randomizer.Next(1, 7) * 5;
                Console.WriteLine();
                Console.WriteLine($"Tạo order tiếp theo sau {nextOrderWait} phút");
                Console.WriteLine();
                await Task.Delay(TimeSpan.FromMinutes(nextOrderWait), stoppingToken);

                Console.WriteLine("===========================================================================");
                Console.WriteLine("ĐANG GIẢ LẬP ORDER...");

                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                    // Fake dữ liệu món
                    int menuCount = context.MenuItem.Count();
                    if (menuCount == 0)
                    {
                        Console.WriteLine("Không có dữ liệu món, không thể tạo order!");
                        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                        return;
                    }

                    var itemToTake = randomizer.Next(1, menuCount + 1);
                    var randomMenuItems = context.MenuItem.OrderBy(x => EF.Functions.Random()).Take(itemToTake).ToList();
                    var orderDetails = randomMenuItems.Select(mi =>
                    {
                        int quantity = randomizer.Next(1, 6);
                        return new OrderDetail
                        {
                            MenuItemID = mi.MenuItemID,
                            MenuItemName = mi.Name,
                            Quantity = quantity,
                            Price = mi.Price,
                            Amount = quantity * mi.Price
                        };
                    }).ToList();

                    // Fake dữ liệu khách hàng
                    Customer? randomCustomer = null;
                    var lstCustomers = context.Customer
                        .Include(c => c.Orders)
                        .Where(c => c.Orders == null || c.Orders.FirstOrDefault(o => o.Status == EnumOrderStatus.Active) == null)
                        .OrderBy(x => EF.Functions.Random())
                        .ToList();

                    if (lstCustomers.Any())
                    {
                        var randomCustomerIndex = randomizer.Next(0, lstCustomers.Count() + 1);
                        if (randomCustomerIndex < lstCustomers.Count())
                        {
                            randomCustomer = lstCustomers.ElementAt(randomCustomerIndex);
                        }
                    }

                    // Fake thời gian
                    DateTime orderDate = DateTime.UtcNow;

                    // Fake dữ liệu bàn
                    var availableTables = context.Table.Where(t => t.Status == EnumTableStatus.Available).OrderBy(x => EF.Functions.Random()).ToList();
                    var tablesToTake = randomizer.Next(0, Math.Min(2, availableTables.Count() + 1));
                    var randomTables = availableTables.Take(tablesToTake).ToList();
                    foreach (Table table in randomTables)
                    {
                        table.Status = EnumTableStatus.Occupied;
                        context.Entry(table).State = EntityState.Modified;
                    }
                    var orderTables = randomTables.Select(t => new OrderTable { TableID = t.TableID }).ToList();
                    var tableName = string.Join(", ", randomTables.Select(t => t.TableName));

                    // Fake dữ liệu order
                    decimal tipAmount = randomizer.Next(0, 5) * 5000;
                    decimal netAmount = orderDetails.Sum(od => od.Amount);

                    var fakeOrder = new Order
                    {
                        CustomerID = randomCustomer?.CustomerID,
                        CustomerName = randomCustomer?.CustomerName ?? $"Khách hàng {randomizer.Next(1, 101)}",
                        OrderDate = orderDate,
                        NetAmount = netAmount,
                        TipAmount = tipAmount,
                        TotalAmount = netAmount + tipAmount,
                        Status = EnumOrderStatus.Active,
                        OrderTables = orderTables,
                        TableName = tableName,
                        OrderDetails = orderDetails
                    };

                    context.Order.Add(fakeOrder);
                    if (context.SaveChanges() > 0)
                    {
                        Console.WriteLine("GIẢ LẬP ORDER THÀNH CÔNG!");
                        Console.WriteLine("===========================================================================");

                        int orderMinutes = randomizer.Next(1, 7) * 5;
                        _ = Task.Run(() => CompleteOrder(fakeOrder.OrderID, orderMinutes));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Có lỗi xảy ra: {ex.Message}");
            }
        }
    }

    private async Task CompleteOrder(Guid orderId, int waitingMinutes)
    {
        try
        {
            Console.WriteLine("===========================================================================");
            Console.WriteLine($"Thanh toán order sau {waitingMinutes} phút");
            await Task.Delay(TimeSpan.FromMinutes(waitingMinutes));

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                var order = context.Order.Include(o => o.OrderTables).ThenInclude(ot => ot.Table).FirstOrDefault(o => o.OrderID == orderId);
                if (order != null)
                {
                    var randomizer = new Random();
                    order.PaidAmount = order.TotalAmount;
                    order.PaymentMethod = (EnumPaymentMethod)randomizer.Next(Enum.GetValues(typeof(EnumPaymentMethod)).Length);
                    order.Status = EnumOrderStatus.Paid;

                    context.Entry(order).State = EntityState.Modified;

                    if (order.OrderTables != null && order.OrderTables.Any())
                    {
                        IEnumerable<Table?> tables = order.OrderTables.Select(ot => ot.Table).ToList();
                        foreach (Table? table in tables)
                        {
                            if (table != null)
                            {
                                table.Status = EnumTableStatus.Available;
                                table.RemoveAllReferences();
                                context.Entry(table).State = EntityState.Modified;
                            }
                        }
                    }

                    order.RemoveAllReferences();
                    if (await context.SaveChangesAsync() > 0)
                    {
                        Console.WriteLine("Thanh toán Order thành công");
                        Console.WriteLine("===========================================================================");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi thanh toán Order: {ex.Message}");
        }
    }
}
