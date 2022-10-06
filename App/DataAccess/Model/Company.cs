namespace App.DataAccess.Model
{
    public record Company
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
    }
}
