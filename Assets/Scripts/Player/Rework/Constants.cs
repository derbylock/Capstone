public class Constants {
  public const int TEAM_NEUTRAL = -1;
  public const int TEAM_ONE = 0;
  public const int TEAM_TWO = 1;

  public const int RESOURCE_UPDATE_MAXIMUM = 0x1; // Renamed RSC_UPDATE_MAXIMUM
  public const int RESOURCE_UPDATE_CURRENT = 0x2; // Renamed RSC_UPDATE_CURRENT
  public const int RESOURCE_UPDATE_REGEN_RATE = 0x4;  // Renamed RSC_UDATE_REGEN_RATE
  
  public const int RSC_UPDATE_MAXIMUM = 0x1;
  public const int RSC_UPDATE_CURRENT = 0x2;
  public const int RSC_UPDATE_REGEN_RATE = 0x4;
  public const float RSC_HEALTH_DEFAULT = 1000f;
  public const float RSC_MANA_DEFAULT = 500f;
  public const float RSC_MANA_REGEN_DEFAULT = 45f;

  public const int WINID_CHAR_SELECT = 1;
  public const int WINID_DEV_MOUSE_SENS = 2;
  public const int WINID_DEV_DEBUG = 3;

  public const float MIN_RECAST_TIME = 0.2f;

  public static bool HasConstant(int target, int flag) {
    return (target & flag) == flag;
  }
  
  public static string[] PLAYABLE_LEVELS = {  "DungeonTest",
                                              "MP_TestLevel"  };
}