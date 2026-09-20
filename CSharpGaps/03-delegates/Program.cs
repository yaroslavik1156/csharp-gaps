namespace _03_delegates;

public delegate void OrderCreatedEventHandler(object sender, Order order);
public delegate TResult Transformer<T, TResult>(T input);

internal class Program
{
    public static void Main(string[] args)
    {
        OrderService orderService = new OrderService();
        orderService.OrderCreated += (sender, order) => Console.WriteLine($"[ОБРАБОТЧИК ЧЕРЕЗ ЛЯМБДУ] {sender} отправил на выполнение заказ под номером{order.Id}");
        orderService.OrderCreated += NamedService;

        orderService.Create("Yaroslav", 1000);
        orderService.Create("Vanya", 2000);
        orderService.Create("Jeffry", 400);

        var filteredOrders = orderService.GetOrders(x => x.Amount > 500);

        Console.WriteLine("\nОтфильтрованные заказы\n");

        foreach (var filteredOrder in filteredOrders)
        {
            Console.WriteLine(filteredOrder.Amount);
        }

        Console.WriteLine("\nAction всех заказов, пример:\n");

        orderService.ProcessOrders((order) =>
        {
            Console.WriteLine(order.Amount);
        });

        //Отписка от именованого обработчика допустим, то есть мы не в Order Service можем только подписываться на событие и отписываться, не меняя само событие как бы, просто меняя реакцию на событие от подписчиков
        Console.WriteLine("\nОтписка от одного обработчика\n");
        orderService.OrderCreated -= NamedService;
        orderService.Create("John", 800);
        //в прошлом коментарии я имел в виду не меняя само событие в том смысле что мы само событие можем поменять онли в классе где это событие
        //event безопаснее делегата потому что делегат можно изменить где угодно, так как сама суть делегата (делегировать). На событие может повлиять только тот класс где оно создано, а остальные могут на него подписаться и когда оно вызывается просто как либо на него реагировать
        //Если не отписываться от события в долгоживущем обьекте то произойдёт утечка памяти (если Handler не static), так как ссылка на обработчик неявно включает в себя ссылку на экземпляр короткоживущего обьекта, и так как событие (долгоживущий обьект) ещё живо, короткоживущий не будет удалён без отписки, даже если он в коде не нужен. 
        Console.WriteLine("\nЗамыкание\n");
        


        int count = 0;
        Console.WriteLine(count);
        Action action = () =>
        {
            for (int i = 0; i < 10; i++)
            {
                count += i;
            }
        };

        //В моменте забыл про вызов делегата
        action?.Invoke(); //В данном случае Invoke не нужен, потому что там не может быть пустоты, но чисто для красоты добавлю

        Console.WriteLine(count);
        //Здесь мы захватываем в лямбду переменную из Main(). То есть пока лямбда жива, переменная тоже будет жить.
        //Интересный факт, если мы пишем лямбду внутри цикла for, то нам нужно будет написать int iCopy = i; для того чтобы все лямбды не захватили одно значение

        //забыл поиспользовать Transform

        Transform(x => $"Число {x}", 42, out string result);
        Console.WriteLine(result);
    }

    public static void Transform(Transformer<int, string> transformer, int num, out string strNum)
    {
        strNum = transformer(num);
    }

    public static void NamedService(object sender, Order order) //Именованый обработчик
    {
        if (sender is OrderService orServ)
        {
            Console.WriteLine($"[ОБРАБОТЧИК ЧЕРЕЗ ИМЕНОВАННЫЙ МЕТОД] Отправитель {orServ.GetType()}, помог покупателю по имени {order.CustomerName} оформить заказ");
        }
    }
}

public class Order
{
    public Guid Id { get; } = Guid.NewGuid();
    public string CustomerName { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}

public class OrderService
{
    public event OrderCreatedEventHandler? OrderCreated;
    private readonly List<Order> _orders = new();

    public void Create(string customerName, decimal amount)
    {
        Order createdOrder = new Order { CustomerName = customerName, Amount = amount };
        _orders.Add(createdOrder);
        OrderCreated?.Invoke(this, createdOrder);
    }

    public IEnumerable<Order> GetOrders(Func<Order, bool> filter)
    {
        return _orders.Where(filter);
    }

    public void ProcessOrders(Action<Order> action)
    {
        foreach (var order in _orders)
        {
            action(order);
        }
    }
}


