using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
/****************************---------CREATE CONNECTION TO RABBITMQ-----************************************/
#region
//start of setup connection

var factory =new ConnectionFactory();
factory.ClientProvidedName = "RabbitMQ-Receiver-1-Service";
var username = "guest";var password = "guest"; var url = "localhost:5672";
factory.Uri = new Uri($"amqp://{username}:{password}@{url}");

IConnection con = factory.CreateConnection();
IModel channel = con.CreateModel();

var exchangeName = "exchange-name-demo";
var routingKey = "routing-key-demo";
var queueName = "queue-name-demo";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false);
channel.QueueBind(queueName,exchangeName, routingKey);

//end of setup connection
#endregion

/****************************---------SETUP LISTEN FOR COMMING MESSAGES FORM RABBITMQ-----************************************/
#region
//to set
//1-param how match size for single message 
//2-param how many messages at same time 
//3-param is this setting for whole queue (isGlobal)
channel.BasicQos(0, 1, false);

var cunsomer=new EventingBasicConsumer(channel);
cunsomer.Received += async (sender, args) =>
{
    //as db operation
    await Task.Delay(1000);
    byte[] messageByte=args.Body.ToArray();
    string messageStr =Encoding.UTF8.GetString(messageByte);
    Console.WriteLine("Received Message : {0} ",messageStr);

    //send ack to RabbitMQ
    channel.BasicAck(args.DeliveryTag, false);
};

//start listening
var cunsomerTag = channel.BasicConsume(queueName, false, cunsomer);

#endregion

//make app running
Console.WriteLine("Listening For Messages (delay 1000ms) ..");
Console.ReadLine();



/****************************---------CLOSE CONNECTION AND CHANNEL TO RABBITMQ-----************************************/
#region

//close consumer
channel.BasicCancel(cunsomerTag);

channel.Close();
con.Close();

#endregion


