namespace _02_interfaces;

internal class Program 
{
    static void Main(string[] args)
    {
        var processors = new List<IPaymentProcessor>
    {
        new CardPaymentProcessor(),
        new PayPalPaymentProcessor()
    };

        foreach (var p in processors)
        {
            p.Process(100m);
            p.Refund("tx-123");
            Console.WriteLine(new string('-', 40));
        }

        // Проверка IReportable через отдельную переменную
        var card = new CardPaymentProcessor();
        card.GenerateReport();

        // Проверка валидации
        try
        {
            new CardPaymentProcessor().Process(-5m);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Ошибка валидации: {ex.Message}");
        }
    }
}

public interface IPaymentProcessor
{
    void Process(decimal amount);
    void Refund(string txId);
    //контракт на реализацию двух методов
}

public interface IReportable
{
    void GenerateReport();
}

public abstract class PaymentProcessorBase : IPaymentProcessor
{
    public void Process(decimal amount)
    {
        ValidateAmount(amount);
        Log($"Начинаю Process на {amount}");
        ProcessCore(amount);
        Log("Process завершен");
    }

    public void Refund(string txId)
    {
        Log($"Начинаю Refund {txId}");
        RefundCore(txId);
        Log("Refund завершён");
    }

    protected abstract void ProcessCore(decimal amount);
    protected abstract void RefundCore(string txId);

    private void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Сумма должна быть больше 0.");
        }
    }

    private void Log(string message) => Console.WriteLine($"[LOG] {message}");
    //Валидация, логирование, порядок выполнения метода Process и Refund, все здесь
}

public class CardPaymentProcessor : PaymentProcessorBase, IReportable // наследует 1 класс, но сколько угодно интерфейсов
{
    protected override void ProcessCore(decimal amount)
    {
        Console.WriteLine($"Списание с карты {amount}");
    }

    protected override void RefundCore(string txId)
    {
        Console.WriteLine($"Возврат по карте {txId}");
    }

    public void GenerateReport()
    {
        Console.WriteLine("Отчёт по карточным операциям оформлен");
    }
    //Общая логика вынесена в абстрактный класс, а детали в наследниках.
}

public class PayPalPaymentProcessor : PaymentProcessorBase
{
    protected override void ProcessCore(decimal amount)
        => Console.WriteLine($"Оплата через PayPal: {amount}");

    protected override void RefundCore(string txId)
        => Console.WriteLine($"Возврат PayPal: {txId}");

    //то же самое 
}

//Общая логика в abstract, а контракт в interface, потому что контракт это как раз таки и есть предназначение интерфейсов (can do), а в абстрактном классе (is a) свою логику как раз таки можно спокойно писать 
//C# не даёт множественное наследование классов, но даёт множественное наследование интерфейсов. Я бы обьяснил это следующим образом. Как я уже написал, интерфейс - can do, абстрактный класс - is a. 
//Приведу пример из реальной жизни: Я, допустим, умею программировать и играть в комп там незнаю или гулять, и все эти навыки (контракты) из разных интерфейсов (IPlayable, IProgramming и т.д.)
//Но при этом я являюсь только человеком. То есть я не могу быть человеком и иденцифицировать себя как дерево. Именно поэтому наследоваться от класса можно только от одного, а интерфейсов скок угодно можно реализовать
//Интерфейс надо выбирать, когда ты точно не знаешь что прилетит, но знаешь, что вот это что прилетит ваще умеет (IEnumerable). Также когда работаешь с ленивым выполнением (yield return) тоже понятное дело что нужно использовать интерфейс
//Абстрактный класс надо выбирать если тебе надо конкретно логику для дочерних обьектов написать чтобы не дублировать код (DRY).
//Здесь есть некие неточности, поэтому если надо будет то я на собесе нормально скажу