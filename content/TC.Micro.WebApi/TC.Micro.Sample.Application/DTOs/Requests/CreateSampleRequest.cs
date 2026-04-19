namespace TC.Micro.Sample.Application.DTOs.Requests;

/// <summary>创建 Sample 请求</summary>
public class CreateSampleRequest
{
    /// <summary>名称（必填，最长 100 字）</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>备注（可选）</summary>
    public string? Remark { get; set; }
}
