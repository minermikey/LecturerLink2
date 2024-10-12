using LecturerLink.Controllers;
using LecturerLink.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InformationControllerTests
{
    public class UnitTest1
    {
        [Fact]
        public void Create_Get_ReturnsViewResult()
        {
            // Arrange
            var environment = new Mock<IWebHostEnvironment>();
            var controller = new InformationController(environment.Object);

            // Act
            var result = controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_WithValidFile_ReturnsRedirectToActionResult()
        {
            // Arrange
            var environment = new Mock<IWebHostEnvironment>();
            environment.Setup(e => e.WebRootPath).Returns("uploads"); // Ensure this is set
            var fileUpload = new Mock<IFormFile>();
            fileUpload.Setup(f => f.Length).Returns(1024);
            fileUpload.Setup(f => f.FileName).Returns("testfile.txt");
            fileUpload.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[1024])); // Provide a stream of data
            var controller = new InformationController(environment.Object);

            // Act
            var result = await controller.Create(fileUpload.Object);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Create_Post_WithInvalidFile_ReturnsViewResultWithError()
        {
            // Arrange
            var environment = new Mock<IWebHostEnvironment>();
            var fileUpload = new Mock<IFormFile>();
            fileUpload.Setup(f => f.Length).Returns(0);
            var controller = new InformationController(environment.Object);

            // Act
            var result = await controller.Create(fileUpload.Object);

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.True(controller.ModelState.ContainsKey("File"));
        }

        [Fact]
        public void Index_ReturnsViewResultWithFileList()
        {
            // Arrange
            var environment = new Mock<IWebHostEnvironment>();
            environment.Setup(e => e.WebRootPath).Returns("uploads");
            var controller = new InformationController(environment.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = (List<string>)viewResult.ViewData.Model;
            Assert.NotEmpty(model);
        }
    }
}