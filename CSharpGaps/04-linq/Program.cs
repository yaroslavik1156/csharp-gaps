namespace _04_linq;
internal class Program
{
    public static void Main()
    {
        List<Customer> customers = new List<Customer>
        {
            new Customer(1, "Charlie", "London"),
            new Customer(2, "Yaroslav", "Moscow"),
            new Customer(3, "Fedya", "Severomorsk"),
            new Customer(4, "Ivan", "Amsterdam"),
            new Customer(5, "Alex", "California"),
            new Customer(6, "Dmitry", "Kazan")
        };

        List<Order> orders = new List<Order>
        {
            new Order(1, 1, 4000, DateTime.Now.AddDays(-40)),
            new Order(2, 1, 700, DateTime.Now.AddDays(-2)),
            new Order(3, 2, 3100, DateTime.Now.AddDays(-3)),
            new Order(4, 2, 52110, DateTime.Now.AddDays(-20)),
            new Order(5, 1, 69000, DateTime.Now.AddDays(-10)),
            new Order(6, 3, 200, DateTime.Now.AddDays(-1)),
            new Order(7, 3, 2250, DateTime.Now.AddDays(-50)),
            new Order(8, 3, 5203, DateTime.Now.AddDays(-12)),
            new Order(9, 1, 40010, DateTime.Now.AddDays(-34)),
            new Order(10, 4, 5010, DateTime.Now.AddDays(-11)),
            new Order(11, 5, 2400, DateTime.Now.AddDays(-13)),
            new Order(12, 2, 500, DateTime.Now.AddDays(-5)),
            new Order(13, 1, 300, DateTime.Now.AddDays(-7)),
            new Order(14, 4, 5400, DateTime.Now.AddDays(-6)),
            new Order(15, 5, 8000, DateTime.Now.AddDays(-4)),
            new Order(16, 1, 1500, DateTime.Now.AddDays(-14))
        };

        //Топ 3 клиента за последние 30 дней по суме заказов

        DateTime boundaryDate = DateTime.Now.AddDays(-30);

        var top3 = customers
            .Select(c => new
            {
                Customer = c,
                TotalAmount = orders.Where(o => o.CustomerId == c.Id && o.CreatedAt >= boundaryDate).Sum(o => o.Amount),
                OrdersCount = orders.Where(o => o.CustomerId == c.Id && o.CreatedAt >= boundaryDate)

            })
            .OrderByDescending(x => x.TotalAmount)
            .Take(3)
            .ToList();

        Console.WriteLine("Топ 3 клиента за последние 30 дней по сумме заказов:\n");
        foreach(var item in top3)
        {
            Console.WriteLine($"Имя - {item.Customer.Name}. Сумма - {item.TotalAmount}. Количество заказов - {item.OrdersCount.Count()}");
        }

        //Средний чек по каждому городу

        var averageCheckByCity = customers
            .Join(orders,
                  c => c.Id,
                  o => o.CustomerId,
                  (c, o) => new { c.City, o.Amount })
            .GroupBy(x => x.City)
            .Select(g => new
            {
                City = g.Key,
                AverageCheck = g.Average(x => x.Amount)
            })
            .OrderByDescending(x => x.AverageCheck) 
            .ToList();

        Console.WriteLine("\nСредний чек по городам\n");
        foreach (var item in averageCheckByCity)
        {
            Console.WriteLine($"- {item.City}: {item.AverageCheck:F2}");
        }

        //Клиенты которые ничего не заказали

        var orderedCustomersId = orders.Select(o => o.CustomerId).ToHashSet();

        var lazyCustomers = customers.Where(c => !orderedCustomersId.Contains(c.Id)).ToList();

        Console.WriteLine("\nКлиенты, которые ничего не заказали\n");
        foreach (var customer in lazyCustomers)
        {
            Console.WriteLine($"ID покупателя - {customer.Id}. Имя покупателя - {customer.Name}");
        }

        //Чисто задачка от себя чтобы лучше понять Join
        //Город и количество сделанных в нем заказов
        var citiesOrders = customers.Join(orders, c => c.Id, o => o.CustomerId, (c, o) => new { c.City, o })
            .GroupBy(x => x.City)
            .Select(g => new
            {
                City = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Count)
            .ToList();

        Console.WriteLine("\nКоличество заказов в каждом городе:\n");
        foreach (var city in citiesOrders)
        {
            Console.WriteLine($"Город - {city.City}, количество заказов - {city.Count}");
        }

        //Заказы за последние 7 дней отсортированные по сумме по убыванию
        //По сумме как я тут понял вы имели ввиду по цене

        DateTime sevenDaysAgo = DateTime.Now.AddDays(-7);

        var ordersByDescendingSevenDays = orders
            .Where(o => o.CreatedAt >= sevenDaysAgo)
            .OrderByDescending(o => o.Amount);

        Console.WriteLine("\nЗаказы за последние 7 дней отсортированные по цене по убыванию:\n");
        foreach(var order in ordersByDescendingSevenDays)
        {
            Console.WriteLine($"Id - {order.Id}, Amount - {order.Amount}, CreatedAt - {order.CreatedAt}");
        }

        //Сгруппируй заказы по клиенту. Для каждого клиента — имя + список его заказов.

        var ordersByClient = customers.Join(orders, c => c.Id, o => o.CustomerId, (c, o) => new { c.Name, o})
            .GroupBy(x => x.Name)
            .Select(g => new
            {
                Name = g.Key,
                Orders = g.Select(x => x.o)
            });

        Console.WriteLine("\nЗаказы по клиенту\n");
        foreach (var clientOrders in ordersByClient) 
        {
            Console.WriteLine($"Имя клиента - {clientOrders.Name}. Id заказов клиента - {string.Join(", ", clientOrders.Orders.Select(x => x.Id))}");
        }

        //Общая сумма всех заказов

        var ordersSum = orders.Sum(x => x.Amount);
        Console.WriteLine($"\nОбщая сумма вообще всех заказов - {ordersSum}");

        //Есть ли заказ больше 10000?

        string yesOrNo = orders.Any(x => x.Amount > 10000) ? "yes" : "no";
        Console.WriteLine($"\nЕсть ли заказ с ценой больше 10000 - {yesOrNo}");

        //Условие - один запрос написать двумя способами 
        var expensiveOrdersQuery = from o in orders where o.Amount > 10000 select o;

        yesOrNo = expensiveOrdersQuery.Any() ? "yes" : "no"; //в query syntax нет Any()
        Console.WriteLine("\nТоже самая задача, но с query syntax");
        Console.WriteLine($"\nЕсть ли заказ с ценой больше 10000 - {yesOrNo}");

        //Все ли заказы больше 100?
        yesOrNo = orders.All(x => x.Amount > 100) ? "yes" : "no";
        Console.WriteLine($"\nВсе ли заказы стоят больше 100 - {yesOrNo}");

    }
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string City { get; set; }

    public Customer(int id, string name, string city)
    {
        Id = id; Name = name; City = city;
    }
}

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt;

    public Order(int id, int customerId, decimal amount, DateTime createdAt)
    {
        Id = id; CustomerId = customerId; Amount = amount;
        CreatedAt = createdAt;
    }
}

