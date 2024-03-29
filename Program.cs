﻿using Microsoft.Extensions.Configuration;
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
    "https://raw.githubusercontent.com/Swimburger/SendMultipleSmsInOrder/main/images/DogMeme1.png",
    "https://raw.githubusercontent.com/Swimburger/SendMultipleSmsInOrder/main/images/DogMeme2.png",
    "https://raw.githubusercontent.com/Swimburger/SendMultipleSmsInOrder/main/images/DogMeme3.png",
    "https://raw.githubusercontent.com/Swimburger/SendMultipleSmsInOrder/main/images/DogMeme4.png",
};

async Task<bool> WaitForDelivery(string messageSid, int amountOfPollingAttempts, TimeSpan pollingInterval)
{
    for (var i = 0; i < amountOfPollingAttempts; i++)
    {
        Console.WriteLine(i);
        var message = await MessageResource.FetchAsync(messageSid).ConfigureAwait(false);
        if (message.Status == MessageResource.StatusEnum.Delivered)
        {
            return true;
        }

        await Task.Delay(pollingInterval).ConfigureAwait(false);
    }

    return false;
}

for (var index = 0; index < images.Length; index++)
{
    var image = images[index];
    var messageResource = await MessageResource.CreateAsync(
        to: new PhoneNumber(to),
        from: new PhoneNumber(from),
        mediaUrl: new List<Uri>
        {
            new(image)
        }
    );
    Console.WriteLine($"Status: {messageResource.Status}");

    // if not the last image
    if (index != images.Length - 1)
    {
        var wasDelivered = await WaitForDelivery(
            messageSid: messageResource.Sid, 
            amountOfPollingAttempts: 20,
            pollingInterval: TimeSpan.FromSeconds(1)
        );
        if (wasDelivered == false)
        {
            Console.WriteLine("Message wasn't delivered in time.");
            break;
        }
    }
}