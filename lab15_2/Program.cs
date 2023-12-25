using System;
using System.IO;
using Newtonsoft.Json;

// Интерфейс репозитория, определяющий методы для работы с данными (добавление, удаление, изменение, получение)
// Класс репозитория, реализующий интерфейс репозитория и хранящий данные в памяти или в базе данных
// В данном случае их 2
public interface IRepository
{
    void Save(string data); // сохранение
}

public class FileRepository : IRepository
{
    private readonly string filename;

    public FileRepository(string filename)
    {
        this.filename = filename;
    }

    public void Save(string data)
    {
        File.AppendAllText(filename, data + Environment.NewLine);
    }
}

public class JsonRepository : IRepository
{
    private readonly string filename;

    public JsonRepository(string filename)
    {
        this.filename = filename;
    }

    public void Save(string data)
    {
        File.AppendAllText(filename, JsonConvert.SerializeObject(data) + Environment.NewLine);
    }
}

public class MyLogger
{
    private readonly IRepository[] reps; // присвоение значения полю может происходить только при объявлении или в конструкторе этого класса

    public MyLogger(params IRepository[] reps)
    {
        this.reps = reps;
    }

    public void Log(string message)
    {
        foreach (var repository in reps)
        {
            repository.Save(message);
        }
    }
}

class lab15_2
{
    static void Main()
    {
        // в lab15_2
        var fileRepo = new FileRepository("..\\..\\..\\" + "text.txt");
        var jsonRepo = new JsonRepository("..\\..\\..\\" + "text.json");

        var logger = new MyLogger(fileRepo, jsonRepo);

        Console.Write("Введите текст для text.txt:");
        string logMessage = Console.ReadLine();

        logger.Log(logMessage);

        Console.WriteLine("The log is written to files");
    }
}