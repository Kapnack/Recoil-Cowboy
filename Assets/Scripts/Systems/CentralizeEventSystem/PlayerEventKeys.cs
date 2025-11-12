namespace Systems.CentralizeEventSystem
{
    public abstract class PlayerEventKeys
    {
        public const string Paused = "PAUSED";
        public const string Attack = "ATTACK";
        public const string LivesChange = "LIVES_CHANGE";
        public const string PointsChange = "POINTS_CHANGE";
        public const string BulletsChange = "BULLETS_CHANGE";
        public const string Dies = "DIES";
        public const string Reload = "RELOAD";
        public const string ReloadOvertime = "RELOAD_OVERTIME";
        public const string OnOneLive = "ON_ONE_LIVE";
        public const string NoLongerInvincible = "NO_LONGER_INVENCIBIBLE";
    }
}