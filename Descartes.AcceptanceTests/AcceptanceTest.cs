using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Xunit;

namespace Descartes.AcceptanceTests
{
    public class AcceptanceTest
    {
        private readonly AmazonLambdaClient _lambdaClient;

        public AcceptanceTest()
        {
            _lambdaClient = new AmazonLambdaClient(RegionEndpoint.USWest2);
        }

        [Fact]
        public async void CanInvokeLambda()
        {
            var testString = "\"lowercase\"";

            var result = await InvokeLambda(testString);
                    
            Assert.True(testString.ToUpper() == result, "Expected " + testString.ToUpper() + " but got " + result);
        }

        [Fact]
        public async void CanReverseString()
        {
            var testString = "\"reverse this string\"";

            var result = await InvokeLambda(testString);

            Assert.True("\"gnirts siht esrever\"" == result, "Expected " + "\"gnirts siht esrever\"" + " but got " + result);
        }

        [Fact]
        public async void CanLambada()
        {
            var testString = "\"lambada\"";

            var result = await InvokeLambda(testString);

            Assert.True("\"The Forbidden Dance.\"" == result, "Expected " + "\"The Forbidden Dance.\"" + " but got " + result);
        }

        private async Task<string> InvokeLambda(string payload)
        {
            var invokeRequest = new InvokeRequest
            {
                FunctionName = "DeveloperPlatform-DeployTest",
                InvocationType = InvocationType.RequestResponse,
                Payload = payload
            };

            var result = await _lambdaClient.InvokeAsync(invokeRequest);

            Assert.True(result.HttpStatusCode == HttpStatusCode.OK);

            if (result.Payload != null)
            {
                using (var ms = result.Payload)
                {
                    var sr = new StreamReader(ms);
                    return sr.ReadToEnd();
                }
            }

            return null;
        }
    }
}