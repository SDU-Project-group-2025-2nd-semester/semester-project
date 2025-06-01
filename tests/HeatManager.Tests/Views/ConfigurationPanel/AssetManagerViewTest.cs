using Avalonia.Headless.XUnit;
using HeatManager.Views.ConfigPanel.Dialogs;
using HeatManager.Core.Models.Producers;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Models.Resources;
using Moq;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.SourceDataProviders;

namespace HeatManager.Tests.Views
{
    public class AssetManagerViewTest
    {
        private readonly Mock<IAssetManager> _mockAssetManager;
        private readonly Mock<ISourceDataProvider> _mockSourceDataProvider;
        private readonly Mock<IOptimizerSettings> _mockOptimizerSettings;
        private readonly Mock<IOptimizerStrategy> _mockOptimizerStrategy;
        private readonly DefaultOptimizer _optimizer;

        private readonly Resource _oil = new("Oil");
        private readonly Resource _gas = new("Gas");
        private readonly Resource _electricity = new("Electricity");

        private readonly HeatProductionUnit heatUnit = new HeatProductionUnit
        {
            Name = "Unit1",
            Cost = 500,
            MaxHeatProduction = 10,
            Emissions = 0.5,
            Resource = new Resource("Gas"),
            ResourceConsumption = 1.2
        };

        private readonly ElectricityProductionUnit elecUnit = new ElectricityProductionUnit
        {
            Name = "ElecUnit",
            Cost = 600,
            MaxElectricity = 5,
            Emissions = 0.3,
            Resource = new Resource("Electricity"),
            ResourceConsumption = 0.8
        };

        public AssetManagerViewTest()
        {
            _mockAssetManager = new Mock<IAssetManager>();
            _mockSourceDataProvider = new Mock<ISourceDataProvider>();
            _mockOptimizerSettings = new Mock<IOptimizerSettings>();
            _mockOptimizerStrategy = new Mock<IOptimizerStrategy>();

            _optimizer = new DefaultOptimizer(
                _mockAssetManager.Object,
                _mockSourceDataProvider.Object,
                _mockOptimizerSettings.Object,
                _mockOptimizerStrategy.Object);
        }

        [AvaloniaFact]
        public void AdditionDialog_Can_Add_Unit()
        {
            // Arrange
            var dialog = new AdditionDialog();

            // Act
            dialog.UnitName = "Test Unit";
            dialog.Cost = "100";
            dialog.MaxHeatProduction = "15";
            dialog.MaxElectricity = "0";
            dialog.ResourceConsumption = "1,0";
            dialog.Emissions = "5,0";
            dialog.Resource = "Gas";

            dialog.GetType().GetMethod("IsUnitValid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(dialog, null);

            var addClickMethod = dialog.GetType().GetMethod("Add_Click", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            addClickMethod?.Invoke(dialog, new object[] { null!, null! });

            // Assert
            Assert.True(dialog.Confirmed);
            Assert.NotNull(dialog.Unit);
            Assert.IsType<HeatProductionUnit>(dialog.Unit);
            Assert.Equal("Test Unit", dialog.Unit?.Name);
        }

        [AvaloniaFact]
        public void DeletionDialog_Shows_UnitName()
        {
            // Arrange
            var unitName = "Unit A";
            var dialog = new DeletionDialog(unitName);

            // Act
            var displayedName = dialog.UnitName;

            // Assert
            Assert.Equal(unitName, displayedName);
        }

        [AvaloniaFact]
        public void DeletionDialog_Cancel_Click_Sets_Confirmed_False()
        {
            // Arrange
            var dialog = new DeletionDialog("Test Unit");

            // Act
            var cancelClickMethod = typeof(DeletionDialog).GetMethod("Cancel_Click", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            cancelClickMethod?.Invoke(dialog, new object[] { null!, null! });

            // Assert
            Assert.False(dialog.Confirmed);
        }

        [AvaloniaFact]
        public void DeletionDialog_Delete_Click_Sets_Confirmed_True()
        {
            // Arrange
            var dialog = new DeletionDialog("Test Unit");

            // Act
            var deleteClickMethod = typeof(DeletionDialog).GetMethod("Delete_Click", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            deleteClickMethod?.Invoke(dialog, new object[] { null!, null! });

            // Assert
            Assert.True(dialog.Confirmed);
        }

        [AvaloniaFact]
        public void EditingDialog_Initializes_From_HeatProductionUnit()
        {
            // Arrange
            var unit = heatUnit;

            // Act
            var dialog = new EditingDialog(unit);

            // Assert
            Assert.Equal("Unit1", dialog.UnitName);
            Assert.Equal("500", dialog.Cost);
            Assert.Equal("10", dialog.MaxHeatProduction);
            Assert.Equal("Gas", dialog.Resource);
            Assert.Equal("1.2", dialog.ResourceConsumption.Replace(',', '.'));
            Assert.Equal("0.5", dialog.Emissions.Replace(',', '.'));
            Assert.Equal("0", dialog.MaxElectricity);
        }

        [AvaloniaFact]
        public void EditingDialog_Initializes_From_ElectricityProductionUnit()
        {
            // Arrange
            var unit = elecUnit;

            // Act
            var dialog = new EditingDialog(unit);

            // Assert
            Assert.Equal("ElecUnit", dialog.UnitName);
            Assert.Equal("5", dialog.MaxElectricity);
        }

        [AvaloniaFact]
        public void EditingDialog_CanChangeUnit_True_When_Valid_Inputs_Provided()
        {
            // Arrange
            var dialog = new EditingDialog(heatUnit)
            {
                UnitName = "Test",
                Cost = "300",
                MaxHeatProduction = "20",
                ResourceConsumption = "1",
                Resource = "Oil"
            };

            // Assert
            Assert.True(dialog.CanChangeUnit);
        }

        [AvaloniaFact]
        public void EditingDialog_Cancel_Click_Sets_Confirmed_False_And_Closes()
        {
            // Arrange
            var dialog = new EditingDialog(heatUnit);

            // Act
            var method = typeof(EditingDialog).GetMethod("Cancel_Click", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(dialog, new object[] { null!, null! });

            // Assert
            Assert.False(dialog.Confirmed);
        }

        [AvaloniaFact]
        public void EditingDialog_Edit_Click_Creates_HeatProductionUnit_When_MaxElectricity_Is_Zero()
        {
            // Arrange
            var dialog = new EditingDialog(heatUnit)
            {
                UnitName = "NewUnit",
                Cost = "500",
                MaxHeatProduction = "15",
                ResourceConsumption = "1.5",
                Resource = "Gas",
                Emissions = "0.2",
                MaxElectricity = "0"
            };

            // Act
            var method = typeof(EditingDialog).GetMethod("Edit_Click", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(dialog, new object[] { null!, null! });

            // Assert
            Assert.True(dialog.Confirmed);
            Assert.IsType<HeatProductionUnit>(dialog.Unit);
        }

        [AvaloniaFact]
        public void EditingDialog_Edit_Click_Creates_ElectricityProductionUnit_When_MaxElectricity_Is_Not_Zero()
        {
            // Arrange
            var dialog = new EditingDialog(elecUnit)
            {
                UnitName = "NewElecUnit",
                Cost = "700",
                MaxHeatProduction = "25",
                MaxElectricity = "5",
                ResourceConsumption = "1.1",
                Resource = "Electricity",
                Emissions = "0.4"
            };

            // Act
            var method = typeof(EditingDialog).GetMethod("Edit_Click", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(dialog, new object[] { null!, null! });

            // Assert
            Assert.True(dialog.Confirmed);
            Assert.IsType<ElectricityProductionUnit>(dialog.Unit);
        }
    }
}
