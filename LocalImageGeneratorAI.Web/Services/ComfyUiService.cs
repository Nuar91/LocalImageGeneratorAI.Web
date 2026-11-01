
                               // Das brauchen wir, um den Prompt in der vorhandenen Workflow-Datei auszutauschen.
using Microsoft.AspNetCore.Hosting; //Microsoft.AspNetCore.Hosting stellt unter anderem IWebHostEnvironment bereit.
using System.Text;
using System.Text.Json.Nodes;  // System.Text.Json.Nodes ermöglicht uns, JSON als veränderbare Baumstruktur zu bearbeiten.
                                    //Darüber können wir den Pfad zum Projektverzeichnis ermitteln.

namespace LocalImageGeneratorAI.Web.Services
{
    public class ComfyUiService
    {
        private readonly IWebHostEnvironment _environment; //IWebHostEnvironment ermöglicht uns den Zugriff auf Umgebungsinformationen,
                                                           //wie z.B. den Pfad zum Projektverzeichnis.
        private readonly HttpClient _httpClient;
        public ComfyUiService(IWebHostEnvironment environment, HttpClient httpClient)
        {
            _environment = environment;
            _httpClient = httpClient;
        }

        public async Task<JsonNode> LoadWorkflowWithPromptAsync(string prompt)
        {
            string filePath = Path.Combine(       // Wir kombinieren den ContentRootPath (Projektverzeichnis) mit dem Dateinamen der Workflow-Datei.
                _environment.ContentRootPath,     // ContentRootPath ist der Pfad zum Projektverzeichnis.
                "LocalImageGenerator_API.json"
            );

            string json = await File.ReadAllTextAsync(filePath); //  

            JsonNode workflow = JsonNode.Parse(json)  // Wir wandeln den JSON-String in ein JsonNode-Objekt um.
                ?? throw new InvalidOperationException(  // ?? prüft, ob das Ergebnis null ist. Wenn ja, wird eine InvalidOperationException ausgelöst.
                    "The workflow JSON is empty."        // Fehlermeldung für diesen Fall.enthält.
                );

            workflow["57:27"]!["inputs"]!["text"] = prompt;

            return workflow;
        }

        public async Task<JsonNode> SendWorkflowToComfyUiAsync(JsonNode workflow)
        {
            JsonObject requestBody = new()
            {
                ["prompt"] = workflow
            };

            string json = requestBody.ToJsonString();

            using StringContent content = new(    /* verpackt unseren JSON-Text als HTTP-Request-Body. Dadurch kann HttpClient ihn als Inhalt einer HTTP-Anfrage an ComfyUI senden.

                                                        Zusätzlich legen wir fest:
                                                        - Encoding.UTF8: Zeichenkodierung.
                                                        - "application/json": Der Inhalt ist JSON.*/
                json,
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await _httpClient.PostAsync(   // Siehe Program.cs. Dort haben wir den HttpClient mit der BaseAddress "http://127.0.0.1:8188/" festgelegt.
                "prompt",                                                 // "prompt" ist der Endpunkt, an den wir die Anfrage senden. "prompt" ist als einer von vielen Endpunkten in ComfyUI definiert.
                content                                                   //  content enthält den JSON-Text, den wir obigen erstelllt haben. Er wird als Body der HTTP-Anfrage gesendet.   
            );




            string responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"ComfyUI returned {(int)response.StatusCode} ({response.StatusCode}): {responseJson}"
                );
            }
      

            // Hier habe ich mich gefragt: "Moment mal, warum verwenden wir hier await,
            // wenn wir den Inhalt doch bereits oben in response gespeichert haben?
            // Und wurde nicht auch schon alles durch response.EnsureSuccessStatusCode() sichergestellt?(Nein! Diese Methode prüft lediglich den HTTP-Statuscode.)"

            // Antwort:
            // Die Sache ist, dass das Auslesen des Antwortinhalts Zeit beanspruchen kann.
            // await sorgt dafür, dass der aktuelle Thread währenddessen nicht blockiert wird.

            // Was ist ein Thread?
            // Einfach erklärt: Ein Thread ist eine Art "Arbeitsstrang" innerhalb eines Programms,
            // auf dem Aufgaben ausgeführt werden.

            // await ermöglicht es uns, auf das Ergebnis einer asynchronen Operation zu warten,
            // ohne den aktuellen Thread währenddessen zu blockieren.
            // Dieser Thread kann in der Zwischenzeit andere Aufgaben übernehmen,
            // anstatt untätig auf das Ergebnis zu warten.

            // Das ist besonders wichtig in Webanwendungen, damit der Server mehrere Anfragen
            // effizient bearbeiten kann und nicht unnötig durch wartende Threads belastet wird.

            
            
            JsonNode result = JsonNode.Parse(responseJson)
                ?? throw new InvalidOperationException(
                    "ComfyUI returned an empty response."
                );

            return result;
        }
    }
}
