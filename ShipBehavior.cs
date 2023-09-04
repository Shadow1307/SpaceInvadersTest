using System;
using TestingTaskFramework;
using VRageMath;

namespace TestingTask
{
    // TODO: Modify 'OnUpdate' method, find asteroids in World (property Ship.World) and shoot them.
    class ShipBehavior : IShipBehavior
    {
        /// <summary>
        /// The ship which has this behavior.
        /// </summary>
        public Ship Ship { get; set; }

        public BoundingBox BoundingBox { get; set; }

        /// <summary>
        /// Called when ship is being updated, Ship property is never null when OnUpdate is called.
        /// </summary>
        public void OnUpdate()
        {
            
            //Can I shoot?
            if (Ship.CanShoot)
                return;

            //Get asteroids near me.
            Ship.World.Query(BoundingBox, );
            //get "best"
            //Can I shoot "best"? Using guninfo.


            Ship.Shoot(Vector3.Forward);
        }
    }
}
