using System;
using System.Text.Json;

namespace EssentialFrame.Serialization.Interfaces;

public interface ISerializer
{
    T Deserialize<T>(string value);
   
    T Deserialize<T>(string value, JsonSerializerOptions options);
    
    T Deserialize<T>(string value, Type type) where T : class;
  
    T Deserialize<T>(string value, Type type, JsonSerializerOptions options) where T : class;
   
    object Deserialize(string value, Type type);
   
    object Deserialize(string value, Type type, JsonSerializerOptions options);
    
    string Serialize<T>(T value);
    
    string Serialize<T>(T value, JsonSerializerOptions options);
}