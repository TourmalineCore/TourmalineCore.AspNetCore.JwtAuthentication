namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling
{
    public enum ErrorTypes
    {
        IncorrectLoginOrPassword = 0,
        RefreshTokenNotFound,
        IncorrectActivationLink,
        DuplicateActivationLink,
        IncorrectResetPasswordLink,
        InvalidPasswordFormat,
        UserNotFound,
        ReCaptchaTokenIncorrect,
        RefreshTokenOrFingerprintNotFound,
        RefreshTokenIsNotInConfidenceInterval,
    }
}