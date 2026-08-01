using KaaDebug.Core.Interfaces.Plants;
using KaaDebug.Core.Models.Plants;

namespace KaaDebug.Views.Plants;

[QueryProperty(nameof(PlantId), "plantId")]
public partial class PlantsEditPage : ContentPage
{
    private readonly IPlantDetailsService _detailsService;
    private readonly IPlantsEditService _editService;

    private string? _plantId;
    private PlantDetails? _currentDetails;

    /// <summary>
    /// Flag que controla se o usuário pediu para REMOVER o dispositivo atual.
    /// Verdadeiro = ao salvar, enviaremos DeviceCode = "" (string vazia)
    /// para o backend, sinalizando desassociação.
    /// </summary>
    private bool _pendingDeviceRemoval;

    public string PlantId
    {
        get => _plantId ?? string.Empty;
        set => _plantId = value;
    }

    public PlantsEditPage(IPlantDetailsService detailsService, IPlantsEditService editService)
    {
        InitializeComponent();
        _detailsService = detailsService;
        _editService = editService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (string.IsNullOrWhiteSpace(_plantId))
        {
            ShowState(loading: false, error: true, form: false);
            return;
        }

        await LoadPlantDataAsync();
    }

    private async void OnRetryClicked(object? sender, EventArgs e) =>
        await LoadPlantDataAsync();

    // ===================== CARREGAMENTO DOS DADOS ATUAIS =====================

    private async Task LoadPlantDataAsync()
    {
        ShowState(loading: true, error: false, form: false);

        try
        {
            var result = await _detailsService.GetPlantDetailsAsync(_plantId!);

            if (!result.Success || result.Details is null)
            {
                ShowState(loading: false, error: true, form: false);
                return;
            }

            _currentDetails = result.Details;
            PreFillForm(_currentDetails);
            ShowState(loading: false, error: false, form: true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar dados para edição: {ex.Message}");
            ShowState(loading: false, error: true, form: false);
        }
    }

    private void PreFillForm(PlantDetails details)
    {
        // Espécie: somente leitura
        SpeciesNameLabel.Text = details.Species;

        // Nome: editável
        PlantNameEntry.Text = details.Name;

        // Dispositivo: mostra o card do atual se existir, ou seção de associação direta
        _pendingDeviceRemoval = false;

        var hasDevice = details.Device.ConnectionStatus != DeviceConnectionStatus.NotAssociated
                        && !string.IsNullOrEmpty(details.Device.DeviceCode);

        if (hasDevice)
        {
            CurrentDeviceCodeLabel.Text = details.Device.DeviceCode;
            CurrentDeviceCard.IsVisible = true;

            // Quando já tem dispositivo, o campo "novo" fica como "substituir"
            NewDeviceSectionTitle.Text = "Substituir por outro dispositivo (opcional)";
            NewDeviceCodeEntry.Placeholder = "Código do novo dispositivo (opcional)";
        }
        else
        {
            CurrentDeviceCard.IsVisible = false;
            NewDeviceSectionTitle.Text = "Associar dispositivo (opcional)";
            NewDeviceCodeEntry.Placeholder = "Ex: ESP32-0001 (opcional)";
        }

        RemoveDeviceWarningBorder.IsVisible = false;
    }

    private void ShowState(bool loading, bool error, bool form)
    {
        LoadingStateLayout.IsVisible = loading;
        ErrorStateLayout.IsVisible = error;
        FormLayout.IsVisible = form;
    }

    // ===================== INTERAÇÕES DO FORMULÁRIO =====================

    private void OnPlantNameChanged(object? sender, TextChangedEventArgs e)
    {
        if (PlantNameErrorLabel.IsVisible)
            PlantNameErrorLabel.IsVisible = false;
        if (GeneralErrorBorder.IsVisible)
            GeneralErrorBorder.IsVisible = false;
    }

    private void OnDeviceCodeChanged(object? sender, TextChangedEventArgs e)
    {
        if (DeviceCodeErrorLabel.IsVisible)
            DeviceCodeErrorLabel.IsVisible = false;
    }

    private void OnRemoveDeviceTapped(object? sender, EventArgs e)
    {
        _pendingDeviceRemoval = true;

        // Oculta o card do dispositivo atual e exibe o aviso de remoção pendente
        CurrentDeviceCard.IsVisible = false;
        RemoveDeviceWarningBorder.IsVisible = true;

        // Muda o rótulo da seção para indicar que agora é "associar novo"
        NewDeviceSectionTitle.Text = "Associar novo dispositivo (opcional)";
        NewDeviceCodeEntry.Placeholder = "Ex: ESP32-0001 (opcional)";
    }

    // ===================== SALVAR =====================

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (!ValidateForm())
            return;

        await SaveChangesAsync();
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(PlantNameEntry.Text))
        {
            PlantNameErrorLabel.IsVisible = true;
            return false;
        }
        return true;
    }

    private async Task SaveChangesAsync()
    {
        SetSaveLoadingState(true);

        try
        {
            // Lógica de resolução do DeviceCode enviado ao backend:
            // - Remoção pendente + sem novo código = "" (desassociar)
            // - Novo código digitado = novo código (associar/substituir)
            // - Nenhuma alteração = null (manter o que está)
            string? resolvedDeviceCode;

            var newCode = NewDeviceCodeEntry.Text?.Trim();
            if (!string.IsNullOrEmpty(newCode))
            {
                resolvedDeviceCode = newCode;         // substituir/associar
            }
            else if (_pendingDeviceRemoval)
            {
                resolvedDeviceCode = string.Empty;    // desassociar
            }
            else
            {
                resolvedDeviceCode = null;            // sem alteração
            }

            var request = new EditPlantRequest
            {
                PlantId = _plantId!,
                Name = PlantNameEntry.Text!.Trim(),
                DeviceCode = resolvedDeviceCode
            };

            var result = await _editService.UpdatePlantAsync(request);

            if (!result.Success)
            {
                // Erro de dispositivo: destaca o campo de código, não o erro geral
                if (result.ErrorMessage?.Contains("dispositivo") == true)
                {
                    DeviceCodeErrorLabel.Text = result.ErrorMessage;
                    DeviceCodeErrorLabel.IsVisible = true;
                }
                else
                {
                    ShowGeneralError(result.ErrorMessage ?? "Não foi possível salvar as alterações.");
                }
                return;
            }

            // Volta para Detalhes, que vai recarregar automaticamente ao aparecer
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao salvar alterações: {ex.Message}");
            ShowGeneralError("Ocorreu um erro inesperado. Tente novamente.");
        }
        finally
        {
            SetSaveLoadingState(false);
        }
    }

    private void SetSaveLoadingState(bool isLoading)
    {
        SaveButton.IsEnabled = !isLoading;
        SaveButton.Text = isLoading ? string.Empty : "Salvar alterações";
        SaveLoadingIndicator.IsVisible = isLoading;
        SaveLoadingIndicator.IsRunning = isLoading;

        PlantNameEntry.IsEnabled = !isLoading;
        NewDeviceCodeEntry.IsEnabled = !isLoading;
        DeleteButton.IsEnabled = !isLoading;
    }

    // ===================== EXCLUIR =====================

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        // Confirmação obrigatória antes de uma ação destrutiva
        bool confirmed = await DisplayAlert(
            "Excluir planta",
            $"Tem certeza que deseja excluir \"{_currentDetails?.Name}\"? Esta ação não pode ser desfeita e todos os dados históricos serão removidos.",
            "Sim, excluir",
            "Cancelar");

        if (!confirmed)
            return;

        await DeletePlantAsync();
    }

    private async Task DeletePlantAsync()
    {
        SetDeleteLoadingState(true);

        try
        {
            var result = await _editService.DeletePlantAsync(_plantId!);

            if (!result.Success)
            {
                ShowGeneralError(result.ErrorMessage ?? "Não foi possível excluir a planta. Tente novamente.");
                return;
            }

            // Após excluir, navega para a Lista de Plantas (2 níveis acima:
            // Detalhes da Planta → volta para Lista, não para Detalhes de algo que não existe mais)
            await Shell.Current.GoToAsync("//PlantsList");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao excluir planta: {ex.Message}");
            ShowGeneralError("Ocorreu um erro inesperado. Tente novamente.");
        }
        finally
        {
            SetDeleteLoadingState(false);
        }
    }

    private void SetDeleteLoadingState(bool isLoading)
    {
        DeleteButton.IsEnabled = !isLoading;
        DeleteButton.Text = isLoading ? string.Empty : "Excluir planta";
        DeleteLoadingIndicator.IsVisible = isLoading;
        DeleteLoadingIndicator.IsRunning = isLoading;

        SaveButton.IsEnabled = !isLoading;
    }

    private void ShowGeneralError(string message)
    {
        GeneralErrorLabel.Text = message;
        GeneralErrorBorder.IsVisible = true;
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}