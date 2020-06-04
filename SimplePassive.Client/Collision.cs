using CitizenFX.Core;
using CitizenFX.Core.UI;

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
        /// The passive activation of this player.
        /// </summary>
        public bool LastActivation { get; set; }
        /// <summary>
        /// The ped that is controlled by this player.
        /// </summary>
        public Ped PlayerKnownPed { get; private set; } = null;
        /// <summary>
        /// The vehicle known used by this player.
        /// </summary>
        public Vehicle PlayerKnownVehicle { get; private set; } = null;
        /// <summary>
        /// The last trailer or towed vehicle used by this pllayer.
        /// </summary>
        public Vehicle PlayerKnownHooked { get; private set; } = null;

        #endregion

        #region Constructor

        public Collision(Player player)
        {
            Owner = player;
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Updates the collisions between the players.
        /// </summary>
        public void UpdateCollisions(bool enabled)
        {
            // If the last activation is not the same as the current one
            if (LastActivation != enabled)
            {
                if (Convars.Debug)
                {
                    Debug.WriteLine($"Activation changed from {LastActivation} to {enabled}");
                }
                // Enable or disable the collisions
                SetCollisions(enabled);
                // Save it and return
                LastActivation = enabled;
                return;
            }

            // Get all of the required entities
            Ped localPed = Game.Player.Character;
            Vehicle localVehicle = localPed.CurrentVehicle;
            Vehicle localHooked = localVehicle?.GetHookedVehicle();
            Ped otherPed = Owner.Character;
            Vehicle otherVehicle = otherPed.CurrentVehicle;
            Vehicle otherHooked = otherVehicle?.GetHookedVehicle();

            // If the previously known entities do not match the current ones
            // Enable the collisions against the old one and Disable them against the new one
            // Once that is done, save the new one
            if (PlayerKnownPed != otherPed)
            {
                if (Convars.Debug)
                {
                    string message = $"Player {Owner.Name} changed the ped!";
                    Debug.WriteLine(message);
                    Screen.ShowNotification(message);
                }
                PlayerKnownPed?.ChangeCollisions(localPed, true);
                PlayerKnownPed?.ChangeCollisions(localVehicle, true);
                PlayerKnownPed?.ChangeCollisions(localHooked, true);
                otherPed?.ChangeCollisions(localPed, enabled);
                otherPed?.ChangeCollisions(localVehicle, enabled);
                otherPed?.ChangeCollisions(localHooked, enabled);
                otherPed?.SetAlpha(enabled ? Convars.Alpha : 255);
                PlayerKnownPed = otherPed;
            }
            if (PlayerKnownVehicle != otherVehicle)
            {
                if (Convars.Debug)
                {
                    string message = $"Player {Owner.Name} changed the vehicle!";
                    Debug.WriteLine(message);
                    Screen.ShowNotification(message);
                }
                PlayerKnownVehicle?.ChangeCollisions(localPed, true);
                PlayerKnownVehicle?.ChangeCollisions(localVehicle, true);
                PlayerKnownVehicle?.ChangeCollisions(localHooked, true);
                otherVehicle?.ChangeCollisions(localPed, enabled);
                otherVehicle?.ChangeCollisions(localVehicle, enabled);
                otherVehicle?.ChangeCollisions(localHooked, enabled);
                otherVehicle?.SetAlpha(enabled ? Convars.Alpha : 255);
                PlayerKnownVehicle = otherVehicle;
            }
            if (PlayerKnownHooked != otherVehicle)
            {
                if (Convars.Debug)
                {
                    string message = $"Player {Owner.Name} changed the hooked vehicle!";
                    Debug.WriteLine(message);
                    Screen.ShowNotification(message);
                }
                PlayerKnownHooked?.ChangeCollisions(localPed, true);
                PlayerKnownHooked?.ChangeCollisions(localVehicle, true);
                PlayerKnownHooked?.ChangeCollisions(localHooked, true);
                otherHooked?.ChangeCollisions(localPed, enabled);
                otherHooked?.ChangeCollisions(localVehicle, enabled);
                otherHooked?.ChangeCollisions(localHooked, enabled);
                otherHooked?.SetAlpha(enabled ? Convars.Alpha : 255);
                PlayerKnownHooked = otherHooked;
            }

            if (lastPed != localPed)
            {
                if (Convars.Debug)
                {
                    string message = $"Local Player changed the ped!";
                    Debug.WriteLine(message);
                    Screen.ShowNotification(message);
                }
                lastPed?.ChangeCollisions(otherPed, true);
                lastPed?.ChangeCollisions(otherVehicle, true);
                lastPed?.ChangeCollisions(otherHooked, true);
                localPed?.ChangeCollisions(otherPed, enabled);
                localPed?.ChangeCollisions(otherVehicle, enabled);
                localPed?.ChangeCollisions(otherHooked, enabled);
                lastPed = localPed;
            }
            if (lastVehicle != localVehicle)
            {
                if (Convars.Debug)
                {
                    string message = $"Local Player changed the vehicle!";
                    Debug.WriteLine(message);
                    Screen.ShowNotification(message);
                }
                lastVehicle?.ChangeCollisions(otherPed, true);
                lastVehicle?.ChangeCollisions(otherVehicle, true);
                lastVehicle?.ChangeCollisions(otherHooked, true);
                localVehicle?.ChangeCollisions(otherPed, enabled);
                localVehicle?.ChangeCollisions(otherVehicle, enabled);
                localVehicle?.ChangeCollisions(otherHooked, enabled);
                lastVehicle = localVehicle;
            }
            if (lastHooked != localHooked)
            {
                if (Convars.Debug)
                {
                    string message = $"Local Player changed the hooked vehicle!";
                    Debug.WriteLine(message);
                    Screen.ShowNotification(message);
                }
                lastHooked?.ChangeCollisions(otherPed, true);
                lastHooked?.ChangeCollisions(otherVehicle, true);
                lastHooked?.ChangeCollisions(otherHooked, true);
                localHooked?.ChangeCollisions(otherPed, enabled);
                localHooked?.ChangeCollisions(otherVehicle, enabled);
                localHooked?.ChangeCollisions(otherHooked, enabled);
                lastHooked = localHooked;
            }
        }
        /// <summary>
        /// Sets a specific activation for the collisions between the players.
        /// </summary>
        public void SetCollisions(bool enabled)
        {
            // Get all of the required entities
            Ped localPed = Game.Player.Character;
            Vehicle localVehicle = localPed.CurrentVehicle;
            Vehicle localHooked = localVehicle?.GetHookedVehicle();
            Ped otherPed = Owner.Character;
            Vehicle otherVehicle = otherPed.CurrentVehicle;
            Vehicle otherHooked = otherVehicle?.GetHookedVehicle();

            // Disable the collisions between them
            otherPed?.ChangeCollisions(localPed, enabled);
            otherPed?.ChangeCollisions(localVehicle, enabled);
            otherPed?.ChangeCollisions(localHooked, enabled);
            otherVehicle?.ChangeCollisions(localPed, enabled);
            otherVehicle?.ChangeCollisions(localVehicle, enabled);
            otherVehicle?.ChangeCollisions(localHooked, enabled);
            otherHooked?.ChangeCollisions(localPed, enabled);
            otherHooked?.ChangeCollisions(localVehicle, enabled);
            otherHooked?.ChangeCollisions(localHooked, enabled);
            // And set the correct alpha
            int alpha = enabled ? Convars.Alpha : 255;
            otherPed?.SetAlpha(alpha);
            otherVehicle?.SetAlpha(alpha);
            otherHooked?.SetAlpha(alpha);

            // Once we have finished, save the entities
            PlayerKnownPed = otherPed;
            PlayerKnownVehicle = otherVehicle;
            PlayerKnownHooked = otherHooked;
        }

        #endregion
    }
}
