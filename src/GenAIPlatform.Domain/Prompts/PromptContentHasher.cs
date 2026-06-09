using System.Security.Cryptography;
using System.Text;

namespace GenAIPlatform.Domain.Prompts;

internal static class PromptContentHasher
{
    public static string Compute(string systemMessage, string userMessageTemplate)
    {
        var content = string.Join(
            '\n',
            systemMessage,
            "---",
            userMessageTemplate);

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(content));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
