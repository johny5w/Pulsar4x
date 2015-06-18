﻿using System.Collections.Generic;
using System.Linq;

namespace Pulsar4X.ECSLib
{
    public static class FactionFactory
    {
        public static Entity CreateFaction(EntityManager globalManager, string factionName)
        {

            List<BaseDataBlob> blobs = new List<BaseDataBlob>();
            NameDB name = new NameDB();
            FactionDB factionDB = new FactionDB();
            FactionAbilitiesDB factionAbilitiesDB = new FactionAbilitiesDB();
            TechDB techDB = new TechDB(StaticDataManager.StaticDataStore.Techs.Values.ToList());
            blobs.Add(name);
            blobs.Add(factionDB);
            blobs.Add(factionAbilitiesDB);
            blobs.Add(techDB);
            Entity factionEntity = new Entity(globalManager, blobs);

            //factionEntity didn't exsist when we created the NameDB, so we have to recreate the name dictionary here.
            name.Name = new JDictionary<Entity, string>() { { factionEntity, factionName } };
            
            return factionEntity;
        }
    }
}