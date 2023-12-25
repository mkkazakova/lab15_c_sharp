
// lab05 07
/*
public interface IObserver
{
    void Update(string dir, List<string> newFiles, List<string> delFiles);
}
public class DirectoryObserver : IObserver
{
    public void Update(string dir, List<string> newFiles, List<string> delFiles)
    {
        Console.WriteLine($"\nИзменения в папке " + dir + ":");
        if (newFiles.Any())
        {
            Console.WriteLine($"    Появились файлы: {string.Join(", ", newFiles)}");
        }
        if (delFiles.Any())
        {
            Console.WriteLine($"    Пропали файлы:   {string.Join(", ", delFiles)}");
        }
    }
}

public class FileSystemWatcher
{
    private string dir;
    private HashSet<string> files;
    private List<IObserver> observers;
    private int timerInterval = 1000; // миллисекунды
    private Timer timer;

    public FileSystemWatcher(string directory)
    {
        this.dir = directory;
        this.files = GetFilesInDirectory();
        this.observers = new List<IObserver>();
        this.timer = new Timer(CheckDirectory, null, 0, timerInterval);
    }

    private HashSet<string> GetFilesInDirectory()
    {
        return new HashSet<string>(Directory.GetFiles(dir).Select(Path.GetFileName));
    }

    private void CheckDirectory(object state)
    {
        var curFiles = GetFilesInDirectory();
        var newFiles = curFiles.Except(files).ToList();
        var delFiles = files.Except(curFiles).ToList();

        if (newFiles.Any() || delFiles.Any())
        {
            NotifyObservers(newFiles, delFiles);
        }

        files = curFiles;
    }

    private void NotifyObservers(List<string> newFiles, List<string> delFiles)
    {
        foreach (var observer in observers)
        {
            observer.Update(dir, newFiles, delFiles);
        }
    }

    public void AddObserver(IObserver observer)
    {
        observers.Add(observer);
    }
}





class lab15_1

{
    static void Main(string[] args)
    {
        string myDir = "c:/1";

        var watcher = new FileSystemWatcher(myDir);

        var observer = new DirectoryObserver();
        watcher.AddObserver(observer);

        Console.WriteLine($"Watching directory: {myDir}");
        Console.ReadLine();
    }
}
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace DirectorySystemWatcher
{
    // класс для хранения информации о пользовательском событии
    public class DirectoryChangedEventArgs : EventArgs
    {
        public string FileName { get; set; }
        public string ChangeType { get; set; }
    }

    // делегат будет использоваться для создания события FileChanged
    public delegate void DirectoryChangedEventHandler(object sender, DirectoryChangedEventArgs args);

    public class DirectorySystemWatcher
    {
        private string dir;
        public Timer timer;
        private List<string> curFiles;

        public event DirectoryChangedEventHandler FileChanged;

        public DirectorySystemWatcher(string dir)
        {
            this.dir = dir;
            curFiles = Directory.GetFiles(dir).ToList();

            timer = new Timer(CheckDirectory, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        // проверяет изменения в директории, сравнивая текущий список файлов с новым
        public void CheckDirectory(object state)
        {
            var files = Directory.GetFiles(dir);

            foreach (var file in files)
            {
                if (curFiles.Contains(file))
                {
                    continue;
                }

                curFiles.Add(file);

                OnFileChanged(file, "added");
            }

            for (int i = curFiles.Count - 1; i >= 0; i--)
            {
                var file = curFiles[i];
                if (!File.Exists(file))
                {
                    curFiles.RemoveAt(i);
                    OnFileChanged(file, "deleted");
                }
            }
        }

        // вызывает событие `FileChanged`, передавая информацию о файле и типе изменения
        protected virtual void OnFileChanged(string fileName, string changeType)
        {
            FileChanged?.Invoke(this, new DirectoryChangedEventArgs { FileName = fileName, ChangeType = changeType });
        }
    }

    // для обработки событий изменения директории (выводит информацию о файле и типе изменения в консоль)
    public class DirectoryChangeHandler
    {
        public void OnFileChanged(object sender, DirectoryChangedEventArgs args)
        {
            Console.WriteLine($"File {args.FileName} has been {args.ChangeType}");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var directorySystemWatcher = new DirectorySystemWatcher("../../../../lab15_1");

            var directoryChangeHandler = new DirectoryChangeHandler();

            directorySystemWatcher.FileChanged += directoryChangeHandler.OnFileChanged;

            while (true)
            {
                directorySystemWatcher.CheckDirectory(directorySystemWatcher.timer);
            }
        }
    }
}
