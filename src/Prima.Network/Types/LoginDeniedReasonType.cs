namespace Prima.Network.Types;

public enum LoginDeniedReasonType : byte
{
    IncorrectPassword = 0x00,
    SomeoneUsedYourAccount = 0x01,
    AccountBlocked = 0x02,
    CredentialsInvalid = 0x03,
    CommunicationProblem = 0x04,
    IgrConcurrencyLimit = 0x05,
    IgrTimeLimit = 0x06,
    IgrGeneralError = 0x07,
}
