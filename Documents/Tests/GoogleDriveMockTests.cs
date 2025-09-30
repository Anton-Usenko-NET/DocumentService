using System;
using Documents;
using Xunit;
using Moq;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Auth.OAuth2;
using System.Text;

namespace DocumentService.Documents.Tests
{
    public class GoogleDriveMockTests
    {
        string _idParentFolder = "1bKfyXXHVQGKnP-gDsMSvsmgj7gt2DiCn";
        DriveFileManager _driveFileManager;
        Google.Apis.Drive.v3.Data.File _fileCopied;
        Google.Apis.Drive.v3.Data.File _fileCopiedParentFolder;
        Google.Apis.Drive.v3.Data.File _fileCopiedWithName;
        string fileName;

        public GoogleDriveMockTests()
        {
            //_driveFileManager = new DriveFileManager();
            //_file = _driveFileManager.GetFile(_idParentFolder);
            _fileCopiedParentFolder = new Google.Apis.Drive.v3.Data.File();
            _fileCopied = new Google.Apis.Drive.v3.Data.File();
            _fileCopiedParentFolder.Parents = new List<string>();
            _fileCopiedParentFolder.Parents.Add(_idParentFolder);

            _fileCopiedWithName = new Google.Apis.Drive.v3.Data.File();
            fileName = "test";
            _fileCopiedWithName.Name = fileName;
        }

        [Fact]
        public void CopyFile_ShouldCallMockedService()
        {
            // Arrange: create mock interface
            var mockService = new Mock<IGoogleDriveService>();
            mockService
                .Setup(s => s.CopyFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>()))
                .Returns(_fileCopied);
            
            // Act
            var result = mockService.Object.CopyFile("1eI6pScDDXcp2JmILP-Fv-_SNPH7hZlVIJUoW_qnRIC4", "Test", _idParentFolder);
            // Assert
            mockService.Verify(s => s.CopyFile("1eI6pScDDXcp2JmILP-Fv-_SNPH7hZlVIJUoW_qnRIC4", "Test", _idParentFolder), Times.Once);
        }
        [Fact]
        public void CopyFile_ShouldSetParrentFolder()
        {
            // Arrange: create mock interface
            var mockService = new Mock<IGoogleDriveService>();
            mockService
                .Setup(s => s.CopyFile(It.IsAny<string>(), It.IsAny<string>(), _idParentFolder))
                .Returns(_fileCopiedParentFolder);

            // Act
            var result = mockService.Object.CopyFile("1eI6pScDDXcp2JmILP-Fv-_SNPH7hZlVIJUoW_qnRIC4", "Test", _idParentFolder);
            // Assert
            
        }
        [Fact]
        public void CopyFile_ShouldSetName()
        {
            // Arrange: создаём мок интерфейса
            var mockService = new Mock<IGoogleDriveService>();
            mockService
                .Setup(s => s.CopyFile(It.IsAny<string>(), fileName, It.IsAny<string>()))
                .Returns(_fileCopiedWithName);

            // Act
            var result = mockService.Object.CopyFile("1eI6pScDDXcp2JmILP-Fv-_SNPH7hZlVIJUoW_qnRIC4", fileName, _idParentFolder);
            // Assert
            Assert.Equal(fileName, result.Name);
            
        }

         
        
    }
}
