namespace App.DataAccess.Model
{
    public record ProjectUser
    {
        public Project Project { get; set; } = null!;

        public User User { get; set; } = null!;

        public string Description { get; set; } = "";
    }
}
