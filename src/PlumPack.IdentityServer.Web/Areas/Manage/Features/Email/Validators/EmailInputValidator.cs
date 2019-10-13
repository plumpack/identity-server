using FluentValidation;
using PlumPack.IdentityServer.Web.Areas.Manage.Features.Email.Models;
using PlumPack.Infrastructure;

namespace PlumPack.IdentityServer.Web.Areas.Manage.Features.Email.Validators
{
    public class EmailInputValidator : AbstractValidator<EmailInputModel>
    {
        public EmailInputValidator()
        {
            When(x => x.ChangeEmailAddress, () =>
            {
                RuleFor(x => x.NewEmail).NotEmpty();
                RuleFor(x => x.NewEmailConfirm).NotEmpty();
                RuleFor(x => x.NewEmailConfirm).Equal(x => x.NewEmail)
                    .WithMessage("The email confirmation is not the same.");
            });
        }
    }
}