using MCPServerApp.Models;
using System.Text.Json.Serialization;

[JsonSerializable(typeof(List<Monkey>))]
sealed partial class MonkeyContext : JsonSerializerContext
{

}