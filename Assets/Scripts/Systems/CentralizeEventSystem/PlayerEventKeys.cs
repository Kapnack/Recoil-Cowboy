namespace Systems.CentralizeEventSystem
{
    public abstract class PlayerEventKeys
    {
        public const string Paused = "PAUSED";
        public const string Attack = "ATTACK";
        public const string LivesChange = "LIVES_CHANGE";
        public const string BulletsChange = "BULLETS_CHANGE";
        public const string Dies = "DIES";
        public const string Wins = "WINS";
        public const string Reload = "RELOAD";
        public const string ReloadOvertime = "RELOAD_OVERTIME";
        public const string InstantReload = "INSTANT_RELOAD";
    }
}