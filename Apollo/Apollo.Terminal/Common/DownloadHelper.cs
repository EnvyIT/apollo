using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Apollo.Util.Logger;
using RestSharp;

namespace Apollo.Terminal.Common
{
    public class DownloadHelper
    {
        #region Member
        private static IApolloLogger<DownloadHelper> Logger = LoggerFactory.CreateLogger<DownloadHelper>();
        #endregion

        #region Constructors
        private DownloadHelper() { } //to be able to use Logger and prevent creation of Instances
        #endregion


        #region StaticMethods
        public static async Task<string> FetchStreamUrl(string trailerUrl, string pattern)
        {
            if (string.IsNullOrWhiteSpace(trailerUrl))
            {
                return null;
            }

            var restClient = new RestClient(trailerUrl);
            var request = new RestRequest("#", Method.GET, DataFormat.Json);
            var jsonSite = await restClient.ExecuteAsync(request);
            var content = Regex.Match(jsonSite.Content, pattern, RegexOptions.IgnoreCase);
            if (content.Success)
            {
                Logger.Here().Info($"Fetched request URL successfully for trailer {trailerUrl}");
                return content.Groups[0].Value.Substring(0, content.Groups[0].Value.Length - 2);
            }

            return null;
        }
        #endregion
    }
}
