#region
using System;
using System.Net;
using System.Web;
#endregion
namespace ProcessDashboard
{
    public class DataSetLocationResolver
    {
        private const string LowerPrefixes = "=.-/!";

        public void ResolveFromToken(string token, out string serverUrl, out string datasetId)
        {
            // the token is designed to be easy for users to type, and to be resilient to typos
            // (such as improper capitalization, extraneous space characters, or confusion between
            // 0 and O). Unpack that token to produce a URL to the Google URL shortening service.
            var shortGoogleUrl = ToUrl(token);

            // contact Google and retrieve the long URL that this short URL maps to.
            var longUrl = LookupUrl(shortGoogleUrl);

            // extract relevant info from the long URL.
            ResolveFromUri(longUrl, out serverUrl, out datasetId);
        }

        public void ResolveFromUri(Uri uri, out string serverUrl, out string datasetId)
        {
            // parse the long URL, and extract relevant info from it.
            var p = HttpUtility.ParseQueryString(uri.Query);
            serverUrl = p.Get("serverUrl");
            datasetId = p.Get("datasetId");
            if (serverUrl == null || datasetId == null)
                throw new ArgumentException("Unrecognized token");
        }

        private string ToUrl(string token)
        {
            token = token.Trim().ToUpper().Replace('0', 'O').Replace('@', '0').Replace(',', '.');
            if (token.Length < 2 || token[0] != 'G')
                throw new ArgumentException("Unrecognized token scheme");

            var result = "http://goo.gl/";
            var numLower = 0;
            var literal = false;
            foreach (var c in token.Substring(1))
            {
                if (literal)
                {
                    result += c;
                    literal = false;
                }
                else if ('A' <= c && c <= 'Z')
                {
                    if (numLower > 0)
                    {
                        result += char.ToLower(c);
                        numLower--;
                    }
                    else
                    {
                        result += c;
                    }
                }
                else if ('0' <= c && c <= '9')
                {
                    result += c;
                }
                else if (c == '=')
                {
                    literal = true;
                }
                else if (numLower <= 0)
                {
                    numLower = LowerPrefixes.IndexOf(c);
                }
            }
            return result;
        }

        private Uri LookupUrl(string shortUrl)
        {
            try
            {
                var req = WebRequest.CreateHttp(shortUrl);
                req.Method = "HEAD";
                req.AllowAutoRedirect = false;
                var resp = (HttpWebResponse) req.GetResponse();
                return new Uri(resp.Headers["Location"]);
            }
            catch (Exception)
            {
                throw new ArgumentException("Unrecognized token");
            }
        }
    }
}