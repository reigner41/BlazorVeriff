namespace KYC.Services;

public interface IVeriffService
{
    Task<VeriffService.VeriffApiResponse> GenerateSession();
}