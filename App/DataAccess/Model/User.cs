namespace App.DataAccess
{
    public record User
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Email { get; set; } = "";

        public int ManagerId { get; set; }

        public int EmployerId { get; set; }
    }
}
