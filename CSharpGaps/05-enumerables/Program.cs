using System.Diagnostics;

namespace _05_enumerables;

internal class Program
{
    public static void Main()
    {
        PartOne();
        Console.WriteLine();
        PartTwo();
        Console.WriteLine();
        PartThree();
        Console.WriteLine();
        PartFour();
    }

    private static void PartOne()
    {
        Console.WriteLine("ЧАСТЬ 1\n");
        Console.WriteLine("Эксперимент 1\n");
        Console.WriteLine("---До вызова---");
        var nums = GetNumbers(); //Так как мы не начали материализацию (foreach, ToList() и тд), произойдёт можно сказать ничего, но не ничего. nums просто запомнит как отдавать i и отдаст их при необходимости.
        Console.WriteLine("---До вызова---");

        Console.WriteLine("Эксперимент 2\n");
        Console.WriteLine("---До вызова---");
        nums = GetNumbers(); //эта строчка не имеет смысла так как мы уже присвоили этот метод пременной nums
        foreach (var num in nums)
        {
            Console.WriteLine($"получаю - {num}"); //Здесь комментарии излишни, я уже всё расписал, могу ток сказать что сначало выполнится код до yield, и потом уже наш "получаю num"
        }
        Console.WriteLine("---После вызова---\n");

        Console.WriteLine("Эксперимент 3");
        Console.WriteLine("---До вызова---");
        var materializedNums = GetNumbers().ToList(); //Весь код нашего метода выполнится так как мы материализовали и заставили ленивого IEnumerable работать
        for (int i = 0; i < 2; i++)
        {
            foreach (var num in materializedNums)
            {
                Console.WriteLine($"получаю - {num}"); //Материализация заново не происходит, так как мы уже её сделали, написав ToList()
            }// А вот если бы мы не написали ToList() и два раза перебирали, то метод работал бы заново
        }
        Console.WriteLine("---После вызова---");
    }

    private static void PartTwo()
    {
        Console.WriteLine("ЧАСТЬ 2\n");

        List<long> numbers = new List<long>(1_000_000);
        for (int i = 1; i <= 1_000_000; i++)
        {
            numbers.Add(i); //Могли написать numbers = Enumerable.Range(1, 1_000_001).ToList(); но из за генерации state machine это медленее чем такой вариант, хотя я сам это узнал тока щас когда в нейронку полез, и мне нейронка сказала мол Jit-компиляция у цикла for максимально производительная и тд и тп
        }
        Stopwatch sw = new();

        sw.Start();
        var query = numbers.Where(x => x % 2 == 0);
        var count = query.Count();
        var sum = query.Sum();
        var first = query.First();
        sw.Stop();

        Console.WriteLine($"Время выполнения без ToList() - {sw.ElapsedMilliseconds}\n");

        sw.Restart();
        var materialized = numbers.Where(x => x % 2 == 0).ToList();
        var betterCount = materialized.Count;
        var betterSum = materialized.Sum();
        var betterFirst = materialized.First();
        sw.Stop();

        Console.WriteLine($"Время выполнения с ToList() - {sw.ElapsedMilliseconds}");
        //To List не всегда будет выгоднее из за аллокации и обхода, но для IQueryable, где каждый обход = запрос к базе данных - выгода огромна
    }

    private static void PartThree()
    {
        Console.WriteLine("ЧАСТЬ 3\n");
        List<int> listSource = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        int[] arraySource = listSource.ToArray();
        HashSet<int> setSource = arraySource.ToHashSet();

        _ = SumEnumerable(listSource);
        _ = SumEnumerable(arraySource);
        _ = SumEnumerable(setSource); //IEnumerable принимает все эти типы, так как все эти типы реализуют интерфейс

        _ = SumCollection(listSource);
        _ = SumCollection(arraySource);
        _ = SumCollection(setSource); //тоже все принимает

        _ = SumList(listSource);
        _ = SumList(arraySource);
        //_ = SumList(setSource); это не принимает, так как у HashSet нету индексов и обращение идёт по hash кодам.
        Console.WriteLine("ничего не выведет этот метод, ищи его в коде");
    }

    private static void PartFour()
    {
        Console.WriteLine("ЧАСТЬ 4\n");
        var evens = GetEvenNumbers(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        Console.WriteLine($"{evens.Count} - прочитано из типа {evens.GetType().Name}"); //На самом деле лист, просто стеклянная оболочка, которую можно взломать кастингом назад к List
        Console.WriteLine($"{evens[0]} - прочитано из типа {evens.GetType().Name}");

        //прочитать можно

        //evens.Add(8);
        //evens[0] = 100;

        //менять нельзя - ошибка компиляции.
        
    }

    private static IEnumerable<int> GetNumbers()
    {
        Console.WriteLine(">>>GetNumbers: старт"); //Выведется как только мы начнём итерировать (foreach и т.д.)
        for (int i = 1; i <= 5; i++)
        {
            Console.WriteLine($">>>GetNumbers: отдаю {i}");
            yield return i; //Итерация - запрос - выдача 1 кирпичика из 5 - сон. При следующей итерации так как компилятор сделает state machine, Метод выдаст второй кирпичик а не первый, и будет выдавать кирпичики пока MoveNext() не вернет false
        }
        Console.WriteLine(">>>GetNumbers: конец"); //MoveNext() вернул false, метод больше не отдаёт кирпичики так как кирпичики в мешке закончились, соответсвенно выводится сообщение и метод заканчиваеться
    }

    private static int SumEnumerable(IEnumerable<int> source)
    {
        int sum = 0;
        foreach (int i in source)
        {
            sum += i;
        }
        return sum;
    }

    private static int SumCollection(ICollection<int> source)
    {
        int sum = 0;
        foreach (int i in source)
        {
            sum += i;
        }
        return sum;
    }

    private static int SumList(IList<int> source)
    {
        int sum = 0;
        foreach (int i in source)
        {
            sum += i;
        }
        return sum;
    }

    private static IReadOnlyList<int> GetEvenNumbers(IEnumerable<int> source) //Лучше возвращать IReadOnlyList из публичного API чтобы полученные данные нельзя было изменить, но можно было прочитать, то есть это как лист, но вместо {get; set;} просто {get;}
    {
        return source.Where(i => i % 2 == 0).ToList(); //так как List реализует IReadOnlyList, можем материализовать в лист и получим только читаемый лист
    }
}