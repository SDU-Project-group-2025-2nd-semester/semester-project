using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Resources;

namespace HeatManager.Views.ConfigPanel.Dialogs
{
    public partial class EditingDialog : Window, INotifyPropertyChanged
    {
        // Backing fields
        private bool _canChangeUnit = false;
        private string _unitName = string.Empty;
        private string _resource = string.Empty;
        private decimal _cost = 0m;
        private double _maxHeatProduction = 0.0;
        private double _maxElectricity = 0.0;
        private double _emissions = 0.0;
        private double _resourceConsumption = 0.0;

        // Properties
        public ProductionUnitBase UnitBase { get; private set; }
        public bool Confirmed { get; private set; }
        public ProductionUnitBase? Unit { get; private set; }

        public bool CanChangeUnit
        {
            get => _canChangeUnit;
            private set
            {
                if (_canChangeUnit != value)
                {
                    _canChangeUnit = value;
                    OnPropertyChanged();
                }
            }
        }

        public string UnitName
        {
            get => _unitName;
            set
            {
                if (_unitName != value)
                {
                    _unitName = value;
                    OnPropertyChanged();
                    IsUnitValid();
                }
            }
        }

        public string Resource
        {
            get => _resource;
            set
            {
                if (_resource != value)
                {
                    _resource = value;
                    OnPropertyChanged();
                    IsUnitValid();
                }
            }
        }

        public string Cost
        {
            get => _cost.ToString();
            set
            {
                if (decimal.TryParse(value, out var parsed))
                {
                    if (_cost != parsed)
                    {
                        _cost = parsed;
                        OnPropertyChanged();
                        IsUnitValid();
                    }
                }
            }
        }

        public string MaxHeatProduction
        {
            get => _maxHeatProduction.ToString();
            set
            {
                if (double.TryParse(value, out var parsed))
                {
                    if (_maxHeatProduction != parsed)
                    {
                        _maxHeatProduction = parsed;
                        OnPropertyChanged();
                        IsUnitValid();
                    }
                }
            }
        }

        public string MaxElectricity
        {
            get => _maxElectricity.ToString();
            set
            {
                if (double.TryParse(value, out var parsed))
                {
                    if (_maxElectricity != parsed)
                    {
                        _maxElectricity = parsed;
                        OnPropertyChanged();
                        IsUnitValid();
                    }
                }
            }
        }

        public string Emissions
        {
            get => _emissions.ToString();
            set
            {
                if (double.TryParse(value, out var parsed))
                {
                    if (_emissions != parsed)
                    {
                        _emissions = parsed;
                        OnPropertyChanged();
                        IsUnitValid();
                    }
                }
            }
        }

        public string ResourceConsumption
        {
            get => _resourceConsumption.ToString();
            set
            {
                if (double.TryParse(value, out var parsed))
                {
                    if (_resourceConsumption != parsed)
                    {
                        _resourceConsumption = parsed;
                        OnPropertyChanged();
                        IsUnitValid();
                    }
                }
            }
        }

        public List<string> ResourceList { get; } = new List<string>
        {
            "Gas",
            "Oil",
            "Electricity"
        };

        // Constructor
        public EditingDialog(ProductionUnitBase unit)
        {
            UnitBase = unit;
            _unitName = unit.Name;
            _cost = unit.Cost;
            _maxHeatProduction = unit.MaxHeatProduction;
            _resource = unit.Resource.Name;
            _resourceConsumption = unit.ResourceConsumption;
            _emissions = unit.Emissions;

            if (unit is ElectricityProductionUnit elecUnit)
            {
                _maxElectricity = elecUnit.MaxElectricity;
            }

            DataContext = this;

            InitializeComponent();
        }

        // Event handlers
        private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Confirmed = false;
            Close();
        }

        private void Edit_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_cost != 0 && _maxHeatProduction != 0.0 && _resourceConsumption != 0.0 && _unitName != "")
            {
                if (_maxElectricity == 0.0)
                {
                    Unit = new HeatProductionUnit
                    {
                        Name = _unitName,
                        Cost = _cost,
                        MaxHeatProduction = _maxHeatProduction,
                        ResourceConsumption = _resourceConsumption,
                        Resource = new Resource(_resource),
                        Emissions = _emissions
                    };
                }
                else
                {
                    Unit = new ElectricityProductionUnit
                    {
                        Name = _unitName,
                        Cost = _cost,
                        MaxHeatProduction = _maxHeatProduction,
                        MaxElectricity = _maxElectricity,
                        ResourceConsumption = _resourceConsumption,
                        Resource = new Resource(_resource),
                        Emissions = _emissions
                    };
                }

                Confirmed = true;
            }

            Close();
        }

        // Validation method
        private void IsUnitValid()
        {
            CanChangeUnit = !string.IsNullOrEmpty(UnitName)
                            && _cost != 0
                            && _maxHeatProduction != 0.0
                            && _resourceConsumption != 0.0
                            && !string.IsNullOrEmpty(Resource);
        }

        // INotifyPropertyChanged implementation
        public new event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
