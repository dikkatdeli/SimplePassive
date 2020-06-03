﻿using CitizenFX.Core;

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
            if (KnownPed != otherPed)
            {
                KnownPed?.ChangeCollisions(localPed, true);
                KnownPed?.ChangeCollisions(localVehicle, true);
                KnownPed?.ChangeCollisions(localHooked, true);
                otherPed?.ChangeCollisions(localPed, enabled);
                otherPed?.ChangeCollisions(localVehicle, enabled);
                otherPed?.ChangeCollisions(localHooked, enabled);
                KnownPed = otherPed;
            }
            if (KnownVehicle != otherVehicle)
            {
                KnownVehicle?.ChangeCollisions(localPed, true);
                KnownVehicle?.ChangeCollisions(localVehicle, true);
                KnownVehicle?.ChangeCollisions(localHooked, true);
                otherVehicle?.ChangeCollisions(localPed, enabled);
                otherVehicle?.ChangeCollisions(localVehicle, enabled);
                otherVehicle?.ChangeCollisions(localHooked, enabled);
                KnownVehicle = otherVehicle;
            }
            if (KnownHooked != otherVehicle)
            {
                KnownHooked?.ChangeCollisions(localPed, true);
                KnownHooked?.ChangeCollisions(localVehicle, true);
                KnownHooked?.ChangeCollisions(localHooked, true);
                otherHooked?.ChangeCollisions(localPed, enabled);
                otherHooked?.ChangeCollisions(localVehicle, enabled);
                otherHooked?.ChangeCollisions(localHooked, enabled);
                KnownHooked = otherHooked;
            }
            if (lastPed != localPed)
            {
                lastPed = localPed;
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

            // And disable the collisions between them
            otherPed?.ChangeCollisions(localPed, enabled);
            otherPed?.ChangeCollisions(localVehicle, enabled);
            otherPed?.ChangeCollisions(localHooked, enabled);
            otherVehicle?.ChangeCollisions(localPed, enabled);
            otherVehicle?.ChangeCollisions(localVehicle, enabled);
            otherVehicle?.ChangeCollisions(localHooked, enabled);
            otherHooked?.ChangeCollisions(localPed, enabled);
            otherHooked?.ChangeCollisions(localVehicle, enabled);
            otherHooked?.ChangeCollisions(localHooked, enabled);

            // Once we have finished, save the entities
            KnownPed = otherPed;
            KnownVehicle = otherVehicle;
            KnownHooked = otherHooked;
        }

        #endregion
    }
}
