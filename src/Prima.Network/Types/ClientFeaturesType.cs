namespace Prima.Network.Types;

/// <summary>
/// Defines the client features that can be enabled or disabled on the server.
/// </summary>
[Flags]
public enum ClientFeatureType : uint
{
    /// <summary>
    /// No features enabled (default).
    /// </summary>
    None = 0x00000000,

    /// <summary>
    /// T2A features (chat, regions).
    /// </summary>
    T2A = 0x00000001,

    /// <summary>
    /// Renaissance features.
    /// </summary>
    Renaissance = 0x00000002,

    /// <summary>
    /// Third Dawn features.
    /// </summary>
    ThirdDawn = 0x00000004,

    /// <summary>
    /// LBR features (skills, map).
    /// </summary>
    LBR = 0x00000008,

    /// <summary>
    /// AOS features (skills, map, spells, fightbook).
    /// </summary>
    AOS = 0x00000010,

    /// <summary>
    /// Enable 6th character slot.
    /// </summary>
    SixthCharacterSlot = 0x00000020,

    /// <summary>
    /// Samurai Empire features.
    /// </summary>
    SE = 0x00000040,

    /// <summary>
    /// Mondain's Legacy features (elven race, spells, skills).
    /// </summary>
    ML = 0x00000080,

    /// <summary>
    /// 8th age splash screen.
    /// </summary>
    EighthAgeSplash = 0x00000100,

    /// <summary>
    /// 9th age splash screen.
    /// </summary>
    NinthAgeSplash = 0x00000200,

    /// <summary>
    /// 10th age features.
    /// </summary>
    TenthAge = 0x00000400,

    /// <summary>
    /// Increased housing and bank storage.
    /// </summary>
    IncreasedStorage = 0x00000800,

    /// <summary>
    /// Enable 7th character slot.
    /// </summary>
    SeventhCharacterSlot = 0x00001000,

    /// <summary>
    /// Kingdom Reborn faces.
    /// </summary>
    KRFaces = 0x00002000,

    /// <summary>
    /// Trial account.
    /// </summary>
    TrialAccount = 0x00004000,

    /// <summary>
    /// Live account.
    /// </summary>
    LiveAccount = 0x00008000,

    /// <summary>
    /// Stygian Abyss features (gargoyle race, spells, skills).
    /// </summary>
    SA = 0x00010000,

    /// <summary>
    /// High Seas Adventure features.
    /// </summary>
    HSA = 0x00020000,

    /// <summary>
    /// Gothic housing tiles.
    /// </summary>
    GothicHousing = 0x00040000,

    /// <summary>
    /// Rustic housing tiles.
    /// </summary>
    RusticHousing = 0x00080000,

    /// <summary>
    /// Jungle housing tiles.
    /// </summary>
    JungleHousing = 0x00100000,

    /// <summary>
    /// Shadowguard housing tiles.
    /// </summary>
    ShadowguardHousing = 0x00200000,

    /// <summary>
    /// Time of Legends features.
    /// </summary>
    TOL = 0x00400000,

    /// <summary>
    /// Endless Journey account.
    /// </summary>
    EndlessJourney = 0x00800000,

    /// <summary>
    /// Common configurations for modern servers.
    /// </summary>
    ModernServer = T2A | Renaissance | ThirdDawn | LBR | AOS | SE | ML | SA | TOL |
                   SixthCharacterSlot | SeventhCharacterSlot | IncreasedStorage
}
