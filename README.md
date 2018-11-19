# VSCodeSnippetConverter

This utility will convert the snippets from VScode to Visual Studio.


## Installation

1. Clone the repo, build with VisualStudio 2017;
2. Or download the [precompiled exe](https://github.com/TMPxyz/VSCodeSnippetConverter/blob/master/Build/json2snippet.7z);

## How to Use

![Snapshot](https://github.com/TMPxyz/VSCodeSnippetConverter/blob/master/pics/Snapshot1.jpg)

1. Select the vscode snippet .json file;
2. Select a dir for generated .snippet files;
3. Select "Convert";

## Limitation

1. vscode snippet "body" section needs to has array form instead of a single string;
2. don't support vscode snippet variable transform;
3. don't support vscode built-in env variables;


## Format

E.g: 
It will converts some VSCode snippets from this:

    "Service locator define" : {
        "prefix" : "$vvvv",
        "body" : [
            "[SerializeField][TypeRestriction(typeof(I${1:Class}))][Tooltip(\"\")]",
            "private MonoBehaviour _mono$1;",
            "private I$1 _${2:var};",
            "public static I$1 $2 { get{ if(Instance._$2 == null) {Instance._Init$1();} return Instance._$2; } }"
        ],
        "description": "used by ServiceLocator"
    },

To this:

    <?xml version="1.0" encoding="utf-8"?>
    <CodeSnippets xmlns="http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet">
      <CodeSnippet Format="1.0.0">
        <Header>
          <SnippetTypes>
            <SnippetType>Expansion</SnippetType>
          </SnippetTypes>
          <Title>Service locator define</Title>
          <Author>zaex</Author>
          <Description>used by ServiceLocator</Description>
          <HelpUrl></HelpUrl>
          <Shortcut>$vvvv</Shortcut>
        </Header>
        <Snippet>
          <Declarations>
            <Literal Editable="true">
              <ID>1</ID>
              <ToolTip>1</ToolTip>
              <Default>1</Default>
              <Function>1+1</Function>
            </Literal>
            <Literal Editable="true">
              <ID>2</ID>
              <ToolTip>2</ToolTip>
              <Default>2</Default>
              <Function>1+1</Function>
            </Literal>

          </Declarations>
          <Code Language="csharp" Delimiter="$"><![CDATA[ [SerializeField][TypeRestriction(typeof(I$1$))][Tooltip("")]
      private MonoBehaviour _mono$1$;
      private I$1$ _$2$;
      public static I$1$ $2$ { get{ if(Instance._$2$ == null) {Instance._Init$1$();} return Instance._$2$; } }
     ]]></Code>
        </Snippet>
      </CodeSnippet>
    </CodeSnippets>

