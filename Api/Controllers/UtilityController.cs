using Api.Application.Documents.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;
using Api.Application.Revisions.Queries;
using System.Net.Http.Headers;
using System.Text.Json;
using Api.Application.Utility.Queries;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilityController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UtilityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET /api/usage
        [HttpGet("usage")]
        public async Task<IActionResult> GetUsage()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Ok(new { data = new object[0] });

            var result = await _mediator.Send(new GetDocumentStorageUsageQuery(userId));
            return Ok(new { data = result });
        }

        // GET /api/revalidate
        [HttpGet("revalidate")]
        public async Task<IActionResult> Revalidate([FromQuery] string? path, [FromQuery] string? tag)
        {
            var result = await _mediator.Send(new RevalidateCacheQuery(path, tag));
            return Ok(new {
                revalidated = result.Revalidated,
                now = result.Now,
                message = result.Message,
                value = result.Value
            });
        }

        // GET /api/og
        [HttpGet("og")]
        public IActionResult GetOg([FromQuery] string? metadata)
        {
            string title = "";
            string subtitle = "";
            if (!string.IsNullOrEmpty(metadata))
            {
                try
                {
                    var doc = System.Text.Json.JsonDocument.Parse(metadata);
                    if (doc.RootElement.TryGetProperty("title", out var t))
                        title = t.GetString() ?? "";
                    if (doc.RootElement.TryGetProperty("subtitle", out var s))
                        subtitle = s.GetString() ?? "";
                }
                catch { /* ignore parse errors, use defaults */ }
            }
            var svg = $@"<svg width='600' height='315' xmlns='http://www.w3.org/2000/svg'>
  <rect width='600' height='315' fill='white'/>
  <text x='50%' y='40%' dominant-baseline='middle' text-anchor='middle' font-size='40' fill='#222'>{System.Net.WebUtility.HtmlEncode(title)}</text>
  <text x='50%' y='60%' dominant-baseline='middle' text-anchor='middle' font-size='24' fill='#666'>{System.Net.WebUtility.HtmlEncode(subtitle)}</text>
</svg>";
            return File(Encoding.UTF8.GetBytes(svg), "image/svg+xml");
        }

        // GET /api/pdf/{url}
        [HttpGet("pdf/{url}")]
        public async Task<IActionResult> GetPdf(string url)
        {
            // Validate and sanitize the URL (basic check)
            if (string.IsNullOrWhiteSpace(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                return BadRequest(new { error = "Invalid URL provided." });
            }
            try
            {
                // Download Chromium if not already present (PuppeteerSharp v20+)
                var fetcher = new BrowserFetcher();
                await fetcher.DownloadAsync();
                using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
                using var page = await browser.NewPageAsync();
                await page.GoToAsync(url, WaitUntilNavigation.Networkidle0);
                var pdfBytes = await page.PdfDataAsync(new PdfOptions
                {
                    Format = PuppeteerSharp.Media.PaperFormat.A4,
                    PrintBackground = true
                });
                return File(pdfBytes, "application/pdf", "document.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "PDF generation failed", details = ex.Message });
            }
        }

        // GET /api/docx/{id}
        [HttpGet("docx/{id}")]
        public async Task<IActionResult> GetDocx(string id, [FromQuery] string? v)
        {
            // Try to parse the revision id
            if (!Guid.TryParse(id, out var revisionId))
                return BadRequest(new { error = "Invalid revision id." });

            // Fetch the revision entity
            var revision = await _mediator.Send(new GetRevisionByIdQuery(revisionId));
            if (revision == null)
                return NotFound(new { error = "Revision not found." });

            // For demo: extract plain text from revision.Data (assume it's JSON with a 'content' property)
            string content = "";
            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(revision.Data);
                if (doc.RootElement.TryGetProperty("content", out var c))
                    content = c.GetString() ?? "";
            }
            catch { content = revision.Data; }

            // Generate DOCX in memory
            using var ms = new MemoryStream();
            using (var wordDoc = WordprocessingDocument.Create(ms, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
            {
                var mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new Document();
                var body = new Body();
                body.Append(new Paragraph(new Run(new Text(content))));
                mainPart.Document.Append(body);
            }
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"{id}.docx");
        }

        
        [HttpGet("completion")]
        public async Task<IActionResult> GetCompletion([FromQuery] string? prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return BadRequest(new { error = "Prompt is required." });

            // Call local Ollama server (llama3.2 model)
            var ollamaUrl = "http://localhost:11434/api/generate";
            var requestBody = new
            {
                model = "llama3.2", // Updated to llama3.2 as requested
                prompt = prompt,
                stream = false,
                options = new { num_predict = 128 }
            };
            using var httpClient = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(ollamaUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { error });
            }
            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            var completion = doc.RootElement.GetProperty("response").GetString()?.Trim() ?? "";
            return Ok(new { completion });
        }
    }
}