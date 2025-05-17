using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Resources;

namespace HeatManager.Views.ConfigPanel.Dialogs
{
    public partial class AdditionDialog : Window, INotifyPropertyChanged
    {
        public bool Confirmed { get; private set; }

        private bool _canAddUnit = false;
        public bool CanAddUnit
        {
            get => _canAddUnit;
            private set
            {
                if (_canAddUnit != value)
                {
                    _canAddUnit = value;
                    OnPropertyChanged();
                }
            }
        }

        public ProductionUnitBase? Unit { get; private set; }

        private string _unitName = string.Empty;
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

        private string _resource = string.Empty;
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

        private decimal _cost = 0m;
        private double _maxHeatProduction = 0.0;
        private double _maxElectricity = 0.0;
        private double _emissions = 0.0;
        private double _resourceConsumption = 0.0;

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

        public AdditionDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Confirmed = false;
            Close();
        }

        private void Add_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_cost != 0 && _maxHeatProduction != 0.0 && _resourceConsumption != 0.0 && Name != "")
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

        private void IsUnitValid()
        {
            CanAddUnit = !string.IsNullOrEmpty(UnitName) && _cost != 0 && _maxHeatProduction != 0.0 && _resourceConsumption != 0.0 && !string.IsNullOrEmpty(Resource);
        }

        public new event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
