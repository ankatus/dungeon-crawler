namespace DungeonCrawler
{
    public record ResolutionSetting
    {
        public int Width { get; init; }
        public int Height { get; init; }
        public string Name { get; init; }
    }
}