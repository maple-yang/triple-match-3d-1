namespace Game.Data.Models
{
    public interface IPersistence
    {
        bool JustCreate { get; set; }
        object Persistence { get; set; }
    }
}