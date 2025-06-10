using System;
using System.Net.Http;
using System.Threading.Tasks;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;

namespace TenXCards.E2E.Tests.Performance;

[TestClass]
public class ApiPerformanceTests
{
    private static readonly HttpClient HttpClient = new();
    private const string BaseUrl = "https://localhost:5001/api";
    
    [TestMethod]
    public void GetCards_ShouldHandleLoad()
    {
        // Configure scenario
        var scenario = ScenarioBuilder
            .CreateScenario("Get Cards API", async context =>
            {
                // Create HTTP request step
                var request = Http.CreateRequest("GET", $"{BaseUrl}/cards")
                    .WithHeader("Content-Type", "application/json");
                
                // Execute request
                var response = await Http.Send(HttpClient, request);
                
                // Return OK if status code is 200
                return response.StatusCode == 200
                    ? Response.Ok(statusCode: 200)
                    : Response.Fail(statusCode: response.StatusCode);
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(
                Simulation.InjectPerSec(rate: 50, during: TimeSpan.FromSeconds(30))
            );

        // Define test name
        var testName = "Cards API load test";
        
        // Run the test
        NBomberRunner
            .RegisterScenarios(scenario)
            .WithReportFormats(ReportFormat.Txt)
            .WithReportFileName(testName)
            .WithReportFolder("./LoadTestResults")
            .Run();
    }
    
    [TestMethod]
    public void CreateCard_ShouldHandleLoad()
    {
        // Configure scenario
        var scenario = ScenarioBuilder
            .CreateScenario("Create Card API", async context =>
            {
                // Create payload - would typically come from test data
                var payload = @"{""title"": ""Performance Test Card"", ""content"": ""Test content""}";
                
                // Create HTTP request step
                var request = Http.CreateRequest("POST", $"{BaseUrl}/cards")
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(new StringContent(payload, System.Text.Encoding.UTF8, "application/json"));
                
                // Execute request
                var response = await Http.Send(HttpClient, request);
                
                // Return OK if status code is 201 (Created)
                return response.StatusCode == 201
                    ? Response.Ok(statusCode: 201)
                    : Response.Fail(statusCode: response.StatusCode);
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(
                Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromSeconds(20))
            );

        // Define test name
        var testName = "Create Cards API load test";
        
        // Run the test
        NBomberRunner
            .RegisterScenarios(scenario)
            .WithReportFormats(ReportFormat.Txt)
            .WithReportFileName(testName)
            .WithReportFolder("./LoadTestResults")
            .Run();
    }
}
