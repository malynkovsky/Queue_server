using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Water_up
{
    class Receive
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
                
            using (var channel = connection.CreateModel())
            {

                channel.QueueDeclare(queue: "Queue_up",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0} from {1}", message, ea.ConsumerTag);
                    int repl = int.Parse(message) + 1;
                    Console.WriteLine(" [x] Processed {0} from {1}", message, ea.ConsumerTag);
                    using (var channel_b = connection.CreateModel())
                    {
                        channel_b.ExchangeDeclare("Processed_data", "direct", false, false);
                        channel_b.QueueDeclare(queue: "",
                                                 durable: false,
                                                 exclusive: true,
                                                 autoDelete: true,
                                                 arguments: null);
                        channel_b.QueueBind("answers", "Processed_data", "success");
                        //IBasicProperties props = channel.CreateBasicProperties();
                        //props.UserId = ea.BasicProperties.UserId;
                        //props.UserId = ea.ConsumerTag;
                        //надо сделать если ещё нет очередь для ответов конкретному пользователю
                        string answer = repl.ToString();
                        var sent = Encoding.UTF8.GetBytes(answer);
                        channel_b.BasicPublish(exchange: "Processed_data",
                                             routingKey: "success",
                                             basicProperties: ea.BasicProperties,
                                             body: sent);
                        Console.WriteLine(" [x] Sent {0} back", sent);
                    }

                };
                channel.BasicConsume(queue: "Queue_up",
                                     autoAck: true,
                                     consumer: consumer);
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
