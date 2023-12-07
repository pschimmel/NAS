using System.Net;
using System.Threading.Tasks;
using NAS.Model;
using RestSharp;
using RestSharp.Serializers;
using RestSharp.Serializers.NewtonsoftJson;

namespace NAS.ViewModel.Helpers
{
  public class UpdateServiceHelper
  {
    #region Fields

    private RestClient client;
    private const string server = @"http://lic.gs-cms.com";
    private const string appName = Globals.ApplicationShortName;

    #endregion

    #region Public Methods

    public async Task<Version> GetLatestVersion()
    {
      var request = GetRequest("GetLatestVersion");
      _ = request.AddQueryParameter("applicationName", appName);
      var response = await client.ExecuteAsync(request);
      if (response.StatusCode == HttpStatusCode.OK)
      {
        string versionString = response.Content?.Trim('"');
        return Version.TryParse(versionString, out var remoteVersion)
          ? remoteVersion
          : throw new ApplicationException($"Cannot convert {response.Content} to a version.");
      }
      else if (response.StatusCode != 0)
      {
        throw new ApplicationException(response.StatusDescription);
      }
      else
      {
        throw new ApplicationException(response.ErrorMessage);
      }
    }

    public async Task<string> GetDownloadURL()
    {
      var request = GetRequest("GetDownloadPath");
      _ = request.AddQueryParameter("applicationName", appName);
      var response = await client.ExecuteAsync(request);
      if (response.StatusCode == HttpStatusCode.OK)
      {
        return response.Content;
      }
      else if (response.StatusCode != 0)
      {
        throw new ApplicationException(response.StatusDescription);
      }
      else
      {
        throw new ApplicationException(response.ErrorMessage);
      }
    }

    public async Task<IEnumerable<Change>> GetChanges(Version version)
    {
      var request = GetRequest("GetChanges");
      _ = request.AddQueryParameter("applicationName", appName);
      _ = request.AddQueryParameter("version", version.ToString());
      var response = await client.ExecuteAsync(request);

      if (response.StatusCode == HttpStatusCode.OK)
      {
        return client.Deserialize<List<Change>>(response).Data;
      }
      else if (response.StatusCode != 0)
      {
        throw new ApplicationException(response.StatusDescription);
      }
      else
      {
        throw new ApplicationException(response.ErrorMessage);
      }
    }

    #endregion

    #region Private Methods

    private RestRequest GetRequest(string resource)
    {
      client = CreateClient(server, "/api/ApplicationInfoAPI");
      var request = new RestRequest(resource, Method.Get)
      {
        RequestFormat = DataFormat.Json
      };
      return request;
    }

    private static RestClient CreateClient(string server, string baseUrl = null)
    {
      string url = server;
      if (!string.IsNullOrEmpty(baseUrl))
      {
        url += baseUrl + "/";
      }

      static void configure(SerializerConfig config) => config.UseNewtonsoftJson();
      var client = new RestClient(baseUrl: new Uri(url), configureSerialization: configure);

      //client.UseNewtonsoftJson();
      // Override with Newtonsoft JSON Handler

      //client.AddHandler("application/json", () => JsonNetSerializer.Default);
      //client.AddHandler("text/json", () => JsonNetSerializer.Default);
      //client.AddHandler("text/x-json", () => JsonNetSerializer.Default);
      //client.AddHandler("text/javascript", () => JsonNetSerializer.Default);
      //client.AddHandler("*+json", () => JsonNetSerializer.Default);
      return client;
    }

    #endregion
  }
}
