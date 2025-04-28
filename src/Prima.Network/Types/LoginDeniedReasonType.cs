namespace Prima.Network.Types;

/// <summary>
/// Defines the possible reasons why a login attempt was denied.
/// Used in the LoginDenied packet to communicate authentication failures.
/// </summary>
public enum LoginDeniedReasonType : byte
{
    /// <summary>
    /// The password provided for the account is incorrect.
    /// </summary>
    IncorrectPassword = 0x00,

    /// <summary>
    /// The account is already in use by another client.
    /// </summary>
    SomeoneUsedYourAccount = 0x01,

    /// <summary>
    /// The account has been blocked or banned from accessing the server.
    /// </summary>
    AccountBlocked = 0x02,

    /// <summary>
    /// The account credentials (username/password) are invalid or do not exist.
    /// </summary>
    CredentialsInvalid = 0x03,

    /// <summary>
    /// There was a communication problem between the client and server.
    /// </summary>
    CommunicationProblem = 0x04,

    /// <summary>
    /// IGR (In-Game Rating) concurrent user limit reached.
    /// </summary>
    IgrConcurrencyLimit = 0x05,

    /// <summary>
    /// IGR (In-Game Rating) time limit reached.
    /// </summary>
    IgrTimeLimit = 0x06,

    /// <summary>
    /// IGR (In-Game Rating) general error occurred.
    /// </summary>
    IgrGeneralError = 0x07,
}
