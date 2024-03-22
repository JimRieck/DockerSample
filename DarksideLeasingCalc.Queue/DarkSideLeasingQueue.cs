using Darkside.LeasingCalc.Contracts.Requests;
using Darkside.LeasingCalc.Contracts.Response;
using Darkside.LeasingCalc.Core.Service;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DarksideLeasingCalc.Queue
{
    public class DarkSideLeasingQueue
    {
        private readonly ILogger<DarkSideLeasingQueue> _logger;
        private readonly ILeaseCalculatorService _leaseCalculatorService;

        public DarkSideLeasingQueue(ILogger<DarkSideLeasingQueue> logger, ILeaseCalculatorService leaseCalculatorService)
        {
            _logger = logger;
            _leaseCalculatorService = leaseCalculatorService;
        }

        [Function(nameof(DarkSideLeasingQueue))]
        public async Task<DailyMileageCalcResponse> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(DarkSideLeasingQueue));
            
            var outputs = new List<string>();
            var request = context.GetInput<DailyMileageCalcRequest>();
            logger.LogInformation($"Creating Quote for car number : {request.CarNumber}");
            // Replace request and input with values relevant for your Durable Functions Activity
            return await context.CallActivityAsync<DailyMileageCalcResponse>(nameof(ProcessQuote), request);
        }

        [Function(nameof(ProcessQuote))]
        public async Task<DailyMileageCalcResponse> ProcessQuote([ActivityTrigger] DailyMileageCalcRequest request, FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("ProcessQuote");
            logger.LogInformation($"Processing Quote: {request.CarNumber}.", request);
            var response = await _leaseCalculatorService.CalculateDailyMilage(request);

            return response;
        }

        [Function("DarkSideLeasingQueue_HttpStart")]
        public async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData httpRequest,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("DarkSideLeasingQueue_HttpStart");

            var httpRequestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
            var apiRequest = JsonConvert.DeserializeObject<DailyMileageCalcRequest>(httpRequestBody);

            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(DarkSideLeasingQueue), apiRequest);

            logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            return client.CreateCheckStatusResponse(httpRequest, instanceId);
        }
    }
}
