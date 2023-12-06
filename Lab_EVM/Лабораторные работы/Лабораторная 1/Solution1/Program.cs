using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.CompilerServices;

class PipeServer
{
    struct Numbers
    {
        public int number1;
        public int number2;
    }

    static void Main()
    {
        // Создаем объект NamedPipeServerStream с именем "Canal"
        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("Canal", PipeDirection.InOut))
        {
            //Console.WriteLine("NamedPipeServerStream object created.");

            // Ждем подключения клиента
            Console.Write("Ожидание подключения клиента...\n");
            pipeServer.WaitForConnection();

            Console.WriteLine("Клиент подключен.\n");

            Numbers cs = new(); // Создаем объект структуры

            Console.Write("Введите число 1: ");// Вводим 1 число и проверяем, чтобы это был int
            cs.number1 = int.TryParse(Console.ReadLine(), out int num1) ? num1 : 0;

            Console.Write("Введите число 2: ");// Вводим 2 число и проверяем, чтобы это был int
            cs.number2 = int.TryParse(Console.ReadLine(), out int num2) ? num2 : 0;

            byte[] bytes = new byte[Unsafe.SizeOf<Numbers>()]; // Переводим структуру в байты
            Unsafe.As<byte, Numbers>(ref bytes[0]) = cs;
            pipeServer.Write(bytes);

            byte[] receivedBytes = new byte[Unsafe.SizeOf<Numbers>()];
            pipeServer.Read(receivedBytes);
            Numbers receivedData = Unsafe.As<byte, Numbers>(ref receivedBytes[0]);
            Console.WriteLine($"\nПолученные данные: \n   Число 1 = {receivedData.number1}\n   Число 2 = {receivedData.number2}");
            Console.ReadKey();
        }
    }
}

