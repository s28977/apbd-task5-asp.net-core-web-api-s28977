using apbd_task5_asp.net_core_web_api_s28977.Database;
using apbd_task5_asp.net_core_web_api_s28977.Models;
using Microsoft.AspNetCore.Mvc;

namespace apbd_task5_asp.net_core_web_api_s28977.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Reservation>> GetReservations(
        [FromQuery] DateOnly? date,
        [FromQuery] string? status,
        [FromQuery] int? roomId)
    {
        var reservations = DataStore.Reservations.AsEnumerable();
        if (date.HasValue)
        {
            reservations = reservations.Where(r => r.Date == date.Value);
        }

        if (status is not null)
        {
            reservations = reservations.Where(r => r.Status == status);
        }

        if (roomId.HasValue)
        {
            reservations = reservations.Where(r => r.RoomId == roomId.Value);
        }
        
        return Ok(reservations.ToList());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Reservation> GetReservationById([FromRoute] int id)
    {
        var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation is null)
        {
            return NotFound($"Reservation with id {id} not found");
        }
        return Ok(reservation);
    }

    [HttpPost]
    public ActionResult<Reservation> CreateReservation([FromBody] Reservation reservation)
    {
        if (reservation.EndTime <= reservation.StartTime)
        {
            return BadRequest("End time must be later than start time");
        }
        
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == reservation.RoomId);
        if (room is null)
        {
            return BadRequest($"Room with id {reservation.RoomId} not found");
        }

        if (!room.IsActive)
        {
            return BadRequest($"Room with id {reservation.RoomId} is not active");
        }
        
        var overlappingReservationId = DataStore.Reservations.FirstOrDefault(r => Overlap(reservation, r))?.Id;
        if (overlappingReservationId.HasValue)
        {
            return Conflict($"Reservation overlaps with reservation with id {overlappingReservationId}");
        }
        
        reservation.Id = DataStore.NextReservationId;
        DataStore.Reservations.Add(reservation);
        return  CreatedAtAction("GetReservationById", new { id = reservation.Id }, reservation);
    }



    [HttpPut("{id:int}")]
    public ActionResult<Reservation> UpdateReservation([FromRoute] int id, [FromBody] Reservation updatedReservation)
    {
        if (updatedReservation.Id != 0 && updatedReservation.Id != id)
        {
            return BadRequest("URL id and body id must match.");
        }
        
        if (updatedReservation.EndTime <= updatedReservation.StartTime)
        {
            return BadRequest("End time must be later than start time");
        }
        
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == updatedReservation.RoomId);
        if (room is null)
        {
            return BadRequest($"Room with id {updatedReservation.RoomId} not found");
        }

        if (!room.IsActive)
        {
            return BadRequest($"Room with id {updatedReservation.RoomId} is not active");
        }
        
        var overlappingReservationId = DataStore.Reservations.FirstOrDefault(r => Overlap(updatedReservation, r))?.Id;
        if (overlappingReservationId.HasValue)
        {
            return Conflict($"Reservation overlaps with reservation with id {overlappingReservationId}");
        }
        
        var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation is null)
        {
            return NotFound($"Reservation with id {id} not found");
        }
        
        reservation.RoomId =  updatedReservation.RoomId;
        reservation.OrganizerName = updatedReservation.OrganizerName;
        reservation.Topic =  updatedReservation.Topic;
        reservation.Date = updatedReservation.Date;
        reservation.StartTime = updatedReservation.StartTime;
        reservation.EndTime = updatedReservation.EndTime;
        reservation.Status = updatedReservation.Status;
        
        return Ok(reservation);
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteReservationById([FromRoute] int id)
    {
        var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation is null)
        {
            return NotFound($"Reservation with id {id} not found");
        }
        DataStore.Reservations.Remove(reservation);
        return NoContent();
    }
    
    private bool Overlap(Reservation r1, Reservation r2)
    {
        return r1.Date == r2.Date && r1.RoomId == r2.RoomId && !(r1.StartTime > r2.EndTime || r2.StartTime > r1.EndTime);
    }
    
  
}