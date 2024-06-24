using KhumaloCrafts.Models;

namespace KhumaloCrafts.Repo
{
    public interface IHomeRepo
    {
        Task<IEnumerable<Craft>> GetCrafts(string sTerm = "", int categoryId = 0);
        Task<IEnumerable<Category>> Categories();
    }
}