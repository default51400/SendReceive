using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Receive
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() 
            {
                HostName = "localhost",
                UserName = "root",
                Password = "root"

            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "CashService",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body.Span);
                        Console.WriteLine($"[x] received {message}");
                    };

                    channel.BasicConsume(queue: "CashService",
                                         autoAck: true,
                                         consumer: consumer);

                    Console.WriteLine("Pr key to exit");
                    Console.ReadLine();
                }
            }
        }
    }
}
