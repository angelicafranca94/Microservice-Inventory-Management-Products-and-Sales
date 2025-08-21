using FluentValidation;
using InventoryService.DTOs;

namespace InventoryService.Application.Validators
{
    public class ProductUpdateValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O Id é obrigatório");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome do produto é obrigatório")
                .MaximumLength(100).WithMessage("O nome não pode passar de 100 caracteres");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("A descrição não pode passar de 500 caracteres");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("O preço deve ser maior que zero");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("O estoque não pode ser negativo");
        }
    }
}
