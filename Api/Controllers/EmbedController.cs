using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace Api.Controllers
{
    [ApiController]
    [Route("embed")]
    public class EmbedController : ControllerBase
    {
        // POST /embed/html
        [HttpPost("html")]
        public IActionResult RenderHtml([FromBody] JsonElement editorState)
        {
            string html = RenderNode(editorState);
            return Content(html, "text/html");
        }

        // Recursively render the editor state to HTML
        private string RenderNode(JsonElement node)
        {
            if (!node.TryGetProperty("type", out var typeProp))
                return string.Empty;
            string type = typeProp.GetString() ?? "";
            switch (type)
            {
                case "root":
                    if (node.TryGetProperty("children", out var children) && children.ValueKind == JsonValueKind.Array)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var child in children.EnumerateArray())
                        {
                            sb.Append(RenderNode(child));
                        }
                        return sb.ToString();
                    }
                    return string.Empty;
                case "paragraph":
                    return $"<p>{RenderChildren(node)}</p>";
                case "heading":
                    string tagStr = node.TryGetProperty("tag", out var tag) && tag.ValueKind == JsonValueKind.String ? tag.GetString() ?? string.Empty : string.Empty;
                    int level = 1;
                    if (!string.IsNullOrEmpty(tagStr) && tagStr.StartsWith("h") && tagStr.Length > 1 && int.TryParse(tagStr.Substring(1), out var l))
                    {
                        level = l;
                    }
                    return $"<h{level}>{RenderChildren(node)}</h{level}>";
                case "text":
                    return node.TryGetProperty("text", out var text) ? System.Net.WebUtility.HtmlEncode(text.GetString() ?? "") : "";
                case "math":
                    return node.TryGetProperty("math", out var math) ? $"<span class='math'>{System.Net.WebUtility.HtmlEncode(math.GetString() ?? "")}</span>" : "";
                // Add more node types as needed
                default:
                    return RenderChildren(node);
            }
        }

        private string RenderChildren(JsonElement node)
        {
            if (node.TryGetProperty("children", out var children) && children.ValueKind == JsonValueKind.Array)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var child in children.EnumerateArray())
                {
                    sb.Append(RenderNode(child));
                }
                return sb.ToString();
            }
            return string.Empty;
        }
    }
}
