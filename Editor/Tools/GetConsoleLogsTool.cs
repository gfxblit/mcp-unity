using McpUnity.Unity;
using McpUnity.Services;
using Newtonsoft.Json.Linq;

namespace McpUnity.Tools
{
    /// <summary>
    /// Tool for retrieving logs from the Unity console
    /// </summary>
    public class GetConsoleLogsTool : McpToolBase
    {
        private readonly IConsoleLogsService _consoleLogsService;

        public GetConsoleLogsTool(IConsoleLogsService consoleLogsService)
        {
            Name = "get_console_logs";
            Description = "Retrieves logs from the Unity console with pagination support to avoid token limits";
            _consoleLogsService = consoleLogsService;
        }
        
        /// <summary>
        /// Execute the GetConsoleLogs tool with the provided parameters synchronously
        /// </summary>
        /// <param name="parameters">Tool parameters as a JObject</param>
        public override JObject Execute(JObject parameters)
        {
            try
            {
                // Extract parameters with defaults matching the Node.js side
                string logType = parameters["logType"]?.ToString();
                if (string.IsNullOrWhiteSpace(logType)) logType = null;
                
                int offset = GetIntParameter(parameters, "offset", 0);
                int limit = GetIntParameter(parameters, "limit", 50);
                bool includeStackTrace = GetBoolParameter(parameters, "includeStackTrace", true);
                
                // Ensure valid ranges
                offset = System.Math.Max(0, offset);
                limit = System.Math.Max(1, System.Math.Min(500, limit));

                // Use the console logs service to get the logs
                JObject result = _consoleLogsService.GetLogsAsJson(logType, offset, limit, includeStackTrace);
                
                // Add formatted message with pagination info
                string typeFilter = logType != null ? $" of type '{logType}'" : "";
                int returnedCount = result["_returnedCount"]?.Value<int>() ?? 0;
                int filteredCount = result["_filteredCount"]?.Value<int>() ?? 0;
                int totalCount = result["_totalCount"]?.Value<int>() ?? 0;
                
                result["message"] = $"Retrieved {returnedCount} of {filteredCount} log entries{typeFilter} (offset: {offset}, limit: {limit}, includeStackTrace: {includeStackTrace}, total: {totalCount})";
                result["success"] = true;
                
                // Remove internal count fields (they're now in the message)
                result.Remove("_totalCount");
                result.Remove("_filteredCount");
                result.Remove("_returnedCount");

                return result;
            }
            catch (System.Exception ex)
            {
                return McpUnitySocketHandler.CreateErrorResponse(
                    $"Failed to get console logs: {ex.Message}",
                    "tool_execution_error"
                );
            }
        }
        
        /// <summary>
        /// Helper method to safely extract integer parameters with default values
        /// </summary>
        /// <param name="parameters">JObject containing parameters</param>
        /// <param name="key">Parameter key to extract</param>
        /// <param name="defaultValue">Default value if parameter is missing or invalid</param>
        /// <returns>Extracted integer value or default</returns>
        private static int GetIntParameter(JObject parameters, string key, int defaultValue)
        {
            if (parameters?[key] != null && int.TryParse(parameters[key].ToString(), out int value))
                return value;
            return defaultValue;
        }

        /// <summary>
        /// Helper method to safely extract boolean parameters with default values
        /// </summary>
        /// <param name="parameters">JObject containing parameters</param>
        /// <param name="key">Parameter key to extract</param>
        /// <param name="defaultValue">Default value if parameter is missing or invalid</param>
        /// <returns>Extracted boolean value or default</returns>
        private static bool GetBoolParameter(JObject parameters, string key, bool defaultValue)
        {
            if (parameters?[key] != null && bool.TryParse(parameters[key].ToString(), out bool value))
                return value;
            return defaultValue;
        }
    }
}