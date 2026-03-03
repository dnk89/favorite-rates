using FavoriteRates.SharedLibrary.Application;
using FavoriteRates.SharedLibrary.ResultPattern;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FavoriteRates.SharedLibrary.UnitTests;

public class ValidationDecoratorTests
{
    private readonly Mock<IHandler<TestRequest, Result>> _handlerMock = new();
    private readonly List<IValidator<TestRequest>> _validators = [];

    private ValidationDecorator<TestRequest, Result> CreateSut() 
        => new(_handlerMock.Object, _validators);

    [Fact]
    public async Task Handle_NoValidators_CallsDecoratedHandler()
    {
        var request = new TestRequest();
        var expectedResult = Result.Success();
        _handlerMock.Setup(h => h.Handle(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        var sut = CreateSut();

        var result = await sut.Handle(request, CancellationToken.None);

        Assert.Equal(expectedResult, result);
        _handlerMock.Verify(h => h.Handle(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationSucceeds_CallsDecoratedHandler()
    {
        var request = new TestRequest();
        var expectedResult = Result.Success();
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _validators.Add(validatorMock.Object);
        _handlerMock.Setup(h => h.Handle(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        var sut = CreateSut();

        var result = await sut.Handle(request, CancellationToken.None);

        Assert.Equal(expectedResult, result);
        _handlerMock.Verify(h => h.Handle(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ReturnsFailureAndDoesNotCallHandler()
    {
        var request = new TestRequest();
        const string errorMessage = "Validation error";
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("Prop", errorMessage)]));
        _validators.Add(validatorMock.Object);
        var sut = CreateSut();

        var result = await sut.Handle(request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Error);
        _handlerMock.Verify(h => h.Handle(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ResultT_ValidationFails_ReturnsFailureResultT()
    {
        var handlerMock = new Mock<IHandler<TestRequest, Result<string>>>();
        var validatorMock = new Mock<IValidator<TestRequest>>();
        const string errorMessage = "Validation error";
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("Prop", errorMessage)]));
        var sut = new ValidationDecorator<TestRequest, Result<string>>(handlerMock.Object, [validatorMock.Object]);

        var result = await sut.Handle(new TestRequest(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Error);
        Assert.Null(result.Value);
        handlerMock.Verify(h => h.Handle(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_MultipleValidators_ReturnsFirstFailure()
    {
        var validator1 = new Mock<IValidator<TestRequest>>();
        validator1.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        
        var validator2 = new Mock<IValidator<TestRequest>>();
        validator2.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("Prop", "Error 2")]));
        
        var validator3 = new Mock<IValidator<TestRequest>>();
        validator3.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("Prop", "Error 3")]));

        var sut = new ValidationDecorator<TestRequest, Result>(_handlerMock.Object, [validator1.Object, validator2.Object, validator3.Object]);

        var result = await sut.Handle(new TestRequest(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Error 2", result.Error);
        _handlerMock.Verify(h => h.Handle(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UnsupportedResultType_ThrowsInvalidOperationException()
    {
        var handlerMock = new Mock<IHandler<TestRequest, string>>();
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("Prop", "Error")]));
        var sut = new ValidationDecorator<TestRequest, string>(handlerMock.Object, [validatorMock.Object]);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Handle(new TestRequest(), CancellationToken.None));
        Assert.Equal("Unsupported result type: String", exception.Message);
    }

    public class TestRequest;
}