using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials.Interfaces;

namespace Charter3HourLogin.Common.Services
{
    public static class IFileSystemExtensions
    {
        
        private static async Task RemoveFilesAsync(DirectoryInfo dirCurrent, string ignorefileString)
        {
            await Task.Run(() =>
            {
                foreach (var fi in dirCurrent.EnumerateFiles())
                {
                    if (ignorefileString.Length > 0 && !fi.Name.Contains(ignorefileString))
                        fi.Delete();
                    else
                        fi.Delete();
                }
            });
        }

        public static async Task<bool> RemoveFile(this IFileSystem fs, string folderPath, string fileName)
        {
            try
            {
                await Task.Run(() => { File.Delete(Path.Combine(folderPath, fileName)); });
                return true;
            }
            catch (Exception ex)
            {
                Debugger.Break();
                return false;
            }
        }

        public static async Task RemoveFilesOutsideThresholdAsync(this IFileSystem fs, DirectoryInfo dirCurrent, string includeFileString, int daysThreshold)
        {
            await Task.Run(() =>
            {
                foreach (var fi in dirCurrent.EnumerateFiles())
                {
                    if (fi.Name.Contains(includeFileString))
                    {
                        if (fi.CreationTime.AddDays(daysThreshold) < DateTime.Now)
                        {
                            fi.Delete();
                        }
                    }
                }
            });
        }

        private static async Task SaveFileToPathInternalAsync(string path, object model, byte[] byteArray = null)
        {
            await Task.Run(() =>
            {
                if (byteArray is null)
                    
                    File.WriteAllText(path, JsonConvert.SerializeObject(model));
                else
                    File.WriteAllBytes(path, byteArray);
            });
        }


        public static async Task<bool> SaveFileAsync(this IFileSystem fs, object model, string name, string subdirectory = null, byte[] byteArray = null)
        {
            try
            {
                if (!Path.HasExtension(name))
                {
                    name = $"{name}.json";
                }
                var path = string.IsNullOrWhiteSpace(subdirectory) ? Path.Combine(fs.AppDataDirectory, name) : Path.Combine(fs.AppDataDirectory, subdirectory, name);

                await SaveFileToPathInternalAsync(path, model, byteArray);
                return true;
            }
            catch (DirectoryNotFoundException ex)
            {
                System.Diagnostics.Trace.WriteLine("directory not found. Creating it.." + ex);
                fs.CreateDirectory(Path.Combine(fs.AppDataDirectory, subdirectory));
                return await SaveFileAsync(fs, model, name, subdirectory, byteArray);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<string> ReadFileAsync(this IFileSystem fs, string fileName, string path)
        {
            try
            {
                var retFile = string.Empty;
                //var path = fs.AppDataDirectory;
                //TODO: This code searches for a string, would me more efficient as a direct search
                await Task.Run(() =>
                {
                    foreach (var fi in new DirectoryInfo(path).EnumerateFiles())
                    {
                        if (fi.Name.Contains(fileName))
                        {
                            retFile = File.ReadAllText(fi.FullName);

                            break;
                        }
                    }
                });

                return string.IsNullOrEmpty(retFile) ? string.Empty : retFile;
            }
#if DEBUG
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
                return string.Empty;
            }
#else
            catch
            {
                return string.Empty;
            }
#endif
        }

        public static async Task<byte[]> ReadFileBytesAsync(this IFileSystem fs, string filePathAndName)
        {
            byte[] result;

            if (!File.Exists(filePathAndName)) return null;

            using (FileStream stream = File.Open(filePathAndName, FileMode.Open))
            {
                result = new byte[stream.Length];
                await stream.ReadAsync(result, 0, (int)stream.Length);
            }

            return result;
        }

        public static bool CreateDirectory(this IFileSystem fs, string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }
    }
}