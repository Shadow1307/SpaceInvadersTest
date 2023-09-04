using System;
using System.Collections.Generic;
using TestingTaskFramework;
using VRageMath;

namespace TestingTask
{
    // TODO: World is really slow now, optimize it.
    // TODO: Fix excessive allocations during run.
    // TODO: Write body of 'PreciseCollision' method.
    class World : IWorld
    {
        List<WorldObject> m_objects = new List<WorldObject>();

        /// <summary>
        /// Time of the world, increased with each update.
        /// </summary>
        public TimeSpan Time { get; private set; }

        /// <summary>
        /// Adds new object into world.
        /// World is responsible for calling OnAdded method on object when object is added.
        /// </summary>
        public void Add(WorldObject obj)
        {
            obj.OnAdded(this);
            m_objects.Add(obj);
        }

        /// <summary>
        /// Removes object from world.
        /// World is responsible for calling OnRemoved method on object when object is removed.
        /// </summary>
        public void Remove(WorldObject obj)
        {
            m_objects.Remove(obj);
            obj.OnRemoved();
        }

        /// <summary>
        /// Called when object is moved in the world.
        /// </summary>
        public void OnObjectMoved(WorldObject obj, Vector3 displacement)
        {
        }

        /// <summary>
        /// Clears whole world and resets the time.
        /// </summary>
        public void Clear()
        {
            Time = TimeSpan.Zero;
            m_objects.Clear();
        }

        /// <summary>
        /// Queries the world for objects in a box. Matching objects are added into result list.
        /// Query should return all overlapping objects.
        /// </summary>
        public void Query(BoundingBox box, List<WorldObject> resultList)
        {
            foreach(var obj in m_objects)
            {
                if (obj.BoundingBox.Contains(box) != ContainmentType.Disjoint)
                    resultList.Add(obj);
            }
        }

        /// <summary>
        /// Updates the world in following order:
        /// 1. Increase time.
        /// 2. Call Update on all objects with NeedsUpdate flag.
        /// 3. Call PostUpdate on all objects with NeedsUpdate flag.
        /// PostUpdate on first object must be called when all other objects are Updated.
        /// </summary>
        public void Update(TimeSpan deltaTime)
        {
            Time += deltaTime;

            var count = m_objects.Count;
            for (int i = 0; i < count; i++)
            {
                var obj = m_objects[i];
                if (obj.NeedsUpdate)
                {
                    obj.Update(deltaTime);
                }
            }

            for (int i = 0; i < count; i++)
            {
                var obj = m_objects[i];
                if (obj.NeedsUpdate)
                {
                    obj.PostUpdate();
                }
            }
        }

        /// <summary>
        /// Calculates precise collision of two moving objects.
        /// Returns exact delta time of touch (e.g. 1 is one second in future from now).
        /// When objects are already touching or overlapping, returns zero. When the objects won't ever touch, returns positive infinity.
        /// </summary>
        public float PreciseCollision(WorldObject a, WorldObject b)
        {

            // Calculate relative position and relative velocity
            Vector3 relativePosition = a.Position - b.Position;
            Vector3 relativeVelocity = a.LinearVelocity - b.LinearVelocity;

            double aCoefficient = relativeVelocity.LengthSquared();
            if (aCoefficient == 0)
            {
                // Objects are not moving relative to each other, no collision will occur
                return float.PositiveInfinity;
            }

            double bCoefficient = Vector3.Dot(relativePosition, relativeVelocity);
            float cCoefficient = relativePosition.LengthSquared() - (a.BoundingRadius + b.BoundingRadius) * (a.BoundingRadius + b.BoundingRadius);

            double discriminant = bCoefficient * bCoefficient - aCoefficient * cCoefficient;

            if (discriminant < 0)
            {
                return float.PositiveInfinity;
            }

            double sqrtDiscriminant = Math.Sqrt(discriminant);

            // Calculate both possible collision times
            double t1 = (-bCoefficient + sqrtDiscriminant) / aCoefficient;
            double t2 = (-bCoefficient - sqrtDiscriminant) / aCoefficient;

            // Return the minimum positive time to collision
            if (t1 > 0 && t2 > 0)
            {
                return (float)Math.Min(t1, t2);
            }
            else if (t1 > 0)
            {
                return (float)t1;
            }
            else if (t2 > 0)
            {
                return (float)t2;
            }
            else
            {
                return float.PositiveInfinity; // No positive collision time
            }
        }
    }
}
