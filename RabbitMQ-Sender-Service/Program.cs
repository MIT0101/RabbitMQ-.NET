using RabbitMQ.Client;
using System.Text;

/****************************---------CREATE CONNECTION TO RABBITMQ-----************************************/
#region 
//start of setup connection
ConnectionFactory factory =new ConnectionFactory();
var username = "guest";var password = "guest"; var url = "localhost:5672";
factory.Uri = new Uri($"amqp://{username}:{password}@{url}");
factory.ClientProvidedName = "RabbitMQ-Sender-Service";

IConnection con =factory.CreateConnection();
IModel channel =con.CreateModel();

var exchangeName = "exchange-name-demo";
var routingKey = "routing-key-demo";
var queueName = "queue-name-demo";

channel.ExchangeDeclare(exchangeName,ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey);

//end of setup connection
#endregion
Console.WriteLine("How Many Message You Need to send");
int n = int.Parse(Console.ReadLine());

/****************************---------SEND MESSAGES USING CHANNEL TO RABBITMQ-----************************************/
#region 

//start of sending messages
for (int i = 1; i <= n; i++)
{
    await Task.Delay(1000);
string messageBodyStr = $"Message {i} from sender";
byte[] messageBodyBytes = Encoding.UTF8.GetBytes(messageBodyStr);
channel.BasicPublish(exchangeName, routingKey,null,messageBodyBytes);
    Console.WriteLine("Message {0} Sent", i);
}
//end of sending messages
#endregion

/****************************---------CLOSE CONNECTION AND CHANNEL TO RABBITMQ-----************************************/
#region

channel.Close();
con.Close();    

#endregion


