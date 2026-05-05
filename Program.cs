using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using MMOngo.Models;
using MMOngo.Services;
using MMOngo.Services.Interfaces;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

await MongoConnection.Init(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/UserLogin/Login";
        options.LogoutPath = "/UserLogin/Logout";
        options.AccessDeniedPath = "/UserLogin/Login";
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IGuildService, GuildService>();
builder.Services.AddScoped<IMissionService, MissionService>();
builder.Services.AddScoped<INpcService, NpcService>();
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

public static class MongoConnection
{
    public static MongoClient GlobalMongoClient { get; private set; } = null!;

    public static IMongoDatabase Database { get; private set; } = null!;

    public static async Task Init(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");  // just replace the default connection string key with the one you have in a hidden file
        string? databaseName = configuration["DatabaseSettings:DatabaseName"];  // now replace the database name key with the one you have in a hidden file

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("MongoDB connection string is missing from appsettings.json.");
        }

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new Exception("MongoDB database name is missing from appsettings.json.");
        }

        GlobalMongoClient = new MongoClient(connectionString);
        Database = GlobalMongoClient.GetDatabase(databaseName);

        await CreateCollectionIfMissingAsync("Players");
        await CreateCollectionIfMissingAsync("PlayerCharacters");
        await CreateCollectionIfMissingAsync("Guilds");
        await CreateCollectionIfMissingAsync("Missions");
        await CreateCollectionIfMissingAsync("NPCs");
        await CreateCollectionIfMissingAsync("Shops");
        await CreateCollectionIfMissingAsync("Home");
        await CreateCollectionIfMissingAsync("Weapons");
        await CreateCollectionIfMissingAsync("Armors");
        await CreateCollectionIfMissingAsync("Tools");
        await CreateCollectionIfMissingAsync("Spells");
        await CreateCollectionIfMissingAsync("Users");
    }

    private static async Task CreateCollectionIfMissingAsync(string collectionName)
    {
        using IAsyncCursor<string> cursor = await Database.ListCollectionNamesAsync();
        List<string> collectionNames = await cursor.ToListAsync();

        if (!collectionNames.Contains(collectionName))
        {
            await Database.CreateCollectionAsync(collectionName);
        }
    }
}