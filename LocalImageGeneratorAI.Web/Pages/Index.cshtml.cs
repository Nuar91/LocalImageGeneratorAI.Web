using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LocalImageGeneratorAI.Web.Services;

namespace LocalImageGeneratorAI.Web.Pages
{
    public class IndexModel : PageModel
    {

        [BindProperty]
        public string Prompt { get; set; } = string.Empty;

        public string? SubmittedPrompt { get; set; }

        public string? ErrorMessage { get; set; }


        private readonly ComfyUiService _comfyUiService;

        public IndexModel(ComfyUiService comfyUiService)
        {
            _comfyUiService = comfyUiService;
        }


        public void OnGet()
        {

        }

        public async Task OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Prompt))
            {
                ErrorMessage = "Please enter a prompt.";
                return;
            }

            SubmittedPrompt = Prompt;

            try
            {
                var workflow = await _comfyUiService.LoadWorkflowWithPromptAsync(Prompt);
                
                var comfyUiResponse = await _comfyUiService.SendWorkflowToComfyUiAsync(workflow);




                // "prompt_id" ist ein von ComfyUI definiertes Feld in der JSON-Antwort.
                // Es enthält die eindeutige Kennung für unseren Bildauftrag bei ComfyUI.
                var promptId = comfyUiResponse["prompt_id"]?.GetValue<string>();

                if (string.IsNullOrEmpty(promptId))
                {
                    ErrorMessage = "ComfyUI did not return a prompt ID.";
                    return;
                }

                var updatedPrompt = workflow["57:27"]?["inputs"]?["text"]?.GetValue<string>();

                if (updatedPrompt != Prompt)
                {
                    ErrorMessage = "The prompt was not updated correctly.";
                    return;
                }

                ErrorMessage = "Test successful: The workflow was loaded and the prompt was updated.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Workflow test failed: {ex.Message}";
            }
        }

    }
}
