# Service Fabric Lab

## What is it?
Service Fabric is a platform for distributed systems.

## What problem does Service Fabric solve?

Running a distributed system (think microservices) is both fun and boring. The boring part is packaging, deploying, health monitoring, scalability, etc. Service Fabric handles these parts for you. However, if you find these parts fun, then you don't need Service Fabric :)

## How do I use Service Fabric?

Both stateful and stateless services can be built with two programming models, [reliable services](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-services-introduction/) and [reliable actors](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-actors-introduction/).

You can create Service Fabric clusters in many environments. This can be in Azure or on premises, on Windows Server or on Linux.

Read more about Service Fabric terminology [here](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-technical-overview/)

## Reliable Services Lifecycle

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

## Scenarios

1. Data computation using reliable services.
2. Rock, Paper, Scissors using reliable actors.

## Prerequisites

1. Visual Studio 2015
2. Install [Azure Fabric SDK for VS2015](http://www.microsoft.com/web/handlers/webpi.ashx?command=getinstallerredirect&appid=MicrosoftAzure-ServiceFabric-VS2015)

Or check the [Prepare your development environment](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-get-started/) page.

## Reliable service lab

1. Tooling
2. Rolling upgrades
3. Health monitoring
4. Rollback

## Reliable actors lab

How does it compare to Reliable services?
