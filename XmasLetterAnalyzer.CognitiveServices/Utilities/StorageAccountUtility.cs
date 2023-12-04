using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmasLetterAnalyzer.Core.Utilities
{
    public static class StorageAccountUtility
    {
        public static async Task<string> DownloadText(string blobSasUrl)
        {
            // Create a BlobClient that represents a blob to download
            BlobClient blob = new BlobClient(new Uri(blobSasUrl));

            // Download the blob
            var response = await blob.DownloadAsync();

            // Convert the downloaded blob to a string
            using (var streamReader = new StreamReader(response.Value.Content))
            {
                return await streamReader.ReadToEndAsync();
            }
        }
    }
}

