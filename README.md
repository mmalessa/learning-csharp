# Pizzeria

Ubuntu 24.04 (.net 9.0.305 version)
```shell
wget https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version 9.0.305

# add to .bashrc
export PATH="$PATH:$HOME/.dotnet:$HOME/.dotnet/tools"

# dotnet tool install --global dotnet-ef --version 9.* # done in Dockerfile
```

###
```shell
# projects
dotnet new console -o src/Pizzeria.Console
dotnet new classlib -o src/Pizzeria.Domain
dotnet new classlib -o src/Pizzeria.Application
dotnet new classlib -o src/Pizzeria.Infrastructure

# add projects to solution
dotnet sln add src/Pizzeria.Console/Pizzeria.Console.csproj
dotnet sln add src/Pizzeria.Domain/Pizzeria.Domain.csproj
dotnet sln add src/Pizzeria.Application/Pizzeria.Application.csproj
dotnet sln add src/Pizzeria.Infrastructure/Pizzeria.Infrastructure.csproj

# project dependencies
dotnet add src/Pizzeria.Application/Pizzeria.Application.csproj reference src/Pizzeria.Domain/Pizzeria.Domain.csproj

dotnet add src/Pizzeria.Infrastructure/Pizzeria.Infrastructure.csproj reference src/Pizzeria.Application/Pizzeria.Application.csproj
dotnet add src/Pizzeria.Infrastructure/Pizzeria.Infrastructure.csproj reference src/Pizzeria.Domain/Pizzeria.Domain.csproj

# optionally Api can refer to Infrastructure for easy DI (extension method) registration
dotnet add src/Pizzeria.Console/Pizzeria.Console.csproj reference src/Pizzeria.Infrastructure/Pizzeria.Infrastructure.csproj
dotnet add src/Pizzeria.Api/Pizzeria.Api.csproj reference src/Pizzeria.Infrastructure/Pizzeria.Infrastructure.csproj
```

### Dev commands
```shell
dotnet build
dotnet run --project src/Pizzeria.Console -- --help
dotnet run --project src/Pizzeria.Console -- pizza --help
dotnet run --project src/Pizzeria.Console -- pizza add 10 "Quattro Formaggi" Medium 34.50
dotnet run --project src/Pizzeria.Console -- pizza remove 10
dotnet run --project src/Pizzeria.Console -- pizza list

dotnet publish src/Pizzeria.Api/Pizzeria.Api.csproj -c Release -o publish
dotnet publish/Pizzeria.Api.dll
```

### Migrations
```shell
# create migrations
dotnet ef migrations add InitialCreate --project src/Pizzeria.Infrastructure
dotnet ef migrations add RemoveIdentityFromPizzaId --project src/Pizzeria.Infrastructure

# update database
dotnet ef database update --project src/Pizzeria.Infrastructure

# list migrations
dotnet ef migrations list --project src/Pizzeria.Infrastructure

# remove migrations
dotnet ef migrations remove --project src/Pizzeria.Infrastructure
```