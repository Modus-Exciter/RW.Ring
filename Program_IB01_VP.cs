using System.Collections.Generic;
using APEX.Server.Properties;
using Notung.Logging;
using Notung.Loader;
using System.Net;
using Notung.Net;
using System;

namespace APEX.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, e) => LogManager.GetLogger(typeof(Program)).Fatal("Fatal error", e.ExceptionObject as Exception);

            var listener = new HttpListener(); 
            listener.Prefixes.Add("http://localhost:14488/ModelService/");

            Console.WriteLine("Используемый протокол: {0}\n", Settings.Default.UseSSL ? "https" : "http");

            foreach (var addr in GetAllHosts())
            {
                if (Settings.Default.UseSSL && !addr.Value)
                    continue;

                Console.WriteLine(addr.Key);
                Console.WriteLine();

                listener.Prefixes.Add(string.Format("{0}://{1}:{2}/", Settings.Default.UseSSL ? "https" : "http", addr.Key, Settings.Default.Port));
            }

            listener.AuthenticationSchemeSelectorDelegate = request =>
            {
                if ("APEX.Launcher".Equals(request.UserAgent))
                    return Settings.Default.Authentication;
                else
                    return Settings.Default.WebAuthentication;
            };

            Console.WriteLine("UseSSL: {0}, Port: {1}, Authentication: {2}", Settings.Default.UseSSL, Settings.Default.Port, Settings.Default.Authentication);
            Console.WriteLine();

            Console.WriteLine("Ожидание подключений...");
            Console.WriteLine();

            LogManager.AddAppender(new ConsoleAppender());
            LogManager.SetMainThreadInfo(new CurrentMainThreadInfo());

            if (Settings.Default.RawTcp)
            {
                using (var host = new TcpServiceHost(new DataContractSerializationFactory(),
                new IPEndPoint(IPAddress.Loopback, Settings.Default.Port), 10))
                {
                    host.AddService(Factory.Wrapper(new ModelService()));
                    host.Start();
                    Console.ReadLine();
                }
            }
            else
            {
                using (var host = new HttpServiceHost(new DataContractSerializationFactory(), listener))
                {
                    host.AddService(Factory.Wrapper(new ModelService()));
                    host.Start();
                    Console.ReadLine();
                }
            }
        }

        private static Dictionary<string, bool> GetAllHosts()
        {
            Dictionary<string, bool> hosts = new Dictionary<string, bool>
            {
                { IPAddress.Loopback.ToString(), false }
            };

            foreach (var addr in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    continue;

                hosts[addr.ToString()] = false;
            }

            foreach (var site in Settings.Default.Sites)
                hosts.Add(site, true);

            return hosts;
        }
    }
}