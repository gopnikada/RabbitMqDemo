using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Receiver1
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            factory.ClientProvidedName = "Rabbit Sender App";

            IConnection connection = factory.CreateConnection();

            IModel channel = connection.CreateModel();

            string exchangeName = "DemoExchange";
            string routingKey = "demo-routing-key";
            string queueName = "DemoQueue";


            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            //durable - save on restart
            //exclusive - only on one connection + close if connection removed
            //autodelete - remove if no consumers
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey, null);

            channel.BasicQos(0, 1, false);

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, args) =>
            {
                byte[] body = args.Body.ToArray();
                Console.WriteLine($"Message:" + Encoding.UTF8.GetString(body));

                channel.BasicAck(args.DeliveryTag, false);
            };

            string consumerTag = channel.BasicConsume(queueName, autoAck: false, consumer);
            Console.ReadLine();

            channel.BasicCancel(consumerTag);

            connection.Close();
        }
    }
}
