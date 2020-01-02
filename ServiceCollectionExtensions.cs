using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

#nullable enable

namespace U2U.CleanArchitecture
{
  public static class ServiceCollectionExtensions
  {
    /// <summary>
    /// Call this method in your ConfigureServices method.
    /// </summary>
    /// <param name="services">The IServiceCollection instance</param>
    /// <param name="configuration">The IConfiguration instance</param>
    /// <returns>The IServiceCollection instance</returns>
    public static IServiceCollection AddAutoConfig(this IServiceCollection services, IConfiguration configuration)
    => services.AddAutoConfig(configuration.GetSection("AutoConfig").Get<AutoConfigOptions>(), configuration);

    public static IServiceCollection AddAutoConfig(this IServiceCollection services, AutoConfigOptions options, IConfiguration configuration)
    => services.AddAutoConfig(options, key => configuration.GetConnectionString(key));

    public static IServiceCollection AddAutoConfig(this IServiceCollection services, AutoConfigOptions options, Func<string, string> lookup)
    {
      if (options.Assemblies != null)
      {
        foreach (string assemblyName in options.Assemblies)
        {
          var asm = Assembly.Load(assemblyName);
          services.AddAutoConfig(options, asm, lookup);
        }
      }
      return services;
    }

    /// <summary>
    /// Scan the assembly, looking for methods with the AutoConfig attribute. 
    /// Call each method, passing in the IServiceCollection instance and optional key and migration assembly.
    /// </summary>
    /// <param name="services">The IServiceCollection instance</param>
    /// <param name="options">Configuration options</param>
    /// <param name="assembly">Assembly to scan</param>
    /// <param name="lookup">Lookup method, normally used to retrieve ICondfiguration setting</param>
    /// <returns></returns>
    public static IServiceCollection AddAutoConfig(this IServiceCollection services, AutoConfigOptions options, Assembly assembly, Func<string, string> lookup)
    {
      foreach (Type type in assembly.GetTypes().Where(t => t.IsDefined(typeof(AutoConfigAttribute))))
      {
        foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                                  .Where(m => m.IsDefined(typeof(AutoConfigAttribute))))
        {
          AutoConfigAttribute attr = method.GetCustomAttribute<AutoConfigAttribute>()!;
          if (string.IsNullOrEmpty(attr.Key))
          {
            method.Invoke(null, new object[] { services });
          }
          else
          {
            string key = lookup(attr.Key);
            method.Invoke(null, new object[] { services, key });
          }
        }
      }
      return services;
    }
  }
}
