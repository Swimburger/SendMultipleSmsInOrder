using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var from = configuration["FromPhoneNumber"];
var to = configuration["ToPhoneNumber"];

TwilioClient.Init(
    configuration["Twilio:AccountSid"],
    configuration["Twilio:AuthToken"]
);

var images = new[]
{
    "",
    ""
};

foreach (var image in images)
{
    var messageResource = await MessageResource.CreateAsync(
        to: new PhoneNumber(to),
        from: new PhoneNumber(from),
        mediaUrl: new List<Uri>
        {
            new(image)
        }
    ).ConfigureAwait(false);
    Console.WriteLine($"Status: {messageResource.Status}");
}

Console.WriteLine("Press any key to stop application.");
Console.ReadKey();