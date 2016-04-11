﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulsar4X.ECSLib;
using System.Collections.ObjectModel;

namespace Pulsar4X.ViewModel
{
    public class CargoVM : ViewModelBase
    {
        private Entity _entity;
        private CargoDB _cargoDB;
        private GameVM _gameVM;
        private StaticDataStore _staicData;
        private Dictionary<Guid, MineralSD> _mineralDictionary = new Dictionary<Guid, MineralSD>();
        private ObservableDictionary<CargoDefinition, double> allEntitysCargo { get; } = new ObservableDictionary<CargoDefinition, double>();

        public ObservableDictionary<CargoType, double> CargoCapacity { get; } = new ObservableDictionary<CargoType, double>();

        public ObservableCollection<CargoData> CargoData { get; } = new ObservableCollection<CargoData>();
        private ObservableCollection<CargoData> allCargoData { get; } = new ObservableCollection<CargoData>();

        public bool ShowMins
        {
            get { return _showMinerals; }
            set { _showMinerals = value; OnPropertyChanged(); }
        }
        private bool _showMinerals;

        public bool ShowMats
        {
            get { return _showMats; }
            set { _showMats = value; OnPropertyChanged(); }
        }
        private bool _showMats;

        public bool ShowSpecies
        {
            get { return _showSpecies; }
            set { _showSpecies = value; OnPropertyChanged(); }
        }
        private bool _showSpecies;

        public bool ShowComponents
        {
            get { return _showComponents; }
            set { _showComponents = value; OnPropertyChanged(); }
        }
        private bool _showComponents;

        public bool ShowFighters
        {
            get { return _showFighters; }
            set { _showFighters = value; OnPropertyChanged(); }
        }
        private bool _showFighters;

        public bool ShowOrdnance
        {
            get { return _showOrdnance; }
            set { _showOrdnance = value; OnPropertyChanged(); }
        }
        private bool _showOrdnance;


        public bool StoresGeneral
        {
            get {
                bool stores = false;
                if (_cargoDB.CargoCapacity.ContainsKey(CargoType.General) && _cargoDB.CargoCapacity[CargoType.General] > 0 || _cargoDB.HasUnlimitedCapacity)
                    stores = true;
                return stores; }      
        }

        public bool StoresSpecies
        {
            get
            {
                bool stores = false;
                if (_cargoDB.CargoCapacity.ContainsKey(CargoType.Colonists) && _cargoDB.CargoCapacity[CargoType.Colonists] > 0 || _cargoDB.HasUnlimitedCapacity)
                    stores = true;
                return stores;
            }
        }

        public bool StoresFuel
        {
            get
            {
                bool stores = false;
                if (_cargoDB.CargoCapacity.ContainsKey(CargoType.Fuel) && _cargoDB.CargoCapacity[CargoType.Fuel] > 0 || _cargoDB.HasUnlimitedCapacity)
                    stores = true;
                return stores;
            }
        }

        public bool StoresOrdnance
        {
            get
            {
                bool stores = false;
                if (_cargoDB.CargoCapacity.ContainsKey(CargoType.Ordnance) && _cargoDB.CargoCapacity[CargoType.Ordnance] > 0 || _cargoDB.HasUnlimitedCapacity)
                    stores = true;
                return stores;
            }
        }

        public bool StoresTroops
        {
            get
            {
                bool stores = false;
                if (_cargoDB.CargoCapacity.ContainsKey(CargoType.Troops) && _cargoDB.CargoCapacity[CargoType.Troops] > 0 || _cargoDB.HasUnlimitedCapacity)
                    stores = true;
                return stores;
            }
        }

        private List<SortEnum> _SortOrder = new List<SortEnum>();

        public CargoVM(GameVM gameVM, Entity entity)
        {
            _staicData = gameVM.Game.StaticData;
            _cargoDB = entity.GetDataBlob<CargoDB>();
            
            foreach (var mineral in _staicData.Minerals)
            {
                _mineralDictionary.Add(mineral.ID, mineral);
            }
            _SortOrder.Add(SortEnum.ItemType);
            _SortOrder.Add(SortEnum.CargoType);
            _SortOrder.Add(SortEnum.ItemName);
            _SortOrder.Add(SortEnum.Amount);
            OnRefresh();
        }

        private void FilterAndSort()
        {
            CargoData.Clear();
            foreach (var item in allCargoData)
            {
                //TODO sorting
                if (item.IndustryType == IndustryType.Mining && ShowMins)
                {
                    CargoData.Add(item);
                }
            }
            if (CargoData.Count == 0)
            {
                CargoData.Add(new ViewModel.CargoData("", new CargoDefinition(_gameVM.Game, _staicData.Minerals[0]), 0));
            }
        }

        private void OnReOrder(SortEnum toTop)
        {
            _SortOrder.Remove(toTop);
            _SortOrder.Insert(0, toTop);                
        }

        private void OnRefresh()
        {
            allCargoData.Clear();
            foreach (var item in allEntitysCargo)
            {

                string name;
                switch (item.Key.IndustryType)
                {
                    case IndustryType.Mining:
                        name = _mineralDictionary[item.Key.ItemGuid].Name;
                        break;
                    case IndustryType.Refining:
                        name = _staicData.RefinedMaterials[item.Key.ItemGuid].Name;
                        break;
                    default: //all other cargoable items should be entites I think...
                        name = _gameVM.Game.GlobalManager.GetGlobalEntityByGuid(item.Key.ItemGuid).GetDataBlob<NameDB>().GetName(_gameVM.CurrentFaction);
                        break;
                }
                CargoData cargodat = new CargoData(name, item.Key, item.Value);
                allCargoData.Add(cargodat);
            }
            OnPropertyChanged(nameof(StoresGeneral));
            OnPropertyChanged(nameof(StoresSpecies));
            OnPropertyChanged(nameof(StoresFuel));
            OnPropertyChanged(nameof(StoresOrdnance));
            OnPropertyChanged(nameof(StoresTroops));

            FilterAndSort();
            
        }


    }

    public enum SortEnum
    {
        ItemType,
        CargoType,
        ItemName,
        Amount,
        SpaceRemaining
    }

    public class CargoData : ViewModelBase
    {
        private CargoDefinition _def;
        public CargoType CargoType { get { return _def.Type; }}
        public IndustryType IndustryType { get { return _def.IndustryType; }}

        private double Weight { get { return _def.Weight; }}

        public string Name { get; private set; }
        public double Amount { get { return _amount; } set { _amount = value; OnPropertyChanged(); } }

        private double _amount;
        public CargoData(string name, CargoDefinition def, double amount)
        {
            _def = def;
            Amount = amount;
            Name = name;            
         }       
    }
}