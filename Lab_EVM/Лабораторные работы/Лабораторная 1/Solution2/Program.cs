using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.CompilerServices;

class PipeClient
{
    // Структура для хранения двух чисел
    struct Numbers
    {
        public int number1;
        public int number2;
    }

    static void Main(string[] args)
    {
        // Создаем новый клиент для именованного канала
        using (NamedPipeClientStream pipeClient =
            new NamedPipeClientStream(".", "Canal", PipeDirection.InOut))
        {
            // Попытка подключения к каналу
            Console.Write("Попытка подсоединения...\n");
            pipeClient.Connect();

            Console.WriteLine("Подключено");

            // Чтение данных из канала
            byte[] bytes = new byte[Unsafe.SizeOf<Numbers>()];
            pipeClient.Read(bytes);
            Numbers receivedData = Unsafe.As<byte, Numbers>(ref bytes[0]);
            Console.WriteLine("Число 1: " + receivedData.number1);
            Console.WriteLine("Число 2: " + receivedData.number2);

            // Обработка данных
            receivedData.number1 = receivedData.number1 + receivedData.number2;
            Console.WriteLine("Новое ЧИСЛО 1 = Число 1 + Число 2 = " + receivedData.number1);

            // Подготовка измененных данных для отправки обратно
            byte[] modifiedBytes = new byte[Unsafe.SizeOf<Numbers>()];
            Unsafe.As<byte, Numbers>(ref modifiedBytes[0]) = receivedData;

            // Отправка данных обратно через канал
            pipeClient.Write(modifiedBytes);
        }

        // Ожидание нажатия Enter перед завершением программы
        Console.Write("Нажмите Enter чтобы продолжить...");
        Console.ReadLine();
    }
}


