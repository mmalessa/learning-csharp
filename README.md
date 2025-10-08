# My Demo Service

##
```shell
make build

make publish
make run
```
Open in browser: http://localhost:5000

### C# notes

Create example project from zero to hero
``` shell
mkdir learning-csharp
cd learning-csharp

# solution
dotnet new sln -n LearningCSharp

# projects
dotnet new webapi -o src/LearningCSharp.Api --no-https

# add projects to solution
dotnet sln add src/LearningCSharp.Api/LearningCSharp.Api.csproj
```

```shell
dotnet restore
dotnet publish src/LearningCSharp.Api/LearningCsharp.Api.csproj -c Release /app/publish
```