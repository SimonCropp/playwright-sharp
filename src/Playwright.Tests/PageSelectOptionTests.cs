using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace Microsoft.Playwright.Tests
{
    [Parallelizable(ParallelScope.Self)]
    public class PageSelectOptionTests : PageTestEx
    {
        [PlaywrightTest("page-select-option.spec.ts", "should select single option")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldSelectSingleOption()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            await Page.SelectOptionAsync("select", "blue");
            Assert.AreEqual(new[] { "blue" }, await Page.EvaluateAsync<string[]>("() => result.onInput"));
            Assert.AreEqual(new[] { "blue" }, await Page.EvaluateAsync<string[]>("() => result.onChange"));
        }

        [PlaywrightTest("page-select-option.spec.ts", "should select single option by value")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldSelectSingleOptionByValue()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            await Page.SelectOptionAsync("select", new SelectOptionValue { Value = "blue" });
            Assert.AreEqual(new[] { "blue" }, await Page.EvaluateAsync<string[]>("() => result.onInput"));
            Assert.AreEqual(new[] { "blue" }, await Page.EvaluateAsync<string[]>("() => result.onChange"));
        }

        [PlaywrightTest("page-select-option.spec.ts", "should select single option by label")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldSelectSingleOptionByLabel()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            await Page.SelectOptionAsync("select", new SelectOptionValue { Label = "Indigo" });
            Assert.AreEqual(new[] { "indigo" }, await Page.EvaluateAsync<string[]>("() => result.onInput"));
            Assert.AreEqual(new[] { "indigo" }, await Page.EvaluateAsync<string[]>("() => result.onChange"));
        }

        [PlaywrightTest("page-select-option.spec.ts", "should select single option by handle")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldSelectSingleOptionByHandle()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            await Page.SelectOptionAsync("select", await Page.QuerySelectorAsync("[id=whiteOption]"));
            Assert.AreEqual(new[] { "white" }, await Page.EvaluateAsync<string[]>("() => result.onInput"));
            Assert.AreEqual(new[] { "white" }, await Page.EvaluateAsync<string[]>("() => result.onChange"));
        }

        [PlaywrightTest("page-select-option.spec.ts", "should select single option by index")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldSelectSingleOptionByIndex()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            await Page.SelectOptionAsync("select", new SelectOptionValue { Index = 2 });
            Assert.AreEqual(new[] { "brown" }, await Page.EvaluateAsync<string[]>("() => result.onInput"));
            Assert.AreEqual(new[] { "brown" }, await Page.EvaluateAsync<string[]>("() => result.onChange"));
        }

        [PlaywrightTest("page-select-option.spec.ts", "should select single option by multiple attributes")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldSelectSingleOptionByMultipleAttributes()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            await Page.SelectOptionAsync("select", new SelectOptionValue { Value = "green", Label = "Green" });
            Assert.AreEqual(new[] { "green" }, await Page.EvaluateAsync<string[]>("() => result.onInput"));
            Assert.AreEqual(new[] { "green" }, await Page.EvaluateAsync<string[]>("() => result.onChange"));
        }

        [PlaywrightTest("page-select-option.spec.ts", "should not select single option when some attributes do not match")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldNotSelectSingleOptionWhenSomeAttributesDoNotMatch()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            await Page.EvalOnSelectorAsync("select", "s => s.value = undefined");
            await PlaywrightAssert.ThrowsAsync<TimeoutException>(() => Page.SelectOptionAsync("select", new SelectOptionValue { Value = "green", Label = "Brown" }, new() { Timeout = 300 }));
            Assert.IsEmpty(await Page.EvaluateAsync<string>("() => document.querySelector('select').value"));
        }

        [PlaywrightTest("page-select-option.spec.ts", "should select only first option")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldSelectOnlyFirstOption()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            await Page.SelectOptionAsync("select", new[] { "blue", "green", "red" });
            Assert.AreEqual(new[] { "blue" }, await Page.EvaluateAsync<string[]>("() => result.onInput"));
            Assert.AreEqual(new[] { "blue" }, await Page.EvaluateAsync<string[]>("() => result.onChange"));
        }

        [PlaywrightTest("page-select-option.spec.ts", "should not throw when select causes navigation")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldNotThrowWhenSelectCausesNavigation()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            await Page.EvalOnSelectorAsync("select", "select => select.addEventListener('input', () => window.location = '/empty.html')");
            await TaskUtils.WhenAll(
                Page.SelectOptionAsync("select", "blue"),
                Page.WaitForNavigationAsync()
            );
            StringAssert.Contains("empty.html", Page.Url);
        }

        [PlaywrightTest("page-select-option.spec.ts", "should select multiple options")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldSelectMultipleOptions()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            await Page.EvaluateAsync("() => makeMultiple()");
            await Page.SelectOptionAsync("select", new[] { "blue", "green", "red" });
            Assert.AreEqual(new[] { "blue", "green", "red" }, await Page.EvaluateAsync<string[]>("() => result.onInput"));
            Assert.AreEqual(new[] { "blue", "green", "red" }, await Page.EvaluateAsync<string[]>("() => result.onChange"));
        }

        [PlaywrightTest("page-select-option.spec.ts", "should select multiple options with attributes")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldSelectMultipleOptionsWithAttributes()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            await Page.EvaluateAsync("() => makeMultiple()");
            await Page.SelectOptionAsync(
                "select",
                new[] {
                    new SelectOptionValue { Value = "blue" },
                    new SelectOptionValue { Label = "Green" },
                    new SelectOptionValue { Index = 4 }
                });
            Assert.AreEqual(new[] { "blue", "gray", "green" }, await Page.EvaluateAsync<string[]>("() => result.onInput"));
            Assert.AreEqual(new[] { "blue", "gray", "green" }, await Page.EvaluateAsync<string[]>("() => result.onChange"));
        }

        [PlaywrightTest("page-select-option.spec.ts", "should respect event bubbling")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldRespectEventBubbling()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            await Page.SelectOptionAsync("select", "blue");
            Assert.AreEqual(new[] { "blue" }, await Page.EvaluateAsync<string[]>("() => result.onBubblingInput"));
            Assert.AreEqual(new[] { "blue" }, await Page.EvaluateAsync<string[]>("() => result.onBubblingChange"));
        }

        [PlaywrightTest("page-select-option.spec.ts", "should throw when element is not a &lt;select&gt;")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldThrowWhenElementIsNotASelect()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            var exception = await PlaywrightAssert.ThrowsAsync<PlaywrightException>(() => Page.SelectOptionAsync("body", string.Empty));
            StringAssert.Contains("Element is not a <select> element.", exception.Message);
        }

        [PlaywrightTest("page-select-option.spec.ts", "should return [] on no matched values")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldReturnEmptyArrayOnNoMatchedValues()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            var result = await Page.SelectOptionAsync("select", Array.Empty<string>());
            Assert.IsEmpty(result);
        }

        [PlaywrightTest("page-select-option.spec.ts", "should return an array of matched values")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldReturnAnArrayOfMatchedValues()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            await Page.EvaluateAsync<string>("() => makeMultiple()");
            var result = await Page.SelectOptionAsync("select", new[] { "blue", "black", "magenta" });
            Assert.AreEqual(new[] { "blue", "black", "magenta" }.OrderBy(v => v), result.OrderBy(v => v));
        }

        [PlaywrightTest("page-select-option.spec.ts", "should return an array of one element when multiple is not set")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldReturnAnArrayOfOneElementWhenMultipleIsNotSet()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            var result = await Page.SelectOptionAsync("select", new[] { "42", "blue", "black", "magenta" });
            Assert.That(result, Has.Count.EqualTo(1));
        }

        [PlaywrightTest("page-select-option.spec.ts", "should return [] on no values")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldReturnEmptyArrayOnNoValues()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            var result = await Page.SelectOptionAsync("select", Array.Empty<string>());
            Assert.IsEmpty(result);
        }

        [PlaywrightTest("page-select-option.spec.ts", "should unselect with null")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldUnselectWithNull()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            await Page.EvaluateAsync("() => makeMultiple()");
            var result = await Page.SelectOptionAsync("select", new[] { "blue", "black", "magenta" });
            Assert.True(result.All(r => new[] { "blue", "black", "magenta" }.Contains(r)));
            await Page.SelectOptionAsync("select", new string[] { });
            Assert.True(await Page.EvalOnSelectorAsync<bool?>("select", "select => Array.from(select.options).every(option => !option.selected)"));
        }

        [PlaywrightTest("page-select-option.spec.ts", "should deselect all options when passed no values for a multiple select")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldDeselectAllOptionsWhenPassedNoValuesForAMultipleSelect()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            await Page.EvaluateAsync("() => makeMultiple()");
            await Page.SelectOptionAsync("select", new[] { "blue", "black", "magenta" });
            await Page.SelectOptionAsync("select", new string[] { });
            Assert.True(await Page.EvalOnSelectorAsync<bool>("select", "select => Array.from(select.options).every(option => !option.selected)"));
        }

        [PlaywrightTest("page-select-option.spec.ts", "should deselect all options when passed no values for a select without multiple")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldDeselectAllOptionsWhenPassedNoValuesForASelectWithoutMultiple()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            await Page.SelectOptionAsync("select", new[] { "blue", "black", "magenta" });
            await Page.SelectOptionAsync("select", Array.Empty<string>());
            Assert.True(await Page.EvalOnSelectorAsync<bool>("select", "select => Array.from(select.options).every(option => !option.selected)"));
        }

        [PlaywrightTest("page-select-option.spec.ts", "should throw if passed wrong types")]
        [Test, Ignore("Not relevant for C#, js specific")]
        public void ShouldThrowIfPassedWrongTypes()
        {
        }

        [PlaywrightTest("page-select-option.spec.ts", "should work when re-defining top-level Event class")]
        [Test, Timeout(TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkWhenReDefiningTopLevelEventClass()
        {
            await Page.GotoAsync(Server.Prefix + "/input/select.html");
            await Page.EvaluateAsync("() => window.Event = null");
            await Page.SelectOptionAsync("select", "blue");
            Assert.AreEqual(new[] { "blue" }, await Page.EvaluateAsync<string[]>("() => result.onInput"));
            Assert.AreEqual(new[] { "blue" }, await Page.EvaluateAsync<string[]>("() => result.onChange"));
        }
    }
}
