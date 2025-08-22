using FluentValidation;
using SalesService.DTOs;

namespace SalesService.Validators
{
    public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
    {
        public OrderCreateDtoValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("O pedido deve conter pelo menos um item");

            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.ProductId)
                    .NotEmpty().WithMessage("O ProductId é obrigatório");

                items.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero");
            });
        }
    }
}
