namespace KaaDebug.Core.Models.Plants
{
    public class CareTip
    {
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class PlantCareInfo
    {
        public string SpeciesName { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string? Curiosity { get; set; }
        public List<CareTip> Tips { get; set; } = new();
    }
}
