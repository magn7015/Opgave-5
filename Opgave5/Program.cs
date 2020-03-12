using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Opgave5
{
    class Program
    {

        private static readonly List<Bog> books = new List<Bog>()
        {
            new Bog("Bog1", "Forfatter1", 114, "3928375647382"),
            new Bog("Bog2", "Forfatter2", 25, "1928362392012"),
            new Bog("Bog3", "Forfatter3", 10, "1173844958473"),
            new Bog("Bog4", "Forfatter4", 593, "8392037352718"),
            new Bog("bog5", "Forfatter5", 725, "8302936251627")
        };

        public static void DoClient(TcpClient socket)
        { 
        NetworkStream ns = socket.GetStream();

            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            sw.AutoFlush = true;

            string line = sr.ReadLine();
            string answer = "";

            while (line != null && line != "")
            {
                string[] messageArray = line.Split(' ');
                string param = line.Substring(line.IndexOf(' ') + 1);
                string command = messageArray[0];

                switch (command)
                {
                    case "GetAll":                       
                        sw.WriteLine(JsonConvert.SerializeObject(books));
                        break;
                    case "Get":                       
                        sw.WriteLine(JsonConvert.SerializeObject(books.Find(id => id.Isbn13 == param)));
                        break;
                    case "Save":                        
                        Bog saveBog = JsonConvert.DeserializeObject<Bog>(param);
                        books.Add(saveBog);
                        break;                  
                }
                line = sr.ReadLine();
            }
                ns.Close();
                socket.Close();
        }
        static void Main(string[] args)
        {   
            //Opret server
            IPAddress ip = IPAddress.Parse("172.17.231.225");

            TcpListener listener = new TcpListener(ip, 7);
            listener.Start();
            Console.WriteLine("Server Started");


            do
            {
                Task.Run(() =>
                {
                    
                    TcpClient tempSocket = listener.AcceptTcpClient();
                    Console.WriteLine("Connected");
                    
                    DoClient(tempSocket);

                });

            } while (true);



          

        }
    }
}
