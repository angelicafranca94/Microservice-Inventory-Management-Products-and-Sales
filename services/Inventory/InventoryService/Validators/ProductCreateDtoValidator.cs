using FluentValidation;
using InventoryService.DTOs;

namespace InventoryService.Validators
{
    public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome do produto é obrigatório")
                .MaximumLength(100).WithMessage("O nome do produto deve ter no máximo 100 caracteres");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("O preço deve ser maior que zero");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("A quantidade não pode ser negativa");
        }
    }
}
