using AdvancedSharpAdbClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ADBTEST
{
    class NoxManager
    {
        // Путь до nox
        string noxPath;
        int[] instancesIndex;
        public List<DeviceData> devices = new() { };

        // Конструктор класса
        public NoxManager(string noxPath, int[] instancesIndex)
        {
            this.noxPath = noxPath;
            this.instancesIndex = instancesIndex;
        }
        // Включить adb
        public void StartAdb()
        {
            // Убиваем adb если он включен
            StopAdb();
            // Создаем процесс
            var process = new Process();
            // Создание процесса
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = @$"/C {noxPath}\adb.exe start-server",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            // Включаем adb
            process.Start();
            // Ждем завершения
            process.WaitForExit();
            // Ждем полного включения
            Thread.Sleep(4000);
        }
        // Выключить adb
        public void StopAdb()
        {
            var process = new Process();
            // Создание процесса
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = @$"/C {noxPath}\adb.exe kill-server",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            // 10 раз ввести команду
            for (var i = 0; i < 10; i++)
            {
                // Запуск процесса
                process.Start();
                // Ждем завершения
                process.WaitForExit();
            }
            Thread.Sleep(4000);

        }
        // Создать instance
        public void Create(int count)
        {

        }
        // Проверить жив ли instance
        public void IsLive(int index)
        {

        }
        // Удалить instance
        public void Remove(int index)
        {

        }
        // Удалить все instances
        public void RemoveAll()
        {

        }
        // Перезапустить instance
        public void Restart(int index)
        {

        }
        // Подключиться к instance
        public void ConnectAll()
        {
            // Для проверки запустился ли эмулятор
            var client = new AdvancedAdbClient();
            client.Connect("127.0.0.1:5037");
            var deviceCount = client.GetDevices().Count;
            // Поочередный запуск
            foreach (var index in instancesIndex)
            {
                // Обьявление процесса
                using var process = new Process();
                // Создание процесса
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = @$"/C {noxPath}\Nox.exe -clone:Nox_{index}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                // Включаем instance
                process.Start();
                // Проверка запустился ли эмулятор
                while (true)
                {
                    Thread.Sleep(2000);
                    // Если еще не запустился
                    if (deviceCount == client.GetDevices().Count) { continue; }
                    deviceCount = client.GetDevices().Count;
                    break;
                }
            }
            devices = client.GetDevices();
        }
        // Отлключить instance
        public void DisconnectAll()
        {
            // Поочередный запуск
            foreach (var index in instancesIndex)
            {
                // Обьявление процесса
                var process = new Process();
                // Создание процесса
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = @$"/C {noxPath}\Nox.exe -clone:Nox_{index} -quit",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                // Выключаем instance
                process.Start();
            }
        }
    }
}
