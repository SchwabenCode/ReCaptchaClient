using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SchwabenCode.ReCaptchaClient
{
    /// <summary>
    /// Client for Google's ReCaptcha
    /// </summary>
    public class ReCaptchaClient
    {
        private readonly string _secret;
        private readonly string _apiUrl;

        /// <summary>
        /// Creates an instance if <see cref="ReCaptchaClient"/>
        /// </summary>
        /// <param name="secret">Your secret from Google's Recaptcha Adminsite</param>
        /// <param name="apiUrl">Optional the API of Google here. Default IP is currently https://www.google.com/recaptcha/api/siteverify</param>
        public ReCaptchaClient( string secret, string apiUrl = "https://www.google.com/recaptcha/api/siteverify" )
        {
            if( secret == null ) throw new ArgumentNullException( nameof( secret ) );
            if( apiUrl == null ) throw new ArgumentNullException( nameof( apiUrl ) );

            _secret = secret;
            _apiUrl = apiUrl;
        }

        /// <summary>
        /// Validates a user request
        /// </summary>
        /// <param name="captchaResponse">The form value of 'g-recaptcha-response'</param>
        /// <param name="remoteIpAddress">IP Adress of the user to validate. It is optional.</param>
        /// <remarks>Each validation is isolated and thread-safe</remarks>
        /// <exception cref="HttpRequestException"> if request fauled</exception>
        public async Task<ReCaptchaValidationResult> ValidateAsync( string captchaResponse, string remoteIpAddress = "" )
        {
            if( string.IsNullOrEmpty( captchaResponse ) ) throw new ArgumentNullException( nameof( captchaResponse ) );

            HttpWebRequest httpWebRequest = await CreateRequest( captchaResponse, remoteIpAddress );
            HttpWebResponse response = ( await httpWebRequest.GetResponseAsync() ) as HttpWebResponse;

            if( response == null )
            {
                throw new HttpRequestException( "Request to the server failed. No answer." );
            }

            if( response.StatusCode == HttpStatusCode.OK )
            {
                return ConvertAnswer( response );
            }
            throw new HttpRequestException( $"HTTP Error Code: {response.StatusCode}: " + ( GetResponseAsString( response ) ) );
        }

        /// <summary>
        /// Create isolated webrequest
        /// </summary>
        private async Task<HttpWebRequest> CreateRequest( string captchaResponse, string remoteIpAddress )
        {
            // Create webrequest as post
            HttpWebRequest http = ( HttpWebRequest )WebRequest.Create( _apiUrl );
            http.Method = "POST";

            // Create and format postdata
            string postData = CreatePostData( captchaResponse, remoteIpAddress );
            byte[ ] data = Encoding.UTF8.GetBytes( postData );

            http.ContentType = "application/x-www-form-urlencoded";
            using( Stream requestStream = await http.GetRequestStreamAsync() )
            {
                requestStream.Write( data, 0, data.Length );
            }

            return http;
        }


        /// <summary>
        /// Creates the post content of https://developers.google.com/recaptcha/docs/verify
        /// </summary>
        protected string CreatePostData( string captchaResponse, string remoteIpAddress = "" )
        {
            return $"secret={_secret}&response={captchaResponse}&remote={remoteIpAddress}";
        }

        /// <summary>
        /// Reads the content from given response message
        /// </summary>
        public static string GetResponseAsString( HttpWebResponse responseMessage )
        {
            using( Stream stream = responseMessage.GetResponseStream() )
            using( StreamReader sr = new StreamReader( stream ) )
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Reads the json answer from Google and converts it into a type of <see cref="ReCaptchaValidationResult"/>
        /// </summary>
        private static ReCaptchaValidationResult ConvertAnswer( HttpWebResponse response )
        {
            string text = GetResponseAsString( response );
            ReCaptchaValidationResult validationResult = JsonConvert.DeserializeObject<ReCaptchaValidationResult>( text );
            validationResult.Raw = text;

            return validationResult;
        }

    }
}
