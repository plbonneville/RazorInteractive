using System;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.DotNet.Interactive.Events;
using Microsoft.DotNet.Interactive.FSharp;
using Microsoft.DotNet.Interactive.Tests.Utility;
using Xunit;

namespace RazorInteractive.Tests
{
    public class RazorInteractiveTests : IDisposable
    {
        private readonly Kernel _kernel;

        public RazorInteractiveTests()
        {
            _kernel = new CompositeKernel
            {
                new CSharpKernel(),
                new FSharpKernel()
            };

            Task.Run(() => new RazorKernelExtension().OnLoadAsync(_kernel))
                .Wait();

            KernelEvents = _kernel.KernelEvents.ToSubscribedList();
        }

        private SubscribedList<KernelEvent> KernelEvents { get; }

        public void Dispose()
        {
            _kernel?.Dispose();
            KernelEvents?.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task It_is_registered_as_a_directive()
        {
            using var events = _kernel.KernelEvents.ToSubscribedList();

            await _kernel.SubmitCodeAsync("#!razor");

            KernelEvents
                .Should()
                .ContainSingle<CommandSucceeded>()
                .Which
                .Command
                .Should()
                .Equals("#!razor");
        }

        [Fact]
        public async Task It_formats_RazorCode()
        {
            using var events = _kernel.KernelEvents.ToSubscribedList();

            await _kernel.SubmitCodeAsync(@"#!razor
<h1>hello world</h>");

            KernelEvents
                .Should()
                .ContainSingle<DisplayEvent>()
                .Which
                .FormattedValues
                .Should()
                .ContainSingle(v => v.MimeType == "text/html");
        }

        [Fact]
        public async Task It_can_interprets_RazorCode()
        {
            const string code = @"#!razor

@{
    string name = ""Alice"";
}

<h2>
    Hello world @name!
</h2>";

            using var events = _kernel.KernelEvents.ToSubscribedList();
            await _kernel.SubmitCodeAsync(code);

            KernelEvents
                .Should()
                .ContainSingle<DisplayEvent>()
                .Which
                .FormattedValues
                .Should()
                .ContainSingle(v => v.MimeType == "text/html")
                .Which
                .Value
                .Should()
                .Contain("Hello world Alice!");
        }

        [Fact]
        public async Task It_can_see_dotnet_kernel_int_variables_in_model()
        {
            // Arrange
            const string before = "BEFORE";
            const string after = "AFTER";

            // Based on:
            //  https://github.com/dotnet/interactive/blob/main/src/Microsoft.DotNet.Interactive.Jupyter.Tests/MagicCommandTests.who_and_whos.cs
            var commands = new[]
                {
                    "var x = 1;",
                    "x = 2;",
                    "var y = \"hi\";",
                    "var z = new object[] { x, y };"
                };

            foreach (var command in commands)
            {
                await _kernel.SendAsync(new SubmitCode(command));
            }

            using var events = _kernel.KernelEvents.ToSubscribedList();

            // Act
            await _kernel.SubmitCodeAsync($@"#!razor
            {before} @Model.x {after}");

            // Assert
            KernelEvents
                .Should()
                .ContainSingle<DisplayEvent>()
                .Which
                .FormattedValues
                .Should()
                .ContainSingle(v => v.MimeType == "text/html")
                .Which
                .Value
                .Should()
                .Contain($"{before} 2 {after}");
        }

        [Fact]
        public async Task It_can_see_dotnet_kernel_string_variables_in_model()
        {
            // Arrange
            const string before = "BEFORE";
            const string after = "AFTER";

            var commands = new[]
                {
                    "var x = 1;",
                    "x = 2;",
                    "var y = \"hi\";",
                    "var z = new object[] { x, y };"
                };

            foreach (var command in commands)
            {
                await _kernel.SendAsync(new SubmitCode(command));
            }

            using var events = _kernel.KernelEvents.ToSubscribedList();

            // Act
            await _kernel.SubmitCodeAsync($@"#!razor
            {before} @Model.y {after}");

            // Assert
            KernelEvents
                .Should()
                .ContainSingle<DisplayEvent>()
                .Which
                .FormattedValues
                .Should()
                .ContainSingle(v => v.MimeType == "text/html")
                .Which
                .Value
                .Should()
                .Contain($"{before} hi {after}");
        }

        [Fact]
        public async Task It_can_see_dotnet_kernel_array_items_variables_in_model()
        {
            // Arrange
            const string before = "BEFORE";
            const string after = "AFTER";

            var commands = new[]
                {
                    "var x = 1;",
                    "x = 2;",
                    "var y = \"hi\";",
                    "var z = new object[] { x, y };"
                };

            foreach (var command in commands)
            {
                await _kernel.SendAsync(new SubmitCode(command));
            }

            using var events = _kernel.KernelEvents.ToSubscribedList();

            // Act
            await _kernel.SubmitCodeAsync($@"#!razor
            {before} @Model.z[0] @Model.z[1] {after}");

            // Assert
            KernelEvents
                .Should()
                .ContainSingle<DisplayEvent>()
                .Which
                .FormattedValues
                .Should()
                .ContainSingle(v => v.MimeType == "text/html")
                .Which
                .Value
                .Should()
                .Contain($"{before} 2 hi {after}");
        }

        [Fact]
        public async Task It_displays_standard_error_when_razor_doesnt_compile_or_render()
        {
            // Arrange
            using var events = _kernel.KernelEvents.ToSubscribedList();

            // Act
            await _kernel.SubmitCodeAsync(@"#!razor
            @variable_does_not_exists");

            // Assert
            KernelEvents
            .Should()
            .ContainSingle<StandardErrorValueProduced>(); // StandardErrorValueProduced

        }

        // TODO: we need to be able to see the other dotnet kernel variables if they are shared.
        // TODO: review if the cell SubmitCode command needs to start with a #!razor or if we can have a #!share above...
        // //         [Fact]
        // //         public async Task It_can_see_dotnet_fsharp_kernel_variables_in_model()
        // //         {
        // //             // Arrange
        // //             var before = "BEFORE";
        // //             var after = "AFTER";

        // //             var commands = new[]
        // //                     {
        // //                         "#!fsharp\nlet mutable x = 1",
        // //                         "#!fsharp\nx <- 2",
        // //                         "#!fsharp\nlet y = \"hi!\"",
        // //                         "#!fsharp\nlet z = [| x :> obj; y :> obj |]",
        // //                     };

        // //             foreach (var command in commands)
        // //             {
        // //                 await _kernel.SendAsync(new SubmitCode(command));
        // //             }

        // //             using var events = _kernel.KernelEvents.ToSubscribedList();

        // //             // Act
        // //             await _kernel.SubmitCodeAsync($@"
        // // #!share --from fsharp x
        // // #!razor
        // // {before} @Model.x {after}");

        // //             // Assert
        // //             KernelEvents
        // //                 .Should()
        // //                 .ContainSingle<DisplayEvent>()
        // //                 .Which
        // //                 .FormattedValues
        // //                 .Should()
        // //                 .ContainSingle(v => v.MimeType == "text/html")
        // //                 .Which
        // //                 .Value
        // //                 .Should()
        // //                 .Contain($"{before} 2 {after}");
        // //         }

        [Fact]
        public async Task It_renders_html_and_not_html_encoded_html()
        {
            // Arrange
            using var events = _kernel.KernelEvents.ToSubscribedList();

            // Act
            await _kernel.SubmitCodeAsync(@"#!razor
            <h1>hello world</h1>");

            // Assert
            KernelEvents
                .Should()
                .ContainSingle<DisplayEvent>()
                .Which
                .FormattedValues
                .Should()
                .ContainSingle(v => v.MimeType == "text/html")
                .Which
                .Value
                .Should()
                .Contain("<h1>hello world</h1>");
        }
    }
}