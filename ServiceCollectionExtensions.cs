using System;
namespace U2U.CleanArchitecture
{
  using System.Linq;
  using System.Reflection;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Configuration;

  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddAutoConfig(this IServiceCollection services, IConfiguration configuration)
    => services.AddAutoConfig( configuration.GetSection("AutoConfig").Get<AutoConfigOptions>(), configuration);
    
    public static IServiceCollection AddAutoConfig(this IServiceCollection services, AutoConfigOptions options, IConfiguration configuration)
    => services.AddAutoConfig(options, key => configuration.GetConnectionString(key));

    public static IServiceCollection AddAutoConfig(this IServiceCollection services, AutoConfigOptions options, Func<string, string> lookup)
    {
      foreach (var assemblyName in options.Assemblies)
      {
        Assembly asm = Assembly.Load(assemblyName);
        services.AddAutoConfig(options, asm, lookup);
      }
      return services;
    }

    public static IServiceCollection AddAutoConfig(this IServiceCollection services, AutoConfigOptions options, Assembly assembly, Func<string, string> lookup)
    {
      foreach (var type in assembly.GetTypes().Where(t => t.IsDefined(typeof(AutoConfigAttribute))))
      {
        foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                                  .Where(m => m.IsDefined(typeof(AutoConfigAttribute))))
        {
          var attr = method.GetCustomAttribute<AutoConfigAttribute>();
          if (string.IsNullOrEmpty(attr.Key))
          {
            method.Invoke(null, new object[] { services });
          }
          else
          {
            var key = lookup(attr.Key);
            method.Invoke(null, new object[] { services, key, options.MigrationAssembly });
          }
        }
      }
      return services;
    }
  }
}
