using LocalImageGeneratorAI.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json.Nodes;

namespace LocalImageGeneratorAI.Web.Pages
{
    public class IndexModel : PageModel
    {

        [BindProperty]
        public string Prompt { get; set; } = string.Empty;

        public string? SubmittedPrompt { get; set; }

        public string? PromptId { get; set; }

        public string? ErrorMessage { get; set; }

        public string? GeneratedImageUrl { get; set; }


        private readonly ComfyUiService _comfyUiService;

        public IndexModel(ComfyUiService comfyUiService)
        {
            _comfyUiService = comfyUiService;
        }


        public void OnGet()
        {

        }
        /*
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

                PromptId = promptId;


                var history = await _comfyUiService.GetHistoryAsync(promptId);

                var imageInfo = history[promptId]?["outputs"]?["9"]?["images"]?[0];

                if (imageInfo == null)
                {
                    ErrorMessage = "No generated image found in ComfyUI history.";
                    return;
                }

                string filename = imageInfo["filename"]!.GetValue<string>();
                string subfolder = imageInfo["subfolder"]!.GetValue<string>();
                string type = imageInfo["type"]!.GetValue<string>();

                byte[] imageBytes = await _comfyUiService.GetImageAsync(
                    filename,
                    subfolder,
                    type
                );

                GeneratedImageUrl = $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";

                ErrorMessage = "The current image was successfully retrieved!";

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

    
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Workflow test failed: {ex.Message}";
            }
        }*/



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

                var comfyUiResponse =
                    await _comfyUiService.SendWorkflowToComfyUiAsync(workflow);

                var promptId = comfyUiResponse["prompt_id"]?.GetValue<string>();

                if (string.IsNullOrEmpty(promptId))
                {
                    ErrorMessage = "ComfyUI did not return a prompt ID.";
                    return;
                }

                PromptId = promptId;

                // Wir warten, bis ComfyUI mit der Bildgenerierung fertig ist.
                JsonNode? history = null;

                for (int i = 0; i < 60; i++)
                {
                    history = await _comfyUiService.GetHistoryAsync(promptId);

                    bool completed =
                        history[promptId]?["status"]?["completed"]?.GetValue<bool>()
                        ?? false;

                    if (completed)
                    {
                        break;
                    }

                    await Task.Delay(1000);
                }

                bool generationCompleted =
                    history?[promptId]?["status"]?["completed"]?.GetValue<bool>()
                    ?? false;

                if (!generationCompleted)
                {
                    ErrorMessage = "Image generation timed out after 60 seconds.";
                    return;
                }

                var imageInfo =
                    history?[promptId]?["outputs"]?["9"]?["images"]?[0];

                if (imageInfo == null)
                {
                    ErrorMessage = "No generated image found in ComfyUI history.";
                    return;
                }

                string filename = imageInfo["filename"]!.GetValue<string>();
                string subfolder = imageInfo["subfolder"]!.GetValue<string>();
                string type = imageInfo["type"]!.GetValue<string>();

                byte[] imageBytes = await _comfyUiService.GetImageAsync(
                    filename,
                    subfolder,
                    type
                );

                GeneratedImageUrl =
                    $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";

                ErrorMessage = "The current image was successfully retrieved!";

                var updatedPrompt =
                    workflow["57:27"]?["inputs"]?["text"]?.GetValue<string>();

                if (updatedPrompt != Prompt)
                {
                    ErrorMessage = "The prompt was not updated correctly.";
                    GeneratedImageUrl = null;
                    return;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Workflow test failed: {ex.Message}";
            }
        }


    }
}
