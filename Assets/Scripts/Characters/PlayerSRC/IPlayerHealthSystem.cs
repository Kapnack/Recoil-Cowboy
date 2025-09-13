namespace Characters.PlayerSRC
{
    public interface IPlayerHealthSystem : IHealthSystem
    {
        public bool Invincible { get; }
        public void InstantDead();
    }
}