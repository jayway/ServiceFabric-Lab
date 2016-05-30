# Service Fabric Lab
This is an exploratory lab session. First, a short introduction.

## What is Service Fabric?
Service Fabric is a platform for distributed systems.

## What problem does Service Fabric solve?
Running a distributed system (think microservices) is both fun and boring. The boring part is packaging, deploying, health monitoring, scalability, etc. Service Fabric handles these parts for you. However, if you find these parts fun, then you don't need Service Fabric :)

## How do I use Service Fabric?
Both stateful and stateless services can be built with two programming models, [reliable services](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-services-introduction/) and [reliable actors](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-actors-introduction/).

You can create Service Fabric clusters in many environments. This can be in Azure or on premises, on Windows Server or on Linux.

Read more about Service Fabric terminology [here](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-technical-overview/)

## What is the lifecycle of Reliable Services?

### Startup

- `CreateServiceInstanceListeners` returns a `IEnumerable<ServiceInstanceListener>` where `ServiceInstanceListener` is a factory for `ICommunicationListener`
- `OpenAsync` is called on `ICommunicationListener` which allows the service to listen for traffic
- `RunAsync` may be used to setup background workers

### Shutdown

- Call upon deletion, upgrade or move to another node
- Cancellation token passed to `RunAsync` is cancelled
- `CloseAsync` is called next
- Promotion of a secondary doesn't occur until `RunAsync` and `CloseAsync` return
- Services immediately lose write access to Reliable Collections upon shutdown

## Reliable Collections

State is persisted in reliable collections. Reliable collections include implementations of `IReliableDictionary` and `IReliableQueue`. Both inherit from `IReliableCollection` which in turn inherit from `IReliableState`.

Implementation of `IReliableState` are sometimes referred to as "reliable state providers" and their lifecycles are managed by `IStateManager`.

# Exploratory lab scenarios

1. Implement RPS (Rock, Paper, Scissors) using Reliable Services.
2. Implement RPS (Rock, Paper, Scissors) using Reliable Actors.

## Prerequisites

1. Visual Studio 2015
2. Install [Azure Fabric SDK for VS2015](http://www.microsoft.com/web/handlers/webpi.ashx?command=getinstallerredirect&appid=MicrosoftAzure-ServiceFabric-VS2015)

Or check the [Prepare your development environment](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-get-started/) page.

## Reliable Service lab

Implement an existing implementation of RPS to use Service Fabric. Here is a list of interesting points:

1. Tooling
2. Rolling upgrades
3. Health monitoring
4. Rollback

This lab has been completed and the result is in [RpsReliableServices](https://github.com/jayway/ServiceFabric-Lab/tree/master/RpsReliableServices). The communication with Service Fabric is in [GameController.cs](https://github.com/jayway/ServiceFabric-Lab/blob/master/RpsReliableServices/RpsService/Controllers/GameController.cs) where a `IReliableStateManager` is the integration point. Read more at [Architecture for stateful and stateless Reliable Services](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-services-platform-architecture/)

## Reliable Actors lab

1. How does it compare to Reliable services?

This lab has not been completed.

## Open questions

1. What does the endpoint for a stateful service look like?
