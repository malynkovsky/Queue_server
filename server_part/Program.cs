using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;

namespace Math_simple
{
    class Two_num
    {
        public int First_num = 0;
        public int Second_num = 0;
    }
    class Receive
    {
        public static void Main()
        {
            Two_num pair = new Two_num();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                
                channel.QueueDeclare(queue: "Queue_math",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    pair = JsonConvert.DeserializeObject<Two_num>(message);//десериализяция
                    Console.WriteLine(" [x] Received {0} from {1}", message,ea.ConsumerTag);
                    int repl = pair.First_num + pair.Second_num;
                    Console.WriteLine(" [x] Processed {0} from {1}", message, ea.ConsumerTag);
                    channel.BasicConsume(queue: "Queue_up",
                                     autoAck: true,
                                     consumer: consumer);
                    channel.ExchangeDeclare("Processed_data", "direct",false,false);
                    channel.QueueDeclare(queue: ea.BasicProperties.UserId,
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);
                    channel.QueueBind(ea.BasicProperties.UserId, "Processed_data", "success");
                    IBasicProperties props = ea.BasicProperties;
                    string answer = repl.ToString();
                    var sent = Encoding.UTF8.GetBytes(answer);

                    channel.BasicPublish(exchange: "Processed_data",
                                         routingKey: "success",
                                         basicProperties: props,
                                         body: sent);
                    Console.WriteLine(" [x] Sent {0} back", message);
                };
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
