using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataService.Http;

namespace PlatformService.Controllers
{
    
    [Route("api/platforms")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(IPlatformRepo repo, IMapper mapper, ICommandDataClient commandDataClient)
        {
            _repository = repo;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("Getting Platforms...");

            var platforms = _repository.GetAllPlatforms();
            var mappedResult = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);

            return Ok(mappedResult);
        }

        [HttpGet("{id}", Name="GetPlatformById")]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatformById(int id)
        {
            Console.WriteLine($"Getting Platform By Id {id}...");

            var platform = _repository.GetById(id);

            if (platform == null)
                return NotFound($"Platform id {id} not found!");

            var mappedResult = _mapper.Map<PlatformReadDto>(platform);
            return Ok(mappedResult);
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformDto)
        {
            Console.WriteLine($"Creating Platform...");

            var model = _mapper.Map<Platform>(platformDto);
            _repository.CreatePlatform(model);
            _repository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(model);

            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"-->Could not send synchronously: {ex.Message} {ex.InnerException.Message}");
            }
            

            return CreatedAtRoute(nameof(GetPlatformById), new { Id= platformReadDto.Id}, platformReadDto);
        }
    }
}