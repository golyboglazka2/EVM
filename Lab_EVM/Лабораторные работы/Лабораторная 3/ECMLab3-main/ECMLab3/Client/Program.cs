using System.IO.Pipes;
using System.Runtime.CompilerServices;

public struct Structure
{
    public double a;
    public double b;
    public double result;
}

class Client
{
    static void Main(string[] args)
    {
        Console.Title = "Clent";

        // Проверка наличия аргументов командной строки
        if (args.Length > 0) 
        {
            using (NamedPipeClientStream Client = new(".", args[0], PipeDirection.InOut))
            {
                Client.Connect();
                try
                {
                    // Бесконечный цикл для чтения и обработки данных
                    while (true)
                    {
                        // Чтение байтов из потока
                        byte[] bytes = new byte[Unsafe.SizeOf<Structure>()];
                        Client.Read(bytes, 0, bytes.Length);

                        // Преобразование байтов в структуру
                        Structure receivedData = Unsafe.As<byte, Structure>(ref bytes[0]);
                        Console.WriteLine($"Received data: a = {receivedData.a}, b = {receivedData.b}");

                        // Извлечение данных из структуры
                        double a = receivedData.a;
                        double b = receivedData.b;
                        int n = 100000;

                        // Вычисление результата с использованием метода трапеции
                        receivedData.result = TrapezoidalRule(a, b, n);
                        Console.WriteLine(receivedData.result);

                        // Преобразование структуры обратно в байты
                        byte[] modified_bytes = new byte[Unsafe.SizeOf<Structure>()];
                        Unsafe.As<byte, Structure>(ref modified_bytes[0]) = receivedData;

                        // Запись измененных байтов в поток
                        Client.Write(modified_bytes, 0, modified_bytes.Length); 
                    }
                }
                catch (Exception) { }
            }
        }
    }

    // Определение функции
    static double Function(double x)
    {
        return 2 * Math.Sin(x);
    }

    // Реализация метода трапеций для численного интегрирования
    static double TrapezoidalRule(double a, double b, int n)
    {
        
        double h = (b - a) / Convert.ToDouble(n); // Вычисление шага
        double result = 0.5 * (Function(a) + Function(b));  // Вычисление половины суммы значений функции на концах интервала

        // Суммирование значений функции на внутренних точках с учетом шага
        for (int i = 1; i < n; i++)
        {

            double x = a + i * h;
            result += Function(x); 
        }

        result *= h;
        Console.WriteLine(result); // Вывод результата на консоль (это можно удалить, если не нужен вывод)
        return result;
    }
}

