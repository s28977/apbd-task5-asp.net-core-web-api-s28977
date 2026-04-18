using apbd_task5_asp.net_core_web_api_s28977.Database;
using apbd_task5_asp.net_core_web_api_s28977.Models;
using Microsoft.AspNetCore.Mvc;

namespace apbd_task5_asp.net_core_web_api_s28977.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Room>> GetAll(
        [FromQuery] int? minCapacity, 
        [FromQuery] bool? hasProjector,
        [FromQuery] bool? activeOnly)
    {
        var rooms = DataStore.Rooms.AsEnumerable();

        if (minCapacity.HasValue)
        {
            rooms = rooms.Where(r => r.Capacity >= minCapacity.Value);
        }

        if (hasProjector.HasValue)
        {
            rooms = rooms.Where(r => r.HasProjector == hasProjector.Value);
        }

        if (activeOnly is true)
        {
            rooms = rooms.Where(r => r.IsActive);
        }
        
        return Ok(rooms.ToList());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Room> GetById([FromRoute] int id)
    {
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);
        if (room is null)
        {
            return NotFound($"Room with id {id} not found");
        }

        return Ok(room);
    }

    [HttpPost]
    public ActionResult<Room> CreateRoom([FromBody] Room room)
    {
        room.Id = DataStore.NextRoomId;
        DataStore.Rooms.Add(room);

        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    [HttpGet("building/{buildingCode}")]
    public ActionResult<List<Room>> GetByBuilding([FromRoute] string buildingCode)
    {
        var rooms = DataStore.Rooms
            .Where(r => r.BuildingCode.Equals(buildingCode, StringComparison.OrdinalIgnoreCase))
            .ToList();
        return Ok(rooms);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Room> UpdateRoom([FromRoute] int id, [FromBody] Room updatedRoom)
    {
        if (updatedRoom.Id != 0 && updatedRoom.Id != id)
        {
            return BadRequest("URL id and body id must match.");
        }
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);
        if (room is null)
        {
            return NotFound($"Room with id {id} not found");
        }

        room.Name =  updatedRoom.Name;
        room.BuildingCode = updatedRoom.BuildingCode;
        room.Floor = updatedRoom.Floor;
        room.Capacity = updatedRoom.Capacity;
        room.HasProjector = updatedRoom.HasProjector;
        room.IsActive = updatedRoom.IsActive;

        return Ok(room);
    }
    
    [HttpDelete("{id:int}")]
    public IActionResult DeleteRoomById([FromRoute] int id)
    {
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);
        if (room is null)
        {
            return NotFound($"Room with id {id} not found");
        }
        
        DataStore.Rooms.Remove(room);
        return NoContent();
    }
}