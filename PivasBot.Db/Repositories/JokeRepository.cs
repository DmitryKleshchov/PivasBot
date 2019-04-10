namespace PivasBot.Db.Repositories
{
    public class JokeRepository : BaseRepository
    {
        public JokeRepository(DbConnection dbConn) : base(dbConn, "yobirbot", "jokes") { }
    }
}
