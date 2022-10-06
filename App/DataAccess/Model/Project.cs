namespace App.DataAccess
{
    public record Project
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public int CompanyId { get; set; }
    }
}
