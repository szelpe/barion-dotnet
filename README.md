# Barion .NET

The Barion .NET library makes it easy to add Barion payment to your .NET application. It is built upon [Barion's Web API](https://doksi.barion.com/).

[![Build status](https://ci.appveyor.com/api/projects/status/ixe1udvb57sbh0hv/branch/master?svg=true)](https://ci.appveyor.com/project/szelpe/barion-dotnet/branch/master)

## Supported operations

- Immediate payment
- Reservation payment
- Refund
- Finish Reservation

## Prerequisites

- .NET 4.5 or later
- Newtonsoft.Json

## Installation

The easiest way to install is to use NuGet:

```
Install-Package BarionClient
```

## Usage

The heart of the library is the `BarionClient` class which provides the `ExecuteAsync` method to execute various operations.
Create the operation class you want to use: `StartPaymentOperation`, `GetPaymentStateOperation`, `RefundOperation` or `FinishReservationOperation` respectively.
After setting the operation properties you can use the `ExecuteAsync` method and pass the opertaion as the parameter.

> Note that `BarionClient` implements `IDisposable`, use it [accordingly](https://msdn.microsoft.com/en-us/library/yh598w02.aspx).

[QuickStart guide](https://github.com/szelpe/barion-dotnet/blob/master/QuickStart.md)

### Example

``` csharp
var barionSettings = new BarionSettings
    {
        BaseUrl = new Uri("https://api.test.barion.com/"),
        POSKey = Guid.Parse("d1bcff3989885d3a98235c1cd768eba2")
    };

using(var barionClient = new BarionClient(barionSettings))
{
	var startPayment = new StartPaymentOperation();

	// add payment parameters to startPayment

	var result = await barionClient.ExecuteAsync(startPayment);

	if(result.IsOperationSuccessful)
	{
		// redirect the user to the payment page
	}
}
```

## Sample website

You can find a complete sample website under the `Samples` directory. Check [BarionController](https://github.com/szelpe/barion-dotnet/blob/master/Samples/SampleWebsite/Controllers/BarionController.cs) for a detailed example on how to use the client.

## Extending the API

You can easily add your own operations by creating a new subclass of `BarionOperation`. E.g. if you want to support the [Reject](https://doksi.barion.com/Payment-Reject-v2) payment operation you need to create a new class:

``` csharp
public class RejectOperation : BarionOperation
{
    public override HttpMethod Method => HttpMethod.Post;
    public override Uri RelativeUri => new Uri("/v2/Payment/Reject", UriKind.Relative);
    public override Type ResultType => typeof(RejectOperationResult);

    public string UserName { get; set; }
    public string Password { get; set; }
    public Guid PaymentId { get; set; }
}
```

And the class which represents the result of the operation:

``` csharp
public class RejectOperationResult : BarionOperationResult
{
    public Guid PaymentId { get; set; }
    public bool IsSuccess { get; set; }
}
```

After this you can use your own operation class the same way as the built in ones.

## Contribute

You're welcome to contribute. To build the source code you'll need **Visual Studio 2015**.

You can use `createPackage.ps1` to create the NuGet package.