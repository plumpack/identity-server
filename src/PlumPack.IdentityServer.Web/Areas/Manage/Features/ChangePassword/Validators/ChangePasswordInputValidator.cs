using FluentValidation;
using PlumPack.IdentityServer.Web.Areas.Manage.Features.ChangePassword.Models;

namespace PlumPack.IdentityServer.Web.Areas.Manage.Features.ChangePassword.Validators
{
    public class ChangePasswordInputValidator : AbstractValidator<ChangePasswordInputModel>
    {
        public ChangePasswordInputValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty();
            RuleFor(x => x.NewPassword).NotEmpty();
            RuleFor(x => x.NewPasswordConfirm).NotEmpty();
            RuleFor(x => x.NewPasswordConfirm).Equal(x => x.NewPassword)
                .WithMessage("The password confirmation is not the same.");
        }
    }
}