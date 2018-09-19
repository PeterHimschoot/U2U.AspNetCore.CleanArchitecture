# U2U.AspNetCore.CleanArchitecture

This package makes it easier keep dependencies between **Infrastructure** and **Web Site** projects seperate,
as described in [Clean Architecture](https://8thlight.com/blog/uncle-bob/2012/08/13/the-clean-architecture.html).

## Usage

In your **Infrastructure** project you create a `DependencyInjection` class with extension
methods that add needed dependencies, attributed with `[AutoConfig]`

``` csharp
[AutoConfig]
public static class DependencyInjection
{
  [AutoConfig]
  public static IServiceCollection AddCurrencyConverter(this IServiceCollection services)
  => services.AddTransient<ICurrencyConverterService, CurrencyConverterService>();

  [AutoConfig("GamesDb")]
  public static IServiceCollection AddGamesDb(this IServiceCollection services, string connectionString, [MigrationAssembly] string migrationAssembly)
    => services.AddDbContext<GamesDb>(optionsBuilder =>
         optionsBuilder.UseSqlServer(connectionString,
           sqlServerOptions => sqlServerOptions.MigrationsAssembly(migrationAssembly)));
```

In your **Web Site** project you call the `AddAutoConfig` method, passing in the 
`Configuration` instance.

``` csharp
services.AddAutoConfig(Configuration);
```

This method will look the the **Infrastructure** project for `AutoConfig` methods and call them.

## Configuration

So how does `AddAutoConfig` know where to look for `AutoConfig` methods?

Your project should have an `AutoConfig` section, containing a list of assemblies to search,
and the `MigrationAssembly` name for EF Migrations. For example:

``` json
  "AutoConfig": {
    "Assemblies": [
      "Infra"
    ],
    "MigrationAssembly":"WebApp"
  }
```
  
## EF Migrations

When working with EF Core migrations you need to specify two things. First of all the name
of the `MigrationsAssembly`, which can be found in configuration. But you also need to specify 
the connection string for each `DbContext`. 
If your **Infrastructure** project calls `AddDbContext<T>` how does it access configuration? 
The `AutoConfig` attribute has an overload, allowing you to specify the
name of the connection string. The `AddAutoConfig` method will then lookup the value of the 
connection string and pass it to the method as the `connectionString` argument.

``` csharp
[AutoConfig("GamesDb")]
public static IServiceCollection AddGamesDb(this IServiceCollection services, string connectionString, [MigrationAssembly] string migrationAssembly)
  => services.AddDbContext<GamesDb>(optionsBuilder =>
       optionsBuilder.UseSqlServer(connectionString,
         sqlServerOptions => sqlServerOptions.MigrationsAssembly(migrationAssembly)));
```
