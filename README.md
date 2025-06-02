# MCPServerApp

A comprehensive Model Context Protocol (MCP) server application built with .NET 9, featuring 50+ AI-powered tools for text processing, creative writing, analysis, and business applications.

## 🚀 Features

- **50+ AI-Powered Tools**: Extensive collection of tools utilizing IChatClient for various tasks
- **Text Analysis**: Sentiment analysis, bias detection, readability assessment, and writing improvement
- **Creative Writing**: Story generation, poetry creation, character development, and dialogue simulation
- **Business Tools**: Email subject generation, testimonials, slogans, and product naming
- **Educational**: Concept explanations, tutorial outlines, debate points, and memory devices
- **Language Processing**: Translation, rhyme generation, word associations, and alternative suggestions
- **Communication**: Conversation starters, icebreakers, and interview questions

## 🛠️ Technology Stack

- **.NET 9**: Latest .NET framework
- **Model Context Protocol (MCP)**: Communication protocol for AI tools
- **Microsoft.Extensions.AI**: AI abstractions and chat client support
- **Docker**: Containerization support
- **C#**: Primary programming language

## 📋 Prerequisites

- .NET 9 SDK
- Docker (optional, for containerized deployment)
- An AI service provider (for IChatClient functionality)

## 🏃‍♂️ Quick Start

### 1. Clone and Build

```bash
git clone <repository-url>
cd MCPServerApp
dotnet build
```

### 2. Run the Application

```bash
dotnet run
```

### 3. Docker Deployment (Optional)

```bash
docker build -t mcpserverapp .
docker run -p 8080:8080 mcpserverapp
```

## 🔧 Configuration

The application uses standard .NET configuration patterns. You can configure:

- AI service endpoints
- Logging levels
- Server settings

Configuration can be provided through:
- `appsettings.json`
- Environment variables
- Command line arguments
- User secrets (for development)

## 📚 Available Tools

### Text Analysis & Processing
- **AnalyzeSentiment**: Analyzes emotional sentiment of text
- **AnalyzeWritingTone**: Evaluates tone and style of written content
- **AnalyzeReadability**: Assesses text readability and suggests improvements
- **AnalyzeBiasInText**: Identifies potential bias and loaded language
- **AnalyzeEmotionalTone**: Analyzes emotional undertones and mood
- **AnalyzeArgumentStructure**: Examines logical structure of arguments
- **ImproveWriting**: Provides writing enhancement suggestions
- **TranslateText**: Translates text between languages

### Creative Writing & Content
- **GeneratePoem**: Creates poetry in various styles (haiku, sonnet, free verse, limerick)
- **GenerateStory**: Generates short stories with specified parameters
- **CreateCharacterProfile**: Develops detailed character profiles for creative writing
- **SimulateDialogue**: Creates realistic conversations between characters
- **GenerateJoke**: Creates humor in different styles (pun, one-liner, knock-knock, dad-joke)
- **GenerateMotivationalQuote**: Creates inspirational quotes
- **GenerateCreativePrompts**: Provides creative inspiration for various mediums
- **CreateMetaphors**: Generates metaphors and analogies for concepts

### Business & Professional
- **GenerateSlogan**: Creates catchy slogans and taglines
- **GenerateProductNames**: Develops creative product and service names
- **GenerateEmailSubjects**: Creates compelling email subject lines
- **GenerateTestimonials**: Produces realistic customer testimonials
- **CreateUserPersonas**: Develops detailed user personas for design and marketing
- **GenerateHashtags**: Creates relevant social media hashtags

### Educational & Explanatory
- **ExplainConcept**: Provides explanations tailored to different audience levels
- **GenerateQuestions**: Creates thought-provoking questions for various contexts
- **CreateTutorialOutline**: Generates structured learning outlines
- **GenerateDebatePoints**: Develops arguments for debate topics
- **CreateMemoryDevice**: Creates mnemonic devices and memory aids
- **CreateAnalogyChain**: Builds progressive analogy chains for complex concepts

### Language & Communication
- **GenerateRhyme**: Finds words that rhyme with given input
- **CreateWordAssociations**: Generates semantic word connections
- **GenerateAlternatives**: Provides alternative words, phrases, or approaches
- **CreateConversationStarters**: Generates conversation starters for various contexts
- **GenerateIcebreakers**: Creates group icebreaker activities
- **CreateAcronym**: Creates memorable acronyms or expands existing ones

### Utility & Helper Tools
- **Echo**: Basic message echoing functionality
- **ReverseEcho**: Returns reversed text
- **Yell**: Converts text to uppercase
- **WordCount**: Counts words in text
- **CharCount**: Counts characters (excluding spaces)
- **VowelCount**: Counts vowels in text
- **Repeat**: Repeats message multiple times
- **IsPalindrome**: Checks if text is a palindrome
- **TitleCase**: Converts text to title case
- **ExtractDigits**: Extracts numerical digits from text
- **LongestWord**: Finds the longest word in text
- **Replace**: Replaces substrings within text
- **ToLowerCase**: Converts to lowercase
- **ToUpperCase**: Converts to uppercase
- **Trim**: Removes leading/trailing whitespace
- **Contains**: Checks for substring presence
- **SplitIntoWords**: Splits text into word arrays
- **JoinWords**: Joins word arrays into strings
- **SummarizeContentFromUrl**: Downloads and summarizes web content

### Data & Domain-Specific
- **GetMonkey**: Retrieves monkey information by name
- **GetMonkeys**: Lists available monkeys

## 🎯 Tool Categories Overview

| Category | Tool Count | Primary Use Cases |
|----------|------------|-------------------|
| Text Analysis | 8 | Content evaluation, improvement suggestions, bias detection |
| Creative Writing | 12 | Story creation, poetry, character development, humor |
| Business Tools | 6 | Marketing copy, branding, customer content |
| Educational | 6 | Learning materials, explanations, memory aids |
| Language Processing | 8 | Translation, word play, semantic analysis |
| Communication | 6 | Social interaction, professional networking |
| Utility Functions | 18 | Text manipulation, basic processing |
| Domain-Specific | 2 | Specialized data retrieval |

## 🔍 Tool Usage Examples

### Sentiment Analysis
```csharp
var result = await AnalyzeSentiment(server, "I absolutely love this new feature!", cancellationToken);
// Returns: Sentiment Analysis: Positive (95% confidence) - Expresses strong enthusiasm...
```

### Creative Writing
```csharp
var poem = await GeneratePoem(server, "autumn leaves", "haiku", cancellationToken);
// Returns: HAIKU - autumn leaves: Golden leaves falling...
```

### Business Content
```csharp
var slogans = await GenerateSlogan(server, "eco-friendly water bottles", "inspirational", 3, cancellationToken);
// Returns: INSPIRATIONAL Slogans for eco-friendly water bottles: ...
```

## 🏗️ Architecture

### Project Structure
```
MCPServerApp/
├── Tools/
│   ├── EchoTool.cs          # Main tool collection (50+ tools)
│   └── MonkeyTools.cs       # Domain-specific tools
├── Models/
│   └── Monkey.cs            # Data models
├── Services/
│   └── MonkeyService.cs     # Business logic services
├── Contexts/
│   └── MonkeyContext.cs     # Data context
└── Program.cs               # Application entry point
```

### Key Components

- **McpServerTool Attributes**: Mark methods as available MCP tools
- **IChatClient Integration**: Leverage AI capabilities for text processing
- **Dependency Injection**: Utilize .NET DI container for service management
- **Configuration System**: Support multiple configuration sources

## 🧪 Development

### Adding New Tools

1. Create a new static method in the appropriate tool class
2. Add the `[McpServerTool]` attribute with name and description
3. Include parameter descriptions using `[Description]` attributes
4. Implement the tool logic using IChatClient when appropriate

Example:
```csharp
[McpServerTool(Name = "MyNewTool"), Description("Description of what the tool does")]
public static async Task<string> MyNewTool(
    IMcpServer thisServer,
    [Description("Input parameter description")] string input,
    CancellationToken cancellationToken = default)
{
    // Tool implementation
    ChatMessage[] messages = [
        new(ChatRole.System, "System prompt for the AI"),
        new(ChatRole.User, $"User prompt with {input}")
    ];
    
    ChatOptions options = new()
    {
        MaxOutputTokens = 200,
        Temperature = 0.5f,
    };
    
    var response = await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
    return $"Result: {response}";
}
```

### Testing

```bash
# Run tests
dotnet test

# Build and run
dotnet build
dotnet run
```

## 📄 License

This project is licensed under the terms specified in LICENSE.txt.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📞 Support

For questions, issues, or contributions, please refer to the project's issue tracker or documentation.

---

*Built with ❤️ using .NET 9 and the Model Context Protocol*