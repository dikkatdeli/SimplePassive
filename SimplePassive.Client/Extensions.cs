using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System.Drawing;

namespace SimplePassive.Client
{
    /// <summary>
    /// Extensions for accessing some stuff quickly.
    /// </summary>
    public static class Extensions
    {
        #region Vehicle

        /// <summary>
        /// Gets the vehicle that is being hooked.
        /// </summary>
        /// <param name="vehicle">The cargobob, truck or towtruck.</param>
        /// <returns>The Vehicle that is being hooked, null if there is nothing.</returns>
        public static Vehicle GetHookedVehicle(this Vehicle vehicle)
        {
            // If the vehicle is invalid, return
            if (vehicle == null || !vehicle.Exists())
            {
                return null;
            }

            // Start by trying to get the vehicle attached as a trailer
            int trailer = 0;
            if (API.GetVehicleTrailerVehicle(vehicle.Handle, ref trailer))
            {
                return Entity.FromHandle(trailer) as Vehicle;
            }

            // Try to get a hooked cargobob vehicle and return it if there is somehing
            Vehicle cargobobHook = Entity.FromHandle(API.GetVehicleAttachedToCargobob(vehicle.Handle)) as Vehicle;
            if (cargobobHook != null && cargobobHook.Exists())
            {
                return cargobobHook;
            }

            // Then, try to get it as a tow truck and return it if it does
            Vehicle towHooked = Entity.FromHandle(API.GetEntityAttachedToTowTruck(vehicle.Handle)) as Vehicle;
            if (towHooked != null && towHooked.Exists())
            {
                return towHooked;
            }

            // If we got here, just send nothing
            return null;
        }

        #endregion

        #region Entities

        /// <summary>
        /// Draws a debug symbol on top of the entity.
        /// </summary>
        /// <param name="entity">The entity to use.</param>
        public static void DrawDebugMarker(this Entity entity, bool isPlayer, bool activated)
        {
            // If the entity does not exists, return
            if (entity == null || !entity.Exists())
            {
                return;
            }

            // If the entity is not on the screen, return
            if (!entity.IsOnScreen)
            {
                return;
            }

            // Create a place to store the color
            int r, g, b;
            // And select the correct one based on the parameters
            if (isPlayer)
            {
                // https://en.wikipedia.org/wiki/Shades_of_pink#Hot_pink
                r = 255;
                g = 105;
                b = 180;
            }
            else if (activated)
            {
                // https://en.wikipedia.org/wiki/Shades_of_green#Lime_green
                r = 50;
                g = 205;
                b = 50;
            }
            else
            {
                // https://en.wikipedia.org/wiki/Shades_of_yellow#Yellow_(RGB)_(X11_yellow)_(color_wheel_yellow)
                r = 255;
                g = 255;
                b = 0;
            }

            // Create a place to store the position of the entity
            Vector3 vector = entity.Position;
            // If this is a ped, get the position of the head
            if (entity is Ped ped)
            {
                vector = ped.Bones[Bone.SKEL_Head].Position;
            }

            // Convert the position to the screen
            PointF pos = Screen.WorldToScreen(vector);
            // And draw a text on the same position
            new Text(entity.Handle.ToString(), pos, 1)
            {
                Color = Color.FromArgb(r, g, b),
                Centered = true
            }.Draw();
        }
        /// <summary>
        /// Changes the collisions between two entities.
        /// </summary>
        /// <param name="one">The first entity.</param>
        /// <param name="two">The second entity.</param>
        public static void ChangeCollisions(this Entity one, Entity two, bool enabled)
        {
            // If one of the entities is null, return
            if (one == null || two == null)
            {
                return;
            }
            // Otherwise, enable the collisions by disabling them only during the next frame
            API.SetEntityNoCollisionEntity(one.Handle, two.Handle, !enabled);
            API.SetEntityNoCollisionEntity(two.Handle, one.Handle, !enabled);
            // And log it if required
            if (Convars.Debug)
            {
                string message = $"Collisions between {one.Handle} and {two.Handle} set to {!enabled}";
                Debug.WriteLine(message);
                Screen.ShowNotification(message);
            }
        }
        /// <summary>
        /// Sets the alpha of an entity.
        /// </summary>
        /// <param name="entity">The entity to change the alpha.</param>
        /// <param name="alpha">The alpha value to set.</param>
        public static void SetAlpha(this Entity entity, int alpha)
        {
            // If the alpha is higher or equal than 255, reset the alpha
            if (alpha >= 255)
            {
                API.ResetEntityAlpha(entity.Handle);
            }
            // Otherwise, set it as usual
            else
            {
                API.SetEntityAlpha(entity.Handle, alpha, 0);
            }
        }

        #endregion
    }
}
