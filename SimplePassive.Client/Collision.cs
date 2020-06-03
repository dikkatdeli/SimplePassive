using CitizenFX.Core;

namespace SimplePassive.Client
{
    public class Collision
    {
        #region Private Fields

        /// <summary>
        /// Last ped known by the local player.
        /// </summary>
        private Ped lastPed = null;
        /// <summary>
        /// Last vehicle known by the local player.
        /// </summary>
        private Vehicle lastVehicle = null;
        /// <summary>
        /// Last hooked entity known by the local player.
        /// </summary>
        private Vehicle lastHooked = null;

        #endregion

        #region Public Properties

        /// <summary>
        /// The player that owns this collision information.
        /// </summary>
        public Player Owner { get; }
        /// <summary>
        /// The ped that is controlled by this player.
        /// </summary>
        public Ped KnownPed { get; private set; } = null;
        /// <summary>
        /// The vehicle known used by this player.
        /// </summary>
        public Vehicle KnownVehicle { get; private set; } = null;
        /// <summary>
        /// The last trailer or towed vehicle used by this pllayer.
        /// </summary>
        public Vehicle KnownHooked { get; private set; } = null;

        #endregion

        #region Constructor

        public Collision(Player player)
        {
            Owner = player;
        }

        #endregion
    }
}
