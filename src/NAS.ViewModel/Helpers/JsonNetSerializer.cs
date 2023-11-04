//using System.IO;
//using Newtonsoft.Json;
//using RestSharp;
//using RestSharp.Serializers;

//namespace NAS.ViewModel.Helpers
//{
//  public class JsonNetSerializer : ISerializer, IDeserializer
//  {
//    private readonly JsonSerializer serializer;

//    public JsonNetSerializer(JsonSerializer serializer)
//    {
//      this.serializer = serializer;
//    }

//    public string ContentType
//    {
//      get => "application/json";  // Probably used for Serialization?
//      set { }
//    }

//    public string DateFormat { get; set; }

//    public string Namespace { get; set; }

//    public string RootElement { get; set; }

//    public string Serialize(object obj)
//    {
//      using var stringWriter = new StringWriter();
//      using var jsonTextWriter = new JsonTextWriter(stringWriter);
//      serializer.Serialize(jsonTextWriter, obj);

//      return stringWriter.ToString();
//    }

//    public T Deserialize<T>(RestResponse response)
//    {
//      string content = response.Content;

//      using var stringReader = new StringReader(content);
//      using var jsonTextReader = new JsonTextReader(stringReader);
//      return serializer.Deserialize<T>(jsonTextReader);
//    }

//    public static JsonNetSerializer Default => new(new JsonSerializer
//    {
//      NullValueHandling = NullValueHandling.Ignore,
//      Formatting = Formatting.Indented
//    });
//  }
//}
