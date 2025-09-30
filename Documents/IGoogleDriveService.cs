using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Documents
{
    public interface IGoogleDriveService
    {
        /// <summary>
        /// Copy file to Google Drive.
        /// </summary>
        /// <param name="fileId">ID file.</param>
        /// <param name="newTitle">Name new file.</param>
        /// /// <param name="parentFolder">Folder where you copy file</param>
        /// <returns> Copied file id.</returns>
        Google.Apis.Drive.v3.Data.File CopyFile(string fileId, string newTitle, string? parentFolder=null);
    }
}
