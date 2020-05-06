using System;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Send
{
    class Human
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var sender = new SendAndReceive();

            Human hum = new Human { Name = "Alex", Age = 34 };
            string json = JsonSerializer.Serialize<Human>(hum);

            Console.WriteLine($" [x] Requesting {json}");
            var response = sender.Call(json);

            Console.WriteLine(" [.] Got '{0}'", response);
            sender.Close();

            #region old code for single send to MQ
            //var factory = new ConnectionFactory()
            //{
            //    HostName = "localhost",
            //    Port = 5672,
            //    UserName = "root",
            //    Password = "root"
            //};

            //Human hum = new Human { Name = "Alex", Age = 34 };
            //string json = JsonSerializer.Serialize<Human>(hum);

            //using (var connection = factory.CreateConnection())
            //{
            //    using (var channel = connection.CreateModel())
            //    {
            //        channel.QueueDeclare(queue: "CashService",
            //                     durable: false,
            //                     exclusive: false,
            //                     autoDelete: false,
            //                     arguments: null);

            //        var body = Encoding.UTF8.GetBytes(json);

            //        channel.BasicPublish(exchange: "",
            //                             routingKey: "CashService",
            //                             basicProperties: null,
            //                             body: body);
            //        Console.WriteLine(" [x] Sent {0}", json);
            //    }
            //}

            //Console.WriteLine("Pr key to exit");
            //Console.ReadLine();
            #endregion
        }
    }
}
