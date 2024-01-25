namespace STGenetics.Shared
{
    public class Animal
    {
        public int AnimalId { get; set; }
        public string? Name { get; set; }
        public string? Breed { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Sex { get; set; } = "Male"; 
        public decimal Price { get; set; }
        public string? Status { get; set; } = "Active"; 
    }
}
