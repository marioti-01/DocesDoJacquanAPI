using System.ComponentModel.DataAnnotations;
using BolosDoJacquin.API.DTOs;

namespace BolosDoJacquin.API.Tests;

public class ValidationTests
{
    private static IList<ValidationResult> Validate(object value)
    {
        var context = new ValidationContext(value);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(value, context, results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void Review_rating_must_be_between_one_and_five()
    {
        Assert.NotEmpty(Validate(new ReviewInputDto(0, "inválida")));
        Assert.Empty(Validate(new ReviewInputDto(5, "comentário válido")));
    }

    [Fact]
    public void Review_comment_must_respect_maximum_length()
    {
        Assert.NotEmpty(Validate(new ReviewInputDto(4, new string('x', 601))));
        Assert.Empty(Validate(new ReviewInputDto(4, null)));
    }

    [Fact]
    public void Product_price_and_descriptions_are_validated()
    {
        var invalid = new ProductInputDto("", 0, "", Guid.Empty, new string('x', 241), new string('x', 2001), true);
        Assert.NotEmpty(Validate(invalid));
    }

    [Fact]
    public void Registration_requires_valid_email_and_password_length()
    {
        Assert.NotEmpty(Validate(new RegisterDto("Cliente", "email-invalido", "123")));
        Assert.Empty(Validate(new RegisterDto("Cliente", "cliente@example.com", "senha-segura")));
    }
}
