using System;
using System.Linq;

namespace SchwabenCode.ReCaptchaClient.ManualConsoleTest
{
    public class Program
    {
        public static void Main( string[ ] args )
        {
            ReCaptchaClient client = new ReCaptchaClient( "-YOUR_SECRET_HERE-" );
            var result = client.ValidateAsync( "g-recaptcha-response from FORM DATA here", "users IP / hostname here" ).Result;

            Console.WriteLine( $"Success: {result.Success}" );
            Console.WriteLine( $"Errors: {result.Errors.Count()}" );
            Console.WriteLine( $"Timestamp: {result.Timestamp}" );
            Console.WriteLine( $"Raw: {result.Raw}" );

            Console.ReadKey();
        }
    }
}
