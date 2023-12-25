// Класс одиночки, имеющий приватный конструктор и статический метод для получения экземпляра класса
// Статическое поле для хранения единственного экземпляра класса
// Возможность доступа к экземпляру класса только через статический метод получения экземпляра

public class SingleRandomizer
{
    private static SingleRandomizer instance; // статическая переменная - ссылка на конкретный экземпляр данного объекта
    private static readonly object lockObject = new object(); // для потокобезопасной инициализации экземпляра
    private Random random;
    private string filePath;

    private SingleRandomizer()
    {
        random = new Random();
        filePath = "..\\..\\..\\" + "text.txt";
    }

    // статический метод для получения экземпляра класса
    public static SingleRandomizer Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new SingleRandomizer();
                    }
                }
            }
            return instance;
        }
    }

    public int GetNextRandomNumber()
    {
        lock (lockObject)
        {
            int randomNumber = random.Next();

            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine(randomNumber);
            }

            return randomNumber;
        }
    }
}

class lab15_3
{
    static void Main()
    {
        var randomizer = SingleRandomizer.Instance;

        // Parallel.For позволяет выполнять итерации цикла параллельно
        Parallel.For(0, 10, i =>
        {
            int randomNumber = randomizer.GetNextRandomNumber();
            Console.WriteLine($"Поток {Task.CurrentId}: {randomNumber}");
        });

        Console.WriteLine("Данные записаны в text.txt");
    }
}
