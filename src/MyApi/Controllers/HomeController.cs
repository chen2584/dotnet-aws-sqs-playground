using Amazon.SQS;
using Microsoft.AspNetCore.Mvc;

namespace MyApi.Controllers;

public class HomeController : ControllerBase
{
    private readonly IAmazonSQS _amazonSqs;
    public HomeController(IAmazonSQS amazonSQS)
    {
        _amazonSqs = amazonSQS;
    }

    [HttpPost("send-sqs-message")]
    public async Task<IActionResult> SendSqsMessage()
    {
        var result = await _amazonSqs.SendMessageAsync("http://sqs.us-east-1.localhost.localstack.cloud:4566/000000000000/test", "some-message", default);
        return Ok(result);
    }
}