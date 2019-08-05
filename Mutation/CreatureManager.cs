using System;
using System.Collections.Generic;
using System.Linq;

namespace Mutation
{
    public class CreatureManager
    {
        /// <summary>
        /// The creatures that are currently being simulated.
        /// </summary>
        private List<Creature> _creatures;

        /// <summary>
        /// The random number generator each Creature will have a reference to.
        /// </summary>
        private Random _random;

        /// <summary>
        /// The base creature models that this simulation is tracking.
        /// </summary>
        private Dictionary<int, Creature> _trackedCreatures;

        public CreatureManager(Random random)
        {
            _creatures = new List<Creature>();
            _trackedCreatures = new Dictionary<int, Creature>();
            _random = random;
        }

        /// <summary>
        /// The total number of creatures that are alive.
        /// </summary>
        public int TotalNumberOfCreatures => _creatures.Count(creature => creature.IsAlive);

        /// <summary>
        /// The number of living creatures with the given id.
        /// </summary>
        /// <param name="id">The id to use.</param>
        /// <returns>The number of living creatures in the simulation with the id.</returns>
        public int CountOfCreatures(int id)
        {
            return _creatures.Count(creature => creature.IsAlive && creature.Id == id);
        }

        public List<int> GetTrackedIds()
        {
            return _trackedCreatures.Keys.ToList();
        }

        /// <summary>
        /// Starts tracking the creature.
        /// </summary>
        /// <param name="creature">The creature to track.</param>
        public void AddCreatureToTrack(Creature creature)
        {
            if (!_trackedCreatures.ContainsKey(creature.Id))
            {
                creature.RandomGenerator = _random;
                _trackedCreatures.Add(creature.Id, creature);
            }
        }

        public void AddCreatureToSimulation(Creature creature)
        {
            if (!_trackedCreatures.ContainsKey(creature.Id))
            {
                AddCreatureToTrack(creature);
            }

            creature.RandomGenerator = _random;
            _creatures.Add(creature);
        }

        /// <summary>
        /// Updates all of the creatures. To be called each clock cycle.
        /// </summary>
        public void Update()
        {
            var creaturesToRemove = new List<Creature>();
            var creaturesToAdd = new List<Creature>();

            foreach (var creature in _creatures)
            {
                var result = creature.Update();

                // If the creature did nothing,
                if (result == 0)
                {
                    continue;
                }

                // If the creature died or replicated itself,
                if (result == creature.Id)
                {
                    if (!creature.IsAlive)
                    {
                        creaturesToRemove.Add(creature);
                    }
                    else
                    {
                        creaturesToAdd.Add(creature);
                    }
                }
                // If the creature mutated,
                else
                {
                    creaturesToAdd.Add(SpawnNewCreature(result));
                }
            }

            _creatures.RemoveAll(creature => creaturesToRemove.Contains(creature));
            _creatures.AddRange(creaturesToAdd);
        }

        /// <summary>
        /// Spawns a new creature with the given id, if the id is being tracked.
        /// </summary>
        /// <param name="idOfCreature">The id for the creature.</param>
        /// <returns>The generated creature if the id was being tracked.</returns>
        private Creature SpawnNewCreature(int idOfCreature)
        {
            if (!_trackedCreatures.ContainsKey(idOfCreature))
            {
                return null;
            }

            var trackedCreature = _trackedCreatures[idOfCreature];
            return new Creature(trackedCreature);
        }
    }
}