﻿// Copyright (c) 2019-2023 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace R3.ObservableEvents.SourceGenerator.EventGenerators;

/// <summary>
/// This class was originally from StyleCop. https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Helpers/XmlSyntaxFactory.cs
/// All credit goes to the StyleCop team.
/// </summary>
internal static class XmlSyntaxFactory
{
    private static readonly string _newLine = "\r\n";

    /// <summary>
    /// Initializes static members of the <see cref="XmlSyntaxFactory"/> class.
    /// </summary>
    static XmlSyntaxFactory()
    {
        // Make sure the newline is included. Otherwise the comment and the method will be on the same line.
        InheritdocSyntax = SyntaxFactory.ParseLeadingTrivia(@"/// <inheritdoc />" + _newLine);
    }

    /// <summary>
    /// Gets a inheritdoc leading trivia comment.
    /// </summary>
    public static SyntaxTriviaList InheritdocSyntax { get; }

    /// <summary>
    /// Generates a summary comment for a method.
    /// </summary>
    /// <param name="summaryText">The text of the summary.</param>
    /// <param name="parameterFormat">The format of each parameter. This is for a string format.</param>
    /// <param name="entity">The method entity to construct the comment for.</param>
    /// <returns>The syntax trivia of the comment.</returns>
    public static SyntaxTriviaList GenerateSummaryComment(string summaryText, string parameterFormat, IMethodSymbol entity)
    {
        var parameters = entity.Parameters.Select(x => (x.Name, string.Format(CultureInfo.InvariantCulture, parameterFormat, x.Name)));

        return GenerateSummaryComment(summaryText, parameters);
    }

    /// <summary>
    /// Generates a summary comment which includes a see also text.
    /// You need to provide a format index for where the see also needs to be inserted.
    /// </summary>
    /// <param name="summaryText">The text with the summary and a format index for the see also.</param>
    /// <param name="seeAlsoText">The uri for the inner see also section.</param>
    /// <returns>The syntax trivia of the comment.</returns>
    public static SyntaxTriviaList GenerateSummarySeeAlsoComment(string summaryText, string seeAlsoText)
    {
        var text = string.Format(CultureInfo.InvariantCulture, summaryText, "<see cref=\"" + seeAlsoText.Replace("<", "{").Replace(">", "}") + "\" />");
        var template = "/// <summary>" + _newLine +
                       $"/// {text}" + _newLine +
                       "/// </summary>" + _newLine;

        return SyntaxFactory.ParseLeadingTrivia(template);
    }

    /// <summary>
    /// Generates a summary comment which includes a see also text.
    /// You need to provide a format index for where the see also needs to be inserted.
    /// </summary>
    /// <param name="textLines">The text for the comment.</param>
    /// <returns>The syntax trivia of the comment.</returns>
    public static SyntaxTriviaList GenerateDocumentationString(params string[] textLines)
    {
        var stringBuilder = new StringBuilder();
        foreach (var textLine in textLines)
        {
            stringBuilder.AppendLine($"// {textLine}");
        }

        return SyntaxFactory.ParseLeadingTrivia(stringBuilder.ToString());
    }

    /// <summary>
    /// Generates a summary comment which includes a see also text.
    /// You need to provide a format index for where the see also needs to be inserted.
    /// It will also provide the ability to set parameters and their comments.
    /// </summary>
    /// <param name="summaryText">The text with the summary and a format index for the see also.</param>
    /// <param name="seeAlsoText">The uri for the inner see also section.</param>
    /// <param name="parameters">Key/Value pairs for the parameters for the comment.</param>
    /// <returns>The syntax trivia of the comment.</returns>
    public static SyntaxTriviaList GenerateSummarySeeAlsoComment(string summaryText, string seeAlsoText, params (string ParamName, string ParamText)[] parameters)
    {
        var text = string.Format(CultureInfo.InvariantCulture, summaryText, "<see cref=\"" + seeAlsoText.Replace("<", "{").Replace(">", "}") + "\" />");
        var sb = new StringBuilder("/// <summary>")
            .AppendLine()
            .Append("/// ").AppendLine(text)
            .AppendLine("/// </summary>");

        foreach (var parameter in parameters)
        {
            sb.Append("/// <param name=\"").Append(parameter.ParamName).Append("\">").Append(parameter.ParamText).AppendLine("</param>");
        }

        return SyntaxFactory.ParseLeadingTrivia(sb.ToString());
    }

    /// <summary>
    /// Generates a summary comment.
    /// </summary>
    /// <param name="summaryText">The text of the summary comment.</param>
    /// <returns>The syntax trivia of the comment.</returns>
    public static SyntaxTriviaList GenerateSummaryComment(string summaryText)
    {
        var template = "/// <summary>" + _newLine +
                       $"/// {summaryText}" + _newLine +
                       "/// </summary>" + _newLine;

        return SyntaxFactory.ParseLeadingTrivia(template);
    }

    /// <summary>
    /// Generates a summary comment with a return statement.
    /// </summary>
    /// <param name="summaryText">The text of the summary comment.</param>
    /// <param name="returnValueText">The text of the return value.</param>
    /// <returns>The syntax trivia of the comment.</returns>
    public static SyntaxTriviaList GenerateSummaryComment(string summaryText, string returnValueText)
    {
        var template = "/// <summary>" + _newLine +
                       $"/// {summaryText}" + _newLine +
                       "/// </summary>" + _newLine +
                       $"/// <returns>{returnValueText}///<returns>" + _newLine;

        return SyntaxFactory.ParseLeadingTrivia(template);
    }

    /// <summary>
    /// Generates a summary comment with documents for parameters.
    /// </summary>
    /// <param name="summaryText">The text of the summary comment.</param>
    /// <param name="parameters">The key/value text of each parameter.</param>
    /// <returns>The syntax trivia of the comment.</returns>
    public static SyntaxTriviaList GenerateSummaryComment(string summaryText, IEnumerable<(string ParamName, string ParamText)> parameters)
    {
        var sb = new StringBuilder("/// <summary>")
            .AppendLine()
            .Append("/// ").AppendLine(summaryText)
            .AppendLine("/// </summary>");

        foreach (var parameter in parameters)
        {
            sb.Append("/// <param name=\"").Append(parameter.ParamName).Append("\">").Append(parameter.ParamText).AppendLine("</param>");
        }

        return SyntaxFactory.ParseLeadingTrivia(sb.ToString());
    }

    /// <summary>
    /// Generates a summary comment with documents for parameters and a return value.
    /// </summary>
    /// <param name="summaryText">The text of the summary comment.</param>
    /// <param name="parameters">The key/value text of each parameter.</param>
    /// <param name="returnValueText">The text of the return value.</param>
    /// <returns>The syntax trivia of the comment.</returns>
    public static SyntaxTriviaList GenerateSummaryComment(string summaryText, IEnumerable<(string ParamName, string ParamText)> parameters, string returnValueText)
    {
        var sb = new StringBuilder("/// <summary>")
            .AppendLine()
            .Append("/// ").AppendLine(summaryText)
            .AppendLine("/// </summary>");

        foreach (var parameter in parameters)
        {
            sb.Append("/// <param name=\"").Append(parameter.ParamName).Append("\">").Append(parameter.ParamText).AppendLine("</param>");
        }

        sb.Append("/// <returns>").Append(returnValueText).AppendLine("</returns>");
        return SyntaxFactory.ParseLeadingTrivia(sb.ToString());
    }

    /// <summary>
    /// Converts the method into a format that can be used in XML documentation.
    /// </summary>
    /// <param name="method">The method to convert.</param>
    /// <returns>A XML friendly version of the method.</returns>
    public static string ConvertToDocument(this IMethodSymbol method)
    {
        var stringBuilder = new StringBuilder(method.ContainingType.GetArityDisplayName() + "." + method.Name).Append('(');

        for (var i = 0; i < method.Parameters.Length; ++i)
        {
            var parameter = method.Parameters[i];

            if (i != 0)
            {
                stringBuilder.Append(", ");
            }

            stringBuilder.Append(parameter.Type.GetArityDisplayName());
        }

        stringBuilder.Append(')');

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Converts the event into a format that can be used in XML documentation.
    /// </summary>
    /// <param name="eventDetails">The event to convert.</param>
    /// <returns>A XML friendly version of the event.</returns>
    public static string ConvertToDocument(this IEventSymbol eventDetails)
    {
        return eventDetails.ContainingType.GetArityDisplayName() + "." + eventDetails.Name;
    }
}
