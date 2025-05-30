using FluentValidation;
using JuniorFeedbackTask.Models;

namespace JuniorFeedbackTask.Validators
{
    public class FeedbackValidator: AbstractValidator<FeedbackDto>
    {
        public FeedbackValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("İsim alanı boş bırakılamaz.");
            RuleFor(x => x.Name).MinimumLength(2).WithMessage("İsim alanı 2 karakterden fazla olmalıdır.");
            RuleFor(x => x.Name).MaximumLength(20).WithMessage("İsim alanı 20 karakterden az olmalıdır.");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email alanı boş bırakılamaz.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Lütfen geçerli bir mail adresi giriniz.");

            RuleFor(x => x.Message).NotEmpty().WithMessage("Mesaj alanı boş bırakılamaz.");

        }
    }
}
