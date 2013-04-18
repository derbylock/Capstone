public class Constants {
  public const int TEAM_NEUTRAL = -1;
  public const int TEAM_ONE = 0;
  public const int TEAM_TWO = 1;

  public const int RESOURCE_UPDATE_MAXIMUM = 0x1;
  public const int RESOURCE_UPDATE_CURRENT = 0x2;
  public const int RESOURCE_UPDATE_REGEN_RATE = 0x4;

  public const int WINID_CHAR_SELECT = 1;

  public const float MIN_RECAST_TIME = 0.2f;

  public static bool HasConstant(int target, int flag) {
    return (target & flag) == flag;
  }
}