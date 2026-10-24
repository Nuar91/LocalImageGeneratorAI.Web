using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LocalImageGeneratorAI.Web.Pages
{
    public class IndexModel : PageModel
    {

        [BindProperty]
        public string Prompt { get; set; } = string.Empty;

        public string? SubmittedPrompt { get; set; }


        public void OnGet()
        {

        }

        public void OnPost()
        { SubmittedPrompt = Prompt; }

    }
}
