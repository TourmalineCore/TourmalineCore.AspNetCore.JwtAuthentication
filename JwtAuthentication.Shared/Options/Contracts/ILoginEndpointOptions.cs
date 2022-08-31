namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options.Contracts
{
    public interface ILoginEndpointOptions
    {
        public string LoginEndpointRoute { get; set; }
    }
}
