FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

ARG UID=1000
ARG USER=ubuntu
RUN useradd -m -u ${UID} ${USER}

ENV PATH="$PATH:/home/${USER}/.dotnet/tools"

USER ${USER}
RUN dotnet tool install --global dotnet-ef

WORKDIR /app
