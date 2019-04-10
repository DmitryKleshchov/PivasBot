namespace PivasBot.Db.Repositories
{
    public class MessageRepository : BaseRepository
    {
        public MessageRepository(DbConnection dbConn) : base(dbConn, "yobirbot", "messages") { }
    }
}
