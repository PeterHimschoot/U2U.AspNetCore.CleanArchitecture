using System;
using System.Reflection;

namespace U2U.CleanArchitecture
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
  public class AutoConfigAttribute : Attribute { 
    public string Key {get;}
    
    public AutoConfigAttribute(string key = null) => this.Key = key;
    
  }
}
