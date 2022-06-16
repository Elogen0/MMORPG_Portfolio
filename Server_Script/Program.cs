using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InflearnServer.DB;
using InflearnServer.Game;
using InflearnServer.Game.Data;
using ServerCore;
using SharedDB;

namespace InflearnServer
{
    class Program
    {
        static Listener _listener = new Listener();
        public static string Name { get; } = "Server 1";
        public static int Port { get; } = 7777;
        public static string IpAddress { get; set; }

        static void GameLogicTask()
        {
            while (true)
            {
                GameLogic.Instance.Update();
                Thread.Sleep(0); //실행권 넘겨줌
            }
        }

        static void DbTask()
        {
            while (true)
            {
                DBTransaction.Instance.Flush();
                Thread.Sleep(0); //실행권 넘겨줌
            }
        }

        static void NetworkTask()
        {
            while (true)
            {
                List<ClientSession> sessions = SessionManager.Instance.GetSessions();
                foreach (ClientSession session in sessions)
                {
                    session.FlushSend();
                }
                Thread.Sleep(0);
            }
        }

        static void StartServerInfoTask()
        {
            var t = new System.Timers.Timer();
            t.AutoReset = true;
            t.Elapsed += new System.Timers.ElapsedEventHandler((s, e) =>
            {
                using (SharedDbContext shared = new SharedDbContext())
                {
                    ServerDb serverDb = shared.Servers.Where(s => s.Name == Name).FirstOrDefault();
                    if (serverDb != null)
                    {
                        serverDb.IpAddress = IpAddress;
                        serverDb.Port = Port;
                        serverDb.BusyScore = SessionManager.Instance.GetBusyScore();
                    }
                    else
                    {
                        serverDb = new ServerDb()
                        {
                            Name = Program.Name,
                            IpAddress = Program.IpAddress,
                            Port = Program.Port,
                            BusyScore = SessionManager.Instance.GetBusyScore()
                        };
                        shared.Servers.Add(serverDb);
                    }
                    shared.SaveChangesEx();
                }
            });
            t.Interval = 10 * 1000;
            t.Start();
        }

        static void Main(string[] args)
        {
            ConfigManager.LoadConfig();
            DataManager.LoadData();

            GameLogic.Instance.Push(() =>
            {
                GameRoom townRoom = GameLogic.Instance.Add(3);
                GameRoom dungeonRoom = GameLogic.Instance.Add(2);
            });
            
            //DNS (Domain Name System) todo: 나중에 Config파일로 빼줌
            string host = Dns.GetHostName(); //내 로컬컴퓨터 호스트 이름
            IPHostEntry ipHost = Dns.GetHostEntry(host); //
            IPAddress ipAddr = ipHost.AddressList[1];// 아이피주소 0 :복잡, 1: 숫자
            IPEndPoint endpoint = new IPEndPoint(ipAddr, 7777); //ip + port

            IpAddress = ipAddr.ToString();

            _listener.Init(endpoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listenling...");

            //FlushRoom();
            //JobTimer.Instance.Push(FlushRoom);

            StartServerInfoTask();

            //DbTask
            {
                Thread t = new Thread(DbTask);
                t.Name = "DB";
                t.Start();
            }

            //NetworkTask
            {
                Thread t = new Thread(NetworkTask);
                t.Name = "Network Send";
                t.Start();
            }

            //Game Logic Task
            Thread.CurrentThread.Name = "GameLogic";
            GameLogicTask();
        }
    }
}
