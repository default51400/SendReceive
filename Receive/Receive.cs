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

                    channel.BasicQos(0, 1, false);

                    var consumer = new EventingBasicConsumer(channel);

                    channel.BasicConsume(queue: "CashService",
                                         autoAck: false, 
                                         consumer: consumer);
                    Console.WriteLine(" [x] Awaiting RPC requests");

                    consumer.Received += (model, ea) =>
                    {
                        string response = null;

                        var body = ea.Body;
                        var props = ea.BasicProperties;
                        var replyProps = channel.CreateBasicProperties();
                        replyProps.CorrelationId = props.CorrelationId;


                        try
                        {
                            var message = Encoding.UTF8.GetString(body.Span);
                            Console.WriteLine($"Taked message:\n {message}");
                            response = "{All is OK}";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(" [.] " + e.Message);
                            response = "";
                        }
                        finally
                        {
                            var responseBytes = Encoding.UTF8.GetBytes(response);
                            channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                              basicProperties: replyProps, body: responseBytes);
                            channel.BasicAck(deliveryTag: ea.DeliveryTag,
                              multiple: false);
                        }
                    };

                    //channel.BasicConsume(queue: "CashService",
                    //                     autoAck: true,
                    //                     consumer: consumer);

                    Console.WriteLine("Pr key to exit");
                    Console.ReadLine();
                }
            }
        }
    }
}
