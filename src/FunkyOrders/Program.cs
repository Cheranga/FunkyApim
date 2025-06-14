using FunkyOrders;
using Microsoft.Extensions.Hosting;

using var host = Bootstrapper.GetHost();
await host.RunAsync();
