using System;
using FluentValidation;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Services.Validations
{
    public class CropValidator : AbstractValidator<CropViewModel>
    {
        public CropValidator()
        {
            RuleFor(x => x.CropName)
                .NotEmpty().WithMessage("Crop name is required.")
                .MaximumLength(100).WithMessage("Crop name must not exceed 100 characters.")
                .Matches(@"^[a-zA-Z\s\-]+$").WithMessage("Crop name can only contain letters, spaces and hyphens.");

            RuleFor(x => x.CropType)
                .NotEmpty().WithMessage("Crop type is required.");

            RuleFor(x => x.FieldId)
                .GreaterThan(0).WithMessage("Please select a valid field.");

            RuleFor(x => x.PlantingDate)
                .NotEmpty().WithMessage("Planting date is required.")
                .LessThan(x => x.ExpectedHarvestDate)
                .WithMessage("Planting date must be before expected harvest date.");

            RuleFor(x => x.ExpectedHarvestDate)
                .NotEmpty().WithMessage("Expected harvest date is required.")
                .GreaterThan(DateTime.Today)
                .WithMessage("Expected harvest date must be in the future.");
        }
    }

    public class FieldValidator : AbstractValidator<FieldViewModel>
    {
        public FieldValidator()
        {
            RuleFor(x => x.FieldName)
                .NotEmpty().WithMessage("Field name is required.")
                .MaximumLength(100).WithMessage("Field name must not exceed 100 characters.");

            RuleFor(x => x.AreaHectares)
                .GreaterThan(0).WithMessage("Area must be greater than 0.");

            RuleFor(x => x.SoilType)
                .NotEmpty().WithMessage("Soil type is required.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.");
        }
    }

    public class ScheduleValidator : AbstractValidator<ScheduleViewModel>
    {
        public ScheduleValidator()
        {
            RuleFor(x => x.CropId)
                .GreaterThan(0).WithMessage("Please select a crop.");

            RuleFor(x => x.ScheduledDate)
                .NotEmpty().WithMessage("Scheduled date is required.")
                .GreaterThan(DateTime.Today)
                .WithMessage("Scheduled date must be in the future.");

            RuleFor(x => x.ExpectedYieldKg)
                .GreaterThan(0).WithMessage("Expected yield must be greater than 0.");
        }
    }

    public class InventoryValidator : AbstractValidator<InventoryViewModel>
    {
        public InventoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Resource name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");

            RuleFor(x => x.Unit)
                .NotEmpty().WithMessage("Unit is required.");
        }
    }
}
