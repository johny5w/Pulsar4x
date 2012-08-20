﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pulsar4X.Entities
{
    public class TaskForce
    {
        public Guid Id { get; set; }
        public Guid FactionId { get; set; }
        private Faction _faction;
        public Faction Faction
        {
            get { return _faction; }
            set
            {
                _faction = value;
                if (_faction != null) FactionId = _faction.Id;
            }
        }
        public string Name { get; set; }

        public List<Fleet> Fleets { get; set; } 
    }
}
