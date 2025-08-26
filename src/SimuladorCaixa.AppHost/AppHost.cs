using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Adiciona o container do MongoDB diretamente
var mongoContainer = builder.AddContainer("mongo", "mongo:latest")
    .WithEndpoint(27017, 27017)
    .WithBindMount("../../Infra/MongoDB/Init", "/docker-entrypoint-initdb.d")
    .WithEnvironment("MONGO_INITDB_DATABASE", "simuladorcaixa")
    .WithLifetime(ContainerLifetime.Persistent);

// Adiciona o container do Redis
var redisContainer = builder.AddContainer("redis", "redis:latest")
    .WithEndpoint(6379, 6379)
    .WithLifetime(ContainerLifetime.Persistent);

// Referencia o container do MongoDB no seu projeto
builder.AddProject<Projects.SimuladorCaixa_Api>("simuladorcaixa-api")
    .WaitFor(mongoContainer)
    .WaitFor(redisContainer);

builder.Build().Run();
