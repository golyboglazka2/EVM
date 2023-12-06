using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.CompilerServices;

public struct Structure
{
    public double a;
    public double b;
    public double result;
}

class PipeServer
{
    // Очередь данных с приоритетом, где ключ - целое число
    private static PriorityQueue<Structure, int> dataQueue = new PriorityQueue<Structure, int>();

    
    private static Mutex mutex = new Mutex();   // Мьютекс для синхронизации доступа к общим данным
    private static Mutex mutFile = new Mutex(); // Дополнительный мьютекс для синхронизации записи в файл
    private static int count = 0; // Счетчик
    private static string path = "C:\\Users\\Татьяна\\Desktop\\Учеба\\ЭВМ\\Лабораторные работы\\Лабораторная 3\\ECMLab3-main\\ECMLab3\\Client\\bin\\Debug\\net7.0\\Client.exe";
    // Поток для записи в файл
    private static StreamWriter file = new StreamWriter($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\output.txt", true);
    // Источник отмены для задач
    private static CancellationTokenSource source = new CancellationTokenSource();
    private static CancellationToken token = source.Token; // Токен отмены
    private static async Task Main()
    {
       
        // Обработчик события прерывания консоли
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true; // Отмена стандартного поведения
            source.Cancel();         // Запрос отмены задач
        };

        try
        {
            await Task.WhenAll(SenderTask(token), ReceiverTask(token)); ; // Ожидание завершения выполнения двух асинхронных задач
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
        }
        finally
        {
            file.Close();
        }
    }

    // Определение асинхронного метода SenderTask с токеном отмены
    static Task SenderTask(CancellationToken token)
    {
        return Task.Run(async () =>
        {
            // Цикл продолжается, пока не запросится отмена токена
            while (!token.IsCancellationRequested)
            {
                double _n = 0, _m = 0;
                int _priority = 0;

                // Циклы для ввода значений А В и Приоритета
                while (!token.IsCancellationRequested)
                {
                    Console.WriteLine("Enter A: ");
                    try
                    {
                        _n = double.Parse(Console.ReadLine());
                        break;
                    }
                    catch 
                    {
                        ColoredMsg("A must be double!", 1);
                    }
                }
                while (!token.IsCancellationRequested)
                {
                    Console.WriteLine("Enter B: ");
                    try
                    {
                        _m = double.Parse(Console.ReadLine());
                        break;
                    }
                    catch
                    {
                        ColoredMsg("B must be double!", 1);
                    }
                }
                while (!token.IsCancellationRequested)
                {
                    Console.WriteLine("Enter Priority: ");
                    try
                    {
                        _priority = int.Parse(Console.ReadLine());
                        break;
                    }
                    catch
                    {
                        ColoredMsg("Priority must be double!", 1);
                    }
                }


                // Создание экземпляра структуры с введенными значениями A и B
                Structure data = new Structure
                {
                    a = _n,
                    b = _m,
                };

                // Захват мьютекса для безопасного доступа к общему ресурсу (очереди данных)
                mutex.WaitOne();
                dataQueue.Enqueue(data, _priority);
                mutex.ReleaseMutex();
                await Task.Delay(1000);
            }
        });
    }

    // Определение асинхронной задачи для приема данных
    static Task ReceiverTask(CancellationToken token)
    {
        return Task.Run(() =>
        {
            // Пока не запрошена отмена задачи
            while (!token.IsCancellationRequested)
            {
                // Создание переменных для хранения данных и приоритета
                Structure st;
                int pr;

                // Ожидание доступа к общим данным с использованием мьютекса
                mutex.WaitOne();
                bool flag = dataQueue.TryDequeue(out st, out pr); // Попытка извлечения данных из очереди
                mutex.ReleaseMutex(); // Освобождение мьютекса после доступа к общим данным

                // Если данные успешно извлечены из очереди
                if (flag)
                {
                    ClientConnect(st, token);
                }
            }
        });
    }

    // Асинхронный метод для обработки подключения клиента
    static async void ClientConnect(Structure st, CancellationToken token)
    {
        try
        {
            // Преобразование структуры в байты
            byte[] dataBytes = new byte[Unsafe.SizeOf<Structure>()];
            Unsafe.As<byte, Structure>(ref dataBytes[0]) = st;

            // Создание именованного канала для сервера
            NamedPipeServerStream pipeServer = new($"channel{count}", PipeDirection.InOut);
            Console.WriteLine("Waiting for client connection...");

            // Создание процесса для запуска клиентского приложения
            Process myProcess = new Process();
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = path;
            myProcess.StartInfo.Arguments = $"channel{count}";
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.Start();

            // Ожидание подключения клиента к каналу
            await pipeServer.WaitForConnectionAsync();
            Console.WriteLine("Client connected");

            // Отправка данных клиенту
            await pipeServer.WriteAsync(dataBytes, 0, dataBytes.Length);

            // Чтение данных от клиента
            byte[] receivedBytes = new byte[Unsafe.SizeOf<Structure>()];
            if (await pipeServer.ReadAsync(receivedBytes, 0, receivedBytes.Length) == receivedBytes.Length)
            {
                // Преобразование байтов обратно в структуру
                st = Unsafe.As<byte, Structure>(ref receivedBytes[0]);
            }

            // Ожидание доступа к файлу
            mutFile.WaitOne();

            // Формирование строки с результатами и их запись в файл и вывод в консоль
            string res = $"a = {st.a}; b = {st.b}; priority = {0}; result = {st.result}";
            file.WriteLine(res);
            Console.WriteLine(res);
            Console.Beep();         // Издание звукового сигнала
            mutFile.ReleaseMutex(); // Освобождение мьютекса для доступа к файлу
            pipeServer.Close();     // Закрытие канала
            count++;
            await myProcess.WaitForExitAsync(token); // Ожидание завершения работы клиентского процесса
        }
        catch (Exception) { }
    }

    // Использование оператора switch для выбора цвета в зависимости от уровня
    static void ColoredMsg(string msg, uint level) 
    {
        switch (level)
        {
            case 0:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(msg);
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case 1:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(msg);
                Console.ForegroundColor = ConsoleColor.White; 
                break;
            case 2:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg);
                Console.ForegroundColor= ConsoleColor.White;
                break;
        }
    }

}


