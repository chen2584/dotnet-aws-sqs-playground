using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;

const string accessKey = "access";
const string secretKey = "secret";
const string queueUrl = "https://sqs.ap-southeast-1.amazonaws.com/test/MyQueue";
static AmazonSQSClient GetSQSClient()
{
    var awsCreds = new BasicAWSCredentials(accessKey, secretKey);

    var amazonSQSConfig = new AmazonSQSConfig()
    {
    };

    return new AmazonSQSClient(awsCreds, amazonSQSConfig);
}

static async Task ConsumeMessageAsync()
{
    var sqsClient = GetSQSClient();

    while (true)
    {
        Console.WriteLine("Receiving Message...");
        var receiveMessageResponse = await sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                VisibilityTimeout = 5,
                MaxNumberOfMessages = 2,
                WaitTimeSeconds = 20
            });

        Console.WriteLine($"Total Message Receive: {receiveMessageResponse.Messages.Count}");
        foreach (var message in receiveMessageResponse.Messages)
        {
            Console.WriteLine($"Receive Message Id: {message.MessageId}, Body: {message.Body}");
            
            var deleteMessageResponse = await sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
            Console.WriteLine($"Delete Message Id: {message.MessageId} with Receipt Handle: {message.ReceiptHandle} with Http Status Code {deleteMessageResponse.HttpStatusCode}");
        }
    }
}

static async Task SendMessageAsync()
{
    var sqsClient = GetSQSClient();

    while (true)
    {
        Console.WriteLine("Sending Message...");
        var sendMessageResponse = await sqsClient.SendMessageAsync(new SendMessageRequest()
            {
                QueueUrl = queueUrl,
                MessageAttributes = new()
                {
                    { "MyAttribute", new MessageAttributeValue() {  StringValue = "This is a book!", DataType = "String" } }
                },
                MessageBody = "Hello World!"
            });

        Console.WriteLine($"Send Message Id: {sendMessageResponse.MessageId} with Http Status Code: {sendMessageResponse.HttpStatusCode}");
        await Task.Delay(3000);
    }
}

var sendMessageTask = SendMessageAsync();
var consumeMessageTask = ConsumeMessageAsync();
await Task.WhenAll(sendMessageTask, consumeMessageTask);