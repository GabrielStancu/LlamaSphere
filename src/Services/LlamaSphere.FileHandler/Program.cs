using LlamaSphere.API.Business.TableStorage;
using LlamaSphere.API.Configuration;
using LlamaSphere.API.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<TableStorageConfiguration>(builder.Configuration.GetSection("FileTableStorage"));
builder.Services.Configure<BlobStorageConfiguration>(builder.Configuration.GetSection("FileBlobStorage"));
builder.Services.AddKeyedScoped(typeof(ITableStorageClient<CvEntity>), "cv", typeof(CvsTableStorageClient));
builder.Services.AddKeyedScoped(typeof(ITableStorageClient<JobEntity>), "job", typeof(JobsTableStorageClient));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
