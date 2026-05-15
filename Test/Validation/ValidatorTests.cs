using Domain.Exceptions;
using Domain.Validation;
using Xunit;
using ValidationException = Domain.Exceptions.ValidationException;

namespace Test.Validation;

file record SampleDto(string? Name, decimal Price, string? Description);

public class ValidatorTests
{
    // ── NotNullOrEmpty ─────────────────────────────────────────────────────

    [Fact]
    public void NotNullOrEmpty_WhenNull_ThrowsValidationException()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            Validator<SampleDto>.For(new SampleDto(null, 100, null))
                .NotNullOrEmpty(r => r.Name, "Name")
                .Validate());

        Assert.Contains("Name must not be empty", ex.Message);
    }

    [Fact]
    public void NotNullOrEmpty_WhenWhitespace_ThrowsValidationException()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            Validator<SampleDto>.For(new SampleDto("  ", 100, null))
                .NotNullOrEmpty(r => r.Name, "Name")
                .Validate());

        Assert.Single(ex.Errors);
    }

    [Fact]
    public void NotNullOrEmpty_WhenValid_DoesNotThrow()
    {
        Validator<SampleDto>.For(new SampleDto("TV", 100, null))
            .NotNullOrEmpty(r => r.Name, "Name")
            .Validate();
    }

    // ── MaxLength ─────────────────────────────────────────────────────────

    [Fact]
    public void MaxLength_WhenExceeded_ThrowsValidationException()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            Validator<SampleDto>.For(new SampleDto(new string('A', 101), 100, null))
                .MaxLength(r => r.Name, 100, "Name")
                .Validate());

        Assert.Contains("must not exceed 100 characters", ex.Message);
    }

    [Fact]
    public void MaxLength_WhenNull_DoesNotThrow()
    {
        Validator<SampleDto>.For(new SampleDto(null, 100, null))
            .MaxLength(r => r.Description, 100, "Description")
            .Validate();
    }

    [Fact]
    public void MaxLength_WhenExactLimit_DoesNotThrow()
    {
        Validator<SampleDto>.For(new SampleDto(new string('A', 100), 100, null))
            .MaxLength(r => r.Name, 100, "Name")
            .Validate();
    }

    // ── GreaterThan ───────────────────────────────────────────────────────

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(-100.0)]
    public void GreaterThan_WhenNotGreater_ThrowsValidationException(double price)
    {
        var ex = Assert.Throws<ValidationException>(() =>
            Validator<SampleDto>.For(new SampleDto("TV", (decimal)price, null))
                .GreaterThan(r => r.Price, 0, "Price")
                .Validate());

        Assert.Contains("Price must be greater than 0", ex.Message);
    }

    [Fact]
    public void GreaterThan_WhenValid_DoesNotThrow()
    {
        Validator<SampleDto>.For(new SampleDto("TV", 0.01m, null))
            .GreaterThan(r => r.Price, 0, "Price")
            .Validate();
    }

    // ── InRange ───────────────────────────────────────────────────────────

    [Theory]
    [InlineData(50.0)]
    [InlineData(1001.0)]
    public void InRange_WhenOutOfRange_ThrowsValidationException(double price)
    {
        var ex = Assert.Throws<ValidationException>(() =>
            Validator<SampleDto>.For(new SampleDto("TV", (decimal)price, null))
                .InRange(r => r.Price, 100, 1000, "Price")
                .Validate());

        Assert.Contains("must be between 100 and 1000", ex.Message);
    }

    [Fact]
    public void InRange_WhenWithinRange_DoesNotThrow()
    {
        Validator<SampleDto>.For(new SampleDto("TV", 500m, null))
            .InRange(r => r.Price, 100, 1000, "Price")
            .Validate();
    }

    // ── Multiple errors collected ─────────────────────────────────────────

    [Fact]
    public void Validate_CollectsAllErrors_NotJustFirst()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            Validator<SampleDto>.For(new SampleDto(null, -5, null))
                .NotNullOrEmpty(r => r.Name, "Name")
                .GreaterThan(r => r.Price, 0, "Price")
                .Validate());

        Assert.Equal(2, ex.Errors.Count);
    }

    // ── ValidationException is HousestockException ────────────────────────

    [Fact]
    public void ValidationException_IsHousestockException()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            Validator<SampleDto>.For(new SampleDto(null, 100, null))
                .NotNullOrEmpty(r => r.Name, "Name")
                .Validate());

        Assert.IsAssignableFrom<HousestockException>(ex);
    }
}
