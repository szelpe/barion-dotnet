# Quickstart guide to integrate your .NET web application with Barion Payment Gateway

You can find a complete sample website for this guide under the Samples folder. Check [BarionController](https://github.com/szelpe/barion-dotnet/blob/master/Samples/SampleWebsite/Controllers/BarionController.cs) for a detailed example on how to use the client.

## Setup

- Install the package `BarionClient`
- It is recommended to add `BarionBaseAddress` and `BarionPOSKey` to the appSettings
- Create a new Controller to handle Barion operations called `BarionController`
- Create a new action called `StartPayment` for `BarionController`
    - This action will call the StartPayment Barion API
- Create a new action called `Callback` for `BarionController`
	- This action will be called by Barion after the payment finished
	- Should have an input parameter called `Guid PaymentId`
- Create a "payment finished" page if not exist
	- The user will be redirected to this page after the Barion payment is finished

## Start payment

- Implement the `StartPayment` action using the `BarionClient` and `StartPaymentOperation`
	- Create the items which are purchased and a transaction for this payment
	- Set the required parameters
	- Make sure to set the `CallbackUrl` to the `Callback` action
- Call `barionClient.ExecuteAsync`
- Check if the operation was successful using the `result.IsOperationSuccessful` property
- If the operation is successfull redirect the user to the `GatewayUrl` to pay

## Callback

- Implement the `Callback` action using the `GetPaymentStateOperation`
	- Set the state of the order according to the returned `Status` of the payment
