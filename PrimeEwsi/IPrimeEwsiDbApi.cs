using System.Collections.Generic;
using PrimeEwsi.Models;

namespace PrimeEwsi
{
    public interface IPrimeEwsiDbApi
    {
        int AddHistoryPack(HistoryPackModel historyPackModel);
        int AddUser(UserModel userModel);
        ConfigModel GetConfigModelByComponent(string component);
        HistoryPackModel GetHistoryPacksByPackId(int packId);
        IEnumerable<HistoryPackModel> GetHistoryPacksByUserId(int userId);
        UserModel GetUser(string userName);
        UserModel GetUser(int userId);
        int UpdateConfig(ConfigModel configModel);
        int UpdateUser(UserModel userModel);
    }
}