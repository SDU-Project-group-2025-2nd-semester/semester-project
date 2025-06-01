using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Resources;

namespace HeatManager.Views.ConfigPanel.Dialogs
{
    /// <summary>
    /// Dialog for adding a new production unit.
    /// Implements INotifyPropertyChanged for data binding.
    /// </summary>
    public partial class AdditionDialog : Window, INotifyPropertyChanged
    {
        // Backing fields for properties
        private bool _canAddUnit = false;
        private string _unitName = string.Empty;
        private string _resource = string.Empty;
        private decimal _cost = 0m;
        private double _maxHeatProduction = 0.0;
        private double _maxElectricity = 0.0;
        private double _emissions = 0.0;
        private double _resourceConsumption = 0.0;

        /// <summary>
        /// Indicates whether the dialog was confirmed (Add button pressed).
        /// </summary>
        public bool Confirmed { get; private set; }

        /// <summary>
        /// Indicates whether the input data is valid for adding a unit.
        /// </summary>
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

        /// <summary>
        /// The new production unit created by this dialog, or null if cancelled.
        /// </summary>
        public ProductionUnitBase? Unit { get; private set; }

        // Properties bound to UI inputs with validation triggering
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
                if (decimal.TryParse(value, out var parsed) && _cost != parsed)
                {
                    _cost = parsed;
                    OnPropertyChanged();
                    IsUnitValid();
                }
            }
        }

        public string MaxHeatProduction
        {
            get => _maxHeatProduction.ToString();
            set
            {
                if (double.TryParse(value, out var parsed) && _maxHeatProduction != parsed)
                {
                    _maxHeatProduction = parsed;
                    OnPropertyChanged();
                    IsUnitValid();
                }
            }
        }

        public string MaxElectricity
        {
            get => _maxElectricity.ToString();
            set
            {
                if (double.TryParse(value, out var parsed) && _maxElectricity != parsed)
                {
                    _maxElectricity = parsed;
                    OnPropertyChanged();
                    IsUnitValid();
                }
            }
        }

        public string Emissions
        {
            get => _emissions.ToString();
            set
            {
                if (double.TryParse(value, out var parsed) && _emissions != parsed)
                {
                    _emissions = parsed;
                    OnPropertyChanged();
                    IsUnitValid();
                }
            }
        }

        public string ResourceConsumption
        {
            get => _resourceConsumption.ToString();
            set
            {
                if (double.TryParse(value, out var parsed) && _resourceConsumption != parsed)
                {
                    _resourceConsumption = parsed;
                    OnPropertyChanged();
                    IsUnitValid();
                }
            }
        }

        /// <summary>
        /// List of available resources for selection.
        /// </summary>
        public List<string> ResourceList { get; } = new List<string>
        {
            "Gas",
            "Oil",
            "Electricity"
        };

        /// <summary>
        /// Constructor initializes component and data context.
        /// </summary>
        public AdditionDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Handles Cancel button click - closes dialog without adding.
        /// </summary>
        private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Confirmed = false;
            Close();
        }

        /// <summary>
        /// Handles Add button click - validates and creates new unit.
        /// </summary>
        private void Add_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_cost != 0 && _maxHeatProduction != 0.0 && _resourceConsumption != 0.0 && !string.IsNullOrEmpty(_unitName))
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

        /// <summary>
        /// Validates if current inputs are valid for enabling the Add button.
        /// </summary>
        private void IsUnitValid()
        {
            CanAddUnit = !string.IsNullOrEmpty(UnitName)
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