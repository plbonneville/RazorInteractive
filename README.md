# .NET Interactive Nootbooks Razor Extension

[![NuGet version (RazorInteractive)](https://img.shields.io/nuget/v/RazorInteractive.svg)](https://www.nuget.org/packages/RazorInteractive/)

To get started with Razor in .NET Interactive Notebooks, first install the `RazorInteractive` NuGet package. In a new `C# (.NET Interactive)` cell enter and run the following:

```
#r "nuget: RazorInteractive, 1.0.0"
```

Using the `#!razor` magic command your code cell will be parsed by a Razor engine and the results displayed using the `"txt/html"` mime type.

```razor
#!razor

@{
    var colors = new [] { "red", "green", "blue" };
}

<ol>
@foreach(var color in colors)
{
    <li style="color: @color;">@color</li>
}
</ol>
```

The dotnet kernel variables are all available through the `@Model` property.

```csharp
var firstname = "John";
var lastname = "Doe";
var colors = new [] { "red", "green", "blue" };
```

```razor
#!razor

<p>Hello <b>@Model.firstname @Model.lastname</b>, what is you favorite a color?</p>

<ol>
@foreach(var color in Model.colors)
{
    <li style="color: @color;">@color</li>
}
</ol>
```
