using AppController.Utils;

namespace Test.Controllers;

public class RequestParserTests
{
    private readonly RequestParser _parser = new();

    // ── find ──────────────────────────────────────────────────────────────

    [Fact]
    public void Parse_FindAll_ReturnsNoArgs()
    {
        var result = _parser.Parse("find all");

        Assert.Equal("find", result.CommandName);
        Assert.Null(result.Get("type"));
        Assert.Null(result.Get("filter"));
    }

    [Fact]
    public void Parse_FindByCategory_ReturnsTypeArg()
    {
        var result = _parser.Parse("find laptops");

        Assert.Equal("find", result.CommandName);
        Assert.Equal("laptops", result.Get("type"));
    }

    [Theory]
    [InlineData("find all price=10;500", "10", "500")]
    [InlineData("find all price=0;9999", "0", "9999")]
    public void Parse_FindByPrice_ReturnsPriceArgs(string input, string min, string max)
    {
        var result = _parser.Parse(input);

        Assert.Equal("find", result.CommandName);
        Assert.Equal("price", result.Get("filter"));
        Assert.Equal(min, result.Get("min"));
        Assert.Equal(max, result.Get("max"));
    }

    // ── cost ──────────────────────────────────────────────────────────────

    [Fact]
    public void Parse_Cost_ReturnsPriceArgs()
    {
        var result = _parser.Parse("cost 100 500");

        Assert.Equal("cost", result.CommandName);
        Assert.Equal("100", result.Get("min"));
        Assert.Equal("500", result.Get("max"));
    }

    // ── show ──────────────────────────────────────────────────────────────

    [Fact]
    public void Parse_Show_ReturnsIdArg()
    {
        var result = _parser.Parse("show 42");

        Assert.Equal("show", result.CommandName);
        Assert.Equal("42", result.Get("id"));
    }

    // ── switch ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("switch admin", "admin")]
    [InlineData("switch user", "user")]
    public void Parse_Switch_ReturnsModeArg(string input, string expectedMode)
    {
        var result = _parser.Parse(input);

        Assert.Equal("switch", result.CommandName);
        Assert.Equal(expectedMode, result.Get("mode"));
    }

    // ── delete / update ───────────────────────────────────────────────────

    [Fact]
    public void Parse_Delete_ReturnsIdArg()
    {
        var result = _parser.Parse("delete 5");

        Assert.Equal("delete", result.CommandName);
        Assert.Equal("5", result.Get("id"));
    }

    [Fact]
    public void Parse_Update_ReturnsIdArg()
    {
        var result = _parser.Parse("update 3");

        Assert.Equal("update", result.CommandName);
        Assert.Equal("3", result.Get("id"));
    }

    // ── edge cases ────────────────────────────────────────────────────────

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_EmptyInput_ReturnsUnknown(string input)
    {
        var result = _parser.Parse(input);

        Assert.Equal("unknown", result.CommandName);
    }

    [Fact]
    public void Parse_UnknownCommand_ReturnsCommandNameAsIs()
    {
        var result = _parser.Parse("abrakadabra");

        Assert.Equal("abrakadabra", result.CommandName);
    }

    [Fact]
    public void Parse_IsCaseInsensitive()
    {
        var result = _parser.Parse("FIND ALL");

        Assert.Equal("find", result.CommandName);
    }
}
