namespace TC.Micro.SampleAdmin.Client.Models;

// ── 认证 ──────────────────────────────────────────────────────────────────

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string? AccessToken  { get; set; }
    public string? RefreshToken { get; set; }
    public long    ExpiresAt    { get; set; }
}

// ── 通用分页 ───────────────────────────────────────────────────────────────

public class PagedResult<T>
{
    public List<T> Items      { get; set; } = new();
    public int     TotalCount { get; set; }
    public int     PageIndex  { get; set; }
    public int     PageSize   { get; set; }
    public bool    HasNext    => PageIndex * PageSize < TotalCount;
}

// ── Gateway ────────────────────────────────────────────────────────────────

public class GatewayServiceDto
{
    public string Id          { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string Status      { get; set; } = string.Empty;
    public int    InstanceCount { get; set; }
}

public class GatewayRouteDto
{
    public string Id          { get; set; } = string.Empty;
    public string RouteId     { get; set; } = string.Empty;
    public string Path        { get; set; } = string.Empty;
    public string ClusterId   { get; set; } = string.Empty;
    public bool   IsPublished { get; set; }
}

// ── Identity ───────────────────────────────────────────────────────────────

public class UserDto
{
    public string Id          { get; set; } = string.Empty;
    public string TenantId    { get; set; } = string.Empty;
    public string Username    { get; set; } = string.Empty;
    public string? Email      { get; set; }
    public string? DisplayName { get; set; }
    public bool   IsActive    { get; set; }
    public long   CreatedAt   { get; set; }
}

public class RoleDto
{
    public string Id          { get; set; } = string.Empty;
    public string TenantId    { get; set; } = string.Empty;
    public string Name        { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int    UserCount   { get; set; }
}

public class TenantDto
{
    public string Id          { get; set; } = string.Empty;
    public string Code        { get; set; } = string.Empty;
    public string Name        { get; set; } = string.Empty;
    public bool   IsActive    { get; set; }
    public long   CreatedAt   { get; set; }
}

public class AbacPolicyDto
{
    public string Id          { get; set; } = string.Empty;
    public string TenantId    { get; set; } = string.Empty;
    public string Name        { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool   IsEnabled   { get; set; }
}
