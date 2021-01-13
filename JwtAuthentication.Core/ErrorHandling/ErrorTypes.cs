namespace JwtAuthentication.Core.ErrorHandling
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
        RefreshTokenOrFingreprintNotFound
    }
}