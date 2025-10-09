# My Demo Service

##
```shell
make build

make publish
make run
```
Open in browser: http://localhost:5000

### .net notes

Ubuntu 24.04 (.net 9.0.305 version)
```shell
wget https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version 9.0.305

# add to .bashrc
export PATH="$PATH:$HOME/.dotnet:$HOME/.dotnet/tools"

# dotnet tool install --global dotnet-ef --version 9.* # done in Dockerfile
```


Create example project from zero to hero
``` shell
mkdir learning-csharp
cd learning-csharp

# solution
dotnet new sln -n LearningCSharp

# projects
dotnet new webapi -o src/LearningCSharp.Api --no-https
dotnet new console -o src/LearningCSharp.Console
dotnet new classlib -o src/LearningCSharp.Domain
dotnet new classlib -o src/LearningCSharp.Application
dotnet new classlib -o src/LearningCSharp.Infrastructure

# add projects to solution
dotnet sln add src/LearningCSharp.Api/LearningCSharp.Api.csproj
dotnet sln add src/LearningCSharp.Console/LearningCSharp.Console.csproj
dotnet sln add src/LearningCSharp.Domain/LearningCSharp.Domain.csproj
dotnet sln add src/LearningCSharp.Application/LearningCSharp.Application.csproj
dotnet sln add src/LearningCSharp.Infrastructure/LearningCSharp.Infrastructure.csproj

# project dependencies
dotnet add src/LearningCSharp.Application/LearningCSharp.Application.csproj reference src/LearningCSharp.Domain/LearningCSharp.Domain.csproj
dotnet add src/LearningCSharp.Infrastructure/LearningCSharp.Infrastructure.csproj reference src/LearningCSharp.Application/LearningCSharp.Application.csproj
dotnet add src/LearningCSharp.Infrastructure/LearningCSharp.Infrastructure.csproj reference src/LearningCSharp.Domain/LearningCSharp.Domain.csproj
dotnet add src/LearningCSharp.Api/LearningCSharp.Api.csproj reference src/LearningCSharp.Application/LearningCSharp.Application.csproj
# optionally Api can refer to Infrastructure for easy DI (extension method) registration
dotnet add src/LearningCSharp.Api/LearningCSharp.Api.csproj reference src/LearningCSharp.Infrastructure/LearningCSharp.Infrastructure.csproj
dotnet add src/LearningCSharp.Api/LearningCSharp.Console.csproj reference src/LearningCSharp.Infrastructure/LearningCSharp.Infrastructure.csproj


## Suggested NuGet's
# in Api
dotnet add src/LearningCSharp.Api package Microsoft.AspNetCore.OpenApi
dotnet add src/LearningCSharp.Api package Swashbuckle.AspNetCore
dotnet add src/LearningCSharp.Api package Serilog.AspNetCore
dotnet add src/LearningCSharp.Api package MediatR.Extensions.Microsoft.DependencyInjection

# in Console
dotnet add src/LearningCSharp.Console package Microsoft.Extensions.Hosting

# in Application
dotnet add src/LearningCSharp.Application package MediatR
dotnet add src/LearningCSharp.Application package FluentValidation

# in Infrastructure
dotnet add src/LearningCSharp.Infrastructure package Microsoft.EntityFrameworkCore --version 9.0.9
dotnet add src/LearningCSharp.Infrastructure package Microsoft.EntityFrameworkCore.Relational --version 9.0.9
dotnet add src/LearningCSharp.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL --version 9.0.9
dotnet add src/LearningCSharp.Infrastructure package Microsoft.EntityFrameworkCore.Tools --version 9.0.9
dotnet add src/LearningCSharp.Infrastructure package Microsoft.EntityFrameworkCore.Design --version 9.0.9
dotnet add src/LearningCSharp.Infrastructure package Confluent.Kafka

```

```shell
dotnet restore
dotnet publish src/LearningCSharp.Api/LearningCsharp.Api.csproj -c Release /app/publish
```