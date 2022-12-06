using Microsoft.EntityFrameworkCore;
using World.Cup.Simulator;
using World.Cup.Simulator.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ServerConnection");
string? connectionStringUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
connectionString = string.IsNullOrEmpty(connectionStringUrl) ? connectionString : connectionStringUrl;

// Add services to the container.
builder.Services.AddDbContext<WorldCupContext>(options =>
{
    options.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString));
});

//Enable CORS
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();


//Enable CORS
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.MapGet("/api/teams/groups", async (WorldCupContext context) =>
{
    var teams = await context.teams.ToListAsync();
    var groups = teams.GroupBy(p => p.Group)
        .OrderBy(p => p.Key)
        .Select(p => p.Select(p => p));

    return Results.Ok(groups);
});


app.MapGet("/teams/{id}",async (WorldCupContext context, int id) =>
{
    var team = await context.teams.FindAsync(id);
    return Results.Ok(team);
});

app.MapGet("/teams", async (WorldCupContext context) =>
{
    var teams = await context.teams.ToListAsync();
    return Results.Ok(teams);
});


app.MapPost("/teams", async(WorldCupContext context, team team) =>
{
    await context.teams.AddAsync(team);
    await context.SaveChangesAsync();

    return Results.Ok(team);

});

app.MapPut("/teams/{id}", async (WorldCupContext context, team team) =>
{
    var dbTeam = await context.teams.FindAsync(team.Id);
    if (dbTeam == null)
        Results.NotFound();

    dbTeam.Name = team.Name;
    dbTeam.Img = team.Img;

    context.teams.Update(dbTeam);
    await context.SaveChangesAsync();

    return Results.Ok(dbTeam);
});

app.MapDelete("/teams/{id}", async (WorldCupContext context, int id) =>
{
    team dbTeam = await context.teams.FindAsync(id);
    if (dbTeam == null)
        return Results.NotFound();

    context.teams.Remove(dbTeam);
    await context.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();

