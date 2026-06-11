using Ashraak.SharedKernel.Results;
using FluentAssertions;
using Xunit;

namespace Ashraak.SharedKernel.Tests.Results;

public sealed class ResultTests
{
    [Fact]
    public void Success_ShouldHaveIsSuccessTrue()
    {
        var result = Result.Success();
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ShouldHaveIsSuccessFalse()
    {
        var error = new Error("Test.Error", "Something went wrong");
        var result = Result.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void GenericSuccess_ShouldExposeValue()
    {
        var result = Result.Success(42);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void GenericFailure_AccessingValue_ShouldThrow()
    {
        var error = Error.NotFound("Test.NotFound", "Not found");
        var result = Result.Failure<int>(error);

        var act = () => result.Value;
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ImplicitConversionFromError_ShouldCreateFailureResult()
    {
        var error = Error.Validation("Test.Validation", "Invalid input");
        Result result = Result.Failure(error);
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void Map_OnSuccess_ShouldTransformValue()
    {
        var result = Result.Success(5);
        var mapped = result.Map(v => v * 2);

        mapped.IsSuccess.Should().BeTrue();
        mapped.Value.Should().Be(10);
    }

    [Fact]
    public void Map_OnFailure_ShouldPropagateError()
    {
        var error = Error.NotFound("Test.NotFound", "Not found");
        var result = Result.Failure<int>(error);
        var mapped = result.Map(v => v * 2);

        mapped.IsFailure.Should().BeTrue();
        mapped.Error.Should().Be(error);
    }
}
