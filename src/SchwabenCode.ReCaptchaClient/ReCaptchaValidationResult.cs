using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SchwabenCode.ReCaptchaClient
{
    /// <summary>
    /// Values regarding the answer of https://developers.google.com/recaptcha/docs/verify#api-request
    /// </summary>
    public class ReCaptchaValidationResult
    {
        private string[ ] _errors;

        [JsonProperty( "success" )]
        public bool Success { get; internal set; }

        [JsonProperty( "challenge_ts" )]
        public DateTime Timestamp { get; internal set; }

        /// <summary>
        /// the hostname of the site where the reCAPTCHA was solved
        /// </summary>
        [JsonProperty( "hostname" )]
        public string Hostname { get; internal set; }

        [JsonProperty( "error-codes" )]
        public string[ ] Errors
        {
            get { return _errors ?? new string[ ] { }; }
            internal set { _errors = value; }
        }

        public String Raw { get; internal set; }
    }
}