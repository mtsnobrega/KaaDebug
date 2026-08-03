using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Plants;

namespace KaaDebug.Views.Plants;

public partial class RegisterPlantPage : ContentPage
{
    private readonly IPlantsRegistrationService _registrationService;
    private readonly IPlantsCatalogService _speciesService; //catalgo de especies

    private PlantSpecies? _selectedSpecies;

    public RegisterPlantPage(IPlantsRegistrationService registrationService, IPlantsCatalogService speciesService)
    {
        InitializeComponent();
        _registrationService = registrationService;
        _speciesService = speciesService;
    }

    // ===================== SELEÇÃO DE ESPÉCIE =====================

    private async void OnSelectSpeciesTapped(object? sender, EventArgs e)
    {
        // Resolve a SelectSpeciesPage via DI para garantir que ela receba
        // o ISpeciesService corretamente configurado.
        var selectPage = Handler?.MauiContext?.Services.GetService<SelectSpeciesPage>()
            ?? new SelectSpeciesPage(_speciesService);

        selectPage.OnSpeciesPicked = OnSpeciesPicked;

        await Navigation.PushModalAsync(new NavigationPage(selectPage));
        //await Navigation.PushModalAsync(selectPage);

    }

    private void OnSpeciesPicked(PlantSpecies species)
    {
        _selectedSpecies = species;
        SelectedSpeciesLabel.Text = species.Name;
        SelectedSpeciesLabel.TextColor = Color.FromArgb("#2E4A30");

        if (SpeciesErrorLabel.IsVisible)
            SpeciesErrorLabel.IsVisible = false;

        PopulateIdealParameters(species.IdealParameters);
    }

    private void PopulateIdealParameters(SpeciesIdealParameters parameters)
    {
        SoilMoistureRangeLabel.Text = FormatRange(parameters.SoilMoisture);
        AirHumidityRangeLabel.Text = FormatRange(parameters.AirHumidity);
        TemperatureRangeLabel.Text = FormatRange(parameters.Temperature);
        LuminosityRangeLabel.Text = FormatRange(parameters.Luminosity);

        IdealParametersCard.IsVisible = true;
    }

    private static string FormatRange(IdealRange range) =>
        $"{range.Min:0.#} - {range.Max:0.#} {range.Unit}";

    // ===================== NOME DA PLANTA =====================

    private void OnPlantNameChanged(object? sender, TextChangedEventArgs e)
    {
        if (PlantNameErrorLabel.IsVisible)
            PlantNameErrorLabel.IsVisible = false;
    }

    // ===================== DISPOSITIVO (OPCIONAL) =====================

    private void OnAssociateDeviceToggled(object? sender, ToggledEventArgs e)
    {
        DeviceCodeSection.IsVisible = e.Value;

        // Ao desligar o switch, limpa o campo e qualquer erro pendente,
        // já que o código não será mais enviado no cadastro.
        if (!e.Value)
        {
            DeviceCodeEntry.Text = string.Empty;
            DeviceCodeErrorLabel.IsVisible = false;
        }
    }

    private void OnDeviceCodeChanged(object? sender, TextChangedEventArgs e)
    {
        if (DeviceCodeErrorLabel.IsVisible)
            DeviceCodeErrorLabel.IsVisible = false;
    }

    // ===================== SALVAR =====================

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (!ValidateFields())
            return;

        await SavePlantAsync();
    }

    private bool ValidateFields()
    {
        bool isValid = true;

        if (_selectedSpecies is null)
        {
            SpeciesErrorLabel.IsVisible = true;
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(PlantNameEntry.Text))
        {
            PlantNameErrorLabel.IsVisible = true;
            isValid = false;
        }

        // Código de dispositivo só é obrigatório se o switch estiver ligado
        if (AssociateDeviceSwitch.IsToggled && string.IsNullOrWhiteSpace(DeviceCodeEntry.Text))
        {
            DeviceCodeErrorLabel.IsVisible = true;
            isValid = false;
        }

        return isValid;
    }

    private async Task SavePlantAsync()
    {
        SetLoadingState(true);

        try
        {
            var request = new CreatePlantRequest
            {
                Name = PlantNameEntry.Text!.Trim(),
                SpeciesId = _selectedSpecies!.Id,
                DeviceCode = AssociateDeviceSwitch.IsToggled
                    ? DeviceCodeEntry.Text!.Trim()
                    : null
            };

            var result = await _registrationService.CreatePlantAsync(request);

            if (!result.Success)
            {
                ShowGeneralError(result.ErrorMessage ?? "Não foi possível cadastrar a planta. Tente novamente.");
                return;
            }

            // Conforme o fluxo definido: Salvar ? Detalhes da Planta,
            // independentemente de ter associado dispositivo ou não.
            await Shell.Current.GoToAsync($"PlantDetails?plantId={result.PlantId}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro inesperado ao cadastrar planta: {ex.Message}");
            ShowGeneralError("Ocorreu um erro inesperado. Tente novamente.");
        }
        finally
        {
            SetLoadingState(false);
        }
    }

    private void SetLoadingState(bool isLoading)
    {
        SaveButton.IsEnabled = !isLoading;
        SaveButton.Text = isLoading ? string.Empty : "Salvar";
        SaveLoadingIndicator.IsVisible = isLoading;
        SaveLoadingIndicator.IsRunning = isLoading;

        PlantNameEntry.IsEnabled = !isLoading;
        DeviceCodeEntry.IsEnabled = !isLoading;
        AssociateDeviceSwitch.IsEnabled = !isLoading;
    }

    private void ShowGeneralError(string message)
    {
        GeneralErrorLabel.Text = message;
        GeneralErrorBorder.IsVisible = true;
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//PlantsList");
    }
}