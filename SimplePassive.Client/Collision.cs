using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace SimplePassive.Client
{
    public class Collision
    {
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
        /// Last ped known by the local player.
        /// </summary>
        public Ped LocalKnownPed { get; private set; } = null;
        /// <summary>
        /// Last vehicle known by the local player.
        /// </summary>
        public Vehicle LocalKnownVehicle { get; private set; } = null;
        /// <summary>
        /// Last hooked entity known by the local player.
        /// </summary>
        public Vehicle LocalKnownHooked { get; private set; } = null;
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

            if (LocalKnownPed != localPed)
            {
                if (Convars.Debug)
                {
                    string message = $"Local Player changed the ped!";
                    Debug.WriteLine(message);
                    Screen.ShowNotification(message);
                }
                LocalKnownPed?.ChangeCollisions(otherPed, true);
                LocalKnownPed?.ChangeCollisions(otherVehicle, true);
                LocalKnownPed?.ChangeCollisions(otherHooked, true);
                localPed?.ChangeCollisions(otherPed, enabled);
                localPed?.ChangeCollisions(otherVehicle, enabled);
                localPed?.ChangeCollisions(otherHooked, enabled);
                LocalKnownPed = localPed;
            }
            if (LocalKnownVehicle != localVehicle)
            {
                if (Convars.Debug)
                {
                    string message = $"Local Player changed the vehicle!";
                    Debug.WriteLine(message);
                    Screen.ShowNotification(message);
                }
                LocalKnownVehicle?.ChangeCollisions(otherPed, true);
                LocalKnownVehicle?.ChangeCollisions(otherVehicle, true);
                LocalKnownVehicle?.ChangeCollisions(otherHooked, true);
                localVehicle?.ChangeCollisions(otherPed, enabled);
                localVehicle?.ChangeCollisions(otherVehicle, enabled);
                localVehicle?.ChangeCollisions(otherHooked, enabled);
                LocalKnownVehicle = localVehicle;
            }
            if (LocalKnownHooked != localHooked)
            {
                if (Convars.Debug)
                {
                    string message = $"Local Player changed the hooked vehicle!";
                    Debug.WriteLine(message);
                    Screen.ShowNotification(message);
                }
                LocalKnownHooked?.ChangeCollisions(otherPed, true);
                LocalKnownHooked?.ChangeCollisions(otherVehicle, true);
                LocalKnownHooked?.ChangeCollisions(otherHooked, true);
                localHooked?.ChangeCollisions(otherPed, enabled);
                localHooked?.ChangeCollisions(otherVehicle, enabled);
                localHooked?.ChangeCollisions(otherHooked, enabled);
                LocalKnownHooked = localHooked;
            }
        }
        /// <summary>
        /// Sets a specific activation for the collisions between the players.
        /// </summary>
        public void SetCollisions(bool enabled, bool log = true)
        {
            // Get all of the required entities
            Ped localPed = Game.Player.Character;
            Vehicle localVehicle = localPed.CurrentVehicle;
            Vehicle localHooked = localVehicle?.GetHookedVehicle();
            Ped otherPed = Owner.Character;
            Vehicle otherVehicle = otherPed.CurrentVehicle;
            Vehicle otherHooked = otherVehicle?.GetHookedVehicle();

            // Disable the collisions between them
            otherPed?.ChangeCollisions(localPed, enabled, log);
            otherPed?.ChangeCollisions(localVehicle, enabled, log);
            otherPed?.ChangeCollisions(localHooked, enabled, log);
            if (!(otherVehicle != null &&
                API.IsPedInVehicle(otherVehicle.Handle, localPed.Handle, false) &&
                otherVehicle.GetPedOnSeat(VehicleSeat.Driver) != localPed))
            {
                otherVehicle?.ChangeCollisions(localPed, enabled, log);
                otherVehicle?.ChangeCollisions(localVehicle, enabled, log);
                otherVehicle?.ChangeCollisions(localHooked, enabled, log);
            }
            otherHooked?.ChangeCollisions(localPed, enabled, log);
            otherHooked?.ChangeCollisions(localVehicle, enabled, log);
            otherHooked?.ChangeCollisions(localHooked, enabled, log);
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
        /// <summary>
        /// Sets the alpha of the entities of the other players based on the activation.
        /// </summary>
        /// <param name="activation">The current activation of this player.</param>
        public void SetAlpha(bool activation)
        {
            // Get all of the required entities
            Vehicle localVehicle = Game.Player.Character.CurrentVehicle;
            Ped otherPed = Owner.Character;
            Vehicle otherVehicle = otherPed.CurrentVehicle;
            Vehicle otherHooked = otherVehicle?.GetHookedVehicle();
            // Restore the alpha of the local vehicle
            localVehicle?.SetAlpha(255);
            // And set the correct alpha for the other entities
            int alpha = activation && !API.GetIsTaskActive(otherPed.Handle, 2) && localVehicle != otherVehicle ? Convars.Alpha : 255;
            otherPed.SetAlpha(alpha);
            otherVehicle?.SetAlpha(alpha);
            otherHooked?.SetAlpha(alpha);
        }
        /// <summary>
        /// Prints the handles of the entities in the console.
        /// </summary>
        public void PrintInfo()
        {
            Debug.WriteLine($"Local Known Ped is {(LocalKnownPed == null ? 0 : LocalKnownPed.Handle)}");
            Debug.WriteLine($"Local Known Vehicle is {(LocalKnownVehicle == null ? 0 : LocalKnownVehicle.Handle)}");
            Debug.WriteLine($"Local Known Hooked is {(LocalKnownHooked == null ? 0 : LocalKnownHooked.Handle)}");
            Debug.WriteLine($"Player Known Ped is {(PlayerKnownPed == null ? 0 : PlayerKnownPed.Handle)}");
            Debug.WriteLine($"Player Known Vehicle is {(PlayerKnownVehicle == null ? 0 : PlayerKnownVehicle.Handle)}");
            Debug.WriteLine($"Player Known Hooked is {(PlayerKnownHooked == null ? 0 : PlayerKnownHooked.Handle)}");
        }

        #endregion
    }
}
