using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Linq;

[McpServerToolType]
public static class EchoTool
{
    [McpServerTool, Description("Echoes the message back to the client.")]
    public static string Echo(string message) => $"Hello from C#: {message}";

    [McpServerTool, Description("Echoes in reverse the message sent by the client.")]
    public static string ReverseEcho(string message) => new string(message.Reverse().ToArray());

    [McpServerTool, Description("Echoes in reverse the message sent by the client.")]
    public static string Yell(string message) => message.ToUpper();

    [McpServerTool, Description("Counts the number of words in the message.")]
    public static int WordCount(string message) =>
        string.IsNullOrWhiteSpace(message) ? 0 : message.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

    [McpServerTool, Description("Repeats the message N times, separated by spaces.")]
    public static string Repeat(string message, int times) =>
        times <= 0 ? "" : string.Join(' ', Enumerable.Repeat(message, times));

    [McpServerTool, Description("Checks if the message is a palindrome (ignores case and spaces).")]
    public static bool IsPalindrome(string message)
    {
        var cleaned = new string(message.Where(char.IsLetterOrDigit).Select(char.ToLower).ToArray());
        return cleaned.SequenceEqual(cleaned.Reverse());
    }

    [McpServerTool, Description("Converts the message to Title Case.")]
    public static string TitleCase(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return message;
        return string.Join(' ', message
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
    }

    [McpServerTool, Description("Counts the number of characters in the message (excluding spaces).")]
    public static int CharCount(string message) =>
        string.IsNullOrEmpty(message) ? 0 : message.Count(c => !char.IsWhiteSpace(c));

    [McpServerTool, Description("Extracts all digits from the message as a string.")]
    public static string ExtractDigits(string message) =>
        new string(message.Where(char.IsDigit).ToArray());

    [McpServerTool, Description("Finds the longest word in the message.")]
    public static string LongestWord(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return "";
        return message
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .OrderByDescending(w => w.Length)
            .FirstOrDefault() ?? "";
    }

    [McpServerTool, Description("Counts the number of vowels in the message.")]
    public static int VowelCount(string message)
    {
        if (string.IsNullOrEmpty(message)) return 0;
        var vowels = "aeiouAEIOU";
        return message.Count(c => vowels.Contains(c));
    }

    [McpServerTool, Description("Replaces all occurrences of a substring with another substring.")]
    public static string Replace(string message, string oldValue, string newValue)
    {
        if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(oldValue)) return message;
        return message.Replace(oldValue, newValue);
    }

    [McpServerTool, Description("Converts the message to lowercase.")]
    public static string ToLowerCase(string message)
    {
        return string.IsNullOrEmpty(message) ? message : message.ToLower();
    }

    [McpServerTool, Description("Converts the message to uppercase.")]
    public static string ToUpperCase(string message)
    {
        return string.IsNullOrEmpty(message) ? message : message.ToUpper();
    }

    [McpServerTool, Description("Trims leading and trailing whitespace from the message.")]
    public static string Trim(string message)
    {
        return string.IsNullOrEmpty(message) ? message : message.Trim();
    }

    [McpServerTool, Description("Checks if the message contains a specific substring.")]
    public static bool Contains(string message, string substring)
    {
        if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(substring)) return false;
        return message.Contains(substring, StringComparison.OrdinalIgnoreCase);
    }

    [McpServerTool, Description("Splits the message into an array of words.")]
    public static string[] SplitIntoWords(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return Array.Empty<string>();
        return message.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    }

    [McpServerTool, Description("Joins an array of words into a single string with spaces.")]
    public static string JoinWords(string[] words)
    {
        if (words == null || words.Length == 0) return "";
        return string.Join(' ', words);
    }

    [McpServerTool(Name = "SummarizeContentFromUrl"), Description("Summarizes content downloaded from a specific URI")]
    public static async Task<string> SummarizeDownloadedContent(
        IMcpServer thisServer,
        HttpClient httpClient,
    [Description("The url from which to download the content to summarize")] string url,
    CancellationToken cancellationToken)
    {
        string content = await httpClient.GetStringAsync(url);

        ChatMessage[] messages =
        [
            new(ChatRole.User, "Briefly summarize the following downloaded content:"),
        new(ChatRole.User, content),
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 256,
            Temperature = 0.3f,
        };        return $"Summary: {await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken)}";
    }

    [McpServerTool(Name = "AnalyzeSentiment"), Description("Analyzes the sentiment of the provided text using AI")]
    public static async Task<string> AnalyzeSentiment(
        IMcpServer thisServer,
        [Description("The text to analyze for sentiment")] string text,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "No text provided for sentiment analysis.";

        ChatMessage[] messages =
        [
            new(ChatRole.System, "You are a sentiment analysis expert. Analyze the sentiment of the following text and provide a brief response indicating whether it's positive, negative, or neutral, along with a confidence score (0-100) and a brief explanation."),
            new(ChatRole.User, $"Analyze the sentiment of this text: \"{text}\"")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 150,
            Temperature = 0.2f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"Sentiment Analysis: {response}";
    }

    [McpServerTool(Name = "GenerateRhyme"), Description("Generates words that rhyme with the provided word using AI")]
    public static async Task<string> GenerateRhyme(
        IMcpServer thisServer,
        [Description("The word to find rhymes for")] string word,
        [Description("Number of rhymes to generate (default: 5)")] int count = 5,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(word))
            return "No word provided for rhyme generation.";

        if (count <= 0 || count > 20)
            count = 5;

        ChatMessage[] messages =
        [
            new(ChatRole.System, "You are a creative writing assistant specializing in wordplay and rhymes. Generate real English words that rhyme with the given word."),
            new(ChatRole.User, $"Generate {count} words that rhyme with '{word}'. Return only the rhyming words, separated by commas.")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 100,
            Temperature = 0.7f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"Rhymes for '{word}': {response}";
    }

    [McpServerTool(Name = "ExplainConcept"), Description("Provides a simple explanation of a concept or term using AI")]
    public static async Task<string> ExplainConcept(
        IMcpServer thisServer,
        [Description("The concept, term, or topic to explain")] string concept,
        [Description("Target audience level: 'child', 'teen', 'adult', 'expert' (default: 'adult')")] string audienceLevel = "adult",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(concept))
            return "No concept provided for explanation.";

        string systemPrompt = audienceLevel.ToLower() switch
        {
            "child" => "You are an educational assistant who explains concepts in simple terms that a child (ages 5-10) can understand. Use simple words and relatable examples.",
            "teen" => "You are an educational assistant who explains concepts for teenagers (ages 13-18). Use clear language with some technical terms when appropriate.",
            "expert" => "You are an expert educator who provides detailed, technical explanations for professionals and advanced learners.",
            _ => "You are an educational assistant who explains concepts clearly for general adult audiences. Use accessible language while being comprehensive."
        };

        ChatMessage[] messages =
        [
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, $"Explain the concept of '{concept}' in 2-3 sentences.")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 200,
            Temperature = 0.3f,
        };
        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"Explanation of '{concept}' (for {audienceLevel} audience): {response}";
    }

    [McpServerTool(Name = "TranslateText"), Description("Translates text from one language to another using AI")]
    public static async Task<string> TranslateText(
        IMcpServer thisServer,
        [Description("The text to translate")] string text,
        [Description("Target language (e.g., 'Spanish', 'French', 'German')")] string targetLanguage,
        [Description("Source language (optional, AI will auto-detect if not specified)")] string sourceLanguage = "",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "No text provided for translation.";

        string prompt = string.IsNullOrWhiteSpace(sourceLanguage) 
            ? $"Translate the following text to {targetLanguage}: \"{text}\""
            : $"Translate the following text from {sourceLanguage} to {targetLanguage}: \"{text}\"";

        ChatMessage[] messages =
        [
            new(ChatRole.System, "You are a professional translator. Provide accurate translations while preserving the original meaning and tone."),
            new(ChatRole.User, prompt)
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 300,
            Temperature = 0.2f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"Translation to {targetLanguage}: {response}";
    }

    [McpServerTool(Name = "GeneratePoem"), Description("Generates a poem based on a given theme or topic")]
    public static async Task<string> GeneratePoem(
        IMcpServer thisServer,
        [Description("The theme or topic for the poem")] string theme,
        [Description("Style of poem: 'haiku', 'sonnet', 'free verse', 'limerick' (default: 'free verse')")] string style = "free verse",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(theme))
            return "No theme provided for poem generation.";

        string styleInstruction = style.ToLower() switch
        {
            "haiku" => "Write a traditional haiku (3 lines, 5-7-5 syllable pattern)",
            "sonnet" => "Write a Shakespearean sonnet (14 lines with ABAB CDCD EFEF GG rhyme scheme)",
            "limerick" => "Write a humorous limerick (5 lines with AABBA rhyme scheme)",
            _ => "Write a free verse poem (no specific rhyme or meter requirements)"
        };

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are a creative poet. {styleInstruction} based on the given theme."),
            new(ChatRole.User, $"Write a {style} poem about: {theme}")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 250,
            Temperature = 0.8f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{style.ToUpper()} - {theme}:\n{response}";
    }

    [McpServerTool(Name = "AnalyzeWritingTone"), Description("Analyzes the tone and style of written text")]
    public static async Task<string> AnalyzeWritingTone(
        IMcpServer thisServer,
        [Description("The text to analyze")] string text,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "No text provided for tone analysis.";

        ChatMessage[] messages =
        [
            new(ChatRole.System, "You are a writing expert. Analyze the tone, style, and emotional qualities of the given text. Identify elements like formality level, emotional tone, writing style, and target audience."),
            new(ChatRole.User, $"Analyze the tone and style of this text: \"{text}\"")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 200,
            Temperature = 0.3f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"Writing Tone Analysis: {response}";
    }

    [McpServerTool(Name = "GenerateStory"), Description("Generates a short story based on given parameters")]
    public static async Task<string> GenerateStory(
        IMcpServer thisServer,
        [Description("Main character or protagonist")] string character,
        [Description("Setting or location")] string setting,
        [Description("Genre: 'mystery', 'romance', 'sci-fi', 'fantasy', 'horror', 'comedy' (default: 'adventure')")] string genre = "adventure",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(character) || string.IsNullOrWhiteSpace(setting))
            return "Both character and setting must be provided for story generation.";

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are a creative storyteller specializing in {genre} stories. Write engaging, well-structured short stories with clear beginning, middle, and end."),
            new(ChatRole.User, $"Write a short {genre} story featuring {character} in {setting}. Keep it to about 150-200 words.")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 300,
            Temperature = 0.7f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{genre.ToUpper()} Story - {character} in {setting}:\n{response}";
    }

    [McpServerTool(Name = "ImproveWriting"), Description("Suggests improvements for written text")]
    public static async Task<string> ImproveWriting(
        IMcpServer thisServer,
        [Description("The text to improve")] string text,
        [Description("Focus area: 'grammar', 'clarity', 'style', 'conciseness', 'all' (default: 'all')")] string focus = "all",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "No text provided for improvement suggestions.";

        string focusInstruction = focus.ToLower() switch
        {
            "grammar" => "Focus primarily on grammar, punctuation, and syntax corrections.",
            "clarity" => "Focus on making the text clearer and easier to understand.",
            "style" => "Focus on improving writing style and flow.",
            "conciseness" => "Focus on making the text more concise and eliminating redundancy.",
            _ => "Provide comprehensive feedback on grammar, clarity, style, and conciseness."
        };

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are a professional editor and writing coach. {focusInstruction} Provide specific, actionable suggestions."),
            new(ChatRole.User, $"Please review and suggest improvements for this text: \"{text}\"")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 250,
            Temperature = 0.3f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"Writing Improvement Suggestions ({focus}): {response}";
    }

    [McpServerTool(Name = "GenerateJoke"), Description("Generates jokes based on a topic or style")]
    public static async Task<string> GenerateJoke(
        IMcpServer thisServer,
        [Description("Topic or subject for the joke")] string topic,
        [Description("Style: 'pun', 'one-liner', 'knock-knock', 'dad-joke', 'clean' (default: 'clean')")] string style = "clean",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(topic))
            return "No topic provided for joke generation.";

        string styleInstruction = style.ToLower() switch
        {
            "pun" => "Create a clever pun-based joke",
            "one-liner" => "Create a short, punchy one-liner joke",
            "knock-knock" => "Create a knock-knock joke format",
            "dad-joke" => "Create a wholesome, groan-worthy dad joke",
            _ => "Create a clean, family-friendly joke"
        };

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are a comedian specializing in clean, family-friendly humor. {styleInstruction} about the given topic."),
            new(ChatRole.User, $"Tell me a {style} joke about: {topic}")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 150,
            Temperature = 0.8f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{style.ToUpper()} Joke about {topic}: {response}";
    }

    [McpServerTool(Name = "CreateMetaphors"), Description("Generates metaphors and analogies for concepts")]
    public static async Task<string> CreateMetaphors(
        IMcpServer thisServer,
        [Description("The concept to create metaphors for")] string concept,
        [Description("Number of metaphors to generate (default: 3)")] int count = 3,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(concept))
            return "No concept provided for metaphor generation.";

        if (count <= 0 || count > 10)
            count = 3;

        ChatMessage[] messages =
        [
            new(ChatRole.System, "You are a creative writing expert skilled in crafting vivid metaphors and analogies. Create original, meaningful comparisons that help illuminate concepts."),
            new(ChatRole.User, $"Create {count} different metaphors or analogies to explain the concept of '{concept}'. Make them vivid and relatable.")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 200,
            Temperature = 0.7f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"Metaphors for '{concept}': {response}";
    }

    [McpServerTool(Name = "GenerateQuestions"), Description("Generates thought-provoking questions about a topic")]
    public static async Task<string> GenerateQuestions(
        IMcpServer thisServer,
        [Description("The topic to generate questions about")] string topic,
        [Description("Type: 'discussion', 'interview', 'research', 'critical-thinking' (default: 'discussion')")] string questionType = "discussion",
        [Description("Number of questions to generate (default: 5)")] int count = 5,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(topic))
            return "No topic provided for question generation.";

        if (count <= 0 || count > 15)
            count = 5;

        string typeInstruction = questionType.ToLower() switch
        {
            "interview" => "Create insightful interview questions that would elicit interesting responses",
            "research" => "Create research questions that could guide academic investigation",
            "critical-thinking" => "Create questions that promote deep analysis and critical thinking",
            _ => "Create engaging discussion questions that stimulate conversation"
        };

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are an expert at crafting meaningful questions. {typeInstruction} about the given topic."),
            new(ChatRole.User, $"Generate {count} {questionType} questions about: {topic}")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 250,
            Temperature = 0.6f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{questionType.ToUpper()} Questions about {topic}: {response}";
    }

    [McpServerTool(Name = "CreateAcronym"), Description("Creates memorable acronyms from phrases or generates phrases from acronyms")]
    public static async Task<string> CreateAcronym(
        IMcpServer thisServer,
        [Description("Input phrase to create acronym from, or existing acronym to expand")] string input,
        [Description("Mode: 'create' (phrase to acronym) or 'expand' (acronym to phrase)")] string mode = "create",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "No input provided for acronym creation/expansion.";

        string instruction = mode.ToLower() == "expand" 
            ? $"Create a meaningful phrase or expansion for the acronym '{input}'. Make it memorable and relevant."
            : $"Create a memorable acronym from this phrase: '{input}'. Extract the first letter of each significant word.";

        ChatMessage[] messages =
        [
            new(ChatRole.System, "You are skilled at creating memorable acronyms and meaningful phrase expansions. Focus on clarity and memorability."),
            new(ChatRole.User, instruction)
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 150,
            Temperature = 0.6f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"Acronym {mode}: {response}";
    }

    [McpServerTool(Name = "GenerateSlogan"), Description("Creates catchy slogans or taglines")]
    public static async Task<string> GenerateSlogan(
        IMcpServer thisServer,
        [Description("Product, service, or concept to create slogan for")] string subject,
        [Description("Tone: 'professional', 'playful', 'inspirational', 'urgent', 'friendly' (default: 'professional')")] string tone = "professional",
        [Description("Number of slogans to generate (default: 3)")] int count = 3,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(subject))
            return "No subject provided for slogan generation.";

        if (count <= 0 || count > 10)
            count = 3;

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are a creative marketing expert. Create catchy, memorable slogans with a {tone} tone. Keep them concise and impactful."),
            new(ChatRole.User, $"Create {count} {tone} slogans for: {subject}")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 150,
            Temperature = 0.8f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{tone.ToUpper()} Slogans for {subject}: {response}";
    }

    [McpServerTool(Name = "AnalyzeReadability"), Description("Analyzes text readability and suggests improvements")]
    public static async Task<string> AnalyzeReadability(
        IMcpServer thisServer,
        [Description("The text to analyze for readability")] string text,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "No text provided for readability analysis.";

        ChatMessage[] messages =
        [
            new(ChatRole.System, "You are a readability expert. Analyze text for clarity, sentence structure, vocabulary complexity, and overall accessibility. Provide specific suggestions for improvement."),
            new(ChatRole.User, $"Analyze the readability of this text and suggest improvements: \"{text}\"")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 250,
            Temperature = 0.3f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"Readability Analysis: {response}";
    }

    [McpServerTool(Name = "GenerateHashtags"), Description("Generates relevant hashtags for social media content")]
    public static async Task<string> GenerateHashtags(
        IMcpServer thisServer,
        [Description("Content or topic to generate hashtags for")] string content,
        [Description("Platform: 'twitter', 'instagram', 'linkedin', 'general' (default: 'general')")] string platform = "general",
        [Description("Number of hashtags to generate (default: 10)")] int count = 10,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(content))
            return "No content provided for hashtag generation.";

        if (count <= 0 || count > 30)
            count = 10;

        string platformGuidance = platform.ToLower() switch
        {
            "twitter" => "Focus on trending, concise hashtags suitable for Twitter",
            "instagram" => "Include a mix of popular and niche hashtags for Instagram discovery",
            "linkedin" => "Create professional hashtags appropriate for LinkedIn",
            _ => "Create versatile hashtags suitable for multiple platforms"
        };

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are a social media expert. {platformGuidance}. Generate relevant, searchable hashtags."),
            new(ChatRole.User, $"Generate {count} hashtags for this content on {platform}: \"{content}\"")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 200,
            Temperature = 0.6f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{platform.ToUpper()} Hashtags for content: {response}";
    }

    [McpServerTool(Name = "CreateAnalogyChain"), Description("Creates a chain of analogies to explain complex concepts")]
    public static async Task<string> CreateAnalogyChain(
        IMcpServer thisServer,
        [Description("Complex concept to explain through analogies")] string concept,
        [Description("Number of analogies in the chain (default: 3)")] int chainLength = 3,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(concept))
            return "No concept provided for analogy chain creation.";

        if (chainLength <= 0 || chainLength > 7)
            chainLength = 3;

        ChatMessage[] messages =
        [
            new(ChatRole.System, "You are an expert at explaining complex concepts through progressive analogies. Create a chain of analogies that build upon each other, starting simple and becoming more sophisticated."),
            new(ChatRole.User, $"Create a chain of {chainLength} analogies to explain '{concept}', starting with the simplest comparison and progressively building complexity.")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 300,
            Temperature = 0.7f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"Analogy Chain for '{concept}': {response}";
    }

    [McpServerTool(Name = "GenerateDebatePoints"), Description("Generates arguments for both sides of a debate topic")]
    public static async Task<string> GenerateDebatePoints(
        IMcpServer thisServer,
        [Description("The debate topic or statement")] string topic,
        [Description("Side: 'for', 'against', 'both' (default: 'both')")] string side = "both",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(topic))
            return "No topic provided for debate point generation.";

        string instruction = side.ToLower() switch
        {
            "for" => $"Generate strong arguments supporting this position: {topic}",
            "against" => $"Generate strong arguments opposing this position: {topic}",
            _ => $"Generate balanced arguments both for and against this topic: {topic}"
        };

        ChatMessage[] messages =
        [
            new(ChatRole.System, "You are a skilled debater who can present logical, well-reasoned arguments. Focus on factual points and logical reasoning."),
            new(ChatRole.User, instruction)
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 300,
            Temperature = 0.4f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"Debate Points ({side}) for '{topic}': {response}";
    }

    [McpServerTool(Name = "CreateWordAssociations"), Description("Generates word associations and semantic connections")]
    public static async Task<string> CreateWordAssociations(
        IMcpServer thisServer,
        [Description("The starting word for associations")] string word,
        [Description("Number of associations to generate (default: 8)")] int count = 8,
        [Description("Type: 'semantic', 'emotional', 'visual', 'conceptual' (default: 'semantic')")] string associationType = "semantic",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(word))
            return "No word provided for association generation.";

        if (count <= 0 || count > 20)
            count = 8;

        string typeInstruction = associationType.ToLower() switch
        {
            "emotional" => "Focus on emotional connections and feelings associated with the word",
            "visual" => "Focus on visual imagery and things you might see related to the word",
            "conceptual" => "Focus on abstract concepts and ideas related to the word",
            _ => "Focus on semantic relationships and meaning-based connections"
        };

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are an expert in linguistics and cognitive associations. {typeInstruction}."),
            new(ChatRole.User, $"Generate {count} {associationType} word associations for: '{word}'")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 150,
            Temperature = 0.7f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{associationType.ToUpper()} Associations for '{word}': {response}";
    }

    [McpServerTool(Name = "GenerateMotivationalQuote"), Description("Creates inspirational and motivational quotes")]
    public static async Task<string> GenerateMotivationalQuote(
        IMcpServer thisServer,
        [Description("Theme or area of life (e.g., 'success', 'perseverance', 'growth')")] string theme,
        [Description("Style: 'inspirational', 'philosophical', 'actionable', 'uplifting' (default: 'inspirational')")] string style = "inspirational",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(theme))
            return "No theme provided for motivational quote generation.";

        string styleInstruction = style.ToLower() switch
        {
            "philosophical" => "Create a deep, thought-provoking quote with philosophical insight",
            "actionable" => "Create a quote that motivates specific action and behavior",
            "uplifting" => "Create an uplifting quote that boosts mood and confidence",
            _ => "Create an inspirational quote that motivates and encourages"
        };

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are a wise motivational speaker. {styleInstruction} about the given theme. Make it memorable and impactful."),
            new(ChatRole.User, $"Create a {style} motivational quote about: {theme}")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 100,
            Temperature = 0.8f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{style.ToUpper()} Quote about {theme}: {response}";
    }

    [McpServerTool(Name = "AnalyzeArgumentStructure"), Description("Analyzes the logical structure of arguments in text")]
    public static async Task<string> AnalyzeArgumentStructure(
        IMcpServer thisServer,
        [Description("The argument or persuasive text to analyze")] string argument,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(argument))
            return "No argument provided for structural analysis.";

        ChatMessage[] messages =
        [
            new(ChatRole.System, "You are a logic and rhetoric expert. Analyze the structure of arguments, identifying premises, conclusions, logical connections, and potential fallacies or weaknesses."),
            new(ChatRole.User, $"Analyze the logical structure of this argument: \"{argument}\"")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 250,
            Temperature = 0.3f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"Argument Structure Analysis: {response}";
    }

    [McpServerTool(Name = "CreateMemoryDevice"), Description("Creates mnemonic devices and memory aids")]
    public static async Task<string> CreateMemoryDevice(
        IMcpServer thisServer,
        [Description("Information or list to create memory device for")] string information,
        [Description("Type: 'acronym', 'rhyme', 'story', 'visualization' (default: 'acronym')")] string deviceType = "acronym",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(information))
            return "No information provided for memory device creation.";

        string typeInstruction = deviceType.ToLower() switch
        {
            "rhyme" => "Create a memorable rhyme or song to help remember the information",
            "story" => "Create a vivid story that incorporates all the elements to remember",
            "visualization" => "Create a visual imagery technique to remember the information",
            _ => "Create an acronym or mnemonic phrase to remember the information"
        };

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are an expert in memory techniques and learning strategies. {typeInstruction}."),
            new(ChatRole.User, $"Create a {deviceType} memory device for: {information}")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 200,
            Temperature = 0.7f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{deviceType.ToUpper()} Memory Device: {response}";
    }

    [McpServerTool(Name = "GenerateAlternatives"), Description("Generates alternative words, phrases, or approaches")]
    public static async Task<string> GenerateAlternatives(
        IMcpServer thisServer,
        [Description("The word, phrase, or concept to find alternatives for")] string input,
        [Description("Type: 'synonyms', 'phrases', 'approaches', 'solutions' (default: 'synonyms')")] string alternativeType = "synonyms",
        [Description("Number of alternatives to generate (default: 6)")] int count = 6,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "No input provided for alternative generation.";

        if (count <= 0 || count > 15)
            count = 6;

        string typeInstruction = alternativeType.ToLower() switch
        {
            "phrases" => "Generate alternative phrases or expressions with similar meaning",
            "approaches" => "Generate different approaches or methods for the given concept",
            "solutions" => "Generate alternative solutions or ways to address the given challenge",
            _ => "Generate synonymous words with similar meanings but different connotations"
        };

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are a linguistic expert skilled in finding alternatives. {typeInstruction}."),
            new(ChatRole.User, $"Generate {count} {alternativeType} for: '{input}'")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 180,
            Temperature = 0.6f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{alternativeType.ToUpper()} for '{input}': {response}";
    }

    [McpServerTool(Name = "CreateCharacterProfile"), Description("Generates detailed character profiles for creative writing")]
    public static async Task<string> CreateCharacterProfile(
        IMcpServer thisServer,
        [Description("Basic character description or name")] string character,
        [Description("Genre context: 'fantasy', 'sci-fi', 'contemporary', 'historical', 'mystery' (default: 'contemporary')")] string genre = "contemporary",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(character))
            return "No character information provided for profile creation.";

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are a creative writing expert specializing in {genre} character development. Create detailed, believable character profiles with personality, background, motivations, and distinctive traits."),
            new(ChatRole.User, $"Create a detailed character profile for {character} in a {genre} setting. Include personality traits, background, motivations, strengths, weaknesses, and distinctive characteristics.")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 350,
            Temperature = 0.7f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{genre.ToUpper()} Character Profile - {character}: {response}";
    }

    [McpServerTool(Name = "GenerateProductNames"), Description("Creates creative names for products or services")]
    public static async Task<string> GenerateProductNames(
        IMcpServer thisServer,
        [Description("Description of the product or service")] string productDescription,
        [Description("Style: 'professional', 'creative', 'technical', 'playful', 'premium' (default: 'professional')")] string namingStyle = "professional",
        [Description("Number of names to generate (default: 5)")] int count = 5,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productDescription))
            return "No product description provided for name generation.";

        if (count <= 0 || count > 15)
            count = 5;

        string styleInstruction = namingStyle.ToLower() switch
        {
            "creative" => "Create imaginative, unique names that stand out and are memorable",
            "technical" => "Create precise, descriptive names that clearly indicate function",
            "playful" => "Create fun, catchy names with wordplay or humor",
            "premium" => "Create sophisticated, luxury-sounding names that convey quality",
            _ => "Create professional, trustworthy names suitable for business use"
        };

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are a branding expert specializing in product naming. {styleInstruction}."),
            new(ChatRole.User, $"Generate {count} {namingStyle} names for this product: {productDescription}")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 150,
            Temperature = 0.8f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{namingStyle.ToUpper()} Product Names: {response}";
    }

    [McpServerTool(Name = "AnalyzeBiasInText"), Description("Identifies potential bias and loaded language in text")]
    public static async Task<string> AnalyzeBiasInText(
        IMcpServer thisServer,
        [Description("The text to analyze for bias")] string text,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "No text provided for bias analysis.";

        ChatMessage[] messages =
        [
            new(ChatRole.System, "You are an expert in media literacy and critical analysis. Identify potential bias, loaded language, assumptions, and subjective framing in text. Be objective and balanced in your analysis."),
            new(ChatRole.User, $"Analyze this text for potential bias, loaded language, or subjective framing: \"{text}\"")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 250,
            Temperature = 0.3f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"Bias Analysis: {response}";
    }

    [McpServerTool(Name = "CreateConversationStarters"), Description("Generates conversation starters for various contexts")]
    public static async Task<string> CreateConversationStarters(
        IMcpServer thisServer,
        [Description("Context or setting: 'networking', 'first date', 'dinner party', 'interview', 'casual' (default: 'casual')")] string context = "casual",
        [Description("Number of conversation starters to generate (default: 5)")] int count = 5,
        CancellationToken cancellationToken = default)
    {
        if (count <= 0 || count > 15)
            count = 5;

        string contextInstruction = context.ToLower() switch
        {
            "networking" => "Create professional conversation starters suitable for business networking events",
            "first date" => "Create engaging, getting-to-know-you questions for first dates",
            "dinner party" => "Create interesting topics that work well for group dinner conversations",
            "interview" => "Create thoughtful questions for job interviews or professional meetings",
            _ => "Create versatile conversation starters for everyday social situations"
        };

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are a social skills expert. {contextInstruction}. Make them engaging and appropriate for the setting."),
            new(ChatRole.User, $"Generate {count} conversation starters for {context} situations.")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 200,
            Temperature = 0.6f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{context.ToUpper()} Conversation Starters: {response}";
    }

    [McpServerTool(Name = "GenerateCreativePrompts"), Description("Creates writing or creative prompts for inspiration")]
    public static async Task<string> GenerateCreativePrompts(
        IMcpServer thisServer,
        [Description("Type: 'writing', 'art', 'photography', 'music', 'general' (default: 'writing')")] string promptType = "writing",
        [Description("Theme or genre (optional)")] string theme = "",
        [Description("Number of prompts to generate (default: 3)")] int count = 3,
        CancellationToken cancellationToken = default)
    {
        if (count <= 0 || count > 10)
            count = 3;

        string typeInstruction = promptType.ToLower() switch
        {
            "art" => "Create inspiring visual art prompts that spark creativity",
            "photography" => "Create photography challenges and creative shooting ideas",
            "music" => "Create musical composition or performance prompts",
            "general" => "Create general creative prompts suitable for any artistic medium",
            _ => "Create engaging writing prompts for stories, poems, or creative writing"
        };

        string themeClause = !string.IsNullOrWhiteSpace(theme) ? $" with a {theme} theme" : "";

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are a creativity coach. {typeInstruction}. Make them inspiring and thought-provoking."),
            new(ChatRole.User, $"Generate {count} {promptType} prompts{themeClause}.")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 250,
            Temperature = 0.8f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{promptType.ToUpper()} Creative Prompts{themeClause}: {response}";
    }

    [McpServerTool(Name = "SimulateDialogue"), Description("Creates realistic dialogue between characters")]
    public static async Task<string> SimulateDialogue(
        IMcpServer thisServer,
        [Description("Character 1 description")] string character1,
        [Description("Character 2 description")] string character2,
        [Description("Scenario or topic of conversation")] string scenario,
        [Description("Tone: 'friendly', 'tense', 'professional', 'romantic', 'argumentative' (default: 'friendly')")] string tone = "friendly",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(character1) || string.IsNullOrWhiteSpace(character2) || string.IsNullOrWhiteSpace(scenario))
            return "Character descriptions and scenario must be provided for dialogue simulation.";

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are a dialogue expert who creates realistic conversations. Write natural dialogue that reflects each character's personality and the {tone} tone of the situation."),
            new(ChatRole.User, $"Create a {tone} dialogue between {character1} and {character2} about: {scenario}. Show 4-6 exchanges between them.")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 350,
            Temperature = 0.7f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{tone.ToUpper()} Dialogue - {character1} & {character2}: {response}";
    }

    [McpServerTool(Name = "CreateDefinitions"), Description("Creates clear, accessible definitions for terms or concepts")]
    public static async Task<string> CreateDefinitions(
        IMcpServer thisServer,
        [Description("Term or concept to define")] string term,
        [Description("Style: 'simple', 'academic', 'technical', 'conversational' (default: 'simple')")] string definitionStyle = "simple",
        [Description("Include examples: true/false (default: true)")] bool includeExamples = true,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(term))
            return "No term provided for definition creation.";

        string styleInstruction = definitionStyle.ToLower() switch
        {
            "academic" => "Create a scholarly, precise definition suitable for academic contexts",
            "technical" => "Create a detailed technical definition with specific terminology",
            "conversational" => "Create a casual, easy-to-understand explanation",
            _ => "Create a clear, simple definition accessible to general audiences"
        };

        string exampleClause = includeExamples ? " Include relevant examples to illustrate the concept." : "";

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are an expert at creating clear definitions. {styleInstruction}.{exampleClause}"),
            new(ChatRole.User, $"Define the term '{term}' in a {definitionStyle} style.")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 200,
            Temperature = 0.3f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{definitionStyle.ToUpper()} Definition of '{term}': {response}";
    }

    [McpServerTool(Name = "GenerateIcebreakers"), Description("Creates icebreaker activities and questions for groups")]
    public static async Task<string> GenerateIcebreakers(
        IMcpServer thisServer,
        [Description("Group context: 'team meeting', 'workshop', 'social event', 'classroom', 'online meeting' (default: 'team meeting')")] string context = "team meeting",
        [Description("Group size: 'small' (3-8), 'medium' (9-20), 'large' (20+) (default: 'medium')")] string groupSize = "medium",
        [Description("Number of icebreakers to generate (default: 3)")] int count = 3,
        CancellationToken cancellationToken = default)
    {
        if (count <= 0 || count > 8)
            count = 3;

        string contextInstruction = context.ToLower() switch
        {
            "workshop" => "Create interactive icebreakers suitable for learning environments",
            "social event" => "Create fun, engaging icebreakers for social gatherings",
            "classroom" => "Create educational icebreakers that also facilitate learning",
            "online meeting" => "Create virtual-friendly icebreakers that work over video calls",
            _ => "Create professional icebreakers suitable for workplace team meetings"
        };

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are an expert facilitator. {contextInstruction} for {groupSize} groups. Make them engaging and appropriate for the setting."),
            new(ChatRole.User, $"Generate {count} icebreaker activities for a {groupSize} group in a {context} setting.")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 300,
            Temperature = 0.7f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{context.ToUpper()} Icebreakers ({groupSize} group): {response}";
    }

    [McpServerTool(Name = "AnalyzeEmotionalTone"), Description("Analyzes the emotional undertones and mood of text")]
    public static async Task<string> AnalyzeEmotionalTone(
        IMcpServer thisServer,
        [Description("The text to analyze for emotional tone")] string text,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "No text provided for emotional tone analysis.";

        ChatMessage[] messages =
        [
            new(ChatRole.System, "You are an expert in emotional intelligence and text analysis. Identify the emotional undertones, mood, and feelings conveyed in text. Consider both explicit emotions and subtle implications."),
            new(ChatRole.User, $"Analyze the emotional tone and mood of this text: \"{text}\"")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 200,
            Temperature = 0.3f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"Emotional Tone Analysis: {response}";
    }

    [McpServerTool(Name = "CreateUserPersonas"), Description("Generates detailed user personas for design and marketing")]
    public static async Task<string> CreateUserPersonas(
        IMcpServer thisServer,
        [Description("Product or service to create personas for")] string product,
        [Description("Target demographic or market segment")] string demographic,
        [Description("Number of personas to create (default: 2)")] int count = 2,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(product) || string.IsNullOrWhiteSpace(demographic))
            return "Both product description and demographic information required for persona creation.";

        if (count <= 0 || count > 5)
            count = 2;

        ChatMessage[] messages =
        [
            new(ChatRole.System, "You are a UX researcher and marketing expert. Create detailed, realistic user personas with demographics, goals, pain points, behaviors, and motivations."),
            new(ChatRole.User, $"Create {count} user personas for {product} targeting {demographic}. Include name, age, occupation, goals, challenges, and relevant characteristics.")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 400,
            Temperature = 0.6f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"User Personas for {product} ({demographic}): {response}";
    }

    [McpServerTool(Name = "GenerateEmailSubjects"), Description("Creates compelling email subject lines")]
    public static async Task<string> GenerateEmailSubjects(
        IMcpServer thisServer,
        [Description("Email content summary or purpose")] string emailPurpose,
        [Description("Tone: 'professional', 'urgent', 'friendly', 'promotional', 'informative' (default: 'professional')")] string tone = "professional",
        [Description("Number of subject lines to generate (default: 5)")] int count = 5,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(emailPurpose))
            return "No email purpose provided for subject line generation.";

        if (count <= 0 || count > 15)
            count = 5;

        string toneInstruction = tone.ToLower() switch
        {
            "urgent" => "Create compelling, action-oriented subject lines that convey urgency",
            "friendly" => "Create warm, approachable subject lines for casual communication",
            "promotional" => "Create attention-grabbing subject lines for marketing emails",
            "informative" => "Create clear, descriptive subject lines that set expectations",
            _ => "Create professional, clear subject lines suitable for business communication"
        };

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are an email marketing expert. {toneInstruction}. Keep them concise and compelling."),
            new(ChatRole.User, $"Generate {count} {tone} email subject lines for: {emailPurpose}")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 150,
            Temperature = 0.7f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{tone.ToUpper()} Email Subjects: {response}";
    }

    [McpServerTool(Name = "CreateTutorialOutline"), Description("Generates structured outlines for tutorials and how-to guides")]
    public static async Task<string> CreateTutorialOutline(
        IMcpServer thisServer,
        [Description("Topic or skill to create tutorial for")] string topic,
        [Description("Skill level: 'beginner', 'intermediate', 'advanced' (default: 'beginner')")] string skillLevel = "beginner",
        [Description("Format: 'video', 'written', 'interactive', 'workshop' (default: 'written')")] string format = "written",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(topic))
            return "No topic provided for tutorial outline creation.";

        string levelInstruction = skillLevel.ToLower() switch
        {
            "intermediate" => "Assume basic knowledge and focus on building more complex skills",
            "advanced" => "Create a comprehensive outline for experienced learners seeking mastery",
            _ => "Start with fundamentals and assume no prior knowledge"
        };

        string formatInstruction = format.ToLower() switch
        {
            "video" => "Structure for video presentation with clear segments and visual elements",
            "interactive" => "Include hands-on exercises and practice opportunities",
            "workshop" => "Design for group learning with activities and discussions",
            _ => "Structure for written step-by-step instructions"
        };

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are an instructional designer. {levelInstruction}. {formatInstruction}. Create clear, logical learning progressions."),
            new(ChatRole.User, $"Create a {skillLevel}-level {format} tutorial outline for: {topic}")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 300,
            Temperature = 0.4f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{skillLevel.ToUpper()} {format.ToUpper()} Tutorial Outline - {topic}: {response}";
    }

    [McpServerTool(Name = "GenerateTestimonials"), Description("Creates realistic testimonials and reviews")]
    public static async Task<string> GenerateTestimonials(
        IMcpServer thisServer,
        [Description("Product or service to create testimonials for")] string productService,
        [Description("Customer type or demographic")] string customerType,
        [Description("Tone: 'enthusiastic', 'professional', 'detailed', 'brief' (default: 'professional')")] string tone = "professional",
        [Description("Number of testimonials to generate (default: 3)")] int count = 3,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productService) || string.IsNullOrWhiteSpace(customerType))
            return "Both product/service and customer type must be provided for testimonial generation.";

        if (count <= 0 || count > 8)
            count = 3;

        string toneInstruction = tone.ToLower() switch
        {
            "enthusiastic" => "Create excited, highly positive testimonials with emotional language",
            "detailed" => "Create comprehensive testimonials with specific benefits and outcomes",
            "brief" => "Create concise, punchy testimonials that get straight to the point",
            _ => "Create credible, balanced testimonials with professional language"
        };

        ChatMessage[] messages =
        [
            new(ChatRole.System, $"You are a marketing copywriter. {toneInstruction}. Make them sound authentic and believable, mentioning specific benefits."),
            new(ChatRole.User, $"Generate {count} {tone} testimonials for {productService} from {customerType} customers.")
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 300,
            Temperature = 0.6f,
        };

        var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"{tone.ToUpper()} Testimonials for {productService}: {response}";
    }
}

