using System.ComponentModel.DataAnnotations;

public class Booking
{
    [Key]
    public int Id { get; set; }  // Identity column, will be auto-generated
    public string EventName { get; set; }
    public DateTime Date { get; set; }
    public string Venue { get; set; }
    public int SeatsBooked { get; set; }
}
