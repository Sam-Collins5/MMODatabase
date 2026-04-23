using MMOngo.Models;
using MMOngo.Services;
using MMOngo.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

MongoConnection.Init();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IGuildService, GuildService>();
builder.Services.AddScoped<IMissionService, MissionService>();
builder.Services.AddScoped<INpcService, NpcService>();
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<IHomeService, HomeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

static class MongoConnection
{
    public static async Task Init()
    {
        // Connect to database
        StreamReader sr = new StreamReader("mongo_connection_string.txt");
        string connectionString = sr.ReadLine();
        sr.Close();

        GlobalMongoClient = new MongoClient(connectionString);

        var cursor = GlobalMongoClient.ListDatabaseNames();
        int i = 0;
        await foreach (var db in cursor.ToAsyncEnumerable())
        {
            if (i == 0)
            {
                Database = GlobalMongoClient.GetDatabase(db);
                i++;
            }
        }

        await Database.CreateCollectionAsync("Players");
        await Database.CreateCollectionAsync("Characters");
        await Database.CreateCollectionAsync("Guilds");
        await Database.CreateCollectionAsync("Missions");
        await Database.CreateCollectionAsync("NPCs");
        await Database.CreateCollectionAsync("Shops");
        await Database.CreateCollectionAsync("Home");
    }

    public static MongoClient GlobalMongoClient;
    public static IMongoDatabase Database;
}
