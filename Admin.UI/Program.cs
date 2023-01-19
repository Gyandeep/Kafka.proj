using Admin.UI.Client;

using Confluent.Kafka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<AdminClientHandle>();
builder.Services.AddSingleton<KafkaDependentAdmin>();
builder.Services.AddSingleton<KafkaClientHandle>();
builder.Services.AddSingleton<KafkaDependentProducer<string, string>>();
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsDevPolicy", b =>
    {
        b.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors("CorsDevPolicy");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllers();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller}/{action=Index}");

//app.MapFallbackToFile("index.html");

app.Run();
