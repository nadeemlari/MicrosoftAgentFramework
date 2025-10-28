using Microsoft.Agents.AI.Workflows;
using MicrosoftAgentFramework.Utilities;
using OpenAI;
using Shared;

// const string model = "openai/gpt-4.1-mini";
const string model = "deepseek/deepseek-v3.2-exp";
var client =
    AIChatClient.GetOpenAI(OpenAI_LLM_Providers.OpenRouter, model);
var legalAgent = client.CreateAIAgent(
    instructions: "You are a legal agent that need to evaluate if a text is legal (use max 200 chars)",
    name: "LegalAgent"
);
var spellCheckAgent = client.CreateAIAgent(
    instructions: "You are a spelling check expert (use max 200 chars)",
    name: "SpellCheckAgent"
);
var workflowAgent = await AgentWorkflowBuilder.BuildConcurrent([legalAgent, spellCheckAgent])
    .AsAgentAsync();

const string legalText = """
                         This Legal Disclaimer (“Agreement”) governs the ownership, maintenance, and care of domesticated ducks 
                         kept as personal pets. By acquiring or housing a duck, the Owner hereby acknowledges and agrees to 
                         comply with all applicable municipal and federal regulations concerning the keeping of live poultry. 
                         The Owner affirms responsibility for providing humane living conditions, including adequate shelter, 
                         food, and access to clean water. Ducks must not be subjected to neglect, cruelty, or abandonment.
                         The Owner shall maintain sanitary standards to prevent odors, noise disturbance, or the spread of 
                         disease to neighboring properties. Local authorities reserve the right to inspect premises upon 
                         reasonable notice to ensure compliance. Any sale or transfer of pet ducks must include written 
                         documentation verifying the animal’s health status and vaccination records where required.
                         This Agreement does not confer any breeding or commercial rights unless expressly authorized in 
                         writing by the relevant agency. The Owner indemnifies and holds harmless all regulatory bodies 
                         against clams arising from damage or injury caused by said animals. Failure to adhere to the 
                         provisions herein may result in fines, forfeiture, or legal action.
                         Acceptance of a duck as a pet constitutes full consent to these terms and any subsequent 
                         amendants or revisions adopted by the governin authority.
                         """;
DisplayUtil.LoadingTask();
var response = await workflowAgent.RunAsync(legalText);
DisplayUtil.StopLoading();
DisplayUtil.Separator();
DisplayUtil.WriteLineInformation("Legal Agent Response:");
DisplayUtil.WriteLineSuccess(response.Text);