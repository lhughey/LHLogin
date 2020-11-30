using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prism.Navigation;
using Xamarin.Essentials.Interfaces;

namespace Charter3HourLogin.Common.Services
{
    public interface IDataRepoService
    {
        string UserSaveFile { get; }
        Task<bool> SaveValue<T>(T obj, string fileName);
        Task<T> GetSavedValue<T>(string fileName) where T : class, new();
    }

    public class DataRepoService : ViewModelBase, IDataRepoService
    {
        private IFileSystem _fileSystem { get; set; }

        public DataRepoService(INavigationService navigationService,  IFileSystem fileSystem) : base(navigationService)
        {
            _fileSystem = fileSystem;
        }

        public string UserSaveFile { get;} = Constants.UserDataFile;

        public async Task<bool> SaveValue<T>(T obj, string fileName)
        {
            try
            {
                return await _fileSystem.SaveFileAsync(obj, fileName, Constants.UserDataDirectory);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                return false;
            }
        }

        public async Task<T> GetSavedValue<T>(string fileName) where T : class, new()
        {
            try
            {
                var path = Path.Combine(_fileSystem.AppDataDirectory, Constants.UserDataDirectory);
                var fileContents = await _fileSystem.ReadFileAsync(fileName, path);
                if (!string.IsNullOrWhiteSpace(fileContents))
                    return JsonConvert.DeserializeObject<T>(fileContents);

                return null;
            }
            catch (Exception ex)
            {
                Debugger.Break();
                return null;
            }
        }

    }
}
