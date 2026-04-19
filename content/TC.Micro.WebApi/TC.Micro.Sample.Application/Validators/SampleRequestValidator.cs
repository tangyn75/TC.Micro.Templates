using FluentValidation;
using TC.Micro.Sample.Application.DTOs.Requests;

namespace TC.Micro.Sample.Application.Validators;

/// <summary>CreateSampleRequest 验证器</summary>
public class CreateSampleRequestValidator : AbstractValidator<CreateSampleRequest>
{
    public CreateSampleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("名称不能为空")
            .MaximumLength(100).WithMessage("名称最长 100 个字符");

        RuleFor(x => x.Remark)
            .MaximumLength(500).WithMessage("备注最长 500 个字符")
            .When(x => x.Remark != null);
    }
}

/// <summary>UpdateSampleRequest 验证器</summary>
public class UpdateSampleRequestValidator : AbstractValidator<UpdateSampleRequest>
{
    public UpdateSampleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("名称不能为空")
            .MaximumLength(100).WithMessage("名称最长 100 个字符");

        RuleFor(x => x.Remark)
            .MaximumLength(500).WithMessage("备注最长 500 个字符")
            .When(x => x.Remark != null);
    }
}
