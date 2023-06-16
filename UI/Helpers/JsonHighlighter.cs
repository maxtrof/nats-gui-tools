using System.IO;
using System.Xml;
using AvaloniaEdit.Highlighting;

namespace UI.Helpers;

public static class JsonHighlighter
{
    // TODO Move to resources
    private const string HighlightScheme = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                    <SyntaxDefinition name=""JsonDark"" extensions="".jsondark"" xmlns=""http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008"">
                    <Color name=""Bool"" foreground=""Aqua"" exampleText=""true | false"" />
                    <Color name=""Number"" foreground=""LightBlue"" exampleText=""3.14"" />
                    <Color name=""String"" foreground=""LightGreen"" exampleText="""" />
                    <Color name=""Null"" foreground=""Olive"" exampleText="""" />
                    <Color name=""FieldName"" foreground=""LightCoral"" />
                    <Color name=""Punctuation"" foreground=""White"" />

                    <RuleSet name=""String"">
                        <Span begin=""\\"" end="".""/>
                    </RuleSet>

                    <RuleSet name=""Object"">
                        <Span color=""FieldName"" ruleSet=""String"">
                    <Begin>""</Begin>
                    <End>""</End>
                    </Span>
                    <Span color=""FieldName"" ruleSet=""String"">
                    <Begin>'</Begin>
                    <End>'</End>
                    </Span>
                    <Span color=""Punctuation"" ruleSet=""Expression"">
                    <Begin>:</Begin>
                    </Span>
                    <Span color=""Punctuation"">
                        <Begin>,</Begin>
                    </Span>
                    </RuleSet>

                    <RuleSet name=""Array"">
                        <Import ruleSet=""Expression""/>
                        <Span color=""Punctuation"">
                        <Begin>,</Begin>
                    </Span>
                    </RuleSet>

                    <RuleSet name=""Expression"">
                        <Keywords color=""Bool"" >
                        <Word>true</Word>
                    <Word>false</Word>
                    </Keywords>
                    <Keywords color=""Null"" >
                        <Word>null</Word>
                    </Keywords>
                    <Span color=""String"" ruleSet=""String"">
                    <Begin>""</Begin>
                    <End>""</End>
                    </Span>
                    <Span color=""String"" ruleSet=""String"">
                    <Begin>'</Begin>
                    <End>'</End>
                    </Span>
                    <Span color=""Punctuation"" ruleSet=""Object"" multiline=""true"">
                    <Begin>\{</Begin>
                        <End>\}</End>
                    </Span>
                    <Span color=""Punctuation"" ruleSet=""Array"" multiline=""true"">
                    <Begin>\[</Begin>
                    <End>\]</End>
                    </Span>
                    <Rule color=""Number"">
                    \b0[xX][0-9a-fA-F]+|(\b\d+(\.[0-9]+)?|\.[0-9]+)([eE][+-]?[0-9]+)?
                    </Rule>
                    </RuleSet>

                    <RuleSet>
                    <Import ruleSet=""Expression""/>
                    </RuleSet>
                    </SyntaxDefinition>";
    public static void LoadJsonHighlighter()
    {
        using (var reader = new XmlTextReader(new StringReader(HighlightScheme)))
        {
            var customHighlighting = AvaloniaEdit.Highlighting.Xshd.
                HighlightingLoader.Load(reader, HighlightingManager.Instance);
            HighlightingManager.Instance.RegisterHighlighting("JsonDark", new string[] { ".jsondark" }, customHighlighting);
        }
    }
}