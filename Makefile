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

.PHONY: init
init:
	@$(MAKE) init-packages
	@$(MAKE) init-database
	@dotnet restore

.PHONY: init-packages
init-packages:
	@$(DC) exec app sh -c 'dotnet add src/LearningCSharp.Api package Microsoft.AspNetCore.OpenApi'
	@$(DC) exec app sh -c 'dotnet add src/LearningCSharp.Console package Microsoft.Extensions.Hosting'
	@$(DC) exec app sh -c 'dotnet add src/LearningCSharp.Infrastructure package Microsoft.EntityFrameworkCore'
	@$(DC) exec app sh -c 'dotnet add src/LearningCSharp.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL'
	@$(DC) exec app sh -c 'dotnet add src/LearningCSharp.Infrastructure package Microsoft.EntityFrameworkCore.Design'

.PHONY: init-database
init-database:
	@$(DC) exec app sh -c 'dotnet ef migrations add InitialCreate --project src/LearningCSharp.Infrastructure'

.PHONY: publish
publish:
	@$(DC) exec app sh -c 'dotnet restore'
	@$(DC) exec app sh -c 'dotnet publish src/LearningCSharp.Api -c Release -o /app/publish'
	@$(DC) exec app sh -c 'dotnet publish src/LearningCSharp.Console -c Release -o /app/publish'
	@dotnet restore # workaround for absolute path's in obj/ files

.PHONY: run
run:
	@$(DC) exec -e ASPNETCORE_URLS=http://+:80 -it app sh -c 'dotnet publish/LearningCSharp.Api.dll'

