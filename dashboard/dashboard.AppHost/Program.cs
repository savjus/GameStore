var builder = DistributedApplication.CreateBuilder(args);

// var api =
builder.AddProject<Projects.GameStore>("gamestore");

builder.Build().Run();
