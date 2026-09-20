using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace _01_types
{
    public class Person
    {
        public string Name { get; set; } = default!;
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== 1. Value type (int) ===");
            int x = 10;
            Mutate(x);
            Console.WriteLine($"x после вызова Mutate: {x}"); // 10, т.к. int - значимый тип
            //мой комментарий: если бы мы воспользовались ref то после mutate было бы 100, а так 10 потому что та x = 100 из метода уже сдохла давно

            Console.WriteLine("\n=== 2. Reference type (int[]) ===");
            int[] arr = { 1, 2, 3 };
            Mutate(arr);
            Console.WriteLine($"arr[0] после вызова: {arr[0]}"); // 99, т.к. ссылка на массив копируется, но данные - общие
            //да, всё верно, так как массив int[] это ссылочный тип, поменяв его элемент в методе, он поменяется везде.

            Console.WriteLine("\n=== 3. String (иммутабельный reference type) ===");
            string s = "hello";
            Mutate(s);
            Console.WriteLine($"s после вызова: {s}"); // "hello", т.к. строки неизменяемы
            //всё верно, но если так подумать то можно было бы использовать StringBuilder(он reference type) и тогда бы значение поменялось но конкретно string неизменяемый, то есть изменяя параметр в методе компилятор просто сделал строку в куче и переприсвоил локальную копию ссылки на неё.

            Console.WriteLine("\n=== 4. Class (Person) ===");
            Person p = new Person { Name = "Yaroslav" };
            Mutate(p);
            Console.WriteLine($"p.Name после вызова: {p.Name}"); // "changed", т.к. меняем свойство по ссылке
            //так как свойство публичное с публичным сеттером, то конечно же свойство изменится, но ваще, существует такая вещь как инкапсуляция, и так никто не делает как тут я думаю, но это учебная темка на понимание так что ладно

            Console.WriteLine("\n=== 5. Переприсвоение ссылки ===");
            Person p2 = new Person { Name = "Original" };
            Reassign(p2);
            //p2 = new Person { Name = "changed" };
            Console.WriteLine($"p2.Name после Reassign: {p2.Name}"); // "Original", т.к. ссылка p2 не изменилась
            //Тут я чёт задумался но по факту получается создается копия ссылки которая в итоге указывает на обьект где Name = "New", но сама ссылка p2 не изменилась

            Console.WriteLine("\n=== 6. Boxing ===");
            // Плохой вариант (Boxing):
            ArrayList list = new ArrayList();
            for (int i = 0; i < 5; i++)
            {
                list.Add(i); // упаковка int в object (boxing)
            }
            Console.WriteLine($"ArrayList.Count: {list.Count}");
            //Плохой вариант потому что ArrayList это не дженерик (то есть можно сказать это List<object>, и из за этого приходится постоянно упаковывать и распаковывать)

            // Хороший вариант (без Boxing):
            List<int> genericList = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                genericList.Add(i); // без боксинга, т.к. List<int> хранит int напрямую
            }
            Console.WriteLine($"List<int>.Count: {genericList.Count}");
            //Хороший вариант так как конкретно generics и лист знает что он обязан содержать инт, соответственно хранить он будет инт а не обджект, и упаковка не потребуется
        }
        
        static void Mutate(int x) { x = 100; }

        static void Mutate(int[] arr) { arr[0] = 99; }

        static void Mutate(string s) { s = "changed"; }

        static void Mutate(Person p) { p.Name = "changed"; }

        static void Reassign(Person p) { p = new Person { Name = "New" }; }
    }
}