using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSyncExample.ViewModels
{
    public class FileData: ViewModelBase
    {
        private DateTimeOffset? _serverLastModifiedDate;
        [JsonProperty("last-modified")]
        public DateTimeOffset? ServerLastModifiedDate
        {
            get => _serverLastModifiedDate;
            set => SetValueIfChanged(() => ServerLastModifiedDate, () => _serverLastModifiedDate, value);
        }

        public string _fileName;
        [JsonProperty("file-name")]
        public string FileName
        {
            get => _fileName;
            set => SetValueIfChanged(() => FileName, () => _fileName, value);
        }

        private string _clientName;
        [JsonProperty("client-name")]
        public string ClientName
        {
            get => _clientName;
            set => SetValueIfChanged(()=>ClientName, () => _clientName, value);
        }

        private bool _didUpdate;
        [JsonIgnore]
        public bool DidUpdate
        {
            get => _didUpdate;
            set => SetValueIfChanged(()=>DidUpdate, ()=>_didUpdate, value);
        }
    }
}
