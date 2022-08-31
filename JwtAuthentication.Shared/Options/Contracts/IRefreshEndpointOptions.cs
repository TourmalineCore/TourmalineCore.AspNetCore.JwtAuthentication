namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options.Contracts
{
    public interface IRefreshEndpointOptions
    {
        public string RefreshEndpointRoute { get; set; }
    }
}
