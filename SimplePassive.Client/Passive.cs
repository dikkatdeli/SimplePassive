﻿using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;

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
        public readonly Dictionary<string, bool> activations = new Dictionary<string, bool>();
        /// <summary>
        /// If the local player has passive enabled or disabled.
        /// </summary>
        public bool localActivation = Convert.ToBoolean(API.GetConvarInt("simplepassive_default", 0));

        #endregion

        #region Network Events

        /// <summary>
        /// Saves the activation of passive mode for another player.
        /// </summary>
        /// <param name="handle">The Server Handle/ID of the player.</param>
        /// <param name="activation">The activation of that player.</param>
        [EventHandler("simplepassive:activationChanged")]
        public void ActivationChanged(string handle, bool activation)
        {
            // Just save the activation of the player
            activations[handle] = activation;
            Debug.WriteLine($"Received Passive Activation of {handle} ({activation})");
        }

        #endregion

        #region Debug Commands

#if DEBUG

        /// <summary>
        /// Lists the activations that the Client knows.
        /// </summary>
        [Command("listactivations")]
        public void ListActivationsCommand()
        {
            // Just iterate and print every single one of them
            Debug.WriteLine("Known Activations:");
            foreach (KeyValuePair<string, bool> activation in activations)
            {
                Debug.WriteLine($"{activation.Key} set to {activation.Value}");
            }
        }

#endif

        #endregion
    }
}