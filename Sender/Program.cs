using System;
using System.Text;
using RabbitMQ.Client;

namespace Sender
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

            byte[] messageBodyBytes = Encoding.UTF8.GetBytes("Hello world2");
            channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);

            channel.Close();
            connection.Close();
        }
    }
}
