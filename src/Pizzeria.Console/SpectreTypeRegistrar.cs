using System;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Pizzeria.Console;

// Rejestrator, który NIE buduje własnego kontenera, tylko deleguje do istniejącego IServiceProvider.
public sealed class ServiceProviderTypeRegistrar(IServiceProvider provider) : ITypeRegistrar
{
    public ITypeResolver Build() => new ServiceProviderTypeResolver(provider);

    // No-op: nie dodajemy nic do osobnej kolekcji — wszystko masz już w host.Services
    public void Register(Type service, Type implementation) { }
    public void RegisterInstance(Type service, object implementation) { }
    public void RegisterLazy(Type service, Func<object> factory) { }
}

public sealed class ServiceProviderTypeResolver(IServiceProvider provider) : ITypeResolver, IDisposable
{
    public object? Resolve(Type? type) => type is null ? null : provider.GetService(type);
    public void Dispose() { /* nic do sprzątania */ }
}