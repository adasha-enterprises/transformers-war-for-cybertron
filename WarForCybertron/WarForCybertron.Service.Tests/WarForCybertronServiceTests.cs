using Moq;
using System;
using System.Collections.Generic;
using WarForCybertron.Model;
using WarForCybertron.Model.DTO;
using WarForCybertron.Service.Interfaces;
using Xunit;

namespace WarForCybertron.Service.Tests
{
    public class WarForCybertronServiceTests
    {
        [Fact]
        public async void CanGetListOfTransformerDTOObjectsFromService()
        {
            // arrange
            var setupResponse = GetGenericServiceResponse<List<TransformerDTO>>(); // GetListOfTransformerDTOObjectsServiceResponse();
            var mockService = new Mock<IWarForCybertronService>();
            mockService.Setup(s => s.GetTransformers(It.IsAny<Allegiance>()).Result).Returns(setupResponse);

            // act
            var serviceResponse = await mockService.Object.GetTransformers(Allegiance.AUTOBOT);

            // assert
            Assert.IsType<List<TransformerDTO>>(serviceResponse.ResponseEntity);
        }

        [Fact]
        public async void CanCreateTransformerDTOObjectFromService()
        {
            // arrange
            var transformerDTO = GetTransformerDTOObject();
            var setupResponse = GetGenericServiceResponse<TransformerDTO>(); // GetTransformerDTOObjectServiceResponse();
            var mockService = new Mock<IWarForCybertronService>();
            mockService.Setup(s => s.CreateTransformer(transformerDTO).Result).Returns(setupResponse);

            // act
            var serviceResponse = await mockService.Object.CreateTransformer(transformerDTO);

            // assert
            Assert.IsType<TransformerDTO>(serviceResponse.ResponseEntity);
        }

        [Theory]
        [InlineData("4b5c5f91-646e-eb11-b25b-380025b039ec")]
        public async void CanGetTransformerDTOObjectFromId(string id)
        {
            // arrange
            var guid = Guid.Parse(id);
            var setupResponse = GetGenericServiceResponse<TransformerDTO>(); // GetTransformerDTOObjectServiceResponse();
            var mockService = new Mock<IWarForCybertronService>();
            mockService.Setup(s => s.GetTransformer(It.IsAny<Guid>()).Result).Returns(setupResponse);

            // act
            var serviceResponse = await mockService.Object.GetTransformer(guid);

            // assert
            Assert.IsType<TransformerDTO>(serviceResponse.ResponseEntity);
        }

        [Fact]
        public async void CanUpdateTransformerDTOObject()
        {
            // arrange
            var transformerDTO = GetTransformerDTOObject();
            var setupResponse = GetGenericServiceResponse<TransformerDTO>(); // GetTransformerDTOObjectServiceResponse();
            var mockService = new Mock<IWarForCybertronService>();
            mockService.Setup(s => s.UpdateTransformer(transformerDTO).Result).Returns(setupResponse);

            // act
            var serviceResponse = await mockService.Object.UpdateTransformer(transformerDTO);

            // assert
            Assert.IsType<TransformerDTO>(serviceResponse.ResponseEntity);
        }

        [Theory]
        [InlineData("4b5c5f91-646e-eb11-b25b-380025b039ec")]
        public async void CanDeleteTransformerDTOObjectFromId(string id)
        {
            // arrange
            var guid = Guid.Parse(id);
            var mockService = new Mock<IWarForCybertronService>();
            mockService.Setup(s => s.DeleteTransformer(It.IsAny<Guid>()).Result).Returns(true);

            // act
            var response = await mockService.Object.DeleteTransformer(guid);

            // assert
            Assert.True(response);
        }

        [Theory]
        [InlineData("4b5c5f91-646e-eb11-b25b-380025b039ec")]
        public async void CanGetScoreFromId(string id)
        {
            // arrange
            var guid = Guid.Parse(id);
            var mockService = new Mock<IWarForCybertronService>();
            mockService.Setup(s => s.GetOverallScore(It.IsAny<Guid>()).Result).Returns(80);

            // act
            var response = await mockService.Object.GetOverallScore(guid);

            // assert
            Assert.IsType<int>(response);
        }

        [Fact]
        public async void CanSimulateWar()
        {
            // arrange
            var setupResponse = GetGenericServiceResponse<WarSimulation>();
            var mockService = new Mock<IWarForCybertronService>();
            mockService.Setup(s => s.SimulateWar().Result).Returns(setupResponse);

            // act
            var serviceResponse = await mockService.Object.SimulateWar();

            // assert
            Assert.IsType<WarSimulation>(serviceResponse.ResponseEntity);
        }

        private ServiceResponse<T> GetGenericServiceResponse<T>() where T : class
        {
            // T serviceResponseEntity = null;

            var serviceResponseEntity = typeof(T) switch
            {
                Type dto when dto == typeof(TransformerDTO) => GetTransformerDTOObject() as T,
                Type list when list == typeof(List<TransformerDTO>) => new List<TransformerDTO> { GetTransformerDTOObject() } as T,
                Type war when war == typeof(WarSimulation) => new WarSimulation(new List<TransformerDTO> { GetTransformerDTOObject() }, new List<TransformerDTO> { GetTransformerDTOObject() }) as T,
                _ => null
            };

            //switch (typeof(T))
            //{
            //    case Type dto when dto == typeof(TransformerDTO):
            //        serviceResponseEntity = GetTransformerDTOObject() as T;
            //        break;
            //    case Type list when list == typeof(List<TransformerDTO>):
            //        serviceResponseEntity = new List<TransformerDTO> { GetTransformerDTOObject() } as T;
            //        break;
            //    case Type war when war == typeof(WarSimulation):
            //        serviceResponseEntity = new WarSimulation(new List<TransformerDTO> { GetTransformerDTOObject() }, new List<TransformerDTO> { GetTransformerDTOObject() }) as T;
            //        break;
            //}

            //if (typeof(T) == typeof(TransformerDTO))
            //{
            //    serviceResponseEntity = GetTransformerDTOObject() as T;
            //}
            //else if (typeof(T) == typeof(List<TransformerDTO>))
            //{
            //    serviceResponseEntity = new List<TransformerDTO> { GetTransformerDTOObject() } as T;
            //}
            //else if (typeof(T) == typeof(WarSimulation))
            //{
            //    serviceResponseEntity = new WarSimulation(new List<TransformerDTO> { GetTransformerDTOObject() }, new List<TransformerDTO> { GetTransformerDTOObject() }) as T;
            //}

            return new ServiceResponse<T>(serviceResponseEntity, string.Empty, true);
        }

        private TransformerDTO GetTransformerDTOObject()
        {
            return new TransformerDTO
            {
                Name = "Bumblebee",
                Allegiance = Allegiance.AUTOBOT,
                Strength = 10,
                Intelligence = 10,
                Speed = 10,
                Endurance = 10,
                Rank = 10,
                Courage = 10,
                Firepower = 10,
                Skill = 10
            };
        }
    }
}
