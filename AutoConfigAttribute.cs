using System;

#nullable enable

namespace U2U.CleanArchitecture
{
  /// <summary>
  /// Apply this attribute to auto-detect DI methods.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
  public class AutoConfigAttribute : Attribute
  {
    /// <summary>
    /// The key to retrieve a connection string from configuration
    /// </summary>
    public string? Key { get; }

    public AutoConfigAttribute(string? key = null)
      => this.Key = key;
  }
}
