using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models.ODataErrors;

namespace MapiExplorer.Services
{
    public static class ODataErrorHelper
    {
        public static void LogODataError(ILogger logger, ODataError error)
        {
            if (error.Error != null)
            {
                logger.LogError($"Code: {error.Error.Code}; Target: {error.Error.Target}; Message: {error.Error.Message}");
                if (error.Error.Details != null)
                {
                    foreach (var details in error.Error.Details)
                    {
                        logger.LogError($"Details: Code: {details.Code}; Target: {details.Target}; Message: {details.Message}");
                    }
                }
            }
            logger.LogError(error.Message);
        }
    }
}