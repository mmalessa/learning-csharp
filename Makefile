DC = docker compose

.DEFAULT_GOAL      = help

.PHONY: help
help:
	@grep -E '(^[a-zA-Z0-9_-]+:.*?##.*$$)|(^##)' Makefile | awk 'BEGIN {FS = ":.*?## "}{printf "\033[32m%-30s\033[0m %s\n", $$1, $$2}' | sed -e 's/\[32m##/[33m/'

.PHONY: build
build:
	@BUILD_TARGET=build $(DC) build

### DEV
.PHONY: up
up: ## Start the project docker containers
	@$(DC) up -d

.PHONY: down
down: ## Down the docker containers
	@$(DC) down --timeout 25

.PHONY: shell
shell:
	@$(DC) exec -it app bash

.PHONY: init-packages
init-packages:
	@$(DC) exec app sh -c 'dotnet add src/Pizzeria.Console package Spectre.Console.Cli'
	@$(DC) exec app sh -c 'dotnet add src/Pizzeria.Console package Microsoft.Extensions.Hosting'
	@$(DC) exec app sh -c 'dotnet add src/Pizzeria.Console package Spectre.Console'
	@$(DC) exec app sh -c 'dotnet add src/Pizzeria.Console package Mediator.SourceGenerator'
	
	@$(DC) exec app sh -c 'dotnet add src/Pizzeria.Application package Microsoft.Extensions.DependencyInjection.Abstractions'
	@$(DC) exec app sh -c 'dotnet add src/Pizzeria.Application package Mediator.Abstractions'
	
	@$(DC) exec app sh -c 'dotnet add src/Pizzeria.Infrastructure package Microsoft.EntityFrameworkCore'
	@$(DC) exec app sh -c 'dotnet add src/Pizzeria.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer'
	@$(DC) exec app sh -c 'dotnet add src/Pizzeria.Infrastructure package Microsoft.EntityFrameworkCore.Design'
	@$(DC) exec app sh -c 'dotnet add src/Pizzeria.Infrastructure package Microsoft.Extensions.Configuration'
	@$(DC) exec app sh -c 'dotnet add src/Pizzeria.Infrastructure package Microsoft.Extensions.Configuration.EnvironmentVariables'
	@$(DC) exec app sh -c 'dotnet add src/Pizzeria.Infrastructure package Microsoft.Extensions.DependencyInjection'
	@$(DC) exec app sh -c 'dotnet add src/Pizzeria.Infrastructure package Mediator.Abstractions'

.PHONY: init-database
init-database:
	@$(DC) exec app sh -c 'dotnet ef database update --project src/Pizzeria.Infrastructure'

.PHONY: compile
compile:
	@$(DC) exec app sh -c 'dotnet publish src/Pizzeria.Console -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o bin'
	@$(DC) exec app sh -c 'dotnet publish src/Pizzeria.Api -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o bin'
