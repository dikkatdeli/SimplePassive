using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace SimplePassive.Client
{
    /// <summary>
    /// Client script that does the real hard work.
    /// </summary>
    public class Passive : BaseScript
    {
        #region Fields

        /// <summary>
        /// The activation of passive mode for specific players.
        /// </summary>
        public readonly Dictionary<int, bool> activations = new Dictionary<int, bool>();
        /// <summary>
        /// Set of collision detection and changes for each individual player.
        /// </summary>
        public readonly Dictionary<int, Collision> collisions = new Dictionary<int, Collision>();
        /// <summary>
        /// Print the entities changed during the next game tick.
        /// </summary>
        public bool printNextTick = false;

        #endregion

        #region Constructor

        public Passive()
        {
            // Add the exports
            Exports.Add("getActivation", new Func<bool>(() => GetPlayerActivation(Game.Player.ServerId)));
            Exports.Add("setActivation", new Action<bool>(SetPassiveActivation));
            // And tell the server that this client is ready to work
            TriggerServerEvent("simplepassive:initialized");
        }

        #endregion

        #region Tools

        /// <summary>
        /// Gets the activation of passive mode for a specific player.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns>True, False or the default value.</returns>
        public bool GetPlayerActivation(int player) => activations.ContainsKey(player) ? activations[player] : Convars.Default;

        #endregion

        #region Exports

        public void SetPassiveActivation(bool activation)
        {
            // Tell the server to change the activation of the current player
            if (Convars.Debug)
            {
                Debug.WriteLine($"Requesting server to change the activation to {activation}");
            }
            TriggerServerEvent("simplepassive:setPassive", activation);
        }

        #endregion

        #region Ticks

        /// <summary>
        /// Shows the text with the debugging information.
        /// </summary>
        [Tick]
        public async Task DebugText()
        {
            // If debug mode is disabled, just return
            if (!Convars.Debug)
            {
                return;
            }

            // Create a text for the debug mode
            string debugText = "Passive Players: ";
            // Iterate over the list of players and add their activations
            foreach (Player player in Players)
            {
                debugText += $" {player.ServerId} ({Convert.ToInt32(GetPlayerActivation(player.ServerId))})";
            }
            // Add the local activation onto the debug text
            debugText += $"\nLocal Status: {GetPlayerActivation(Game.Player.ServerId)}";
            // And draw it if the debug mode is enabled
            new Text(debugText, new PointF(0, 0), 0.5f).Draw();
        }

        /// <summary>
        /// Draws the entity handle on top of the entities.
        /// </summary>
        [Tick]
        public async Task DebugMarkers()
        {
            // If debug mode is disabled, just return
            if (!Convars.Debug)
            {
                return;
            }

            // Otherwise, iterate over the player
            foreach (Player player in Players)
            {
                // Get the correct parameters
                bool isLocal = player == Game.Player;
                bool activation = GetPlayerActivation(player.ServerId);

                // Get the entities of the player
                Ped ped = player.Character;
                Vehicle vehicle = ped.CurrentVehicle;
                Vehicle hooked = vehicle?.GetHookedVehicle();
                // And draw the correct markers
                ped?.DrawDebugMarker(isLocal, activation);
                vehicle?.DrawDebugMarker(isLocal, activation);
                hooked?.DrawDebugMarker(isLocal, activation);
            }
        }

        /// <summary>
        /// Applies the collision changes based on the passive mode activations.
        /// </summary>
        [Tick]
        public async Task ApplyCollisions()
        {
            // Get the activation of the local player
            bool local = GetPlayerActivation(Game.Player.ServerId);

            // Iterate over the list of players
            foreach (Player player in Players)
            {
                // Make sure that we skip the current player
                if (player == Game.Player)
                {
                    continue;
                }

                // Get the activation of the other player
                bool other = GetPlayerActivation(player.ServerId);

                // If the player does not has an entry for the collision changes, create a new object
                if (!collisions.ContainsKey(player.ServerId))
                {
                    collisions[player.ServerId] = new Collision(player);
                }

                // And tell the game to update the collisions
                collisions[player.ServerId].UpdateCollisions(other || local);
            }
        }

        /// <summary>
        /// Tick event that handles the collisions of Passive Mode.
        /// </summary>
        /// <returns></returns>
        /// [Tick]
        public async Task HandleCollisions()
        {
            // Create some references to the local player ped and vehicle
            Player localPlayer = Game.Player;
            Ped localPed = localPlayer.Character;
            Vehicle localVehicle = localPed.CurrentVehicle;
            Vehicle localHooked = localVehicle?.GetHookedVehicle();

            // Set the alpha of the player vehicle to maximum if is present
            if (localVehicle != null)
            {
                API.ResetEntityAlpha(localVehicle.Handle);
            }

            // Get the activation of the local player for later use
            bool localActivation = GetPlayerActivation(localPlayer.ServerId);

            // If the local player has passive mode enabled
            if (localActivation)
            {
                // If the player is not allowed to fight other players
                if (Convars.DisableCombat)
                {
                    // There are some values that we set on the activationChanged event
                    // If is not on this chunk, is probably on that event

                    // Disable the firing of weapons
                    API.DisablePlayerFiring(localPlayer.Handle, true);
                    // And disable the controls related to attacking
                    Game.DisableControlThisFrame(0, Control.MeleeAttack1);
                    Game.DisableControlThisFrame(0, Control.MeleeAttack2);
                    Game.DisableControlThisFrame(0, Control.Attack);
                    Game.DisableControlThisFrame(0, Control.Attack2);
                    Game.DisableControlThisFrame(0, Control.VehicleAttack);
                    Game.DisableControlThisFrame(0, Control.VehicleAttack2);
                    Game.DisableControlThisFrame(0, Control.VehiclePassengerAttack);
                    Game.DisableControlThisFrame(0, Control.VehicleFlyAttack);
                    Game.DisableControlThisFrame(0, Control.VehicleFlyAttack2);
                }
            }

            // On debug mode, draw markers on top of the player entities
            if (Convars.Debug)
            {
                /*
                localPed?.DrawDebugMarker(255, 255, 255);
                localVehicle?.DrawDebugMarker(255, 255, 255);
                localHooked?.DrawDebugMarker(255, 255, 255);
                */
            }

            // Then, iterate over the list of players
            foreach (Player player in Players)
            {
                // Get the correct activation for this player
                bool playerActivation = GetPlayerActivation(player.ServerId);
                bool disableCollisions = playerActivation || localActivation;

                // Save the ped and vehicle of the other player
                Ped otherPed = player.Character;
                Vehicle otherVehicle = otherPed.CurrentVehicle;
                Vehicle otherHooked = otherVehicle?.GetHookedVehicle();

                // If the player is the same as the local one, skip this iteration
                if (player == localPlayer)
                {
                    continue;
                }

                // Set the correct alpha for the other entities (just in case the resource restarted with passive enabled)
                int alpha = disableCollisions && !API.GetIsTaskActive(otherPed.Handle, 2) && localVehicle != otherVehicle ? Convars.Alpha : 255;
                otherPed.SetAlpha(alpha);
                otherVehicle?.SetAlpha(alpha);
                otherHooked?.SetAlpha(alpha);

                // If passive mode is activated by the other or local player
                if (disableCollisions)
                {
                    // If the other player is using a vehicle, we are seated on it and we are not the driver, continue
                    if (otherVehicle != null &&
                        API.IsPedInVehicle(otherVehicle.Handle, localPed.Handle, false) &&
                        otherVehicle.GetPedOnSeat(VehicleSeat.Driver) != localPed)
                    {
                        continue;
                    }

                    // Otherwise, disable the collisions

                    /*
                    // Local Player vs Other Player
                    localPed.DisableCollisionsThisFrame(otherPed, printNextTick);
                    // Local Player vs Other Vehicle (if present)
                    localPed.DisableCollisionsThisFrame(otherVehicle, printNextTick);
                    // Local Player vs Other Hooked (if present)
                    localPed.DisableCollisionsThisFrame(otherHooked, printNextTick);

                    // Local Vehicle vs Other Player
                    localVehicle?.DisableCollisionsThisFrame(otherPed, printNextTick);
                    // Local Vehicle vs Other Vehicle (if present)
                    localVehicle?.DisableCollisionsThisFrame(otherVehicle, printNextTick);
                    // Local Vehicle vs Other Hooked (if present)
                    localVehicle?.DisableCollisionsThisFrame(otherHooked, printNextTick);

                    // Local Hooked vs Other Player
                    localHooked?.DisableCollisionsThisFrame(otherPed, printNextTick);
                    // Local Hooked vs Other Vehicle (if present)
                    localHooked?.DisableCollisionsThisFrame(otherVehicle, printNextTick);
                    // Local Hooked vs Other Hooked (if present)
                    localHooked?.DisableCollisionsThisFrame(otherHooked, printNextTick);


                    // Other Player vs Local Player
                    otherPed.DisableCollisionsThisFrame(localPed, printNextTick);
                    // Other Player vs Local Vehicle (if present)
                    otherPed.DisableCollisionsThisFrame(localVehicle, printNextTick);
                    // Other Player vs Local Hooked (if present)
                    otherPed.DisableCollisionsThisFrame(localHooked, printNextTick);

                    // Other Vehicle vs Local Player
                    otherVehicle?.DisableCollisionsThisFrame(localPed, printNextTick);
                    // Other Vehicle vs Local Vehicle (if present)
                    otherVehicle?.DisableCollisionsThisFrame(localVehicle, printNextTick);
                    // Other Vehicle vs Local Hooked (if present)
                    otherVehicle?.DisableCollisionsThisFrame(localHooked, printNextTick);

                    // Other Hooked vs Local Player
                    otherHooked?.DisableCollisionsThisFrame(localPed, printNextTick);
                    // Other Hooked vs Local Vehicle (if present)
                    otherHooked?.DisableCollisionsThisFrame(localVehicle, printNextTick);
                    // Other Hooked vs Local Hooked (if present)
                    otherHooked?.DisableCollisionsThisFrame(localHooked, printNextTick);
                    */

                    // On debug mode, draw markers over the other player entities (if found)
                    if (Convars.Debug)
                    {
                        /*
                        otherPed?.DrawDebugMarker(100, 75, 80);
                        otherVehicle?.DrawDebugMarker(100, 75, 80);
                        otherHooked?.DrawDebugMarker(100, 75, 80);
                        */
                    }
                }
            }

            // Finally, disable the printing during the next tick (if enabled)
            printNextTick = false;
        }

        #endregion

        #region Network Events

        /// <summary>
        /// Saves the activation of passive mode for another player.
        /// </summary>
        /// <param name="handle">The Server Handle/ID of the player.</param>
        /// <param name="activation">The activation of that player.</param>
        [EventHandler("simplepassive:activationChanged")]
        public void ActivationChanged(int handle, bool activation)
        {
            // Just save the activation of the player
            activations[handle] = activation;
            if (Convars.Debug)
            {
                Debug.WriteLine($"Received Passive Activation of {handle} ({activation})");
            }

            // If the passive activation is for the current player
            if (handle == Game.Player.ServerId)
            {
                // Set the correct activation for drive by-s
                API.SetPlayerCanDoDriveBy(Game.Player.Handle, (!activation && Convars.DisableCombat) || !Convars.DisableCombat);
            }
        }

        /// <summary>
        /// Does some cleanup for a specific player.
        /// </summary>
        /// <param name="id">The ID of the player.</param>
        [EventHandler("simplepassive:doCleanup")]
        public void DoCleanup(int id)
        {
            // If there is an activation for the player, remove it
            if (activations.ContainsKey(id))
            {
                activations.Remove(id);
            }
            // And then do the same for collision activation
            if (collisions.ContainsKey(id))
            {
                collisions.Remove(id);
            }
        }

        #endregion

        #region Debug Commands

        /// <summary>
        /// Prints the collisions changed during the next tick.
        /// </summary>
        [Command("passiveprinttick")]
        public void ShowNextTickCommand()
        {
            // If debug mode is enabled, set printNextTick to true
            if (Convars.Debug)
            {
                printNextTick = true;
            }
        }
        /// <summary>
        /// Prints the entities known by the collision information.
        /// </summary>
        [Command("passiveentities")]
        public void EntitiesCommand(int source, List<object> args, string raw)
        {
            // If debug mode is disabled or no parameters were specified, return
            if (!Convars.Debug || args.Count == 0)
            {
                return;
            }
            // If the first argument is invalid, return
            if (!int.TryParse(args[0].ToString(), out int id))
            {
                return;
            }

            // Otherwise, get the player
            Player player = Players[id];
            // If is invalid, silently return
            if (player == null)
            {
                return;
            }

            // Otherwise, print the handles to the console
            collisions[player.Handle].PrintInfo();
        }

        #endregion
    }
}
