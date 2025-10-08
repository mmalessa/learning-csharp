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

.PHONY: publish
publish:
	@$(DC) exec app sh -c 'dotnet restore'
	@$(DC) exec app sh -c 'dotnet publish src/LearningCSharp.Api -c Release -o /app/publish'

.PHONY: run
run:
	@$(DC) exec -e ASPNETCORE_URLS=http://+:80 -it app sh -c 'dotnet publish/LearningCSharp.Api.dll'

