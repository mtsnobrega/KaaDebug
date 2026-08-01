using KaaDebug.Core.Interfaces.Diagnostic;

namespace KaaDebug.Views.Diagnostic;

[QueryProperty(nameof(PlantId), "plantId")]
public partial class IADiagnosisPage : ContentPage
{
    private readonly IDiagnosisService _diagnosisService;
    private string? _plantId;
    private byte[]? _capturedImageBytes;

    public string PlantId
    {
        get => _plantId ?? string.Empty;
        set => _plantId = value;
    }

    public IADiagnosisPage(IDiagnosisService diagnosisService)
    {
        InitializeComponent();
        _diagnosisService = diagnosisService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadHistoryAsync();
    }

    // ===================== CÂMERA / GALERIA =====================

    private async void OnOpenCameraClicked(object? sender, EventArgs e)
    {
        // Verifica permissão de câmera antes de abrir
        var status = await Permissions.RequestAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            await DisplayAlert(
                "Permissão necessária",
                "O Budflow precisa de acesso à câmera para fazer o diagnóstico.",
                "OK");
            return;
        }

        await CapturePhotoAsync(fromCamera: true);
    }

    private async void OnOpenGalleryClicked(object? sender, EventArgs e) =>
        await CapturePhotoAsync(fromCamera: false);

    private async Task CapturePhotoAsync(bool fromCamera)
    {
        try
        {
            FileResult? photo;

            if (fromCamera)
            {
                photo = await MediaPicker.Default.CapturePhotoAsync();
            }
            else
            {
                photo = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Selecionar foto da planta"
                });
            }

            if (photo is null) return;

            await using var stream = await photo.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            _capturedImageBytes = memoryStream.ToArray();

            // Exibe o preview da imagem capturada
            CapturedImageView.Source = ImageSource.FromStream(() => new MemoryStream(_capturedImageBytes));
            ImagePlaceholderBorder.IsVisible = false;
            ImagePreviewBorder.IsVisible = true;
            AnalyzeButton.IsVisible = true;
            AnalyzeButton.IsEnabled = true;
            AnalysisErrorBorder.IsVisible = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao capturar imagem: {ex.Message}");
            await DisplayAlert("Erro", "Não foi possível capturar a imagem. Tente novamente.", "OK");
        }
    }

    // ===================== ANÁLISE =====================

    private async void OnAnalyzeClicked(object? sender, EventArgs e)
    {
        if (_capturedImageBytes is null) return;

        SetAnalyzingState(true);

        try
        {
            var result = await _diagnosisService.AnalyzeImageAsync(_plantId!, _capturedImageBytes);

            if (!result.Success || result.Diagnosis is null)
            {
                ShowAnalysisError(result.ErrorMessage ?? "Não foi possível analisar a imagem. Tente novamente.");
                return;
            }

            var diagnosis = result.Diagnosis;

            // Exibe resultado
            OverallObservationLabel.Text = diagnosis.OverallObservation;

            if (diagnosis.IsHealthy)
            {
                HealthyObservationLabel.Text = diagnosis.OverallObservation;
                HealthyResultCard.IsVisible = true;
                IssuesResultSection.IsVisible = false;
            }
            else
            {
                IssuesCollectionView.ItemsSource = diagnosis.Issues;
                HealthyResultCard.IsVisible = false;
                IssuesResultSection.IsVisible = true;
            }

            CapturePanel.IsVisible = false;
            ResultPanel.IsVisible = true;

            // Atualiza o histórico com o novo diagnóstico
            await LoadHistoryAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro na análise: {ex.Message}");
            ShowAnalysisError("Ocorreu um erro inesperado. Tente novamente.");
        }
        finally
        {
            SetAnalyzingState(false);
        }
    }

    private void SetAnalyzingState(bool isAnalyzing)
    {
        CapturePanel.IsVisible = !isAnalyzing;
        AnalyzingPanel.IsVisible = isAnalyzing;
        AnalysisErrorBorder.IsVisible = false;
    }

    private void ShowAnalysisError(string message)
    {
        AnalysisErrorLabel.Text = message;
        AnalysisErrorBorder.IsVisible = true;
        CapturePanel.IsVisible = true;
        AnalyzingPanel.IsVisible = false;
    }

    private void OnNewAnalysisClicked(object? sender, EventArgs e)
    {
        // Reseta o painel para nova captura
        _capturedImageBytes = null;
        CapturedImageView.Source = null;
        ImagePreviewBorder.IsVisible = false;
        ImagePlaceholderBorder.IsVisible = true;
        AnalyzeButton.IsVisible = false;
        AnalyzeButton.IsEnabled = false;
        AnalysisErrorBorder.IsVisible = false;
        ResultPanel.IsVisible = false;
        CapturePanel.IsVisible = true;
    }

    // ===================== HISTÓRICO =====================

    private async Task LoadHistoryAsync()
    {
        try
        {
            var history = await _diagnosisService.GetDiagnosisHistoryAsync(_plantId!);
            HistoryCollectionView.ItemsSource = history;
            HistorySection.IsVisible = history.Count > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar histórico: {ex.Message}");
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("..");
}