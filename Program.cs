using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.DeviceCommands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace ADBTEST
{
    class NoxThread
    {
        // Номер потока
        int numThread;
        AdvancedAdbClient client;
        DeviceData device;
        Random rnd = new();
        // Котегории товаров
        string[] categories =
        {
            "1Arts & Crafts",
            "1Automotive",
            "1Baby",
            "1Beauty & Personal Care",
            "1Computers",
            "1Books",
            "1Digital Music",
            "1Electronics",
            "1Women's Fashion",
            "1Men's Fashion",
            "1Girls' Fashion",
            "1Boys' Fashion",
            "1Health & Household",
            "2Home & Kitchen",
            "2Industrial & Scientific",
            "2Kindle Store",
            "2Luggage",
            "2Movies & Television",
            "2Music, CDs & Vinyl",
            "2Pet Supplies",
            "2Software",
            "2Sports & Outdoors",
            "2Tools & Home Improvement",
            "2Toys & Games",
            "2Video Games",
        };
        // Конструктор класса
        public NoxThread(int numThread, AdvancedAdbClient client, DeviceData device)
        {
            this.numThread = numThread;
            this.client = client;
            this.device = device;

        }
        // Запуск windscribe
        public void WindscribeStart()
        {
            //--------------------------------------------------------------
            PackageManager manager = new PackageManager(client, device);
            Console.WriteLine(manager.Packages.Keys);
            foreach(var i in manager.Packages.Keys)
            {
                Console.WriteLine(i);
            }
            foreach (var i in manager.Packages.Values)
            {
                Console.WriteLine(i);
            }

            Console.WriteLine(device.Features);
            Console.WriteLine(device.Model);
            Console.WriteLine(device.State);
            Console.WriteLine(device.Product);
            Console.WriteLine(client.GetFeatureSet(device).Count);
            //--------------------------------------------------------------
            // Запускаем windscribe
            client.StartApp(device, "com.windscribe.vpn");
            // Включаем
            client.FindElement(device, @"//*[@resource-id='com.windscribe.vpn:id/on_off_button']", TimeSpan.FromSeconds(30)).Click();
            // Ждем когда включится
            while (true)
            { // com.windscribe.vpn:id/tv_connection_state
                Thread.Sleep(2000);
                var connectionState = client.FindElement(device, @"//*[@resource-id='com.windscribe.vpn:id/tv_connection_state']", TimeSpan.FromSeconds(30));
                if(connectionState.attributes["text"] == "ON")
                {
                    client.HomeBtn(device);
                    Thread.Sleep(1200);
                    return;
                }
            }

        }
        // Рандомный элемент из массива
        public string RndStr(string[] arr) { return arr[rnd.Next(0, arr.Length)]; }
        // Запуск амазона
        public void Amazon()
        {
            Console.WriteLine($"Поток {numThread} стартанул");
            // Врубаем windscribe
            WindscribeStart();
            // Врубаем amazon
            client.StartApp(device, "com.amazon.mShop.android.shopping");
            // Заходим в меню
            client.FindElement(device, @"//*[@content-desc='Menu. Contains your orders, your account, shop by department, programs and features, settings, and customer service Tab 4 of 4']", TimeSpan.FromSeconds(30)).Click();
            // Открываем котегории
            client.FindElement(device, @"//*[@text='Shop by Department']", TimeSpan.FromSeconds(30)).Click();
            // Выбираем категорию
            var category = RndStr(categories);
            // Свайп если 1
            if (category.StartsWith("1"))
            {
                // Свайп
                client.Swipe
                    (
                    device,
                    client.FindElement(device, "//*[@text=\"Arts & Crafts\"]", TimeSpan.FromSeconds(10)),
                    client.FindElement(device, @"//*[@text='Shop by Department']", TimeSpan.FromSeconds(10)),
                    1000
                    );
            }
            // Свайп если 2
            else
            {
                // Свайп
                client.Swipe
                    (
                    device,
                    client.FindElement(device, "//*[@text=\"Boys' Fashion\"]", TimeSpan.FromSeconds(10)),
                    client.FindElement(device, "//*[@text=\"Arts & Crafts\"]", TimeSpan.FromSeconds(10)),
                    1000
                    );
                // Свайп
                client.Swipe
                    (
                    device,
                    client.FindElement(device, "//*[@text=\"Industrial & Scientific\"]", TimeSpan.FromSeconds(10)),
                    client.FindElement(device, "//*[@text=\"Health & Household\"]", TimeSpan.FromSeconds(10)),
                    1000
                    );
            }
            // Клик на категорию
            //client.FindElement(device, $"//*[@text=\"{category[1..]}\"]", TimeSpan.FromSeconds(30)).Click();
            client.FindElement(device, $"//*[@text=\"Arts & Crafts\"]", TimeSpan.FromSeconds(30)).Click();
            Element first = client.FindElement(device, @"//*[@class='android.webkit.WebView']/*[@class='android.webkit.WebView']/*[@class='android.view.View']/*[@class='android.view.View'][7]/*[@class='android.view.View'][4]/*[@class='android.view.View'][2]", TimeSpan.FromSeconds(30));
            Element second = client.FindElement(device, @"//*[@class='android.webkit.WebView']/*[@class='android.webkit.WebView']/*[@class='android.view.View']/*[@class='android.view.View'][7]/*[@class='android.view.View'][2]/*[@class='android.view.View'][2]", TimeSpan.FromSeconds(30));
            client.Swipe(device, first, second, 100);
            client.FindElement(device, @"//*[@content-desc='See all results']", TimeSpan.FromSeconds(30)).Click();
        }
    }
    class Program
    {
        static AdvancedAdbClient client;
        static NoxManager nox;
        static void Main()
        {
            // Экземпляр класса 
            Console.WriteLine("Запуск ADB");
            nox = new NoxManager
                (
                    noxPath: @"E:\Nox\bin",
                    instancesIndex: new int[] { 0 }
                );
            // Запускаем adb
            nox.StartAdb();
            Console.WriteLine("ADB включен");
            // Запускаем эмуляторы
            nox.ConnectAll();
            Console.WriteLine("Эмуляторы включены");
            // Подключение к adb
            client = new AdvancedAdbClient();
            client.Connect("127.0.0.1:5037");
            Console.WriteLine("ADB подключен");
            // Создаем список запущенных потоков
            var instancesNox = new List<NoxThread> { };
            for (var num = 0; num < nox.devices.Count; num++)
            {
                instancesNox.Add(new NoxThread(numThread: num, client: client, device: nox.devices[num]));
                new Thread(instancesNox[num].Amazon).Start();
            }
            Console.ReadKey();
            nox.DisconnectAll();
        }
    }
}
