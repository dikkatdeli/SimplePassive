using CitizenFX.Core;

namespace SimplePassive.Client
{
    public class Collision
    {
        #region Properties

        /// <summary>
        /// The player that owns this collision information.
        /// </summary>
        public Player Owner { get; }
        /// <summary>
        /// The vehicle known used by this player.
        /// </summary>
        public Vehicle Vehicle { get; private set; } = null;
        /// <summary>
        /// The last trailer or towed vehicle used by this pllayer.
        /// </summary>
        public Vehicle Hooked { get; private set; } = null;

        #endregion

        #region Constructor

        public Collision(Player player)
        {
            Owner = player;
        }

        #endregion
    }
}
