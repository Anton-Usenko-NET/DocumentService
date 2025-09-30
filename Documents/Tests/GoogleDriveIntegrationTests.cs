using Documents;
using Google.Apis.Auth.OAuth2;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DocumentService.Documents.Tests
{
    public class GoogleDriveIntegrationTests
    {
        string _idFileToCopy = "1wdy7If4VLSU4lqM7r0NaAmpSdP9oVBUOg8TdSbYw3T4";
        string _idFolderTestCopy = "192k3y5sICeNWs6V-MR7hVdPh14NeHqiV";
        [Test]
        public void UploadFile_ToGoogleDrive_ShouldSucceed()
        {
            var driveService = new DriveFileManager();
            var file = driveService.CopyFile(_idFileToCopy,"test", _idFolderTestCopy);

            Assert.That(file, Is.Not.Null);

            // cleanup
            driveService.DeleteFile(file.Id);
        }
    }
}
