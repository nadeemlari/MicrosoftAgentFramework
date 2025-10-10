using Microsoft.Extensions.AI;

namespace MicrosoftAgentFramework.Utilities.Extensions;

public static class UsageDetailsExtensions
{
    private const string ReasonTokenCountKey = "OutputTokenDetails.ReasoningTokenCount";

    public static long? GetOutputTokensUsedForReasoning(this UsageDetails? usageDetails)
    {
        if (usageDetails?.AdditionalCounts?.TryGetValue(ReasonTokenCountKey, out var tokenCount) ?? false)
        {
            return tokenCount;
        }

        return null;
    }
}