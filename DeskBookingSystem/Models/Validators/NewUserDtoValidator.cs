using DeskBookingSystem.Entities;
using DeskBookingSystem.Models;
using FluentValidation;

namespace DeskBookingSystem.Models.Validators
{
    public class NewUserDtoValidator : AbstractValidator<NewUserDto>
    {
        public NewUserDtoValidator(BookingSystemDbContext dbContext)
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();

            RuleFor(x => x.Password).MinimumLength(8).MaximumLength(45);

            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password);

            RuleFor(x => x.Email).Custom((value, context) =>
            {
                if (dbContext.Users.Any(u => u.Email == value))
                {
                    context.AddFailure("Email", "That email is taken");
                }
            });
        }

    }
}