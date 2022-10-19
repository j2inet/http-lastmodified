using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using FileSyncExample.Native;

namespace FileSyncExample.ViewModels
{
    internal class MainViewModel: ViewModelBase
    {
        public ObservableCollection<FileData> Files { get; private set; } = new ObservableCollection<FileData>();

        public MainViewModel()
        {
            Files.Add(new FileData() { FileName = "61lLJ85GYXL._AC_SL1000_.jpg", ClientName="j2i.net" });
            Files.Add(new FileData() { FileName = "61qfFAQ3xKL._AC_SL1500_.jpg", ClientName = "j2i.net" });
            Files.Add(new FileData() { FileName = "71PKvcmV6DL._AC_SX679_.jpg", ClientName = "j2i.net" });
            Files.Add(new FileData() { FileName = "61TUVbqPhLL.AC_SL1500.jpg", ClientName = "j2i.net" });

            Files.Add(new FileData() { FileName = "413oiguJDiL._AC_.jpg", ClientName = "j2i.net" });
            Files.Add(new FileData() { FileName = "413E4NVbSwL._SY445_SX342_QL70_FMwebp_.jpg", ClientName = "j2i.net" });
            Files.Add(new FileData() { FileName = "61ZM4GBs28L._AC_SL1500_.jpg", ClientName = "j2i.net" });
            Files.Add(new FileData() { FileName = "61LZrId7FvL._AC_SL1000_.jpg", ClientName = "j2i.net" });


            Files.Add(new FileData() { FileName= "71fOsWX9qlL._AC_UY327_FMwebp_QL65_.jpg", ClientName = "j2i.net" });
            RefreshMetadata();
            DownloadFiles();
        }



        void RefreshMetadata()
        {
            DirectoryInfo cacheDataDirectory = new DirectoryInfo(Settings.Default.CachePath);
            if (!cacheDataDirectory.Exists)
                return;
            foreach(var file in Files)
            {
                var fileInfo = new FileInfo(Path.Combine(cacheDataDirectory.FullName, file.FileName));
                if (!fileInfo.Exists)
                    continue;
                //Great! The file exists! Let's load the metadata for it!
                var metaFilePath = $"{fileInfo.FullName}:meta.json";
                var fileHandle = NativeMethods.CreateFileW(metaFilePath, NativeConstants.GENERIC_READ,
                                    0,//NativeConstants.FILE_SHARE_WRITE,
                                    IntPtr.Zero,
                                    NativeConstants.OPEN_ALWAYS,
                                    0,
                                    IntPtr.Zero);
                using (StreamReader sr = new StreamReader(new FileStream(fileHandle, FileAccess.Read)))
                {
                    var metaString = sr.ReadToEnd();
                    var readFileData = JsonConvert.DeserializeObject<FileData>(metaString);
                    file.ServerLastModifiedDate = readFileData.ServerLastModifiedDate;
                }

            }
        }
        async void DownloadFiles()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.ConnectionClose = true;

            var downloadDirectory = new DirectoryInfo(Settings.Default.CachePath);
            if(!downloadDirectory.Exists)
            {
                downloadDirectory.Create();
            }

            foreach (FileData file in Files)
            {

                FileData savedData = null;

                var requestUrl = $"https://m.media-amazon.com/images/I/{file.FileName}";
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

                if(file.ServerLastModifiedDate.HasValue)
                {
                    request.Headers.Add("If-Modified-Since", file.ServerLastModifiedDate.Value.ToString("R"));
                }

                var response = await client.SendAsync(request);
                if(response.StatusCode == System.Net.HttpStatusCode.NotModified)
                {
                    continue;
                }
                var lastModified = response.Content.Headers.LastModified;
                if(lastModified.HasValue)
                {
                    file.ServerLastModifiedDate = lastModified;
                }
                try
                {
                    response.EnsureSuccessStatusCode();
                    using (FileStream outputStream = new FileStream(Path.Combine(Settings.Default.CachePath, file.FileName), FileMode.Create, FileAccess.Write))
                    {
                        var data = await response.Content.ReadAsByteArrayAsync();
                        outputStream.Write(data, 0, data.Length);
                    }
                    //Putting the metadata in an alternative stream named meta.json
                    var fileMetadata = JsonConvert.SerializeObject(file);
                    Debug.WriteLine(fileMetadata);
                    var metaFilePath = Path.Combine(Settings.Default.CachePath, $"{file.FileName}:meta.json");
                    var fileHandle = NativeMethods.CreateFileW(metaFilePath, NativeConstants.GENERIC_WRITE,
                                        0,//NativeConstants.FILE_SHARE_WRITE,
                                        IntPtr.Zero,
                                        NativeConstants.OPEN_ALWAYS,
                                        0,
                                        IntPtr.Zero);
                    if(fileHandle != IntPtr.MinValue)
                    {
                        using(StreamWriter sw = new StreamWriter(new FileStream(fileHandle, FileAccess.Write)))
                        {
                            sw.Write(fileMetadata);
                        }
                    }

                }
                catch(Exception exc)
                {

                }
            }
        }
    }
}
